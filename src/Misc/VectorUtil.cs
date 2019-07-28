using Microsoft.Xna.Framework;
using System;
namespace NastyEngine
{
    public class VectorUtil
    {

        public static Vector2 Round(Vector2 v)
        {
            return new Vector2((float)Math.Round(v.X), (float)Math.Round(v.Y));
        }
        public static Vector2 Floor(Vector2 v)
        {
            return new Vector2((float)Math.Floor(v.X), (float)Math.Floor(v.Y));
        }
        public static Vector2 Ceil(Vector2 v)
        {
            return new Vector2((float)Math.Ceiling(v.X), (float)Math.Ceiling(v.Y));
        }

    }
}
