using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db4oExploration
{
    public class Container
    {
        public IEnumerable<Item> Items { get; set; }

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

    public class Item
    {
        public Container Container
        {
            get { throw new NotImplementedException(); }
            set
            {
                if (null != _container)
                {
                    _container.Remove(this);
                }

                _container = value;

                if (null != _container)
                {
                    _container.Add(this);
                }
            }
        }

        private Container _container;
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var containerOne = new Container();

            var itemOne = new Item();

            var itemTwo = new Item();

            itemOne.Container = containerOne;

            itemTwo.Container = containerOne;

            var containerTwo = new Container();

            itemTwo.Container = containerTwo;

            Console.WriteLine("Done!");
        }
    }
}