using Akka.Cluster.Sharding;

namespace Infrastructure.Sharding
{
    public class CustomMessageExtractor : HashCodeMessageExtractor
    {
        public CustomMessageExtractor() : base(maxNumberOfShards: 100) { }

        public override string EntityId(object message)
        {
            switch (message)
            {
                case ShardEnvelope shardEnvelope:
                    return shardEnvelope.EntityId;
                case ShardRegion.StartEntity start:
                    return start.EntityId;
                default:
                    return null;
            }
        }

        public override object EntityMessage(object message)
        {
            switch (message)
            {
                case ShardEnvelope shardEnvelope:
                    return shardEnvelope.Message;
                case ShardRegion.StartEntity start:
                    return start.EntityId;
                default:
                    return null;
            }
        }
    }
}