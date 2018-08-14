using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Infrastructure.Config;
using Infrastructure.Sharding;
using InventoryManagement.Domain;

namespace InventoryManagement.Service
{
    public class InventoryManagementService
    {
        protected ActorSystem ClusterSystem;

        public bool Start()
        {
            var config = ConfigurationLoader.Load();
            ClusterSystem = ActorSystem.Create("akka-playground", config);

            var bookNameGuard = ClusterSystem.ActorOf(Props.Create<BookNameGuardActor>());

            var clusterSharding = ClusterSharding.Get(ClusterSystem);
            clusterSharding.Start(
                typeName: nameof(BookActor),
                entityProps: Props.Create(() => new BookValidationProxyActor(bookNameGuard)), 
                settings: ClusterShardingSettings.Create(ClusterSystem).WithRole("InventoryManagement"),
                messageExtractor: new CustomMessageExtractor()
            );

            return true;
        }

        public Task Stop()
        {
            return CoordinatedShutdown.Get(ClusterSystem).Run();
        }

        public Task WhenTerminated => ClusterSystem.WhenTerminated;
    }
}