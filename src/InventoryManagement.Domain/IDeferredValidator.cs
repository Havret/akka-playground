namespace InventoryManagement.Domain
{
    public interface IDeferredValidator
    {
        bool IsReady { get; }
    }
}