using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NastyEngine.Input
{
    public class InputManager
    {
        public enum ActionTypes { NONE = 0, LEFT = 1, RIGHT = 2, UP = 4, DOWN = 8, JUMP = 16, ATTACK = 32 }

        private static PlayerIndex keyboardPlayerIndex = 0;

        private static Dictionary<PlayerIndex, GamePadState> oldGState;
        private static KeyboardState oldKState;

        private static MouseInput MouseInput;
        private static InputMapping mapInstance_;
        private static KeyInput keyInput;

        public static Vector2 MousePosition { get { return new Vector2(Mouse.GetState().X / Engine.ScreenRatio, Mouse.GetState().Y / Engine.ScreenRatio) - (Engine.ScreenIndent / Engine.ScreenRatio); } }


        public InputManager()
        {
            MouseInput = new MouseInput();
            keyInput = new KeyInput();
            mapInstance_ = new InputMapping();
            oldGState = new Dictionary<PlayerIndex, GamePadState>();
            oldGState.Add(PlayerIndex.One, GamePad.GetState(0));
            oldGState.Add(PlayerIndex.Two, GamePad.GetState(1));
            oldGState.Add(PlayerIndex.Three, GamePad.GetState(2));
            oldGState.Add(PlayerIndex.Four, GamePad.GetState(3));
        }

        public void Update()
        {
            MouseInput.Update();
            keyInput.Update();
        }

        public static bool GetActionDown(PlayerIndex player, ActionTypes action)
        {
            GamePadState currentGState = GamePad.GetState(player);
            KeyboardState currentKState = Keyboard.GetState();

            bool n = currentGState.IsButtonDown(InputMapping.GetMappedButton(action)) ||
                (currentKState.IsKeyDown(InputMapping.GetMappedKey(action)) && player == keyboardPlayerIndex);

            bool o = oldGState[player].IsButtonDown(InputMapping.GetMappedButton(action)) ||
                            (oldKState.IsKeyDown(InputMapping.GetMappedKey(action)) && player == keyboardPlayerIndex);



            oldGState[player] = currentGState;
            oldKState = currentKState;
            return n && !o;
        }

        public static bool GetActionUp(PlayerIndex player, ActionTypes action)
        {
            GamePadState currentGState = GamePad.GetState(player);
            KeyboardState currentKState = Keyboard.GetState();

            bool n = currentGState.IsButtonUp(InputMapping.GetMappedButton(action)) ||
                (currentKState.IsKeyUp(InputMapping.GetMappedKey(action)) && player == keyboardPlayerIndex);

            bool o = oldGState[player].IsButtonDown(InputMapping.GetMappedButton(action)) ||
                            (oldKState.IsKeyDown(InputMapping.GetMappedKey(action)) && player == keyboardPlayerIndex);



            oldGState[player] = currentGState;
            oldKState = currentKState;
            return n && o;
        }

        public static bool GetMouseButton()
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed;
        }

        public static bool GetActionHeld(PlayerIndex player, ActionTypes action)
        {

            //if (GetActionDown(player, action))
            //    pressDic[player] |= action;
            //
            bool b = GamePad.GetState(player).IsButtonDown(InputMapping.GetMappedButton(action)) || (Keyboard.GetState().IsKeyDown(InputMapping.GetMappedKey(action)) && player == keyboardPlayerIndex);//(pressDic[player] & action) > 0;
            //
            //if (GetActionUp(player, action))
            //    pressDic[player] &= ~action;

            return b;

        }
    }

    public class MouseInput
    {
        public bool Enabled = true;
        public MouseState PreviousMouseState;
        public MouseState CurrentMouseState;

        public static Vector2 Position
        {
            get
            {
                return (new Vector2(Instance_.CurrentMouseState.X, Instance_.CurrentMouseState.Y));
            }
        }

        public static Vector2 WorldPosition
        {
            get
            {
                //Vector2 pos = Engine.PixelPerfect ? Vector2.Transform(Position, SceneManager.GetCurrentScene().camera.InverseMatrix) : Position + (SceneManager.GetCurrentScene().camera.Position) - new Vector2(Engine.Viewport.X, Engine.Viewport.Y);
                //return Vector2.Transform(pos, Matrix.Invert(Engine.ScreenMatrix));
                return GetWorldPos(Instance_.CurrentMouseState);
            }
        }

        public static Vector2 DeltaWorldPosition
        {
            get
            {
                Vector2 cPos = WorldPosition;
                Vector2 pPos = GetWorldPos(Instance_.PreviousMouseState);
                return cPos - pPos;
            }
        }

        public static Vector2 DeltaPosition
        {
            get
            {
                Vector2 cPos = Position;
                Vector2 pPos = new Vector2(Instance_.PreviousMouseState.X, Instance_.PreviousMouseState.Y);
                return cPos - pPos;
            }
        }

        private static Vector2 GetWorldPos(MouseState ms)
        {
            Vector2 pos = new Vector2(ms.X, ms.Y) / SceneManager.GetCurrentScene().camera.Zoom;
            pos = pos + (SceneManager.GetCurrentScene().camera.Position) - new Vector2(Engine.Viewport.X, Engine.Viewport.Y);
            return Vector2.Transform(pos, Matrix.Invert(Engine.ScreenMatrix));
        }

        public static float ScrollDelta
        {
            get
            {
                return (Instance_.CurrentMouseState.ScrollWheelValue - Instance_.PreviousMouseState.ScrollWheelValue) / 120.0f;
            }
        }

        private static MouseInput Instance_;

        internal MouseInput()
        {
            if (Instance_ != null)
                return;

            Instance_ = this;
        }

        internal void Update()
        {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public enum MouseButton { LEFT, RIGHT, MIDDLE }

        public static bool Check(MouseButton mb = MouseButton.LEFT)
        {
            if (!Instance_.Enabled)
                return false;
            ButtonState state;
            switch (mb)
            {
                case MouseButton.RIGHT:
                    state = Instance_.CurrentMouseState.RightButton;
                    break;
                case MouseButton.MIDDLE:
                    state = Instance_.CurrentMouseState.MiddleButton;
                    break;
                case MouseButton.LEFT:
                default:
                    state = Instance_.CurrentMouseState.LeftButton;
                    break;

            }

            return state == ButtonState.Pressed;
        }

        public static bool Pressed(MouseButton mb = MouseButton.LEFT)
        {
            if (!Instance_.Enabled)
                return false;

            ButtonState state;
            ButtonState pState;
            switch (mb)
            {
                case MouseButton.RIGHT:
                    state = Instance_.CurrentMouseState.RightButton;
                    pState = Instance_.PreviousMouseState.RightButton;
                    break;
                case MouseButton.MIDDLE:
                    state = Instance_.CurrentMouseState.MiddleButton;
                    pState = Instance_.PreviousMouseState.MiddleButton;
                    break;
                case MouseButton.LEFT:
                default:
                    state = Instance_.CurrentMouseState.LeftButton;
                    pState = Instance_.PreviousMouseState.LeftButton;
                    break;

            }

            return state == ButtonState.Pressed && pState != ButtonState.Pressed;
        }

        public static bool Released(MouseButton mb = MouseButton.LEFT)
        {
            if (!Instance_.Enabled)
                return false;

            ButtonState state;
            ButtonState pState;

            switch (mb)
            {
                case MouseButton.RIGHT:
                    state = Instance_.CurrentMouseState.RightButton;
                    pState = Instance_.PreviousMouseState.RightButton;
                    break;
                case MouseButton.MIDDLE:
                    state = Instance_.CurrentMouseState.MiddleButton;
                    pState = Instance_.PreviousMouseState.MiddleButton;
                    break;
                case MouseButton.LEFT:
                default:
                    state = Instance_.CurrentMouseState.LeftButton;
                    pState = Instance_.PreviousMouseState.LeftButton;
                    break;

            }

            return state != ButtonState.Pressed && pState == ButtonState.Pressed;
        }
    }

    public class KeyInput
    {
        public bool Enabled = true;
        public KeyboardState PreviousKeyState;
        public KeyboardState CurrentKeyState;

        public static char InputChar;

        private static KeyInput Instance_;

        internal KeyInput()
        {
            if (Instance_ != null)
                return;

            Instance_ = this;
        }

        internal void Update()
        {
            PreviousKeyState = CurrentKeyState;
            CurrentKeyState = Keyboard.GetState();
        }


        public static bool Check(Keys key)
        {
            if (!Instance_.Enabled)
                return false;

            return Instance_.CurrentKeyState.IsKeyDown(key);
        }

        // multi-key check
        public static bool Check(params Keys[] keys)
        {
            bool b = true;
            foreach (var k in keys)
            {
                if (!Check(k))
                    b = false;
            }

            return b;
        }


        public static bool Pressed(Keys key)
        {
            if (!Instance_.Enabled)
                return false;

            return Instance_.CurrentKeyState.IsKeyDown(key) && !Instance_.PreviousKeyState.IsKeyDown(key);
        }

        public static bool Released(Keys key)
        {
            if (!Instance_.Enabled)
                return false;

            return !Instance_.CurrentKeyState.IsKeyDown(key) && Instance_.PreviousKeyState.IsKeyDown(key);
        }

        public static bool AnyKeyCheck()
        {
            if (!Instance_.Enabled)
                return false;

            return Instance_.CurrentKeyState.GetPressedKeys().Count() > 0;
        }

        public static bool AnyKeyPressed()
        {
            if (!Instance_.Enabled)
                return false;

            return Instance_.CurrentKeyState.GetPressedKeys().Count() > Instance_.PreviousKeyState.GetPressedKeys().Count();
        }

        public static bool AnyKeyReleased()
        {
            if (!Instance_.Enabled)
                return false;

            return Instance_.CurrentKeyState.GetPressedKeys().Count() < Instance_.PreviousKeyState.GetPressedKeys().Count();
        }

    }

}
