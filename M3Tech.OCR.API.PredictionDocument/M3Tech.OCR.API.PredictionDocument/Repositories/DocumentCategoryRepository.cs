using M3Tech.OCR.API.PredictionDocument.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace M3Tech.OCR.API.PredictionDocument.Repositories
{
    public interface IDocumentCategoryRepository
    {
        Task<List<DocumentCategory>> GetDocumentCategoriesAsync();
    }
    public class DocumentCategoryRepository : IDocumentCategoryRepository
    {
        private readonly IDbContextFactory<DocgenieContext> _docgenieDbContext;
        public DocumentCategoryRepository(IDbContextFactory<DocgenieContext> docgenieDbContext) 
        {
            _docgenieDbContext = docgenieDbContext;
        }

        public async Task<List<DocumentCategory>> GetDocumentCategoriesAsync()
        {
            using var db = _docgenieDbContext.CreateDbContext();
            return await db.DocumentCategories.ToListAsync();
        }
    }
}
