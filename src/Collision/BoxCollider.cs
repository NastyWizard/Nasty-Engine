using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NastyEngine
{
    public class BoxCollider : Collider
    {
        public BoxCollider(Rectangle bounds) : base(bounds)
        {

        }
        public BoxCollider(Rectangle bounds, bool isStatic) : base(bounds,isStatic)
        {

        }

        public override bool CheckOverlap(Collider other)
        {
            return m_bounds.Intersects(other.m_bounds);
        }

        public override void DebugDraw()
        {
            base.DebugDraw();
            Draw.RectOutline(m_bounds, Color.LawnGreen);
        }
    }
}
