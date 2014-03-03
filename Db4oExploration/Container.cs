using System.Collections.Generic;

namespace Db4oExploration
{
    public class Container
    {
        public IEnumerable<Item> Items
        {
            get { return _items; }
        }

        public string Name { get; set; }

        internal void Add(Item item)
        {
            _items.Add(item);
        }

        internal void Remove(Item item)
        {
            _items.Remove(item);
        }

        private readonly HashSet<Item> _items = new HashSet<Item>();
    }
}