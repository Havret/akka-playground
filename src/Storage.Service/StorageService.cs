using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using Infrastructure.Config;
using Storage.Service.Book;
using Storage.Service.ResumableProjection;

namespace Storage.Service
{
    internal class StorageService
    {
        protected ActorSystem ClusterSystem;

        public void Start()
        {
            var config = ConfigurationLoader.Load();
            ClusterSystem = ActorSystem.Create("akka-playground", config);
            ClusterSystem.ActorOf(Props.Create(() => new BookViewBuilder(new MongoResumableProjection())));
            ClusterSystem.ActorOf(Props.Create<BookView>().WithRouter(FromConfig.Instance), "book-query-handlers");
        }

        public Task Stop() => CoordinatedShutdown.Get(ClusterSystem).Run();

        public Task WhenTerminated => ClusterSystem.WhenTerminated;

    }
}