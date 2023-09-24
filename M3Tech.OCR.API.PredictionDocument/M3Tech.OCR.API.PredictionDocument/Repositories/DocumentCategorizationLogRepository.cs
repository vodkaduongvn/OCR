using M3Tech.OCR.API.PredictionDocument.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace M3Tech.OCR.API.PredictionDocument.Repositories
{
    public interface IDocumentCategorizationLogRepository
    {
        Task<DocumentCategorizationLog> AddDocumentCategorizationLogAsync(DocumentCategorizationLog documentCateLog);
    }
    public class DocumentCategorizationLogRepository : IDocumentCategorizationLogRepository
    {
        private readonly IDbContextFactory<DocgenieContext> _docgenieDbContext;
        public DocumentCategorizationLogRepository(IDbContextFactory<DocgenieContext> docgenieDbContext) 
        {
            _docgenieDbContext = docgenieDbContext;
        }

        public async Task<DocumentCategorizationLog> AddDocumentCategorizationLogAsync(DocumentCategorizationLog documentCateLog)
        {
            using var db = await _docgenieDbContext.CreateDbContextAsync();
            await db.DocumentCategorizationLogs.AddAsync(documentCateLog);
            await db.SaveChangesAsync();

            return documentCateLog;
        }
    }
}
