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

            var filter = Builders<GridFSFileInfo>.Filter.Eq("metadata.espetaculo_nome", espetaculoNome);
            var fileInfo = _bucket.Find(filter).FirstOrDefault();

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
    }
}