using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using System.Collections.Generic;

namespace ALT.CVL.Core
{
    public enum CollectionStructureConstants
    {
        AddItematFirst,
        AddItematLast
    }
    /// <summary>
    /// 개수가 제한된 ObservableCollection 클래스 입니다.
    /// </summary>
    /// <typeparam name="T">
    /// type of Item
    /// </typeparam>
    public class LimitedObservableCollection<T> : ObservableCollection<T>
    {
        private CollectionStructureConstants structure;
        private readonly int capacity;

        /// <summary>
        /// 배열 접근 Property
        /// </summary>
        /// <param name="index">
        /// Capacity 내의 범위로 접근 해야함
        /// </param>
        /// <returns></returns>
        public new T this[int index]
        {
            get
            {
                if (index >= capacity)
                    throw new System.ArgumentOutOfRangeException("Capacity를 벗어난 배열 입니다.");

                return base[index];
            }
            set
            {
                if (index >= capacity)
                    throw new System.ArgumentOutOfRangeException("Capacity를 벗어난 배열 입니다.");

                base[index] = value;
            }
        }

        public int Capacity => capacity;
        public CollectionStructureConstants CollectionStructure => structure;
        public LimitedObservableCollection(int capacity, CollectionStructureConstants structure) : base()
        {
            this.capacity = capacity;
            this.structure = structure;
            //base.CollectionChanged += LimitedObservableCollection_CollectionChanged;
        }

        public LimitedObservableCollection(int capacity, CollectionStructureConstants structure, IEnumerable<T> anotherCollection) : base(anotherCollection) 
        {
            this.capacity = capacity;
            this.structure = structure;
        }

        public new void Add(T item)
        {
            switch (structure)
            {
                case CollectionStructureConstants.AddItematLast:
                    if (Dispatcher.CurrentDispatcher.Equals(Application.Current.Dispatcher))
                        base.Add(item);
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() => Add(item));
                        return;
                    }

                    if (Count > capacity)
                        RemoveAt(0);
                    break;
                case CollectionStructureConstants.AddItematFirst:
                    if (Dispatcher.CurrentDispatcher.Equals(Application.Current.Dispatcher))
                        base.Insert(0, item);
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() => Add(item));
                        return;
                    }

                    if (Count > Capacity)
                        RemoveAt(IndexOf(this.LastOrDefault()));
                    break;
            }
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

            if (Count > Capacity)
                Remove(this.LastOrDefault());
        }
    }
}
