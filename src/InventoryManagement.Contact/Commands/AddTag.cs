using System;
using Infrastructure;

namespace InventoryManagement.Contact.Commands
{
    public class AddTag : ICommand
    {
        public AddTag(string tag)
        {
            Tag = tag;
        }

        public Guid Id { get; private set; }
        public string Tag { get; }

        public AddTag WithId(Guid id)
        {
            var clone = (AddTag)MemberwiseClone();
            clone.Id = id;
            return clone;
        }
    }
}