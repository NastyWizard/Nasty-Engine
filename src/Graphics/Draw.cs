using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NastyEngine
{
    public class Draw
    {
        private static Texture2D pixel;

        private static SpriteBatch spriteBatch;
        public static SpriteBatch SpriteBatch
        {
            get
            {
                if (spriteBatch == null)
                {
                    init();
                }
                return spriteBatch;
            }
        }

        private static SFont defaultFont;
        public static SFont DefaultFont
        {
            get
            {
                if (defaultFont == null)
                {
                    defaultFont = new SFont(ResourceManager.GetTexture("FontFilled"), 8, 8);
                }
                return defaultFont;
            }
        }

        private static void init()
        {
            spriteBatch = new SpriteBatch(Engine.Instance_.GraphicsDevice);
            pixel = new Texture2D(Engine.Instance_.GraphicsDevice, 1, 1);
            var colors = new Color[1];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.White;
            pixel.SetData<Color>(colors);
        }

        public static void Begin(Effect e = null)
        {
            if (!Engine.PixelPerfect)
                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, effect: e, transformMatrix: Engine.ScreenMatrix * SceneManager.GetCurrentScene().camera.Matrix);
            else
                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, effect: e, transformMatrix: Engine.ScreenMatrix * SceneManager.GetCurrentScene().camera.Matrix);
        }

        public static void End()
        {
            SpriteBatch.End();
            Engine.Instance_.GraphicsDevice.SetRenderTarget(null);
            Engine.Instance_.GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            Rect(Vector2.Zero, Engine.ViewWidth, Engine.ViewHeight, Color.Red);
            SpriteBatch.Draw(Engine.MainImage, Vector2.Zero, Color.White); // TODO LOOK INTO RESOLUTION BEING GREATER THAN GAMESIZE
            SpriteBatch.End();
        }

        #region SHAPES
        public static void Dot(Vector2 position, Color? color = null)
        {
            if (color == null)
                color = Color.Black;
            spriteBatch.Draw(pixel, position, (Color)color);
        }

        public static void Line(Vector2 start, Vector2 end, Color? color = null)
        {
            Vector2 dir = end - start;
            float len = dir.Length();
            LineAngle(start, (float)Math.Atan2(dir.Y, dir.X), len, color);
        }

        public static void Line(float x, float y, float x1, float y1, Color? color = null)
        {
            Vector2 dir = new Vector2(x1, y1) - new Vector2(x, y);
            float len = dir.Length();
            LineAngle(new Vector2(x, y), (float)Math.Atan2(dir.Y, dir.X), len, color);
        }

        public static void LineAngle(Vector2 start, float angle, float length, Color? color = null)
        {
            if (color == null)
                color = Color.Black;
            SpriteBatch.Draw(pixel, start, null, (Color)color, angle, new Vector2(0.0f,0.5f), new Vector2(length, 1), SpriteEffects.None, 0);
        }

        public static void Rect(Vector2 start, float width, float height, Color? color = null)
        {
            if (color == null)
                color = Color.Black;
            Rectangle rect = new Rectangle((int)start.X, (int)start.Y, (int)width, (int)height);
            SpriteBatch.Draw(pixel, rect, (Color)color);
        }

        public static void Rect(float x, float y, float x1, float y1, Color? color = null)
        {
            float width = x1 - x;
            float height = y1 - y;
            float xx = width < 0 ? x1 : x;
            float yy = height < 0 ? y1 : y;
            width = Math.Abs(width);
            height = Math.Abs(height);

            Rect(new Vector2(xx, yy), width, height, color);
        }

        public static void Rect(Rectangle rect, Color? color = null)
        {
            Rect(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, color);
        }

        public static void Rect(Vector2 start, Vector2 end, Color? color = null)
        {
            Rect(start.X, start.Y, end.X, end.Y, color);
        }

        public static void RectOutline(Vector2 start, Vector2 end, Color? color = null)
        {
            // top 
            Line(start.X, start.Y, end.X, start.Y, color);
            // bottom
            Line(start.X, end.Y, end.X, end.Y, color);
            // left
            Line(start.X, start.Y, start.X, end.Y, color);
            // right
            Line(end.X, start.Y, end.X, end.Y, color);

        }

        public static void RectOutline(Vector2 start, float width, float height, Color? color = null)
        {
            RectOutline(start, new Vector2(start.X + width, start.Y + height), color);
        }

        public static void RectOutline(Rectangle rect, Color? color = null)
        {
            RectOutline(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, color);
        }

        public static void Circle(Vector2 position, float radius, Color? color = null)
        {
            SpriteBatch.End();

            Begin(ResourceManager.GetEffect("Circle"));
            Rect(position - Vector2.One * radius, radius * 2, radius * 2, color);
            SpriteBatch.End();

            Begin();
        }

        public static void CircleOutline(Vector2 position, float radius, Color? color = null)
        {
            SpriteBatch.End();

            Begin(ResourceManager.GetEffect("CircleOutline"));
            Rect(position - Vector2.One * radius, radius * 2, radius * 2, color);
            SpriteBatch.End();

            Begin();
        }
        #endregion

        #region TEXT

        public static void Text(string text, float x, float y, Vector2 scale)
        {
            SFont font = DefaultFont;
            int i = 0;
            foreach (var ch in text)
            {
                char A = 'A';
                char a = 'a';
                char zero = '0';

                char questionMark = '?';
                char exclamationMark = '!';
                char period = '.';
                char plus = '+';
                char minus = '-';
                char space = ' ';

                int character = 0;
                Vector2 pos = new Vector2(x + i * font.CharWidth, y);
                int lower = 0;

                if (char.IsDigit(ch))
                {
                    character = (25+(ch - zero)) * font.CharWidth;
                }
                else if (char.IsLower(ch))
                {
                    lower = 1;
                    character = (ch - a) * font.CharWidth;
                }
                else if (char.IsUpper(ch))
                {
                    character = (ch - A) * font.CharWidth;
                }
                

                if (ch.Equals(questionMark))
                {
                    character = 37 * font.CharWidth;
                }
                else if (ch.Equals(exclamationMark))
                {
                    character = 38 * font.CharWidth;
                }
                else if (ch.Equals(period))
                {
                    character = 36 * font.CharWidth;
                }
                else if (ch.Equals(plus))
                {
                    character = 40 * font.CharWidth;
                }
                else if (ch.Equals(minus))
                {
                    character = 39 * font.CharWidth;
                }
                else if (ch.Equals(space))
                {
                    lower = 1;
                    character = 40 * font.CharWidth;
                }

                SpriteBatch.Draw(font.FontImage, new Rectangle((int)pos.X +  (i * (int)scale.X * font.CharWidth)/3, (int)pos.Y, (int)scale.X * font.CharWidth, (int)scale.Y * font.CharHeight), new Rectangle(character, lower * font.CharHeight, font.CharWidth, font.CharWidth), Color.White);

                i++;
            }
        }


        public static void Text(string text, float x, float y)
        {
            Text(text, x, y, Vector2.One);
        }

        public static void Text(string text, Vector2 pos, Vector2 scale)
        {
            Text(text, pos.X, pos.Y, scale);
        }

        public static void Text(string text, Vector2 pos)
        {
            Text(text, pos.X, pos.Y, Vector2.One);
        }

        #endregion

    }
}
