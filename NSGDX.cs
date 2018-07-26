using System;

using dotNSGDX.Utility;

namespace dotNSGDX
{
    public abstract class NSGDX
    {
        protected RenderUtil.IRenderer Renderer { get; set; }

        protected ObjectPoolCluster poolCluster;

        public NSGDX(RenderUtil.IRenderer renderer, int poolSize = 2048)
        {
            Renderer = renderer;

            poolCluster = new ObjectPoolCluster(poolSize, Environment.ProcessorCount);
        }

        public void Dispose()
        {
            poolCluster.Dispose();
        }

        public virtual void Setup()
        {
            
        }

        public virtual void Loop()
        {
            poolCluster.Tick();
            poolCluster.Render(Renderer);
        }
    }
}
