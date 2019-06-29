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
    public class SFont
    {

        public int CharWidth, CharHeight;

        public Texture2D FontImage;

        public SFont(Texture2D fontImage, int charWidth, int charHeight)
        {
            FontImage = fontImage;
            CharWidth = charWidth;
            CharHeight = charHeight;
        }
    }
}
