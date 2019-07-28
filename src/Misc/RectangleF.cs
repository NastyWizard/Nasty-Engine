using System;

using Microsoft.Xna.Framework;

namespace NastyEngine
{
    public class RectangleF
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Vector2 Position
        {
            get
            {
                return new Vector2(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public RectangleF(float x, float y, float w, float h)
        {
            Init(x, y, w, h);
        }

        public RectangleF(Vector2 p, float w, float h)
        {
            Init(p.X, p.Y, w, h);
        }
        public RectangleF(Rectangle r)
        {
            Init(r.X, r.Y, r.Width, r.Height);
        }

        private void Init(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public bool Overlaps(RectangleF other)
        {
            return X + Width >= other.X &&
                X <= other.X + other.Width &&
                Y + Height >= other.Y &&
                Y <= other.Y + other.Height;
        }

        public Rectangle ToRectangle() { return new Rectangle((int)X, (int)Y, (int)Width, (int)Height); }

    }
}
