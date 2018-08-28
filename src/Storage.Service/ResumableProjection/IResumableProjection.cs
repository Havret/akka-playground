using Akka.Streams.Util;
using System.Threading.Tasks;

namespace Storage.Service.ResumableProjection
{
    public interface IResumableProjection
    {
        Task<bool> StoreLatestOffset(string identifier, long offset);
        Task<Option<long>> FetchLatestOffset(string identifier);
    }
}