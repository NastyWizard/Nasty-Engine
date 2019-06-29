using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace NastyEngine
{

    public class SpriteRenderer : Component
    {
        public Texture2D Sprite;
        // used to cut from a sprite sheet
        public Rectangle? SourceRect = null;
        // offset for the sprites origin
        public Vector2 Offset = Vector2.Zero;

        // reference to parent transform
        private Transform pTransform;
        private Shader shader;

        public SpriteRenderer(Texture2D img, Shader shdr = null)
        {
            shader = shdr;
            Sprite = img;
        }

        public SpriteRenderer(string img, Shader shdr = null)
        {
            shader = shdr;
            Sprite = ResourceManager.GetTexture(img);
        }

        public override void OnInit()
        {
            base.OnInit();

            pTransform = Parent.GetComponent<Transform>();
        }

        public override void OnRender()
        {
            base.OnRender();
            if (Sprite != null)
            {
                // check if using custom shader
                if (shader != null)
                {
                    Draw.SpriteBatch.End();
                    Draw.Begin();
                    shader.ApplyShader();
                }

                if (pTransform == null)
                    pTransform = Parent.GetComponent<Transform>();
                // draw the sprite
                Draw.SpriteBatch.Draw(Sprite, pTransform.Position, SourceRect, Color.White, MathHelper.ToRadians(pTransform.rotation), Offset, pTransform.scale, SpriteEffects.None, 1);

                if (shader != null)
                {
                    Draw.SpriteBatch.End();
                    Draw.Begin();
                    ResourceManager.GetEffect("Default").CurrentTechnique.Passes[0].Apply();
                }
            }
        }

    }
}
