using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using System.Collections.Generic;

namespace ALT.CVL.Log.Model
{
    internal class LimitedObservableCollection<T> : ObservableCollection<T>
    {
        public int Capacity { get; set; }
        public LimitedObservableCollection(int capacity) : base()
        {
            Capacity = capacity;
            //base.CollectionChanged += LimitedObservableCollection_CollectionChanged;
        }

        public LimitedObservableCollection(int capacity, IEnumerable<T> anotherCollection) : base(anotherCollection) 
        {
            Capacity = capacity;
        }
        public new void Add(T item)
        {
            if (Dispatcher.CurrentDispatcher.Equals(Application.Current.Dispatcher))
                base.Add(item);
            else
            {
                Application.Current.Dispatcher.Invoke(() => Add(item));
                return;
            }

            if (Count > Capacity++)
                RemoveAt(0);
        }

        public new void Insert(int offset, T item)
        {
            if (Dispatcher.CurrentDispatcher.Equals(Application.Current.Dispatcher))
                base.InsertItem(offset, item);
            else
            {
                Application.Current.Dispatcher.Invoke(() => Insert(offset, item));
                return;
            }

            if (Count > Capacity++)
                Remove(this.LastOrDefault());
        }
    }
}
