using M3Tech.OCR.API.PredictionDocument.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace M3Tech.OCR.API.PredictionDocument.Repositories
{
    public interface ICallTypeRepository
    {
        Task<CallType> AddCallTypeAsync(CallType callType);
        Task<CallType> GetCallTypeByNameAsync(string name);
    }
    public class CallTypeRepository : ICallTypeRepository
    {
        private readonly IDbContextFactory<DocgenieContext> _docgenieDbContext;
        public CallTypeRepository(IDbContextFactory<DocgenieContext> docgenieDbContext) 
        {
            _docgenieDbContext = docgenieDbContext;
        }

        public async Task<CallType> AddCallTypeAsync(CallType callType)
        {
            using var db = _docgenieDbContext.CreateDbContext();
            await db.CallTypes.AddAsync(callType);
            await db.SaveChangesAsync();
            return callType;
        }

        public async Task<CallType> GetCallTypeByNameAsync(string name)
        {
            using var db = _docgenieDbContext.CreateDbContext();
            return await db.CallTypes.FirstOrDefaultAsync(x=>x.CallNameEn.Contains(name));
        }
    }
}
