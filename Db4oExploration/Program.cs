using System;
using System.IO;
using System.Linq;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;

namespace Db4oExploration
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var applicationWorkingDirectory = Environment.CurrentDirectory;

            var databaseFileName = Path.Combine(applicationWorkingDirectory, "Peabody.db4o");

            File.Delete(databaseFileName);

            var readingConfiguration = Db4oEmbedded.NewConfiguration();
            readingConfiguration.Common.Add(new TransparentActivationSupport());

            var writingConfiguration = Db4oEmbedded.NewConfiguration();
            writingConfiguration.Common.ObjectClass(typeof (Item)).CascadeOnUpdate(true);
            writingConfiguration.Common.ObjectClass(typeof (Container)).CascadeOnUpdate(true);

            using (var objectContainer = Db4oEmbedded.OpenFile(writingConfiguration, databaseFileName))
            {
                foreach (var seriesNumber in Enumerable.Range(0, 1000))
                {
                    MakeAndStoreTwoItemsInTwoContainers(objectContainer, seriesNumber);
                }
            }


            using (var objectContainer = Db4oEmbedded.OpenFile(readingConfiguration, databaseFileName))
            {
                var allContainers = objectContainer.Query<Container>();

                foreach (var container in allContainers)
                {
                    Console.WriteLine("Container: {0}", container.Name);

                    foreach (var item in container.Items)
                    {
                        Console.WriteLine("Contained item: {0}, {1}, contained by: {2}", item.Name, item.GetHashCode(),
                            item.Container.Name);
                    }
                }

                var allItems = objectContainer.Query<Item>();

                foreach (var item in allItems)
                {
                    Console.WriteLine("Item: {0}, {1}, contained by: {2}", item.Name, item.GetHashCode(),
                        item.Container.Name);
                }
            }

            Console.WriteLine("Done!");
        }

        private static void MakeAndStoreTwoItemsInTwoContainers(IEmbeddedObjectContainer objectContainer,
            int seriesNumber)
        {
            var evenIndex = 2 * seriesNumber;

            var oddIndex = 1 + evenIndex;


            var fred = new Container {Name = String.Format("Fred_{0}", evenIndex)};

            var itemOne = new Item {Name = String.Format("Odd_{0}", oddIndex)};

            var itemTwo = new Item {Name = String.Format("Even_{0}", evenIndex)};

            itemOne.Container = fred;

            itemTwo.Container = fred;


            {
                objectContainer.Store(itemOne);

                objectContainer.Commit();

                var ethel = new Container {Name = String.Format("Ethel_{0}", evenIndex)};

                var oldContainer = itemTwo.Container;

                itemTwo.Container = ethel;

                if (oldContainer != fred)
                {
                    throw new Exception("No!");
                }

                if (oldContainer.Items.Count() != 1)
                {
                    throw new Exception("No");
                }


                objectContainer.Store(oldContainer);
                objectContainer.Store(itemTwo);
            }
        }
    }
}