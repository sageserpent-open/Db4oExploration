using System;
using System.IO;
using System.Linq;
using Db4objects.Db4o;

namespace Db4oExploration
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var applicationWorkingDirectory = Environment.CurrentDirectory;

            var databaseFileName = Path.Combine(applicationWorkingDirectory, "Peabody.db4o");

            File.Delete(databaseFileName);

            foreach (var seriesNumber in Enumerable.Range(0, 5))
            {
                MakeAndStoreTwoItemsInTwoContainers(databaseFileName, seriesNumber);
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

        private static void MakeAndStoreTwoItemsInTwoContainers(string databaseFileName, int seriesNumber)
        {
            var evenIndex = 2 * seriesNumber;

            var oddIndex = 1 + evenIndex;


            var containerOne = new Container {Name = String.Format("Fred_{0}", evenIndex)};

            var itemOne = new Item {Name = String.Format("Odd_{0}", oddIndex)};

            var itemTwo = new Item {Name = String.Format("Even_{0}", evenIndex)};

            itemOne.Container = containerOne;

            itemTwo.Container = containerOne;

            var config = Db4oEmbedded.NewConfiguration();
            config.Common.ObjectClass(typeof (Item)).CascadeOnUpdate(true);
            config.Common.ObjectClass(typeof (Container)).CascadeOnUpdate(true);

            using (var objectContainer = Db4oEmbedded.OpenFile(databaseFileName))
            {
                objectContainer.Store(itemOne);

                objectContainer.Commit();

                var containerTwo = new Container {Name = String.Format("Ethel_{0}", evenIndex)};

                var oldContainer = itemTwo.Container;

                itemTwo.Container = containerTwo;

                objectContainer.Store(itemTwo);
                objectContainer.Store(oldContainer);
            }
        }
    }
}