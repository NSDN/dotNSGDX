using System;
using System.Threading;
using System.Collections.Generic;

namespace dotNSGDX.Utility
{
    public class ObjectPoolCluster
    {
        protected class ObjectPool
        {
            public struct PoolInfo
            {
                public int interval;
            }

            private static readonly object _lock = new object();
            private static int _counter = 0;
            public static int Counter
            {
                get
                {
                    try
                    {
                        Monitor.Enter(_lock);
                        return _counter;
                    }
                    finally
                    {
                        Monitor.Exit(_lock);
                    }
                }
                set
                {
                    Monitor.Enter(_lock);
                    _counter = value;
                    Monitor.Exit(_lock);
                }
            }
            protected int nowCounter;

            protected LinkedList<IObject> objectPool;
            protected LinkedList<IObject> cachePool;
            protected WaitCallback waitCallback;

            private bool _running;
            public bool IsRunning
            {
                get
                {
                    try
                    {
                        Monitor.Enter(this);
                        return _running;
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
                }
                set
                {
                    Monitor.Enter(this);
                    _running = value;
                    Monitor.Exit(this);
                }
            }

            public bool Finished
            {
                get
                {
                    try
                    {
                        Monitor.Enter(this);
                        return (nowCounter == Counter);
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
                }
            }

            public ObjectPool()
            {
                nowCounter = -1;
                objectPool = new LinkedList<IObject>();
                cachePool = new LinkedList<IObject>();
                
                this.waitCallback = (state) =>
                {
                    while (IsRunning)
                    {
                        int interval = 10;
                        if (state is PoolInfo)
                            interval = ((PoolInfo)state).interval;

                        if (nowCounter != Counter)
                        {
                            Monitor.Enter(this);

                            int time = Environment.TickCount;
                            cachePool.Clear();
                            foreach (IObject obj in objectPool)
                                cachePool.AddLast(obj);
                            foreach (IObject obj in cachePool)
                                if (obj.OnUpdate(nowCounter) == Result.END)
                                    objectPool.Remove(obj);
                            time = Environment.TickCount - time;

                            Monitor.Exit(this);

                            nowCounter = Counter;

                            Thread.Sleep(Math.Abs(interval - time));
                        }
                        else Thread.Sleep(interval);
                    }
                };

                IsRunning = true;
            }

            public WaitCallback Callback
            {
                get
                {
                    return waitCallback;
                }
            }

            public void Dispose()
            {
                objectPool.Clear();
                cachePool.Clear();
                objectPool = null;
                cachePool = null;
            }

            public void Render(RenderUtil.IRenderer renderer)
            {
                Monitor.Enter(this);

                foreach (IObject obj in objectPool)
                    obj.OnRender(renderer);

                Monitor.Exit(this);
            }

            public void Add(IObject obj)
            {
                Monitor.Enter(this);
                objectPool.AddLast(obj);
                Monitor.Exit(this);
            }

            public void Add(IObject[] objs)
            {
                Monitor.Enter(this);
                foreach (IObject obj in objs)
                    objectPool.AddLast(obj);
                Monitor.Exit(this);
            }

            public void Clear()
            {
                Monitor.Enter(this);
                objectPool.Clear();
                Monitor.Exit(this);
            }

            public int Count
            {
                get
                {
                    try
                    {
                        Monitor.Enter(this);
                        return objectPool.Count;
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
                }
            }

            public void RemoveLast()
            {
                Monitor.Enter(this);
                objectPool.RemoveLast();
                Monitor.Exit(this);
            }

            public IObject GetLast()
            {
                try
                {
                    Monitor.Enter(this);
                    return objectPool.Last.Value;
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }

            public IEnumerable<IObject> Array
            {
                get
                {
                    return objectPool;
                }
            }
        }

        protected LinkedList<ObjectPool> poolCluster;
        public int PoolSize { get; protected set; }
        public int Capacity { get; protected set; }

        public int TickTime { get; protected set; }
        public bool Finished
        {
            get
            {
                bool state = true;
                foreach (ObjectPool i in poolCluster)
                    state &= i.Finished;
                return state;
            }
        }

        public ObjectPoolCluster(int poolSize, int capacity)
        {
            poolCluster = new LinkedList<ObjectPool>();
            PoolSize = poolSize;
            Capacity = capacity;
            
            ThreadPool.SetMaxThreads(capacity, capacity);
            AddPool();
        }

        public override string ToString()
        {
            string str = "";

            int worker = 0, io = 0;
            ThreadPool.GetAvailableThreads(out worker, out io);

            str += ("poolCluster info {\n");
            str += ("    ");
            str += ("pool count: ");
            str += (poolCluster.Count);
            str += ("\n");
            str += ("    ");
            str += ("worker threads: ");
            str += (worker);
            str += ("\n");
            str += ("    ");
            str += ("async I/O threads: ");
            str += (io);
            str += ("\n");
            str += ("    ");
            str += ("obj count: ");
            foreach (ObjectPool i in poolCluster)
            {
                str += (i.Count);
                str += ("    ");
            }
            str += ("\n");
            str += ("    ");
            str += ("obj sum: ");
            int sum = 0;
            foreach (ObjectPool i in poolCluster) sum += i.Count;
            str += (sum);
            str += ("\n}");

            return str;
        }

        private void AddPool()
        {
            ObjectPool pool = new ObjectPool();
            poolCluster.AddLast(pool);
            ThreadPool.QueueUserWorkItem(
                pool.Callback, new ObjectPool.PoolInfo() { interval = 10 }
            );
        }

        private void RemovePool(ObjectPool pool)
        {
            if (!poolCluster.Contains(pool)) return;
            pool.IsRunning = false;
            poolCluster.Remove(pool);
            pool.Dispose();
        }

        protected ObjectPool First
        {
            get
            {
                return poolCluster.First.Value;
            }
        }
        protected ObjectPool Last
        {
            get
            {
                return poolCluster.Last.Value;
            }
        }

        public void Add(IObject obj)
        {
            First.Add(obj);
        }

        public void Add(IObject[] objs)
        {
            First.Add(objs);
        }

        public void Balance()
        {
            if (First.Count > PoolSize && poolCluster.Count < Capacity)
            {
                AddPool();
            }
            else if (First.Count == 0 && poolCluster.Count > 1)
            {
                First.Clear();
                RemovePool(First);
            }
            else
            {
                int size = (Last.Count - First.Count) / 2;
                for (int i = 0; i < size; i++)
                {
                    First.Add(Last.GetLast());
                    Last.RemoveLast();
                }
            }
            DoSort();
        }

        protected void DoSort()
        {
            ObjectPool[] objects = new ObjectPool[poolCluster.Count];
            poolCluster.CopyTo(objects, 0);
            Array.Sort(objects, (a, b) => a.Count - b.Count);

            var it = poolCluster.First;
            foreach (ObjectPool e in objects)
            {
                it.Value = e;
                it = it.Next;
            }
        }

        public void Clear()
        {
            foreach (ObjectPool pool in poolCluster)
                pool.Clear();
        }

        public void Dispose()
        {
            Clear();
            foreach (ObjectPool pool in poolCluster)
            {
                pool.IsRunning = false;
                pool.Dispose();
            }
        }

        public void SetTick(int tick)
        {
            ObjectPool.Counter = tick;
        }

        public void Tick()
        {
            if (Finished)
            {
                SetTick(TickTime);
                if (TickTime % 60 == 0)
                    Balance();
                TickTime += 1;
            }
        }

        public void Render(RenderUtil.IRenderer renderer)
        {
            renderer.Begin();
            foreach (ObjectPool i in poolCluster)
                i.Render(renderer);
            renderer.End();
        }

        public IObject[] ToArray()
        {
            List<IObject> list = new List<IObject>();
            foreach (ObjectPool pool in poolCluster)
            {
                list.AddRange(pool.Array);
            }
            return list.ToArray();
        }
    }
}
