namespace BackEnd.Aventura.Domain
{
    public sealed class EspetaculoImagem
    {
        public EspetaculoImagem(Stream conteudo, string contentType, string nomeArquivo)
        {
            Conteudo = conteudo ?? throw new ArgumentNullException(nameof(conteudo));
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            NomeArquivo = nomeArquivo ?? throw new ArgumentNullException(nameof(nomeArquivo));
        }

        public Stream Conteudo { get; }
        public string ContentType { get; }
        public string NomeArquivo { get; }
    }
}