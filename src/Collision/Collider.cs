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
        protected bool m_isStatic;
        private SpriteRenderer m_spriteRenderer;
        public Rectangle m_bounds;

        internal List<Collider> m_others;

        public Collider(Rectangle bounds)
        {
            m_others = new List<Collider>();
            m_bounds = bounds;
            CollisionManager.Register(this);
            m_isStatic = false;
        }
        public Collider(Rectangle bounds, bool isStatic)
        {
            m_others = new List<Collider>();
            m_bounds = bounds;
            CollisionManager.Register(this);
            m_isStatic = isStatic;
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
            if (m_isStatic)
                return;

            if (m_others.Count > 0)
            {
                for(int i = 0; i < m_others.Count; i++)
                    OnOverlapStay(m_others[i]);
            }


            if (m_spriteRenderer != null)
                m_bounds.Location = (Parent.transform.Position - m_spriteRenderer.Offset).ToPoint();
            else
                m_bounds.Location = Parent.transform.Position.ToPoint();

            base.OnUpdate();
        }

        public void ForcePositionUpdate()
        {
            if (m_spriteRenderer != null)
                m_bounds.Location = (Parent.transform.Position - m_spriteRenderer.Offset).ToPoint();
            else
                m_bounds.Location = Parent.transform.Position.ToPoint();
        }

        public abstract bool CheckOverlap(Collider other);
        public abstract bool CheckOverlap(Vector2 offset, Collider other);

        public delegate void OverlapDelegate(Collider c);

        public OverlapDelegate OverlapEnter;
        public OverlapDelegate OverlapExit;
        public OverlapDelegate OverlapStay;

        internal virtual void OnOverlapEnter(Collider other)
        {
            m_others.Add(other);
            if(OverlapEnter != null)
                OverlapEnter(other);
        }

        internal virtual void OnOverlapExit(Collider other)
        {
            m_others.Remove(other);
            if (OverlapExit != null)
                OverlapExit(other);
        }

        internal virtual void OnOverlapStay(Collider other)
        {
            if (OverlapStay != null)
                OverlapStay(other);
        }
        public virtual void DebugDraw(){}

        public bool IsStatic() { return m_isStatic; }
        public void SetStatic(bool b) { m_isStatic = b; }
    }
}
