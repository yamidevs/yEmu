using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Core.BufferPool
{
   public class BufferPool<T>
    {
        private readonly Func<T> _factoryMethod;
        private ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public BufferPool(Func<T> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public void Allocate(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _queue.Enqueue(_factoryMethod());

            }
        }

        public T Dequeue()
        {
            T buffer;
            return !_queue.TryDequeue(out buffer) ? _factoryMethod() : buffer;
        }

        public void Enqueue(T buffer)
        {
            _queue.Enqueue(buffer);
        }
    }
}
