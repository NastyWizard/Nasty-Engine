using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;

namespace NastyEngine.Input
{
    public class InputMapping
    {
        private Dictionary<InputManager.ActionTypes, Keys> KeyboardMap;
        private Dictionary<InputManager.ActionTypes, Buttons> ControllerMap;

        private static InputMapping instance_;

        public InputMapping()
        {
            if (instance_ == null)
            {
                KeyboardMap = new Dictionary<InputManager.ActionTypes, Keys>();
                ControllerMap = new Dictionary<InputManager.ActionTypes, Buttons>();

                instance_ = this;
                InputMapping.SetDefaultMapping();
            }
        }

        public static void ChangeKeyMapping(InputManager.ActionTypes type, Keys key)
        {
            instance_.KeyboardMap[type] = key;
        }

        public static void ChangeButtonMapping(InputManager.ActionTypes type, Buttons button)
        {
            instance_.ControllerMap[type] = button;
        }

        public static Buttons GetMappedButton(InputManager.ActionTypes type)
        {
            return instance_.ControllerMap[type];
        }

        public static Keys GetMappedKey(InputManager.ActionTypes type)
        {
            return instance_.KeyboardMap[type];
        }

        public static void LoadMappingFromFile() // TODO: this )^:< 
        {

        }

        public static void SetDefaultMapping()
        {
            instance_.KeyboardMap[InputManager.ActionTypes.LEFT] =    Keys.A;
            instance_.KeyboardMap[InputManager.ActionTypes.RIGHT] =   Keys.D;
            instance_.KeyboardMap[InputManager.ActionTypes.UP] =      Keys.W;
            instance_.KeyboardMap[InputManager.ActionTypes.DOWN] =    Keys.S;
            instance_.KeyboardMap[InputManager.ActionTypes.JUMP] =    Keys.Z;
            instance_.KeyboardMap[InputManager.ActionTypes.ATTACK] =  Keys.X;


            instance_.ControllerMap[InputManager.ActionTypes.LEFT] =  Buttons.DPadLeft;
            instance_.ControllerMap[InputManager.ActionTypes.RIGHT] = Buttons.DPadRight;
            instance_.ControllerMap[InputManager.ActionTypes.UP] =    Buttons.DPadUp;
            instance_.ControllerMap[InputManager.ActionTypes.DOWN] =  Buttons.DPadDown;
            instance_.ControllerMap[InputManager.ActionTypes.JUMP] =  Buttons.A;
            instance_.ControllerMap[InputManager.ActionTypes.ATTACK]= Buttons.B;
        }

    }
}
