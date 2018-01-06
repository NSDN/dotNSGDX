using System;

namespace dotNSGDX.Entity
{
    public abstract class Executor : Utility.IObject
    {
        public abstract Utility.Result OnUpdate(int t);

        public Utility.Result OnRender(Utility.IRenderer renderer)
        {
            return Utility.Result.DONE;
        }
    }
}
