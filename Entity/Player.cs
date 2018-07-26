using System;

using dotNSGDX.Utility;
using static dotNSGDX.Utility.RenderUtil;

namespace dotNSGDX.Entity
{
    public class Player
    {
        protected Vec2 pos;
        protected Vec2 dir;
        protected Vec2 eye;
        
        private IDrawable[] tex;
        protected float scale;

        public Player(IDrawable tex)
        {
            this.tex = new IDrawable[] { tex };
            pos = (0, 0);
            dir = (0, 0);
            eye = (0, 0);
            scale = 1.0F;
        }

        public Player(IDrawable[] tex)
        {
            this.tex = tex;
            pos = (0, 0);
            dir = (0, 0);
            eye = (0, 0);
            scale = 1.0F;
        }

        public void Flash(Vec2 vec) => pos = new Vec2(vec);

        public void Move(Vec2 vec) => pos += vec;

        public void Look(Vec2 vec) => eye = new Vec2(vec);

        public Result OnUpdate(int t)
        {
            dir = pos - eye;
            return Result.DONE;
        }

        public Result OnRender(IRenderer renderer)
        {
            foreach (IDrawable d in tex)
                renderer.Draw(d, (float)pos.X, (float)pos.Y, (float)dir.A, scale, (1.0F, 1.0F, 1.0F));
            return Result.DONE;
        }
    }
}
