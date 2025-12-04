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
    public class ObservableCollectionBase<TType> : IList, IList<TType>, INotifyPropertyChanged,
        INotifyCollectionChanged, IReadOnlyCollection<TType>
    {
        /// <summary>
        ///     _listLocker is used to lock the current collection object
        /// </summary>
        private readonly object _listLocker = new object();

        /// <summary>
        ///     _inputCollection is the source where the operations takes place
        /// </summary>
        private readonly IList<TType> _inputCollection;

        private object _syncRoot;

        public ObservableCollectionBase(IEnumerable<TType> inputCollection)
        {
            _inputCollection = inputCollection.ToList();
        }

        public ObservableCollectionBase(IList<TType> inputCollection)
        {
            _inputCollection = inputCollection;
        }

        public ObservableCollectionBase()
        {
            _inputCollection = new List<TType>();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        ///     Count property is used get the count of the input collection items
        /// </summary>
        public int Count
        {
            get
            {
                lock (_listLocker)
                {
                    return _inputCollection.Count;
                }
            }
        }


        bool IList.IsFixedSize
        {
            get
            {
                var collection = _inputCollection as IList;
                return collection?.IsFixedSize ?? _inputCollection.IsReadOnly;
            }
        }

        bool IList.IsReadOnly => _inputCollection.IsReadOnly;

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot != null)
                    return _syncRoot;
                lock (_listLocker)
                {
                    if (_inputCollection is ICollection collection)
                        _syncRoot = collection.SyncRoot;
                    else
                        Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        bool ICollection<TType>.IsReadOnly => _inputCollection.IsReadOnly;

        public TType this[int index]
        {
            get
            {
                lock (_listLocker)
                {
                    return _inputCollection[index];
                }
            }
            set
            {
                lock (_listLocker)
                {
                    _inputCollection[index] = value;
                }
            }
        }

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (TType) value;
        }

        public void Add(TType item)
        {
            lock (_listLocker)
            {
                _inputCollection.Add(item);
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        public void AddRange(IList<TType> objects)
        {
            lock (_listLocker)
            {
                ((List<TType>) _inputCollection).AddRange(objects);
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChangedMultiItem(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, objects));
            }
        }

        public void Clear()
        {
            lock (_listLocker)
            {
                _inputCollection.Clear();
            }
        }

        public bool Contains(TType item)
        {
            lock (_listLocker)
            {
                return _inputCollection.Contains(item);
            }
        }

        public void CopyTo(TType[] array, int arrayIndex)
        {
            lock (_listLocker)
            {
                _inputCollection.CopyTo(array, arrayIndex);
            }
        }

        public int IndexOf(TType item)
        {
            lock (_listLocker)
            {
                return _inputCollection.IndexOf(item);
            }
        }

        public void Insert(int index, TType item)
        {
            lock (_listLocker)
            {
                _inputCollection.Insert(index, item);
            }
        }

        public bool Remove(TType item)
        {
            lock (_listLocker)
            {
                if (!_inputCollection.Remove(item))
                    return false;
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                return true;
            }
        }

        public void RemoveAll(Predicate<TType> predicate)
        {
            lock (_listLocker)
            {
                var collection = (List<TType>) _inputCollection;
                var predicate1 = (Func<TType, bool>) (x => predicate(x));
                var objs = collection.Where(predicate1);
                var match = predicate;
                collection.RemoveAll(match);
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChangedMultiItem(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, objs));
            }
        }

        public void RemoveAt(int index)
        {
            lock (_listLocker)
            {
                var obj = _inputCollection[index];
                _inputCollection.RemoveAt(index);
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj));
            }
        }

        public void RemoveRange(int begin, int end)
        {
            lock (_listLocker)
            {
                var collection = (List<TType>) _inputCollection;
                var index1 = begin;
                var count1 = end;
                collection.RemoveRange(index1, count1);
                var index2 = begin;
                var count2 = end;
                var range = collection.GetRange(index2, count2);
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChangedMultiItem(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, range));
            }
        }

        int IList.Add(object value)
        {
            Add((TType) value);
            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            return Contains((TType) value);
        }

        void ICollection.CopyTo(Array inputArray, int index)
        {
            lock (_listLocker)
            {
                if (inputArray.Rank != 1)
                    throw new ArgumentException("Multidimension arrays are not supported");
                if (inputArray.GetLowerBound(0) != 0)
                    throw new ArgumentException("Non-zero lower bound arrays are not supported");
                if (index < 0)
                    throw new ArgumentOutOfRangeException();
                if (inputArray.Length - index < _inputCollection.Count)
                    throw new ArgumentException("Array is too small");
                if (!(inputArray is TType[] typeArray))
                    throw new ArrayTypeMismatchException("Invalid inputArray type");
                _inputCollection.CopyTo(typeArray, index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_listLocker)
            {
                return _inputCollection.GetEnumerator();
            }
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((TType) value);
        }

        void IList.Insert(int index, object value)
        {
            var obj = (TType) value;
            Insert(index, obj);
        }

        void IList.Remove(object value)
        {
            Remove((TType) value);
        }

        IEnumerator<TType> IEnumerable<TType>.GetEnumerator()
        {
            lock (_listLocker)
            {
                return _inputCollection.GetEnumerator();
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            var handler = CollectionChanged;
            if (handler == null)
                return;

            if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate { handler(this, args); }));

            // Application.Current.Dispatcher.BeginInvoke((Delegate)(() => handler((object)this, args)));
            else
                handler(this, args);
        }

        private void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
        {
            var collectionChanged = CollectionChanged;

            if (collectionChanged == null)
                return;

            foreach (var invocation in collectionChanged.GetInvocationList())
            {
                var view = invocation.Target as CollectionView;

                if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                    Application.Current.Dispatcher.BeginInvoke
                    (view != null
                        ? delegate { view.Refresh(); }
                        : new Action(delegate { collectionChanged(this, e); }));
                // Application.Current.Dispatcher.BeginInvoke(view != null ? (Delegate)(() => view.Refresh()) : (Delegate)(() => collectionChanged((object)this, e)));
                else if (view != null)
                    view.Refresh();
                else
                    collectionChanged(this, e);
            }
        }

        private void OnPropertyChanged(string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            var e = new PropertyChangedEventArgs(propertyName);
            propertyChanged(this, e);
        }
    }
}