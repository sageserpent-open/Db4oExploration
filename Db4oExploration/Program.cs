using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;

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

    public class Item
    {
        public string Name { get; set; }

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
            var applicationWorkingDirectory = Environment.CurrentDirectory;

            var databaseFileName = Path.Combine(applicationWorkingDirectory, "Peabody.db4o");

            File.Delete(databaseFileName);

            var containerOne = new Container {Name = "Fred"};

            var itemOne = new Item {Name = "One"};

            var itemTwo = new Item {Name = "Two"};

            itemOne.Container = containerOne;

            itemTwo.Container = containerOne;

            var config = Db4oEmbedded.NewConfiguration();
            config.Common.ObjectClass(typeof (Item)).CascadeOnUpdate(true);
            config.Common.ObjectClass(typeof (Container)).CascadeOnUpdate(true);

            using (var objectContainer = Db4oEmbedded.OpenFile(databaseFileName))
            {
                objectContainer.Store(itemOne);

                objectContainer.Commit();

                var containerTwo = new Container {Name = "Ethel"};

                itemTwo.Container = containerTwo;

                objectContainer.Store(itemTwo);
            }

            using (var objectContainer = Db4oEmbedded.OpenFile(databaseFileName))
            {
                var allContainers = objectContainer.Query<Container>();

                foreach (var container in allContainers)
                {
                    Console.WriteLine("Container: {0}", container.Name);

                    foreach (var item in container.Items)
                    {
                        Console.WriteLine("Contained item: {0}, {1}", item.Name, item.GetHashCode());
                    }
                }

                var allItems = objectContainer.Query<Item>();

                foreach (var item in allItems)
                {
                    Console.WriteLine("Item: {0}, {1}", item.Name, item.GetHashCode());
                }
            }

            Console.WriteLine("Done!");
        }
    }
}