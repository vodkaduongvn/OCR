using M3Tech.OCR.API.PredictionDocument.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace M3Tech.OCR.API.PredictionDocument.Repositories
{
    public interface IDocumentRepository
    {
        Task<List<Document>> GetDocuments();
        Task<Document> AddDocumentAsync(Document document);

        Task<Document> UpdateDocumentAsync(Document document, int id);

        Task<Document> AddOrUpdateDocumentAsync(Document document);
        Task<Document> GetExistingDocumentAsync(Document document);
    }
    public class DocumentRepository:IDocumentRepository
    {
        private readonly IDbContextFactory<DocgenieContext> _docgenieDbContext;
        public DocumentRepository(IDbContextFactory<DocgenieContext> docgenieDbContext) 
        {
            _docgenieDbContext = docgenieDbContext;
        }

        public async Task<List<Document>> GetDocuments()
        {
            using var db = _docgenieDbContext.CreateDbContext();
            return await db.Documents.ToListAsync();
        }

        public async Task<Document> AddDocumentAsync(Document document)
        {
            using var db = await _docgenieDbContext.CreateDbContextAsync();
            await db.Documents.AddAsync(document);
            await db.SaveChangesAsync();

            return document;
        }

        public async Task<Document> UpdateDocumentAsync(Document document, int id)
        {
            using var db = await _docgenieDbContext.CreateDbContextAsync();
            db.Entry(document).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return document;
        }

        public async Task<Document> AddOrUpdateDocumentAsync(Document document)
        {
            var existingDoc = await GetExistingDocumentAsync(document);
            if (existingDoc != null)
            {
                document.DocumentId = existingDoc.DocumentId;
                return await UpdateDocumentAsync(document, document.DocumentId);
            }
            return await AddDocumentAsync(document);
        }

        public async Task<Document> GetExistingDocumentAsync(Document document)
        {
            using var db = await _docgenieDbContext.CreateDbContextAsync();
            return await db.Documents.FirstOrDefaultAsync(x => x.DocumentName.Contains(document.DocumentName));
        }
    }
}
