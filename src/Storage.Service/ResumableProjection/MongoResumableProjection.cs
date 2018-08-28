using System.Threading.Tasks;
using Akka.Streams.Util;
using MongoDB.Driver;

namespace Storage.Service.ResumableProjection
{
    class MongoResumableProjection : IResumableProjection
    {
        readonly StorageContext _storageContext = new StorageContext();

        public async Task<bool> StoreLatestOffset(string identifier, long offset)
        {
            var updateDefinition = Builders<ProjectionOffset>.Update.Set(x => x.Offset, offset);
            var updateOptions = new UpdateOptions { IsUpsert = true };
            await _storageContext.ProjectionOffsets.UpdateOneAsync(x => x.Id == identifier, updateDefinition, updateOptions);
            return true;
        }

        public async Task<Option<long>> FetchLatestOffset(string identifier)
        {
            var cursor = await _storageContext.ProjectionOffsets.FindAsync(x => x.Id == identifier);
            var offset = await cursor.FirstOrDefaultAsync();
            return offset?.Offset ?? Option<long>.None;
        }
    }
}