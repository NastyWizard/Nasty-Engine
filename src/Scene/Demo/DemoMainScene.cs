using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NastyEngine
{
    public class DemoMainScene : Scene
    {
        private SpriteFont font;
        private string text;
        private bool toggle = false;
        public DemoMainScene()
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

            //TODO: fill out these scenes
            if (IMGUI.Button("IMGUI", 140))
            {
                toggle = !toggle;
            }
            IMGUI.Separator();

            if (IMGUI.Button("COLLISIONS", 140))
            { }

            IMGUI.Separator();

            if (IMGUI.Button("INPUT", 140))
            { }

            IMGUI.Separator();

            if (IMGUI.Button("TILEMAP", 140))
            { }

            IMGUI.Separator();


            IMGUI.EndWindow();



            if (toggle)
            {

                IMGUI.BeginWindow("IMGUI", Engine.GameWidth / 2 - 70 + 180, Engine.GameHeight / 2);
                if (IMGUI.Button("BUTTON", 140))
                { }
                IMGUI.TextField("TEXT-FIELD", ref text);
                IMGUI.EndWindow();

            }


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
