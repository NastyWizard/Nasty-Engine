using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NastyEngine
{
    public class Scene : SceneNode
    {

        public Camera camera = new Camera();
        public Color clearColor = Color.Black;
        //private RenderTarget2D outlineTarget;

        protected Texture2D backgroundTex;
        protected Color outlineColor = Color.Black;

        public virtual void RenderBegin()
        {
            //if (outlineTarget == null)
            //    outlineTarget = new RenderTarget2D(gd, gameWidth / ratio, gameHeight / ratio, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.Depth16);
            //// Draw to render target
            //gd.SetRenderTarget(outlineTarget);
            //gd.Clear(Color.Transparent);
            //
            //g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            //ResourceManager.GetEffect("Default").CurrentTechnique.Passes[0].Apply();
            Draw.Begin();
        }

        public override void Render()
        {
            //RenderBegin(time, g, gd, gameX, gameY, gameWidth, gameHeight, ratio);

            //gd.SetRenderTarget(null);
            //g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            //ResourceManager.GetEffect("Default").CurrentTechnique.Passes[0].Apply();
            base.Render();
            //g.End();

            //RenderEnd(time, g, gd, gameX, gameY, gameWidth, gameHeight, ratio);
        }

        public virtual void RenderEnd()
        {
            // Draw render target with outline effect

            //gd.SetRenderTarget(null);
            //gd.Clear(Color.Black);
            //
            //g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            //ResourceManager.GetEffect("OutlinePP").Parameters["_ScreenWidth"].SetValue((float)48 * 8 * 2);
            //ResourceManager.GetEffect("OutlinePP").Parameters["_ScreenHeight"].SetValue((float)27 * 8 * 2);
            //if (backgroundTex != null)
            //    ResourceManager.GetEffect("OutlinePP").Parameters["_Background"].SetValue(backgroundTex);
            //ResourceManager.GetEffect("OutlinePP").Parameters["_OutlineColor"].SetValue(outlineColor.ToVector4());
            //ResourceManager.GetEffect("OutlinePP").CurrentTechnique.Passes[0].Apply();
            //
            //g.Draw(outlineTarget, new Rectangle(gameX, gameY, gameWidth, gameHeight), Color.White);
            Draw.End();
        }

    }
}
