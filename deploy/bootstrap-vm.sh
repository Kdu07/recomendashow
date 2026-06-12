#!/usr/bin/env bash
# Bootstrap da VM de inferência (Ubuntu/OCI). Idempotente — pode rodar mais de uma vez.
# Instala Docker + compose, cria swap de 2 GB e abre 80/443 no iptables.
set -euo pipefail

echo "==> [1/3] Docker Engine + plugin compose"
if ! command -v docker >/dev/null 2>&1; then
    sudo apt-get update
    sudo apt-get install -y ca-certificates curl gnupg
    sudo install -m 0755 -d /etc/apt/keyrings
    if [ ! -f /etc/apt/keyrings/docker.gpg ]; then
        curl -fsSL https://download.docker.com/linux/ubuntu/gpg \
            | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
        sudo chmod a+r /etc/apt/keyrings/docker.gpg
    fi
    echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] \
https://download.docker.com/linux/ubuntu $(. /etc/os-release && echo "$VERSION_CODENAME") stable" \
        | sudo tee /etc/apt/sources.list.d/docker.list >/dev/null
    sudo apt-get update
    sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin
    sudo usermod -aG docker "$USER"
    echo "    Docker instalado. (Re-login pode ser necessário para usar docker sem sudo.)"
else
    echo "    Docker já instalado: $(docker --version)"
fi

echo "==> [2/3] Swapfile de 2 GB (VM de 1 GB de RAM; pico de memória do primeiro batch)"
if [ ! -f /swapfile ]; then
    sudo fallocate -l 2G /swapfile
    sudo chmod 600 /swapfile
    sudo mkswap /swapfile
    sudo swapon /swapfile
    if ! grep -q '^/swapfile' /etc/fstab; then
        echo '/swapfile none swap sw 0 0' | sudo tee -a /etc/fstab >/dev/null
    fi
    echo "    Swap criado e ativado."
else
    echo "    Swapfile já existe."
fi

echo "==> [3/3] Firewall local (iptables — imagens Ubuntu da OCI não usam ufw)"
for port in 80 443; do
    if ! sudo iptables -C INPUT -p tcp --dport "$port" -j ACCEPT 2>/dev/null; then
        # Insere antes da regra REJECT padrão da OCI
        sudo iptables -I INPUT 5 -p tcp --dport "$port" -m state --state NEW -j ACCEPT
        echo "    Porta $port liberada."
    else
        echo "    Porta $port já liberada."
    fi
done
if [ -f /etc/iptables/rules.v4 ]; then
    sudo sh -c 'iptables-save > /etc/iptables/rules.v4'
    echo "    Regras persistidas em /etc/iptables/rules.v4."
fi

echo "==> Bootstrap concluído. (Lembrete: liberar 80/443 também na Security List da OCI Console.)"
