using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace NastyEngine
{
    public class Animation
    {
        public int CurrentFrame { get; private set; }

        public Rectangle DrawRect { get; private set; }
        private List<Rectangle> frames = new List<Rectangle>();

        /// <summary>
        /// frames per second
        /// </summary>
        public float AnimSpeed = 0.1f;

        public void MakeAnim(int tileWidth, int tileHeight, int row, int numColumns, int columnIndent = 0, bool reverse = false)
        {
            for (int x = 0; x < numColumns; x++)
            {
                frames.Add(new Rectangle((columnIndent * tileWidth) + x * tileWidth, row * tileHeight, tileWidth, tileHeight));
            }
            if (reverse)
                frames.Reverse();
            CurrentFrame = 0;
            DrawRect = frames[CurrentFrame];
        }

        public void AddFrame(Rectangle frame)
        {
            frames.Add(frame);
        }

        public void SetFrame(int frame)
        {
            CurrentFrame = frame;
            CurrentFrame %= frames.Count;

            DrawRect = frames[CurrentFrame];
        }

        public Rectangle GetFrameRect(int frame)
        {
            return frames[frame];
        }
    }
}
