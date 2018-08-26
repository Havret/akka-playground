using Akka.Actor;

namespace ApiGateway
{
    public delegate IActorRef CreateBookActor();
    public delegate IActorRef CreateBookQueryHandler();
}