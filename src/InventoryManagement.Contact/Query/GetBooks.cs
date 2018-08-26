namespace InventoryManagement.Contact.Query
{
    public class GetBooks
    {
        public GetBooks(string tag = null)
        {
            Tag = tag;
        }

        public string Tag { get; }
    }
}