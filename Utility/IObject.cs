using System;

namespace dotNSGDX.Utility
{
    public enum Result { DONE, END }

    public interface IObject
    {
        Result OnUpdate(int t);
        Result OnRender(IRenderer renderer);
    }
}
