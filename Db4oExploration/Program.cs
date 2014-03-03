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
            var applicationWorkingDirectory = Environment.CurrentDirectory;

            var databaseFileName = Path.Combine(applicationWorkingDirectory, "Peabody.db4o");

            File.Delete(databaseFileName);

            var containerOne = new Container();

            var itemOne = new Item();

            var itemTwo = new Item();

            itemOne.Container = containerOne;

            itemTwo.Container = containerOne;

            var config = Db4oEmbedded.NewConfiguration();
            config.Common.ObjectClass(typeof(Item)).CascadeOnUpdate(true);

            using (var objectContainer = Db4oEmbedded.OpenFile(databaseFileName))
            {
                objectContainer.Store(itemOne);
            }

            using (var objectContainer = Db4oEmbedded.OpenFile(databaseFileName))
            {
                var containerTwo = new Container();

                itemTwo.Container = containerTwo;
            }

            Console.WriteLine("Done!");
        }
    }
}