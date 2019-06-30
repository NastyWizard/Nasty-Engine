using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NastyEngine
{
    public class DemoScene : Scene
    {
        private SpriteFont font;
        private string text;
        public DemoScene()
        {
        }

        public override void Init()
        {
            font = ResourceManager.Content.Load<SpriteFont>("Fonts/TestFont");

            base.Init();
        }

        public override void Update()
        {
            IMGUI.Begin(font);

            IMGUI.BeginWindow("NASTY-ENGINE", Engine.GameWidth / 2 - 70, Engine.GameHeight / 2);
            if (IMGUI.Button("Button", 140))
            { }
            IMGUI.Separator();
            IMGUI.TextField("Text Field", ref text,140);
            IMGUI.Label("Label");
            IMGUI.EndWindow();

            IMGUI.End();
            base.Update();
        }

        public override void RenderEnd()
        {
            base.RenderEnd();
            IMGUI.Render();
        }

    }
}
