using System;
using System.Collections.Generic;

using dotNSGDX.Utility;
using static dotNSGDX.Utility.RenderUtil;

namespace dotNSGDX.Entity
{
    public class Bullet
    {
        protected Vec2 pos;
        protected Vec2 vel;
        protected Vec2 acc;
        protected Vec2 aim;
        protected Vec2 dir;

        private IDrawable[] drawables;
        protected float scale;
        protected Color4 color;

        public bool grazed;
        protected LinkedList<IObject> targets;

        public Bullet(Vec2 pos, Vec2 aim, IDrawable drawable) : this(pos, aim, drawable, 1.0F)
        {
        }

        public Bullet(Vec2 pos, Vec2 aim, IDrawable drawable, float scale)
        {
            this.pos = new Vec2(pos); this.aim = new Vec2(aim);
            this.drawables = new IDrawable[] { drawable }; this.scale = scale;

            vel = (0, 0); acc = (0, 0);
            dir = (0, 0);
            color = (1.0F, 1.0F, 1.0F);

            grazed = false;
            targets = new LinkedList<IObject>();
        }

        public Bullet(Vec2 pos, Vec2 aim, IDrawable[] drawables, float scale)
        {
            this.pos = new Vec2(pos); this.aim = new Vec2(aim);
            this.drawables = drawables; this.scale = scale;

            vel = (0, 0); acc = (0, 0);
            dir = (0, 0);
            color = (1.0F, 1.0F, 1.0F);

            grazed = false;
            targets = new LinkedList<IObject>();
        }

        /*
        * TODO: 需检测玩家态, 玩家通常不可被销毁, 若销毁要先设置成销毁态, 使子弹注销玩家
        * */

        public Bullet Register(IObject target)
        {
            if (targets.Contains(target))
                targets.Remove(target);
            targets.AddLast(target);
            return this;
        }

        public Bullet DeRegister(IObject target)
        {
            if (targets.Contains(target))
                targets.Remove(target);
            return this;
        }

        public Bullet Vel(Vec2 vel) { this.vel = new Vec2(vel); return this; }

        public Bullet Acc(Vec2 acc) { this.acc = new Vec2(acc); return this; }

        public Bullet Color(Color4 color) { this.color = new Color4(color); return this; }

        public Result OnUpdate(int t)
        {
            vel += acc;
            pos += vel;
            dir = pos - aim;
            return Result.DONE;
        }

        public Result OnRender(IRenderer renderer)
        {
            foreach (IDrawable d in drawables)
                renderer.Draw(d, (float)pos.X, (float)pos.X, (float)dir.A, scale, color);
            return Result.DONE;
        }
    }
}
