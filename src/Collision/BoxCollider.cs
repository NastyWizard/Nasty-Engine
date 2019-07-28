using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NastyEngine
{
    public class BoxCollider : Collider
    {

        private RectangleF m_rbounds;

        public BoxCollider(RectangleF bounds) : base()
        {
            m_rbounds = bounds;
            m_bounds = m_rbounds.ToRectangle();
        }
        public BoxCollider(RectangleF bounds, bool isStatic) : base(isStatic)
        {
            m_rbounds = bounds;
            m_bounds = m_rbounds.ToRectangle();
        }
        public override bool CheckOverlap(Collider other)
        {

            if (other.GetType() == typeof(BoxCollider))
            {
                var o = (BoxCollider)other;

                return m_rbounds.Overlaps(o.m_rbounds);
            }

            throw new Exception("ERROR: Unexpected collider type.");
        }

        public override bool CheckOverlap(Vector2 offset, Collider other)
        {
            if (other.GetType() == typeof(BoxCollider))
            {
                var o = (BoxCollider)other;
                RectangleF tempBounds = m_rbounds;
                tempBounds.X += offset.X;
                tempBounds.Y += offset.Y;

                return tempBounds.Overlaps(o.m_rbounds);
            }

            throw new Exception("ERROR: Unexpected collider type.");
        }
        public override void DebugDraw()
        {
            base.DebugDraw();
            Draw.RectOutline(m_bounds, (m_others.Count == 0) ? Color.MonoGameOrange : Color.LimeGreen);
        }

        public override void ForcePositionUpdate()
        {
            if (m_spriteRenderer != null)
                m_rbounds.Position = Parent.transform.Position - m_spriteRenderer.Offset;
            else
                m_rbounds.Position = Parent.transform.Position;

            m_bounds = m_rbounds.ToRectangle();
    }

        public override void OnRender()
        {
            if (CollisionManager.DebugRender)
                DebugDraw();
            base.OnRender();
        }

    }
}
