using System;
using System.Collections.Generic;

using dotNSGDX.Utility;

namespace dotNSGDX
{
    public class NSGDX
    {
        protected IRenderer Renderer { get; set; }

        protected ObjectPoolCluster poolCluster;
        
        protected int counter = 0;

        public NSGDX(IRenderer renderer, int poolSize = 16)
        {
            Renderer = renderer;

            poolCluster = new ObjectPoolCluster(poolSize, Environment.ProcessorCount);

            poolCluster.SetWork((pool) =>
            {
                LinkedList<IObject> cachePool = new LinkedList<IObject>(pool);
                foreach (IObject i in cachePool)
                {
                    if (i.OnUpdate(counter) == Result.END)
                        pool.Remove(i);
                }
                counter += 1;

                if (counter % 100 == 0) poolCluster.Balance();
            });
        }

        public void Add(IObject obj)
        {
            poolCluster.Add(obj);
        }

        public void Add(IObject[] objs)
        {
            poolCluster.Add(objs);
        }

        public void Clear()
        {
            poolCluster.Clear();
        }

        public IObject[] ToArray()
        {
            return poolCluster.ToArray();
        }

        public void Dispose()
        {
            poolCluster.Dispose();
        }

        public void OnRender()
        {
            Renderer.Begin();
            foreach (ObjectPoolCluster.ObjectPool pool in poolCluster.Cluster())
                foreach (IObject i in pool.Pool)
                    i.OnRender(Renderer);
            Renderer.End();
        }
    }
}
