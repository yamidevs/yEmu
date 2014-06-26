using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yEmu.Collections
{
   public class ConcurrentList<T> : IEnumerable<T>
    {
        private readonly List<T> underlyingList = new List<T>();
        private readonly ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
        private readonly ConcurrentQueue<T> underlyingQueue;
        private bool isDirty;

        public ConcurrentList()
        {
            underlyingQueue = new ConcurrentQueue<T>();
        }

        public ConcurrentList(IEnumerable<T> items)
        {
            underlyingQueue = new ConcurrentQueue<T>(items);
        }

        private void UpdateLists()
        {
            if (!isDirty)
            {
                return;
            }

            m_lock.EnterWriteLock();

            T temp;
            while (underlyingQueue.TryDequeue(out temp))
            {
                underlyingList.Add(temp);
            }

            m_lock.ExitWriteLock();
        }

        public IEnumerator<T> GetEnumerator()
        {
            UpdateLists();
            return new SafeEnumerator<T>(underlyingList);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            underlyingQueue.Enqueue(item);
            isDirty = true;
        }

        public T this[int index]
        {
            get
            {
                UpdateLists();
                return underlyingList[index];
            }
            set
            {
                UpdateLists();
                underlyingList[index] = value;
            }
        }

        public int Count
        {
            get
            {
                UpdateLists();
                return underlyingList.Count;
            }
        }

        public bool Remove(T item)
        {
            UpdateLists();
            return underlyingList.Remove(item);
        }

        public void Clear()
        {
            UpdateLists();
            underlyingList.Clear();
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return underlyingList.AsReadOnly();
        }
    }

    public class SafeEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> m_Inner;

        public SafeEnumerator(IEnumerable<T> inner)
        {
            m_Inner = inner.ToList().GetEnumerator();
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            return m_Inner.MoveNext();
        }

        public void Reset()
        {
            m_Inner.Reset();
        }

        public T Current
        {
            get { return m_Inner.Current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
