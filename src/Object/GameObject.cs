using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NastyEngine
{
    public class GameObject : SceneNode
    {
        public enum FacingDir { LEFT = -1, RIGHT = 1 }
        public Transform transform { get { return GetComponent<Transform>(); } }

        private List<Component> components = new List<Component>();
        public int NumComponents { get { return components.Count; } }

        public Vector2 StartPos;

        public FacingDir Facing = FacingDir.RIGHT;

        public int tileHeight; // current tile level for this object

        public Vector2 m_PreviousPos { get; private set; }

        public GameObject()
        {
            tileHeight = 0;
            var t = new Transform();
            AddComponent(t);
        }



        public Component AddComponent(Component comp)
        {
            comp.Parent = this;
            comp.Enabled = true;
            components.Add(comp);
            return comp;
        }

        public T GetComponent<T>()
        {

            foreach (var comp in components)
            {
                if (comp.GetType() == typeof(T))
                    return (T)Convert.ChangeType(comp, typeof(T));
            }

            return default(T);
        }

        public override void Init()
        {

            if (NumComponents > 0)
            {
                for (int i = 0; i < NumComponents; i++)
                {
                    if (components[i].Enabled)
                        components[i].OnInit();
                }
            }

            base.Init();
            StartPos = transform.LocalPosition;
        }

        public override void PreUpdate()
        {
            m_PreviousPos = transform.Position;
            if (NumComponents > 0)
            {
                for (int i = 0; i < NumComponents; i++)
                {
                    if (components[i].Enabled)
                        components[i].OnPreUpdate();
                }
            }
            base.PreUpdate();
        }

        public override void Update()
        {
            if (NumComponents > 0)
            {
                for (int i = 0; i < NumComponents; i++)
                {
                    if (components[i].Enabled)
                        components[i].OnUpdate();
                }
            }

            base.Update();
        }

        public override void PostUpdate()
        {
            if (NumComponents > 0)
            {
                for (int i = 0; i < NumComponents; i++)
                {
                    if (components[i].Enabled)
                        components[i].OnPostUpdate();
                }
            }

            base.PostUpdate();
        }
        public override void Render()
        {
            if (NumComponents > 0)
            {
                for (int i = 0; i < NumComponents; i++)
                {
                    if (components[i].Enabled)
                        components[i].OnRender();
                }
            }

            base.Render();
        }
    }
}
