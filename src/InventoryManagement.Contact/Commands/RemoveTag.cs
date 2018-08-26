using System;
using Infrastructure;

namespace InventoryManagement.Contact.Commands
{
    public class RemoveTag : ICommand
    {
        public RemoveTag(string tag)
        {
            Tag = tag;
        }

        public Guid Id { get; private set; }
        public string Tag { get; }

        public RemoveTag WithId(Guid id)
        {
            var clone = (RemoveTag)MemberwiseClone();
            clone.Id = id;
            return clone;
        }
    }
}