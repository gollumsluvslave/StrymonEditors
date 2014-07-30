using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Threading;

namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// Helper collection class to allow easy two-way binding of collections / lists
    /// Handles Dispatcher / threading issues - similar to ObservableCollection, but better
    /// Note each public method has a private counterpart that does the work, each public wethod will
    /// determine wthere or not to invoke on the dispatcher thread if necessary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindableCollection<T> : IList<T>, INotifyCollectionChanged
    {
        private IList<T> collection = new List<T>();
        private Dispatcher dispatcher;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public BindableCollection()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Add an item
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (Thread.CurrentThread == dispatcher.Thread)
                DoAdd(item);
            else
                dispatcher.Invoke((Action)(() =>
                {
                    DoAdd(item);
                }));
        }
        private void DoAdd(T item)
        {
            collection.Add(item);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        /// Clear the collection
        /// </summary>
        public void Clear()
        {
            if (Thread.CurrentThread == dispatcher.Thread)
                DoClear();
            else
                dispatcher.Invoke((Action)(() =>
                {
                    DoClear();
                }));
        }
        private void DoClear()
        {
            collection.Clear();
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Determine whether the item is present
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            var result = collection.Contains(item);
            return result;
        }
        
        /// <summary>
        /// Copy to the supplied array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the count of elements
        /// </summary>
        public int Count
        {
            get
            {
                var result = collection.Count;
                return result;
            }
        }

        /// <summary>
        /// Return whether or not this collection is readonly
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return collection.IsReadOnly;
            }
        }

        /// <summary>
        /// Remove the supplied item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            if (Thread.CurrentThread == dispatcher.Thread)
                return DoRemove(item);
            else
            {
                var op = dispatcher.BeginInvoke(new Func<T, bool>(DoRemove), item);
                if (op == null || op.Result == null)
                    return false;
                return (bool)op.Result;
            }
        }
        private bool DoRemove(T item)
        {
            var index = collection.IndexOf(item);
            if (index == -1)
            {
                return false;
            }
            var result = collection.Remove(item);
            if (result && CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return result;
        }

        /// <summary>
        /// Returns the Enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }
        public int IndexOf(T item)
        {
            var result = collection.IndexOf(item);
            return result;
        }

        /// <summary>
        /// Insert the supplied item at the supplied index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            if (Thread.CurrentThread == dispatcher.Thread)
                DoInsert(index, item);
            else
                dispatcher.Invoke((Action)(() =>
                {
                    DoInsert(index, item);
                }));
        }
        private void DoInsert(int index, T item)
        {
            collection.Insert(index, item);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Remove the item at the specified index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (Thread.CurrentThread == dispatcher.Thread)
                DoRemoveAt(index);
            else
                dispatcher.Invoke((Action)(() =>
                {
                    DoRemoveAt(index);
                }));
        }
        private void DoRemoveAt(int index)
        {
            if (collection.Count == 0 || collection.Count <= index)
            {
                return;
            }
            collection.RemoveAt(index);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Index return property
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                var result = collection[index];
                return result;
            }
            set
            {
                if (collection.Count == 0 || collection.Count <= index)
                {
                    return;
                }
                collection[index] = value;
            }
        }
    }
}
