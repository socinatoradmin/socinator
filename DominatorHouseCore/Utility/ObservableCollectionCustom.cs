#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;

#endregion

namespace DominatorHouseCore.Utility
{
    public sealed class ObservableCollectionCustom<T> : IList, IList<T>, INotifyPropertyChanged,
        INotifyCollectionChanged, IReadOnlyCollection<T>
    {
        private readonly object listLock = new object();
        private readonly IList<T> collection;
        private object syncRoot;

        public ObservableCollectionCustom(IList<T> collection)
        {
            this.collection = collection;
        }

        public ObservableCollectionCustom()
        {
            collection = new List<T>();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count
        {
            get
            {
                lock (listLock)
                {
                    return collection.Count;
                }
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                if (!(this.collection is IList collection))
                    return this.collection.IsReadOnly;
                return collection.IsFixedSize;
            }
        }

        bool IList.IsReadOnly => collection.IsReadOnly;

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot
        {
            get
            {
                if (syncRoot != null)
                    return syncRoot;
                lock (listLock)
                {
                    if (this.collection is ICollection collection)
                        syncRoot = collection.SyncRoot;
                    else
                        Interlocked.CompareExchange<object>(ref syncRoot, new object(), null);
                }

                return syncRoot;
            }
        }

        bool ICollection<T>.IsReadOnly => collection.IsReadOnly;

        public T this[int index]
        {
            get
            {
                lock (listLock)
                {
                    return collection[index];
                }
            }
            set
            {
                lock (listLock)
                {
                    collection[index] = value;
                }
            }
        }

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T) value;
        }

        public void Add(T item)
        {
            lock (listLock)
            {
                collection.Add(item);
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        public void AddRange(IList<T> objects)
        {
            lock (listLock)
            {
                ((List<T>) collection).AddRange(objects);
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChangedMultiItem(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, objects));
            }
        }

        public void Clear()
        {
            lock (listLock)
            {
                collection.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (listLock)
            {
                return collection.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (listLock)
            {
                collection.CopyTo(array, arrayIndex);
            }
        }

        public int IndexOf(T item)
        {
            lock (listLock)
            {
                return collection.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (listLock)
            {
                collection.Insert(index, item);
            }
        }

        public bool Remove(T item)
        {
            lock (listLock)
            {
                if (!collection.Remove(item))
                    return false;
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                return true;
            }
        }

        public void RemoveAll(Predicate<T> predicate)
        {
            lock (listLock)
            {
                var collection = (List<T>) this.collection;
                var objs = collection.Where(x => predicate(x));
                collection.RemoveAll(predicate);
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChangedMultiItem(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, objs));
            }
        }

        public void RemoveAt(int index)
        {
            lock (listLock)
            {
                var obj = collection[index];
                collection.RemoveAt(index);
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj));
            }
        }

        public void RemoveRange(int begin, int end)
        {
            lock (listLock)
            {
                var collection = (List<T>) this.collection;
                collection.RemoveRange(begin, end);
                var range = collection.GetRange(begin, end);
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChangedMultiItem(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, range));
            }
        }

        int IList.Add(object value)
        {
            Add((T) value);
            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            return Contains((T) value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            lock (listLock)
            {
                if (array.Rank != 1)
                    throw new ArgumentException("Multidimension arrays are not supported");
                if (array.GetLowerBound(0) != 0)
                    throw new ArgumentException("Non-zero lower bound arrays are not supported");
                if (index < 0)
                    throw new ArgumentOutOfRangeException();
                if (array.Length - index < collection.Count)
                    throw new ArgumentException("Array is too small");
                if (!(array is T[] array1))
                    throw new ArrayTypeMismatchException("Invalid array type");
                collection.CopyTo(array1, index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (listLock)
            {
                return collection.GetEnumerator();
            }
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T) value);
        }

        void IList.Insert(int index, object value)
        {
            var obj = (T) value;
            Insert(index, obj);
        }

        void IList.Remove(object value)
        {
            Remove((T) value);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (listLock)
            {
                return collection.GetEnumerator();
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            // ISSUE: reference to a compiler-generated field
            var handler = CollectionChanged;
            if (handler == null)
                return;
            if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                Application.Current.Dispatcher.BeginInvoke(new Action(() => handler(this, args)));
            else
                handler(this, args);
        }

        private void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
        {
            // ISSUE: reference to a compiler-generated field
            var collectionChanged = CollectionChanged;
            if (collectionChanged == null)
                return;
            foreach (var invocation in collectionChanged.GetInvocationList())
            {
                var view = invocation.Target as CollectionView;
                if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                    Application.Current.Dispatcher.BeginInvoke(view != null
                        ? () => view.Refresh()
                        : new Action(() => collectionChanged(this, e)));
                else if (view != null)
                    view.Refresh();
                else
                    collectionChanged(this, e);
            }
        }

        private void OnPropertyChanged(string propertyName = null)
        {
            // ISSUE: reference to a compiler-generated field
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}