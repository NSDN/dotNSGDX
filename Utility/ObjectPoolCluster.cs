using System;
using System.Threading;
using System.Collections.Generic;

namespace dotNSGDX.Utility
{
    public class ObjectPoolCluster
    {
        public delegate void Work(LinkedList<IObject> objectPool);

        public class ObjectPool
        {
            protected LinkedList<IObject> objectPool;
            protected TimerCallback callback;
            protected Timer timer;
            protected Work work;

            public int PoolSize { get; protected set; }
            public int Count
            {
                get
                {
                    return objectPool.Count;
                }
            }

            public LinkedList<IObject> Pool
            {
                get
                {
                    return objectPool;
                }
            }

            public ObjectPool(int poolSize) : this(poolSize, (obj) => { })
            {

            }

            public ObjectPool(int poolSize, Work work)
            {
                PoolSize = poolSize;
                callback = (obj) =>
                {
                    if (!(obj is LinkedList<IObject>)) return;
                    this.work.Invoke((LinkedList<IObject>)obj);
                };
                this.work = work;

                objectPool = new LinkedList<IObject>();
                timer = new Timer(callback, objectPool, 1000, 10);
            }

            public void Dispose()
            {
                timer.Dispose();
            }

            public void SetWork(Work work)
            {
                this.work = work;
            }  

            public void Clear()
            {
                objectPool.Clear();
            }

        }

        protected List<ObjectPool> poolCluster;
        protected Work work;

        public int PoolSize { get; protected set; }

        public int Capacity { get; protected set; }
        public int Count
        {
            get
            {
                return poolCluster.Count;
            }
        }
        public ObjectPool this[int index]
        {
            get
            {
                return poolCluster[index];
            }
        }

        protected ObjectPool FirstPool
        {
            get
            {
                return poolCluster[0];
            }
        }
        protected ObjectPool LastPool
        {
            get
            {
                return poolCluster[Count - 1];
            }
        }

        public ObjectPoolCluster(int poolSize, int capacity)
        {
            PoolSize = poolSize;
            Capacity = capacity;

            poolCluster = new List<ObjectPool>();

            work = (obj) => { };
            poolCluster.Add(new ObjectPool(PoolSize, work));
        }

        public List<ObjectPool> Cluster()
        {
            return poolCluster;
        }

        public void Dispose()
        {
            foreach (ObjectPool pool in poolCluster)
                pool.Dispose();
        }

        public void SetWork(Work work)
        {
            this.work = work;
        }

        public void Balance()
        {
            if (LastPool.Count > PoolSize)
            {
                if (FirstPool.Count > PoolSize && poolCluster.Count < Capacity)
                {
                    poolCluster.Add(new ObjectPool(PoolSize));
                }
                else if (FirstPool.Count == 0)
                {
                    FirstPool.Dispose();
                    poolCluster.Remove(FirstPool);
                }
                else
                {
                    for (int i = 1; i < poolCluster.Count; i++)
                    {
                        int countPerPool = poolCluster[i].Count / poolCluster.Count;
                        for (int c = 1; c < countPerPool; c++)
                        {
                            FirstPool.Pool.AddLast(poolCluster[i].Pool.Last);
                            poolCluster[i].Pool.RemoveLast();
                        }
                    }
                    poolCluster[0].Pool.AddLast(poolCluster[poolCluster.Count - 1].Pool.Last);
                    poolCluster[poolCluster.Count - 1].Pool.RemoveLast();
                }
            }
            poolCluster.Sort((a, b) => a.Count - b.Count);
        }

        public void Add(IObject obj)
        {
            poolCluster[0].Pool.AddLast(obj);
        }

        public void Add(IObject[] objs)
        {
            foreach (IObject obj in objs)
            {
                poolCluster[0].Pool.AddLast(obj);
            }
        }

        public void Clear()
        {
            foreach (ObjectPool pool in poolCluster)
                pool.Clear();
        }

        public IObject[] ToArray()
        {
            List<IObject> list = new List<IObject>();
            foreach (ObjectPool pool in poolCluster)
            {
                list.AddRange(pool.Pool);
            }
            return list.ToArray();
        }
    }
}
