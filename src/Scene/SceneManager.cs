using System;
using Microsoft.Xna.Framework;

namespace NastyEngine
{
    public class SceneManager
    {
        public Scene CurrentScene { get; private set; }

        public static SceneManager Instance_;

        public SceneManager(Scene startScene)
        {
            Instance_ = this;
            Instance_.CurrentScene = startScene;
        }

        public static void SetScene(Scene scene)
        {
            Instance_.CurrentScene = scene;
            Instance_.CurrentScene.Init();
        }

        public static void InitCurrentScene()
        {
            if (!GetCurrentScene().HasInit)
                GetCurrentScene().Init();
        }

        public static void UpdateCurrentScene()
        {
            if (GetCurrentScene().Enabled)
            {
                GetCurrentScene().PreUpdate();
                GetCurrentScene().Update();
                GetCurrentScene().PostUpdate();
            }
        }

        public static Scene GetCurrentScene()
        {
            return Instance_.CurrentScene;
        }

        public static void ResetLevel()
        {
            Type type = Instance_.CurrentScene.GetType();

            Instance_.CurrentScene = null;
            Instance_.CurrentScene = (Scene)type.Assembly.CreateInstance(type.FullName);
            Instance_.CurrentScene.Init();
        }
    }
}
