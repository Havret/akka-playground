using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Routing;
using Infrastructure.Config;
using Infrastructure.Sharding;
using Microsoft.Extensions.DependencyInjection;

namespace ApiGateway.Config
{
    public static class ActorSystemExtensions
    {
        public static void AddActorSystem(this IServiceCollection services)
        {
            var config = ConfigurationLoader.Load();
            var clusterSystem = ActorSystem.Create("akka-playground", config);

            var clusterSharding = ClusterSharding.Get(clusterSystem);
            var bookActor = clusterSharding.StartProxy("BookActor", "InventoryManagement", new CustomMessageExtractor());
            var bookQueryHandler = clusterSystem.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "book-query-handler");

            services.AddSingleton<CreateBookQueryHandler>(() => bookQueryHandler);
            services.AddSingleton<CreateBookActor>(() => bookActor);
        }
    }
}