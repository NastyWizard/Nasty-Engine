using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NastyEngine
{
    public class Button : GameObject
    {
        public string Text;
        public Texture2D Image;
        private Color color;
        public Color IdleColor;
        public Color OutlineColor;
        public Color HighlightColor;
        public Color SelectColor;

        private bool selected = false;

        public Rectangle bounds;

        private int width, height;

        private SpriteFont font;

        public Button(Vector2 position, int width = 128, int height = 64, SpriteFont font = null, string text = "")
        {
            transform.Position = position;
            Text = text;

            this.width = width;
            this.height = height;
            this.font = font;

            bounds = new Rectangle((int)position.X, (int)position.Y, 128, 64);


            IdleColor = new Color(45, 45, 48);
            OutlineColor = new Color(59, 59, 65);
            HighlightColor = new Color(45 + 5, 45 + 5, 48 + 5);
            SelectColor = Color.MonoGameOrange;
        }

        public Button(Vector2 position, int width = 128, int height = 64, Texture2D image = null)
        {
            transform.Position = position;
            Image = image;

            this.width = width;
            this.height = height;

            bounds = new Rectangle((int)position.X, (int)position.Y, 128, 64);

            IdleColor = new Color(45, 45, 48);
            OutlineColor = new Color(59, 59, 65);
            HighlightColor = new Color(45 + 5, 45 + 5, 48 + 5);
            SelectColor = Color.MonoGameOrange;
        }

        public bool Check()
        {
            if (bounds.Contains(Input.MouseInput.Position.X, Input.MouseInput.Position.Y))
            {
                if (Input.MouseInput.Released())
                {
                    if(selected)
                        return true;
                }
            }
            return false; 
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Render()
        {

            Draw.Rect(transform.Position, width, height, color);
            Draw.RectOutline(transform.Position, width, height, OutlineColor);


            if (Text != "" && Text != null)
            {
                Vector2 indent = new Vector2((width / 2) - (Draw.DefaultFont.CharWidth * Text.Length), 0);
                if (indent.X < 0)
                    indent = Vector2.Zero;
                Draw.Text(Text, transform.Position + indent, new Vector2(2, 2));
            }

            if (Image != null)
                Draw.SpriteBatch.Draw(Image, new Rectangle((int)transform.Position.X, (int)transform.Position.Y, width, height), Color.White);

            //if (font != null)
            //    Draw.SpriteBatch.DrawString(font, Text, transform.Position, Color.White);
            base.Render();
        }

        public override void Update()
        {
            bounds = new Rectangle((int)transform.Position.X, (int)transform.Position.Y, width, height);

            if (bounds.Contains(Input.MouseInput.Position.X, Input.MouseInput.Position.Y))
            {
                if (Input.MouseInput.Pressed())
                    selected = true;

                if (Input.MouseInput.Check() && selected)
                    color = SelectColor;
                else
                    color = HighlightColor;
            }
            else
            {
                color = IdleColor;
            }

            base.Update();
        }
    }
}
