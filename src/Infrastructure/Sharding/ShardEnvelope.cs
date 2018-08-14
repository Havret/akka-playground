namespace Infrastructure.Sharding
{
    public class ShardEnvelope
    {
        public ShardEnvelope(string entityId, object message)
        {
            EntityId = entityId;
            Message = message;
        }

        public string EntityId { get; }
        public object Message { get; }
    }
}