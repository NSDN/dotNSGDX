using System;

namespace dotNSGDX.Utility
{
    public interface IRenderer
    {
        void Begin();
        void End();
        void Draw(Entity.Shape shape, float x, float y, float rotate);
    }
}
