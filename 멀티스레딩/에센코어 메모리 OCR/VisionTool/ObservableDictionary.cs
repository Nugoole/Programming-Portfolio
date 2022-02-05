using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace TIS_OCRVisionTool
{
    internal class ObservableDictionary<TKey, TValue> : DictionaryBase, IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public TValue this[TKey key]
        {
            get
            {
                return (TValue)InnerHashtable[key];
            }
            set
            {
                InnerHashtable[key] = value;
                RaisePropertyChanged();
            }
        }

        public ICollection<TKey> Keys => InnerHashtable.Keys.Cast<TKey>().ToList();
        public ICollection<TValue> Values => InnerHashtable.Values.Cast<TValue>().ToList();
        public bool IsReadOnly { get; }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public void Add(TKey key, TValue value)
        {
            InnerHashtable.Add(key, value);
            var kvPair = new KeyValuePair<TKey, TValue>(key, value);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>[] { kvPair }));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            InnerHashtable.Add(item.Key, item.Value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>[] { item }));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return InnerHashtable.ContainsKey(item.Key) && InnerHashtable[item.Key].Equals(item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return InnerHashtable.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            InnerHashtable.CopyTo(array, arrayIndex);
        }

        public bool Remove(TKey key)
        {
            if (!InnerHashtable.ContainsKey(key))
                return false;

            try
            {
                var kvPair = new KeyValuePair<TKey, TValue>(key, (TValue)InnerHashtable[key]);
                InnerHashtable.Remove(key);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>[] { kvPair }));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!Contains(item))
                return false;

            InnerHashtable.Remove(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!InnerHashtable.ContainsKey(key))
            {
                value = default(TValue);
                return false;
            }


            value = (TValue)InnerHashtable[key];
            return true;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (DictionaryEntry item in InnerHashtable)
            {
                dictionary.Add((TKey)item.Key, (TValue)item.Value);
            }

            return dictionary.GetEnumerator();
        }
    }
}
