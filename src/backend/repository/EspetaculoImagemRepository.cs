using System.Globalization;
using System.Text;
using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Infrastructure;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace BackEnd.Aventura.Repository
{
    public sealed class EspetaculoImagemRepository : IEspetaculoImagemRepository
    {
        private const string DefaultContentType = "application/octet-stream";

        private readonly GridFSBucket _bucket;

        public EspetaculoImagemRepository(IMongoConnectionFactory mongoConnectionFactory)
        {
            if (mongoConnectionFactory is null) throw new ArgumentNullException(nameof(mongoConnectionFactory));

            _bucket = new GridFSBucket(mongoConnectionFactory.GetDatabase());
        }

        public EspetaculoImagem? GetByEspetaculoNome(string espetaculoNome)
        {
            if (string.IsNullOrWhiteSpace(espetaculoNome))
            {
                throw new ArgumentException("Nome do espetáculo é obrigatório.", nameof(espetaculoNome));
            }

            var fileInfo = FindFileInfo(espetaculoNome);
            if (fileInfo is null)
            {
                return null;
            }

            var contentType = fileInfo.Metadata is not null && fileInfo.Metadata.Contains("content_type")
                ? fileInfo.Metadata["content_type"].AsString
                : DefaultContentType;

            var downloadStream = _bucket.OpenDownloadStream(fileInfo.Id);

            return new EspetaculoImagem(downloadStream, contentType, fileInfo.Filename);
        }

        private GridFSFileInfo? FindFileInfo(string espetaculoNome)
        {
            // 1) Tentativa rápida: correspondência exata (caso comum quando o nome
            // cadastrado no Mongo já é idêntico ao nome do espetáculo no Postgres).
            var exactFilter = Builders<GridFSFileInfo>.Filter.Eq("metadata.espetaculo_nome", espetaculoNome);
            var exactMatch = _bucket.Find(exactFilter).FirstOrDefault();
            if (exactMatch is not null)
            {
                return exactMatch;
            }

            // 2) Fallback tolerante: o nome cadastrado no Mongo pode ter capitalização,
            // acentuação ou pontuação diferentes (ex.: "Mamma Mia" vs "Mamma Mia!"),
            // ou ser uma versão abreviada do nome completo (ex.: "Viva" vs
            // "Viva – A Vida é Uma Festa In Concert"). Compara-se a versão normalizada
            // (sem acentos, minúscula, sem pontuação) de cada documento contra o nome buscado.
            // NOTA: isto varre todos os documentos do bucket; aceitável para a escala atual
            // (dezenas de imagens). Se a coleção crescer muito, considerar persistir o nome
            // já normalizado como campo indexado em espetaculo_images/metadata.
            var normalizedTarget = Normalize(espetaculoNome);
            if (normalizedTarget.Length == 0)
            {
                return null;
            }

            var allFiles = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();

            GridFSFileInfo? bestMatch = null;
            var bestMatchLength = -1;

            foreach (var candidate in allFiles)
            {
                var candidateNome = candidate.Metadata is not null && candidate.Metadata.Contains("espetaculo_nome")
                    ? candidate.Metadata["espetaculo_nome"].AsString
                    : null;

                if (string.IsNullOrWhiteSpace(candidateNome))
                {
                    continue;
                }

                var normalizedCandidate = Normalize(candidateNome);
                if (normalizedCandidate.Length == 0)
                {
                    continue;
                }

                var isMatch = normalizedTarget == normalizedCandidate
                    || StartsWithWholeWord(normalizedTarget, normalizedCandidate)
                    || StartsWithWholeWord(normalizedCandidate, normalizedTarget);

                // Entre múltiplos candidatos compatíveis, prioriza o de maior sobreposição
                // (string normalizada mais longa), reduzindo o risco de prefixos curtos
                // demais (ex.: "elis") capturarem um espetáculo sem relação real.
                if (isMatch && normalizedCandidate.Length > bestMatchLength)
                {
                    bestMatch = candidate;
                    bestMatchLength = normalizedCandidate.Length;
                }
            }

            return bestMatch;
        }

        private static string Normalize(string value)
        {
            var decomposed = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(decomposed.Length);

            foreach (var ch in decomposed)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (char.IsLetterOrDigit(ch))
                {
                    builder.Append(char.ToLowerInvariant(ch));
                }
                else if (char.IsWhiteSpace(ch) && builder.Length > 0 && builder[^1] != ' ')
                {
                    builder.Append(' ');
                }
            }

            return builder.ToString().Trim();
        }

        /// <summary>
        /// Verifica se <paramref name="text"/> começa com <paramref name="prefix"/> respeitando
        /// limite de palavra (o caractere seguinte ao prefixo deve ser espaço ou fim da string).
        /// Evita falsos positivos como "elis" casando com um espetáculo cujo nome normalizado
        /// começa com "elisabete" sem ter relação real. Exige também um tamanho mínimo de
        /// prefixo, para que nomes muito curtos não gerem correspondências espúrias.
        /// </summary>
        private static bool StartsWithWholeWord(string text, string prefix)
        {
            const int minPrefixLength = 3;

            if (prefix.Length < minPrefixLength || prefix.Length >= text.Length)
            {
                return false;
            }

            if (!text.StartsWith(prefix, StringComparison.Ordinal))
            {
                return false;
            }

            return text[prefix.Length] == ' ';
        }
    }
}