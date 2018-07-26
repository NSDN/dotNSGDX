using System;

using dotNSGDX.Utility;

namespace dotNSGDX.Entity
{
    public abstract class Executor : IObject
    {
        public abstract Result OnUpdate(int t);

        public Result OnRender(RenderUtil.IRenderer renderer)
        {
            return Result.DONE;
        }
    }
}
