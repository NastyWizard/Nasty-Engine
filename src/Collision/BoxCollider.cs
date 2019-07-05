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

        public override bool CheckOverlap(Vector2 offset, Collider other)
        {
            Rectangle tempBounds = m_bounds;
            tempBounds.Location += offset.ToPoint();

            return tempBounds.Intersects(other.m_bounds);
        }

        public override void DebugDraw()
        {
            base.DebugDraw();
            Draw.RectOutline(m_bounds, (m_others.Count == 0) ? Color.MonoGameOrange : Color.LimeGreen);
        }

        public override void OnRender()
        {
            if (CollisionManager.DebugRender)
                DebugDraw();
            base.OnRender();
        }

    }
}
