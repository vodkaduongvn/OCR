using M3Tech.OCR.API.PredictionDocument.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace M3Tech.OCR.API.PredictionDocument.Repositories
{
    public interface ICallRepository
    {
        Task<Call> AddCallAsync(Call call);
    }
    public class CallRepository : ICallRepository
    {
        private readonly IDbContextFactory<DocgenieContext> _docgenieDbContext;
        public CallRepository(IDbContextFactory<DocgenieContext> docgenieDbContext) 
        {
            _docgenieDbContext = docgenieDbContext;
        }

        public async Task<Call> AddCallAsync(Call call)
        {
            using var db = _docgenieDbContext.CreateDbContext();
            await db.Calls.AddAsync(call);
            await db.SaveChangesAsync();
            return call;
        }
    }
}
