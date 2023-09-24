using M3Tech.OCR.API.PredictionDocument.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace M3Tech.OCR.API.PredictionDocument.Repositories
{
    public interface IConsumerRepository
    {
        Task<Consumer> AddConsumerAsync(Consumer consumer);
        Task<Consumer> GetConsumerByNameAsync(string name);
    }
    public class ConsumerRepository : IConsumerRepository
    {
        private readonly IDbContextFactory<DocgenieContext> _docgenieDbContext;
        public ConsumerRepository(IDbContextFactory<DocgenieContext> docgenieDbContext) 
        {
            _docgenieDbContext = docgenieDbContext;
        }

        public async Task<Consumer> AddConsumerAsync(Consumer consumer)
        {
            using var db = _docgenieDbContext.CreateDbContext();
            await db.Consumers.AddAsync(consumer);
            await db.SaveChangesAsync();
            return consumer;
        }

        public async Task<Consumer> GetConsumerByNameAsync(string name)
        {
            using var db = _docgenieDbContext.CreateDbContext();
            return await db.Consumers.FirstOrDefaultAsync(x => x.ConsumerNameEn.Contains(name));
        }
    }
}
