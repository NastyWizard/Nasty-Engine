using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NastyEngine
{
    public class Transform : Component
    {
        public Vector2 LocalPosition = Vector2.Zero;

        public Vector2 Position
        {
            get
            {
                if (Parent.Parent != null)
                {
                    if (!Parent.Parent.GetType().IsSubclassOf(typeof(GameObject)))
                        return LocalPosition;
                    return LocalPosition + (((GameObject)Parent.Parent) == null ? Vector2.Zero : ((GameObject)Parent.Parent).transform.LocalPosition);
                }
                return LocalPosition;
            }

            set
            {
                if (Parent.Parent is GameObject)
                    LocalPosition = value - ((GameObject)Parent.Parent).transform.LocalPosition;
                else
                    LocalPosition = value;
            }
        }

        public float rotation = 0;

        public Vector2 scale = Vector2.One;
    }
}
