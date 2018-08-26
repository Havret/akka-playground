using Akka.Streams.Util;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Storage.Service.ResumableProjection
{
    public interface IResumableProjection
    {
        Task<bool> StoreLatestOffset(string identifier, long offset);
        Task<Option<long>> FetchLatestOffset(string identifier);
    }

    class MongoResumableProjection : IResumableProjection
    {
        readonly StorageContext _storageContext = new StorageContext();

        public async Task<bool> StoreLatestOffset(string identifier, long offset)
        {
            var updateDefinition = Builders<ProjectionOffset>.Update.Set(x => x.Offset, offset);
            var updateOptions = new UpdateOptions { IsUpsert = true };
            await _storageContext.ProjectionOffsets.UpdateOneAsync(x => x.Identifier == identifier, updateDefinition, updateOptions);
            return true;
        }

        public async Task<Option<long>> FetchLatestOffset(string identifier)
        {
            var projectionOffset = await _storageContext.ProjectionOffsets.Find(x => x.Identifier == identifier).FirstOrDefaultAsync();
            return projectionOffset?.Offset ?? Option<long>.None;
        }
    }
}