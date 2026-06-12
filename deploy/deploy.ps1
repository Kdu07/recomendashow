# Deploy manual do sistema de recomendação para a VM OCI (Windows -> Linux x64).
# Pré-requisitos: Docker Desktop (modo Linux), OpenSSH (ssh/scp), deploy/deploy.config
# e .env.deploy preenchidos, chave-back.key na raiz do repo.
# Uso: powershell -File deploy\deploy.ps1
$ErrorActionPreference = 'Stop'

$RepoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $RepoRoot

# ---------- 1. Config e validações ----------
$configPath = Join-Path $RepoRoot 'deploy\deploy.config'
if (-not (Test-Path $configPath)) {
    throw "deploy/deploy.config não encontrado. Copie deploy/deploy.config.example e preencha."
}
$config = @{}
Get-Content $configPath | ForEach-Object {
    $line = $_.Trim()
    if ($line -and -not $line.StartsWith('#') -and $line.Contains('=')) {
        $parts = $line.Split('=', 2)
        $config[$parts[0].Trim()] = $parts[1].Trim()
    }
}
foreach ($key in 'VM_HOST', 'VM_USER', 'SSH_KEY', 'REMOTE_DIR') {
    if (-not $config[$key]) { throw "deploy.config: chave '$key' ausente." }
}
$VmHost = $config['VM_HOST']; $VmUser = $config['VM_USER']
$SshKey = Join-Path $RepoRoot $config['SSH_KEY']; $RemoteDir = $config['REMOTE_DIR']

if ($VmHost -like '<*') { throw "deploy.config: VM_HOST ainda é placeholder ($VmHost)." }
if (-not (Test-Path $SshKey)) { throw "Chave SSH não encontrada: $SshKey" }
if (-not (Test-Path (Join-Path $RepoRoot '.env.deploy'))) {
    throw ".env.deploy não encontrado. Copie .env.deploy.example e preencha (confirme banco/porta/SSL)."
}
if (-not (Test-Path (Join-Path $RepoRoot 'data\dataset\dataset.csv'))) {
    throw "data/dataset/dataset.csv não encontrado (gitignored — gere com 'dotnet run preprocess' em src/ml)."
}

$Remote = "$VmUser@$VmHost"
$sshArgs = @('-i', $SshKey, '-o', 'StrictHostKeyChecking=accept-new')

function Invoke-Remote([string]$cmd) {
    ssh @sshArgs $Remote $cmd
    if ($LASTEXITCODE -ne 0) { throw "Comando remoto falhou (exit $LASTEXITCODE): $cmd" }
}

# ---------- 2. Build local + docker save ----------
Write-Host "==> [1/5] Build da imagem (local, linux-x64)" -ForegroundColor Cyan
docker build -f src/inference-service/Dockerfile -t aventura-recsys-inference:latest .
if ($LASTEXITCODE -ne 0) { throw "docker build falhou." }

# Arquivo temporário: NÃO usar pipe docker save | ssh — o pipeline do
# PowerShell 5.1 corrompe dados binários.
$imageTar = Join-Path $RepoRoot 'inference-image.tar'
docker save -o $imageTar aventura-recsys-inference:latest
if ($LASTEXITCODE -ne 0) { throw "docker save falhou." }

# ---------- 3. Tarball do repo (só o que está commitado) ----------
Write-Host "==> [2/5] git archive HEAD" -ForegroundColor Cyan
$repoTar = Join-Path $RepoRoot 'repo.tar'
git archive HEAD -o $repoTar
if ($LASTEXITCODE -ne 0) { throw "git archive falhou." }

# ---------- 4. Transferência (scp -C: compressão no fio) ----------
Write-Host "==> [3/5] Transferência para $Remote" -ForegroundColor Cyan
Invoke-Remote "mkdir -p $RemoteDir/data/dataset $RemoteDir/data/models $RemoteDir/deploy/certs"

scp @sshArgs -C $repoTar "${Remote}:$RemoteDir/repo.tar"
if ($LASTEXITCODE -ne 0) { throw "scp do repo.tar falhou." }
scp @sshArgs -C $imageTar "${Remote}:$RemoteDir/inference-image.tar"
if ($LASTEXITCODE -ne 0) { throw "scp da imagem falhou." }
scp @sshArgs -C "$RepoRoot\.env.deploy" "${Remote}:$RemoteDir/.env.deploy"
if ($LASTEXITCODE -ne 0) { throw "scp do .env.deploy falhou." }
scp @sshArgs -C "$RepoRoot\data\dataset\dataset.csv" "${Remote}:$RemoteDir/data/dataset/"
if ($LASTEXITCODE -ne 0) { throw "scp do dataset.csv falhou." }
# Cobre artefatos regenerados localmente e ainda não commitados
scp @sshArgs -C (Get-ChildItem "$RepoRoot\data\models\*" -File | ForEach-Object { $_.FullName }) "${Remote}:$RemoteDir/data/models/"
if ($LASTEXITCODE -ne 0) { throw "scp dos modelos falhou." }

# ---------- 5. Na VM: extrair, bootstrap, load, cert, up ----------
Write-Host "==> [4/5] Provisionamento e subida na VM" -ForegroundColor Cyan
Invoke-Remote "cd $RemoteDir && tar -xf repo.tar && rm repo.tar"
# sed remove CRLF caso o script tenha sido commitado/extraído com fim de linha Windows
Invoke-Remote "cd $RemoteDir && sed -i 's/\r$//' deploy/bootstrap-vm.sh && bash deploy/bootstrap-vm.sh"
Invoke-Remote "cd $RemoteDir && sudo docker load -i inference-image.tar && rm inference-image.tar"
Invoke-Remote "cd $RemoteDir && if [ ! -f deploy/certs/selfsigned.crt ]; then openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout deploy/certs/selfsigned.key -out deploy/certs/selfsigned.crt -subj '/CN=$VmHost'; fi"
Invoke-Remote "cd $RemoteDir && sudo docker compose -f docker-compose.deploy.yml up -d"

# ---------- 6. Smoke test ----------
Write-Host "==> [5/5] Smoke test (o healthcheck só fica verde após o carregamento dos artefatos)" -ForegroundColor Cyan
$healthy = $false
foreach ($attempt in 1..24) {   # ~2 min (24 x 5s) + tempo das chamadas
    # curl.exe nativo do Windows; -k por causa do cert self-signed.
    # 503 durante o carregamento / connection refused enquanto o nginx aguarda
    # service_healthy — seguir tentando.
    $status = curl.exe -k -s -o NUL -w "%{http_code}" --max-time 10 "https://$VmHost/api/diagnostics/health"
    if ($status -eq '200') { $healthy = $true; break }
    Start-Sleep -Seconds 5
}

Write-Host ''
Invoke-Remote "cd $RemoteDir && sudo docker compose -f docker-compose.deploy.yml logs --tail 30 inference"

if ($healthy) {
    Write-Host "`n==> Deploy OK: https://$VmHost/api/diagnostics/health respondeu 200." -ForegroundColor Green
} else {
    Write-Warning ("Health ainda não respondeu 200 (pode ser o primeiro batch/carregamento em curso). " +
        "Acompanhe com: ssh -i `"$SshKey`" $Remote 'cd $RemoteDir && sudo docker compose ps'")
}

Remove-Item $imageTar, $repoTar -Force -ErrorAction SilentlyContinue
