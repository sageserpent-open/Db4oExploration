using System;

namespace Db4oExploration
{
    public class Item
    {
        public string Name { get; set; }

        public Container Container
        {
            get { return _container; }
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
}