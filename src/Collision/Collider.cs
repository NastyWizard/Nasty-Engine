using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NastyEngine
{
    public abstract class Collider : Component
    {
        /// <summary>
        /// A collider that will never move
        /// </summary>
        private bool m_isStatic;
        private SpriteRenderer m_spriteRenderer;
        public Rectangle m_bounds;

        internal Collider m_other = null;

        public Collider(Rectangle bounds)
        {
            m_bounds = bounds;
            CollisionManager.Register(this);
            m_isStatic = false;
        }

        public override void OnInit()
        {
            m_spriteRenderer = Parent.GetComponent<SpriteRenderer>();

            if (m_spriteRenderer != null)
                m_bounds.Location = (Parent.transform.Position - m_spriteRenderer.Offset).ToPoint();
            else
                m_bounds.Location = Parent.transform.Position.ToPoint();
            base.OnInit();
        }

        public override void OnUpdate()
        {
            if (m_other != null)
                OnOverlapStay(m_other);
            if (m_isStatic)
                return;

            if (m_spriteRenderer != null)
                m_bounds.Location = (Parent.transform.Position - m_spriteRenderer.Offset).ToPoint();
            else
                m_bounds.Location = Parent.transform.Position.ToPoint();

            base.OnUpdate();
        }

        public Collider(Rectangle bounds, bool isStatic)
        {
            m_bounds = bounds;
            CollisionManager.Register(this);
            m_isStatic = isStatic;
        }

        public abstract bool CheckOverlap(Collider other);

        public virtual void OnOverlapEnter(Collider other)
        {
            m_other = other;
        }

        public virtual void OnOverlapExit(Collider other)
        {
            if(m_other == other)
                m_other = null;
        }

        public virtual void OnOverlapStay(Collider other) { }
        public virtual void DebugDraw(){}
    }
}
