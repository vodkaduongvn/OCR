using M3Tech.OCR.API.PredictionDocument.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace M3Tech.OCR.API.PredictionDocument.Repositories
{
    public interface ILabelRepository
    {
        Task<List<Label>> GetLabelsAsync();
    }
    public class LabelRepository : ILabelRepository
    {
        private readonly IDbContextFactory<DocgenieContext> _docgenieDbContext;
        public LabelRepository(IDbContextFactory<DocgenieContext> docgenieDbContext) 
        {
            _docgenieDbContext = docgenieDbContext;
        }

        public async Task<List<Label>> GetLabelsAsync()
        {
            using var db = _docgenieDbContext.CreateDbContext();
            return await db.Labels.ToListAsync();
        }
    }
}
