using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NastyEngine
{
    public class Component
    {
        public GameObject Parent;

        private bool isEnabled = true;

        public bool Enabled
        {
            get { return isEnabled; }
            set { if (isEnabled = value) OnEnable(); }
        }

        private bool hasInit = false;

        public virtual void OnEnable()
        {
            //if (!hasInit) OnInit();
        }

        public virtual void OnInit() { hasInit = true; }
        public virtual void OnUpdate() { }
        public virtual void OnPreUpdate() { }
        public virtual void OnPostUpdate() { }
        public virtual void OnRender() { }
    }
}
