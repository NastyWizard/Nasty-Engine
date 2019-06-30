using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NastyEngine.Input;

namespace NastyEngine
{
    public class IMGUI
    {

        #region DRAWABLE TYPES
        private abstract class GUIDrawable
        {
            public uint id;
            public Rectangle bounds;
            public Rectangle sourceBounds;

            public Texture2D backgroundTexture;
            public Color color;
            public IMGUIDrawableFlags flags;

            public GUIWindow window;

            public bool hasFocus;
            public bool destroy;

            public abstract void Render();

        }

        private class GUILabel : GUIDrawable
        {
            public Texture2D backgroundTexture;
            public string text;
            public Vector2 textPos;

            public GUILabel()
            {
                backgroundTexture = Pixel;
            }

            public override void Render()
            {
                if ((flags & IMGUIDrawableFlags.DECORATED) > 0)
                {
                    Draw.SpriteBatch.Draw(backgroundTexture, bounds, sourceBounds, color);
                    Draw.RectOutline(bounds, outlineColor);
                }
                Draw.SpriteBatch.DrawString(font, text, textPos, textColor);
            }
        }

        private class GUISeparator : GUIDrawable
        {
            public override void Render()
            {
                bounds.Width = window.bounds.Width - (int)gContext.style.indent.X * 2;
                Draw.Rect(bounds, gContext.style.outlineColor);
            }
        }

        private class GUITextButton : GUIDrawable
        {
            public string text;
            public Vector2 textPos;
            public bool active, hasFocus, pressed;

            public GUITextButton()
            {
                backgroundTexture = Pixel;
            }

            public override void Render()
            {
                if ((flags & IMGUIDrawableFlags.MATCH_WINDOW_WIDTH) > 0)
                    bounds.Width = window.bounds.Width - 2 * (int)gContext.style.indent.X;

                Vector2 textSize = font.MeasureString(text);
                textPos = new Vector2(bounds.X + (bounds.Width - (int)textSize.X) / 2, bounds.Y + 3);

                if ((flags & IMGUIDrawableFlags.DISABLED) > 0)
                {
                    color = gContext.style.clearColor;
                    Draw.SpriteBatch.Draw(backgroundTexture, bounds, sourceBounds, color);
                    Draw.RectOutline(bounds, gContext.style.outlineColor);
                    Draw.SpriteBatch.DrawString(font, text, textPos, gContext.style.menuColor);
                }
                else if ((flags & IMGUIDrawableFlags.DECORATED) > 0)
                {
                    if (active && pressed) color = gContext.style.pressedColor;
                    else if (hasFocus) color = gContext.style.selectedColor;
                    else color = gContext.style.menuColor;

                    if ((flags & IMGUIDrawableFlags.FORCE_ACTIVE) > 0) color = gContext.style.pressedColor;

                    Draw.SpriteBatch.Draw(backgroundTexture, bounds, sourceBounds, color);
                    Draw.RectOutline(bounds, outlineColor);
                    Draw.SpriteBatch.DrawString(font, text, textPos, gContext.style.textColor);
                }
                else
                {
                    if (active && pressed) color = gContext.style.selectedColor;
                    else if (hasFocus) color = gContext.style.menuColor;
                    else color = gContext.style.clearColor;

                    if ((flags & IMGUIDrawableFlags.FORCE_ACTIVE) > 0) color = gContext.style.pressedColor;
                    Draw.SpriteBatch.Draw(backgroundTexture, bounds, sourceBounds, color);
                    Draw.SpriteBatch.DrawString(font, text, textPos, gContext.style.textColor);
                }

            }
        }

        private class GUITextureButton : GUIDrawable
        {
            public Texture2D texture;
            public Rectangle imageBounds;
            public bool active, hasFocus, pressed;

            public GUITextureButton()
            {
                backgroundTexture = Pixel;
            }

            public override void Render()
            {
                if ((flags & IMGUIDrawableFlags.DISABLED) > 0)
                {
                    color = gContext.style.clearColor;
                    Draw.SpriteBatch.Draw(backgroundTexture, bounds, sourceBounds, color);
                    Draw.RectOutline(bounds, gContext.style.outlineColor);
                    Draw.SpriteBatch.Draw(texture, imageBounds, Color.White);
                }
                else if ((flags & IMGUIDrawableFlags.DECORATED) > 0)
                {
                    if (active && pressed) color = gContext.style.pressedColor;
                    else if (hasFocus) color = gContext.style.selectedColor;
                    else color = gContext.style.menuColor;

                    if ((flags & IMGUIDrawableFlags.FORCE_ACTIVE) > 0) color = gContext.style.pressedColor;

                    Draw.SpriteBatch.Draw(backgroundTexture, bounds, sourceBounds, color);
                    Draw.RectOutline(bounds, outlineColor);
                    Draw.SpriteBatch.Draw(texture, imageBounds, Color.White);
                }
                else
                {
                    if (active && pressed) color = gContext.style.selectedColor;
                    else if (hasFocus) color = gContext.style.menuColor;
                    else color = gContext.style.clearColor;

                    if ((flags & IMGUIDrawableFlags.FORCE_ACTIVE) > 0) color = gContext.style.pressedColor;
                    Draw.SpriteBatch.Draw(backgroundTexture, bounds, sourceBounds, color);
                    Draw.SpriteBatch.Draw(texture, imageBounds, Color.White);
                }

            }
        }

        private class GUISpriteSheetButton : GUIDrawable
        {
            public Texture2D texture;
            public string text;
            public Vector2 textPos;
            public int active = -1, hasFocus = -1, pressed = -1;
            public int gridX, gridY;

            public GUISpriteSheetButton()
            {
                backgroundTexture = ResourceManager.GetTexture("Checker");
            }

            public override void Render()
            {
                // draw the tiled bg
                Draw.SpriteBatch.End();
                Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap);

                Draw.SpriteBatch.Draw(backgroundTexture, bounds, new Rectangle(0, 0, bounds.Width, bounds.Height), Color.White);

                Draw.SpriteBatch.End();
                Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
                Draw.RectOutline(bounds, gContext.style.outlineColor);
                //
                int maxIndex = (bounds.Width / gridX) * (bounds.Height+1 / gridY);

                if (active >= 0 && active < maxIndex)
                {
                    // draw the active tile
                    Color col = gContext.style.menuColor;
                    col.A /= 2;
                    Draw.Rect(new Rectangle(bounds.X + ((active * gridX) % bounds.Width), bounds.Y + ((active * gridX) / bounds.Width) * gridY, gridX, gridY), col);
                }

                if (hasFocus >= 0 && hasFocus < maxIndex)
                {
                    // draw the selected tile
                    Color col = gContext.style.selectedColor;
                    col.A /= 2;
                    Draw.Rect(new Rectangle(bounds.X + ((hasFocus * gridX) % bounds.Width), bounds.Y + ((hasFocus * gridX) / bounds.Width) * gridY, gridX, gridY), col);
                }

                if (pressed >= 0 && pressed < maxIndex)
                {
                    // draw the pressed tile
                    Color col = gContext.style.pressedColor;
                    col.A /= 2;
                    Draw.Rect(new Rectangle(bounds.X + ((pressed * gridX) % bounds.Width), bounds.Y + ((pressed * gridX) / bounds.Width) * gridY, gridX, gridY), col);
                }

                // draw the actual sprite sheet
                Draw.SpriteBatch.Draw(texture, bounds, Color.White);

                // draw the separators
                if ((flags & IMGUIDrawableFlags.DECORATED) > 0)
                {
                    //x 
                    for (int x = 0; x < bounds.Width; x += gridX)
                    {
                        Draw.Line(new Vector2(bounds.X + x, bounds.Y), new Vector2(bounds.X + x, bounds.Y + bounds.Height), gContext.style.outlineColor);
                    }

                    //y 
                    for (int y = 0; y < bounds.Height; y += gridX)
                    {
                        Draw.Line(new Vector2(bounds.X, bounds.Y + y), new Vector2(bounds.X + bounds.Width, bounds.Y + y), gContext.style.outlineColor);
                    }
                }
            }
        }

        private class GUITextField : GUIDrawable
        {
            public string label;
            public string text;
            public Vector2 labelPos;
            public Rectangle fieldBounds;

            public GUITextField()
            {
                backgroundTexture = Pixel;
            }

            public override void Render()
            {

                //Vector2 labelSize = font.MeasureString(label);

                //Rectangle fieldBounds = bounds;
                //fieldBounds.Y += (int)labelSize.Y;
                //fieldBounds.Height -= (int)labelSize.Y;

                Draw.SpriteBatch.DrawString(font, label, labelPos, textColor);

                Draw.SpriteBatch.Draw(backgroundTexture, fieldBounds, sourceBounds, color);
                Draw.RectOutline(fieldBounds, outlineColor);
                Draw.SpriteBatch.DrawString(font, text, new Vector2(fieldBounds.X + 3, fieldBounds.Y + 4), textColor);
            }
        }

        private enum IMGUITypes { BUTTON, TEXTFIELD, LABEL, MENU, MENUITEM };

        #endregion

        private static GUIContext gContext = null;
        private class GUIContext
        {
            public List<GUIWindow> dwindows; // sorted by display order
            public Dictionary<uint, GUIWindow> windows; // windows stored with id for easy access 

            public GUIWindow currentWindow;

            public GUIStyle style;



            public GUIContext()
            {
                style = new GUIStyle("");
                dwindows = new List<GUIWindow>();
                windows = new Dictionary<uint, GUIWindow>();
            }
        }


        #region TEXTURES
        private static Texture2D pixel;
        public static Texture2D Pixel
        {
            get
            {
                if (pixel == null)
                {
                    pixel = new Texture2D(Engine.Instance_.GraphicsDevice, 1, 1);
                    var colors = new Color[1];
                    for (int i = 0; i < colors.Length; i++)
                        colors[i] = Color.White;
                    pixel.SetData<Color>(colors);
                }
                return pixel;
            }
        }
        #endregion

        #region MENU_DATA
        public enum IMGUIWindowFlags { IMGUIWINDOW_CANMOVE };
        private class GUIWindow
        {
            public string title;
            public uint id; // hash of title
            public List<GUIDrawable> drawables;

            public Rectangle bounds;
            public Rectangle prevBounds;
            public Rectangle fullBounds;

            public IMGUIWindowFlags windowFlags;

            public GUIMenuBar guiMenuBar;

            public bool dragging;

            public int x, y;

            public bool destroy;

            public void Render()
            {
                // draw menu title bar
                Vector2 titleMeasure = font.MeasureString(title);
                Rectangle titleBounds = new Rectangle(bounds.X, bounds.Y - (int)titleMeasure.Y, Math.Max((int)titleMeasure.X, bounds.Width), (int)titleMeasure.Y);
                Draw.Rect(titleBounds, BGColor);
                Draw.RectOutline(titleBounds, outlineColor);
                Draw.SpriteBatch.DrawString(font, title, new Vector2(titleBounds.X + ((titleBounds.Width - indent.X * 2) - titleMeasure.X) / 2, titleBounds.Y + 1), textColor);
                // draw menu
                Draw.Rect(bounds, clearColor);
                Draw.RectOutline(bounds, outlineColor);

                // draw elements
                for (int i = 0; i < drawables.Count; i++)
                {
                    if (drawables[i].destroy)
                    {
                        drawables.RemoveAt(i);
                        continue;
                    }
                    drawables[i].Render();
                    drawables[i].destroy = true;
                }

                prevBounds = bounds;
            }

            public GUIWindow(string name)
            {
                title = name;
                id = HashStr(name);
                destroy = false;
                drawables = new List<GUIDrawable>();
                bounds = Rectangle.Empty;
                prevBounds = bounds;
            }
        }

        private static GUIMenuBar guiMenuBar;
        private class GUIMenuBar
        {
            // TODO: change this to a list
            public Dictionary<uint, GUIMenu> menus;
            public Rectangle bounds;

            public int currentWidth;

            public GUIMenuBar()
            {
                menus = new Dictionary<uint, GUIMenu>();
            }
        }

        private class GUIMenu
        {
            public uint id;
            public bool active;
            public string title;
            public Rectangle bounds;
            public Color bgColor;

            public Rectangle itemBounds;

            public Dictionary<uint, GUIMenuItem> items;

            public GUIMenu(string key)
            {
                id = HashStr(key);
                items = new Dictionary<uint, GUIMenuItem>();
            }

            public void Render()
            {
                Draw.Rect(bounds, bgColor);
                Draw.RectOutline(bounds, gContext.style.menuColor);
                Draw.SpriteBatch.DrawString(font, title, new Vector2(bounds.X + 3, bounds.Y + 3), gContext.style.textColor);
                if (!active) return;

                Draw.Rect(itemBounds, gContext.style.BGColor);
                Draw.RectOutline(itemBounds, gContext.style.menuColor);

                foreach (GUIMenuItem item in items.Values)
                {
                    item.Render();
                }
            }

        }

        private class GUIMenuItem
        {
            public uint id;
            public string title;
            public bool hasFocus;
            public Color bgColor;
            public Rectangle bounds;
            public Rectangle bgBounds;

            public GUIMenuItem(string title)
            {
                id = HashStr(title);
                bgColor = gContext.style.BGColor;
                hasFocus = false;
            }

            public void Render()
            {
                Draw.Rect(bgBounds, bgColor);
                Draw.SpriteBatch.DrawString(font, title, new Vector2(bounds.X, bounds.Y), gContext.style.textColor);
            }
        }

        // style stuff

        private static SpriteFont font;
        private static Vector2 indent;

        private struct GUIStyle
        {
            public string name;
            public Vector2 indent;

            public Color BGColor;
            public Color clearColor;
            public Color outlineColor;
            public Color menuColor;
            public Color selectedColor;
            public Color pressedColor;

            public Color textColor;

            public GUIStyle(string name = "")
            {
                this.name = name;

                indent = new Vector2(5, 5);
                BGColor = new Color(16, 16, 16);
                clearColor = new Color(32, 32, 32);
                outlineColor = new Color(77, 77, 77);
                menuColor = new Color(45, 45, 48);
                selectedColor = new Color(65, 65, 69);
                pressedColor = Color.MonoGameOrange;

                textColor = Color.White;
            }
        }
        #endregion

        #region COLOR
        private static Color BGColor = new Color(16, 16, 16);
        private static Color clearColor = new Color(32, 32, 32);
        private static Color outlineColor = new Color(77, 77, 77);
        private static Color menuColor = new Color(45, 45, 48);
        private static Color selectedColor = new Color(65, 65, 69);
        private static Color pressedColor = Color.MonoGameOrange;

        private static Color textColor = Color.White;
        #endregion

        #region MISC
        private static string currentDragMenu;
        #endregion

        #region TEXT_MANIPULATION
        private static float inputLineTimer;
        private static float? inputTimer;
        #endregion

        //--------------------------------------------------------------------------//
        //--------------------------------------------------------------------------//
        //--------------------------------------------------------------------------//

        #region INIT
        public static void Begin(SpriteFont font)
        {
            if (gContext == null)
                gContext = new GUIContext();

            inputLineTimer += GTime.Delta;
            inputLineTimer = inputLineTimer % 6000.0f;

            IMGUI.font = font;
        }


        // The real meat and potatoes of this, does all the drawing and dragging
        public static void End()
        {

            // menu focus
            if (Input.MouseInput.Pressed())
            {
                for (int k = gContext.dwindows.Count - 1; k >= 0; k--)
                {
                    GUIWindow window = gContext.dwindows[k];
                    Vector2 titleMeasure = font.MeasureString(window.title);
                    Rectangle titleBounds = new Rectangle(window.bounds.X, window.bounds.Y - (int)titleMeasure.Y, Math.Max((int)titleMeasure.X, window.bounds.Width), (int)titleMeasure.Y);

                    if (titleBounds.Contains(Input.MouseInput.Position.X, Input.MouseInput.Position.Y) && currentDragMenu == null)
                    {
                        currentDragMenu = window.title;
                        window.dragging = true;
                    }

                    if (window.fullBounds.Contains(Input.MouseInput.Position.X, Input.MouseInput.Position.Y))
                    {
                        gContext.dwindows.Remove(window);
                        gContext.dwindows.Add(window);
                        break;
                    }
                }
            }

            // menu dragging
            if (Input.MouseInput.Check())
            {
                for (int k = gContext.dwindows.Count - 1; k >= 0; k--)
                {
                    GUIWindow window = gContext.dwindows[k];

                    if (window.title == currentDragMenu)
                    {
                        window.x += (int)Input.MouseInput.DeltaPosition.X;
                        window.y += (int)Input.MouseInput.DeltaPosition.Y;
                        break;
                    }
                }
            }

            if (Input.MouseInput.Released())
            {
                for (int k = gContext.dwindows.Count - 1; k >= 0; k--)
                {
                    GUIWindow window = gContext.dwindows[k];
                    if (window.title == currentDragMenu)
                    {
                        // bounds checking, if out of bounds bring back in bounds
                        if (window.x < 0) window.x = 0;
                        if (guiMenuBar != null)
                        {
                            if (window.y < font.MeasureString(" ").Y * 2 + 3) window.y = (int)font.MeasureString(" ").Y * 2 + 3;
                        }
                        else if (window.y < 0) window.y = 0;

                        if (window.x > Engine.ScreenWidth - 10) window.x = Engine.ScreenWidth - 10;
                        if (window.y > Engine.ScreenHeight - 10) window.y = Engine.ScreenHeight - 10;
                        //

                        currentDragMenu = null;
                        window.dragging = false;
                        break;
                    }
                }
            }
        }

        public static void Render()
        {
            Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            // draw all windows, ordered by creation then focus (last one created on top, or first one focused on top)
            for (int k = 0; k < gContext.dwindows.Count; k++)
            {
                GUIWindow window = gContext.dwindows[k];

                // remove unused windows
                if (window.destroy)
                {
                    DeleteWindow(window);
                    continue;
                }
                window.Render();

                window.destroy = true;
            }

            // draw the menu bar if it exists
            if (guiMenuBar != null)
            {
                Draw.Rect(guiMenuBar.bounds, gContext.style.BGColor);
                Draw.RectOutline(guiMenuBar.bounds, gContext.style.menuColor);

                foreach (var menu in guiMenuBar.menus.Values)
                {
                    menu.Render();
                }

            }

            Draw.SpriteBatch.End();
        }

        #endregion

        #region WINDOWS

        // THIS IS THE IMPORTANT ONE ----------------------------------------
        public static void BeginWindow(string title, int posX = 0, int posY = 0, IMGUIWindowFlags flags = IMGUIWindowFlags.IMGUIWINDOW_CANMOVE)
        {
            GUIWindow window = FindWindowByTitle(title);

            if (window == null)
            {
                window = CreateWindow(title);
                window.x = posX;
                window.y = posY;
            }
            gContext.currentWindow = window;

            window.destroy = false;
            window.windowFlags = flags;
            window.bounds = new Rectangle(window.x, window.y, 5 * 2, 5 * 2);
            indent = new Vector2(5, 5);
        }

        public static void BeginWindow(string title, Vector2 pos)
        {
            int xx = (int)pos.X, yy = (int)pos.Y;

            BeginWindow(title, xx, yy);
        }

        public static void EndWindow()
        {
            Vector2 titleMeasure = font.MeasureString(gContext.currentWindow.title);
            gContext.currentWindow.fullBounds = gContext.currentWindow.bounds;
            gContext.currentWindow.fullBounds.Y -= (int)titleMeasure.Y;
            gContext.currentWindow.fullBounds.Height += (int)titleMeasure.Y;

            gContext.currentWindow = null;
        }

        private static GUIWindow FindWindowByTitle(string title)
        {
            return FindWindowByID(HashStr(title));
        }

        private static GUIWindow FindWindowByID(uint id)
        {
            if (gContext.windows.ContainsKey(id))
                return gContext.windows[id];
            else
                return null;
        }

        private static GUIWindow CreateWindow(string title)
        {
            GUIWindow window = new GUIWindow(title);
            gContext.dwindows.Add(window);
            gContext.windows.Add(HashStr(title), window);
            return window;
        }

        private static void DeleteWindow(GUIWindow window)
        {
            gContext.dwindows.Remove(window);
            gContext.windows.Remove(window.id);
            if (gContext.currentWindow == window) gContext.currentWindow = null;
        }
        #endregion

        #region MENUS

        public static void BeginMenuBar()
        {
            //TODO: Menu bar shit

            if (gContext.currentWindow != null)
            {
                // add a menu bar to the current window
            }
            else
            {
                if (guiMenuBar == null)
                    guiMenuBar = new GUIMenuBar();
                guiMenuBar.bounds = new Rectangle(0, 0, Engine.ScreenWidth + 1, (int)font.MeasureString(" ").Y + 3);
            }
        }

        public static void EndMenuBar()
        {
            //TODO: Menu bar shit
            guiMenuBar.currentWidth = 0;
        }

        private static GUIMenu currentMenu;
        public static bool BeginMenu(string title)
        {
            string key = IMGUITypes.MENU + title;
            GUIMenu menu = FindGUIMenuByTitle(key);
            title = "  " + title + "  ";
            Vector2 titleSize = font.MeasureString(title);
            Rectangle titleBounds = new Rectangle(guiMenuBar.currentWidth, 0, (int)titleSize.X + 6, guiMenuBar.bounds.Height);

            if (menu == null)
            {
                menu = new GUIMenu(key)
                {
                    title = title,
                    itemBounds = new Rectangle(titleBounds.X + 6, guiMenuBar.bounds.Y + guiMenuBar.bounds.Height, 0, 0)
                };
            }

            if (currentMenu == null)
            {
                currentMenu = menu;
            }
            else
            {
                throw new Exception("ERROR: currentMenu already exists in IMGUI, did you forget to call EndMenu()?");
            }

            guiMenuBar.currentWidth += (int)titleSize.X + 6;

            menu.bounds = titleBounds;

            menu.bgColor = gContext.style.BGColor;
            if (titleBounds.Contains(MouseInput.Position))
            {
                menu.bgColor = gContext.style.menuColor;
                if (MouseInput.Check())
                {
                    menu.bgColor = gContext.style.pressedColor;
                    DisableAllMenus();
                    menu.active = true;
                }
            }
            else if (!menu.itemBounds.Contains(MouseInput.Position))
            {
                menu.active = false;
            }

            // TODO: this will have to change eventually if I ever add menubars to windows.


            if (!guiMenuBar.menus.ContainsKey(HashStr(key)))
            {
                guiMenuBar.menus.Add(HashStr(key), menu);
            }

            return menu.active;
        }

        public static void EndMenu()
        {
            currentMenu = null;
        }

        public static bool MenuItem(string title)
        {
            string key = IMGUITypes.MENUITEM + title;
            title = "     " + title;
            GUIMenuItem mItem = FindGUIMenuItemByTitle(key);
            Vector2 titleSize = font.MeasureString(title);
            if (mItem == null)
            {
                mItem = new GUIMenuItem(key)
                {
                    title = title,
                    bounds = new Rectangle(currentMenu.itemBounds.X, currentMenu.itemBounds.Y + currentMenu.itemBounds.Height, (int)titleSize.X, (int)titleSize.Y)
                };
            }

            mItem.bgBounds = mItem.bounds;

            UpdateMenuItemBounds(mItem.bounds);
            mItem.bgBounds.Width = currentMenu.itemBounds.Width - 1;

            //
            if (!currentMenu.items.ContainsKey(HashStr(key)))
            {
                currentMenu.items.Add(HashStr(key), mItem);
            }

            // Logic
            mItem.bgColor = gContext.style.BGColor;
            if (mItem.bgBounds.Contains(MouseInput.Position))
            {
                mItem.bgColor = gContext.style.menuColor;
                if (MouseInput.Pressed())
                {
                    mItem.bgColor = gContext.style.pressedColor;
                    DisableAllMenus();
                    return true;
                }
            }


            return false;
        }

        #region HELPERS

        private static void UpdateMenuItemBounds(Rectangle compareBounds)
        {
            currentMenu.itemBounds.Width = Math.Max(currentMenu.itemBounds.Width, compareBounds.Width + (int)indent.X * 2);
            currentMenu.itemBounds.Height = Math.Max(currentMenu.itemBounds.Height, (compareBounds.Y - currentMenu.itemBounds.Y) + compareBounds.Height + (int)indent.Y);
        }

        private static void DisableAllMenus()
        {
            foreach (uint id in guiMenuBar.menus.Keys)
            {
                guiMenuBar.menus[id].active = false;
            }
        }

        private static GUIMenuItem FindGUIMenuItemByTitle(string title)
        {
            return FindGUIMenuItemByID(HashStr(title));
        }

        private static GUIMenuItem FindGUIMenuItemByID(uint id)
        {
            if (currentMenu.items.ContainsKey(id))
                return currentMenu.items[id];
            else
                return null;
        }

        private static GUIMenu FindGUIMenuByTitle(string title)
        {
            return FindGUIMenuByID(HashStr(title));
        }

        private static GUIMenu FindGUIMenuByID(uint id)
        {
            if (guiMenuBar.menus.ContainsKey(id))
                return guiMenuBar.menus[id];
            else
                return null;
        }

        private static GUIDrawable FindGUIDrawableByKey(GUIWindow window, string title)
        {
            return FindGUIDrawableByID(window, HashStr(title));
        }

        private static GUIDrawable FindGUIDrawableByID(GUIWindow window, uint id)
        {
            return window.drawables.FirstOrDefault(value => value.id == id);
        }

        private static void CreateDrawableInWindow(GUIWindow window, GUIDrawable drawable)
        {
            window.drawables.Add(drawable);
        }
        #endregion
        #endregion

        #region GUI DRAWABLE CALLS

        #region SEPARATOR

        public static void Separator()
        {
            GUIWindow window = gContext.currentWindow;
            GUISeparator drawableSeparator = new GUISeparator();
            drawableSeparator.window = window;
            window.drawables.Add(drawableSeparator);

            drawableSeparator.bounds = window.bounds;
            drawableSeparator.bounds.Y += window.bounds.Height;
            drawableSeparator.bounds.Height = 1;
            drawableSeparator.bounds.X += (int)gContext.style.indent.X;
            drawableSeparator.bounds.Width -= (int)gContext.style.indent.X * 2;
            UpdateMenuBounds(drawableSeparator.bounds);
        }

        #endregion

        #region LABEL
        public enum IMGUILabelFlags { NONE = 0, CHOP_FRONT = 1 << 1 }
        public static void Label(string text, int maxSize = -1, IMGUILabelFlags labelFlags = IMGUILabelFlags.NONE, IMGUIDrawableFlags flags = IMGUIDrawableFlags.NONE)
        {
            Label(IMGUITypes.LABEL + text, text, maxSize, labelFlags, flags);
        }

        public static void Label(string key, string text, int maxSize = -1, IMGUILabelFlags labelFlags = IMGUILabelFlags.NONE, IMGUIDrawableFlags flags = IMGUIDrawableFlags.NONE)
        {
            GUIWindow window = gContext.currentWindow;

            //important stuff
            GUILabel drawableText = (GUILabel)FindGUIDrawableByKey(window, key);
            if (drawableText == null)
            {
                drawableText = new GUILabel();
                CreateDrawableInWindow(window, drawableText);
            }
            drawableText.destroy = false;
            drawableText.id = HashStr(key);

            Vector2 textSize = font.MeasureString(text);
            // this is probably horrible for bigger strings
            if (maxSize > 0)
            {
                bool didRemove = false;
                while (textSize.X > maxSize)
                {
                    if ((labelFlags & IMGUILabelFlags.CHOP_FRONT) > 0)
                        text = text.Remove(0, 1);
                    else
                        text = text.Remove(text.Length - 1, 1);
                    textSize = font.MeasureString(text);
                    didRemove = true;
                }

                if (didRemove)
                {

                    if ((labelFlags & IMGUILabelFlags.CHOP_FRONT) > 0)
                    {
                        text = text.Remove(0, 3);
                        text = text.Insert(0, "...");
                    }
                    else
                    {
                        text = text.Remove(text.Length - 4, 3);
                        text += "...";
                    }

                    textSize = font.MeasureString(text);
                }

            }

            // data
            drawableText.text = text;
            Rectangle textBounds = new Rectangle(window.bounds.X + (int)indent.X, window.bounds.Y + window.bounds.Height, (int)textSize.X, (int)textSize.Y);
            drawableText.bounds = textBounds;
            drawableText.textPos = new Vector2(textBounds.X + 2, textBounds.Y + 3);
            drawableText.color = textColor;
            //

            UpdateMenuBounds(textBounds);
        }

        #endregion

        #region BUTTONS

        public enum IMGUIDrawableFlags { NONE = 1 << 1, DECORATED = 1 << 2, FORCE_ACTIVE = 1 << 3, HORIZONTAL_ALIGN = 1 << 4, DISABLED = 1 << 5, MATCH_WINDOW_WIDTH = 1 << 6 }

        #region Text Button
        /// <summary>
        /// it's a button
        /// </summary>
        /// <param name="text"></param> displayed text, also used as the id for storing the button in the window
        /// <param name="width"></param> Leave at -1 to calculate based on text size
        /// <param name="height"></param> Leave at -1 to calculate based on text size
        /// <param name="flags"></param> 
        /// <returns></returns>
        public static bool Button(string text, int width = -1, int height = -1, IMGUIDrawableFlags flags = IMGUIDrawableFlags.DECORATED)
        {
            return Button(IMGUITypes.BUTTON + text, text, width, height, flags);
        }
        /// <summary>
        /// Same as button, but with the ability to force an id
        /// </summary>
        /// <param name="id"></param> unique id
        /// <param name="text"></param> displayed text, also used as the id for storing the button in the window
        /// <param name="width"></param> Leave at -1 to calculate based on text size
        /// <param name="height"></param> Leave at -1 to calculate based on text size
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool Button(string key, string text, int width = -1, int height = -1, IMGUIDrawableFlags flags = IMGUIDrawableFlags.DECORATED)
        {
            GUIWindow window = gContext.currentWindow;
            Vector2 textSize = font.MeasureString(text);

            // important stuff
            GUITextButton drawableButton = (GUITextButton)FindGUIDrawableByKey(window, key);
            if (drawableButton == null)
            {
                drawableButton = new GUITextButton();
                CreateDrawableInWindow(window, drawableButton);
            }
            drawableButton.window = window;
            drawableButton.flags = flags;
            drawableButton.id = HashStr(key);
            drawableButton.destroy = false;
            // data
            Rectangle bounds = new Rectangle(window.bounds.X + (int)indent.X, window.bounds.Y, width, height);
            drawableButton.bounds = bounds;
            TryAlign(window, ref drawableButton, flags);

            if (width == -1)
            {
                drawableButton.bounds.Width = (int)textSize.X + 4;
            }

            if (height == -1)
            {
                drawableButton.bounds.Height = (int)textSize.Y + 4;
            }


            bounds = drawableButton.bounds;


            // Text
            drawableButton.text = text;
            //

            UpdateMenuBounds(bounds);

            // this is pretty hacky and I hate it
            if ((flags & IMGUIDrawableFlags.MATCH_WINDOW_WIDTH) > 0)
                bounds.Width = window.prevBounds.Width;

            drawableButton.hasFocus = false;
            drawableButton.pressed = false;
            if (bounds.Contains(MouseInput.Position.X, MouseInput.Position.Y) && (flags & IMGUIDrawableFlags.DISABLED) == 0)
            {
                if (CanFocus(window))
                {
                    drawableButton.hasFocus = true;
                    if (Input.MouseInput.Check())
                    {
                        drawableButton.pressed = true;
                    }

                    if (MouseInput.Pressed())
                        drawableButton.active = true;

                    if (Input.MouseInput.Released() && CanFocus(window))
                    {
                        if (drawableButton.active)
                        {
                            drawableButton.active = false;
                            return true;
                        }
                    }
                }
            }

            if (MouseInput.Released())
            {
                drawableButton.active = false;
            }

            return false;
        }
        #endregion

        #region Image Button

        public static bool Button(Texture2D texture, int width = -1, int height = -1, IMGUIDrawableFlags flags = IMGUIDrawableFlags.DECORATED)
        {
            return Button(IMGUITypes.BUTTON + texture.Name, texture, width, height, flags);
        }
        /// <summary>
        /// Same as button, but with the ability to force an id
        /// </summary>
        /// <param name="id"></param> unique id
        /// <param name="texture"></param> displayed texture, also used as the id for storing the button in the window
        /// <param name="width"></param> Leave at -1 to calculate based on text size
        /// <param name="height"></param> Leave at -1 to calculate based on text size
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool Button(string key, Texture2D texture, int width = -1, int height = -1, IMGUIDrawableFlags flags = IMGUIDrawableFlags.DECORATED)
        {
            GUIWindow window = gContext.currentWindow;
            Vector2 textSize = new Vector2(texture.Width, texture.Height);

            // important stuff
            GUITextureButton drawableButton = (GUITextureButton)FindGUIDrawableByKey(window, key);
            if (drawableButton == null)
            {
                drawableButton = new GUITextureButton();
                CreateDrawableInWindow(window, drawableButton);
            }
            drawableButton.window = window;
            drawableButton.flags = flags;
            drawableButton.id = HashStr(key);
            drawableButton.destroy = false;
            // data
            Rectangle bounds = new Rectangle(window.bounds.X + (int)indent.X, window.bounds.Y, width, height);
            drawableButton.bounds = bounds;
            TryAlign(window, ref drawableButton, flags);

            if (width == -1)
            {
                drawableButton.bounds.Width = (int)textSize.X + 4;
            }

            if (height == -1)
            {
                drawableButton.bounds.Height = (int)textSize.Y + 4;
            }


            bounds = drawableButton.bounds;

            // Text
            drawableButton.texture = texture;
            drawableButton.imageBounds = new Rectangle(bounds.X + (bounds.Width - (int)textSize.X) / 2, bounds.Y + 2, texture.Width, texture.Height);// new Vector2(bounds.X + (bounds.Width - (int)textSize.X) / 2, bounds.Y + 3);
            //

            UpdateMenuBounds(bounds);

            drawableButton.hasFocus = false;
            drawableButton.pressed = false;
            if (bounds.Contains(MouseInput.Position.X, MouseInput.Position.Y) && (flags & IMGUIDrawableFlags.DISABLED) == 0)
            {
                if (CanFocus(window))
                {
                    drawableButton.hasFocus = true;
                    if (Input.MouseInput.Check())
                    {
                        drawableButton.pressed = true;
                    }

                    if (MouseInput.Pressed())
                        drawableButton.active = true;

                    if (Input.MouseInput.Released() && CanFocus(window))
                    {
                        if (drawableButton.active)
                        {
                            drawableButton.active = false;
                            return true;
                        }
                    }
                }
            }

            if (MouseInput.Released())
            {
                drawableButton.active = false;
            }

            return false;
        }
        #endregion

        #region Sprite Sheet Button
        public static int SpriteSheetGridSelect(Texture2D image, ref int currentIndex, int gridWidth, int gridHeight, IMGUIDrawableFlags flags = IMGUIDrawableFlags.DECORATED)
        {
            return SpriteSheetGridSelect(image.Name, image, ref currentIndex, gridWidth, gridHeight, flags);
        }

        public static int SpriteSheetGridSelect(string key, Texture2D image, ref int currentIndex, int gridWidth, int gridHeight, IMGUIDrawableFlags flags = IMGUIDrawableFlags.DECORATED)
        {
            GUIWindow window = gContext.currentWindow;
            GUISpriteSheetButton drawableSSButton = (GUISpriteSheetButton)FindGUIDrawableByKey(window, key);
            if (drawableSSButton == null)
            {
                drawableSSButton = new GUISpriteSheetButton();
                CreateDrawableInWindow(window, drawableSSButton);
            }
            drawableSSButton.window = window;
            drawableSSButton.destroy = false;
            drawableSSButton.id = HashStr(key);
            drawableSSButton.flags = flags;
            drawableSSButton.gridX = gridWidth;
            drawableSSButton.gridY = gridHeight;
            drawableSSButton.texture = image;
            //

            // set up bounds
            Rectangle bounds = image.Bounds;
            bounds.X = window.bounds.X + (int)gContext.style.indent.X;
            bounds.Y = window.bounds.Y + window.bounds.Height;
            drawableSSButton.bounds = bounds;

            // handle input
            drawableSSButton.hasFocus = -1;
            if (bounds.Contains(MouseInput.Position))
            {
                if (CanFocus(window))
                {
                    int x = (int)MouseInput.Position.X - bounds.X; // position relative to window
                    int y = (int)MouseInput.Position.Y - bounds.Y; // position relative to window
                    x /= gridWidth;
                    y /= gridHeight;

                    int w = image.Bounds.Width / gridWidth;

                    drawableSSButton.hasFocus = x + y * w;

                    if (MouseInput.Pressed())
                    {
                        drawableSSButton.pressed = drawableSSButton.hasFocus;
                        currentIndex = drawableSSButton.pressed;
                    }

                    if (MouseInput.Released())
                        drawableSSButton.pressed = -1;
                }
            }

            UpdateMenuBounds(bounds);
            drawableSSButton.active = currentIndex;
            return drawableSSButton.active;
        }
        #endregion

        #endregion

        #region TEXT_INPUT
        public enum IMGUITextFieldFlags
        {
            NONE, H_ALLIGN_LABEL = 1 << 1
        }
        public static void TextField(string label, ref string text, int width = -1, char minChar = (char)32, char maxChar = (char)126, IMGUIDrawableFlags flags = IMGUIDrawableFlags.DECORATED, IMGUITextFieldFlags textFieldFlags = IMGUITextFieldFlags.NONE)
        {
            Vector2 textSize = font.MeasureString(" " + text);
            Vector2 labelSize = font.MeasureString(label);

            GUIWindow window = gContext.currentWindow;
            string key = IMGUITypes.TEXTFIELD + label;

            // Background
            GUITextField drawableTextField = (GUITextField)FindGUIDrawableByKey(window, key);
            if (drawableTextField == null)
            {
                drawableTextField = new GUITextField();
                CreateDrawableInWindow(window, drawableTextField);
            }
            drawableTextField.id = HashStr(key);
            drawableTextField.label = label;
            drawableTextField.destroy = false;

            Rectangle bounds = new Rectangle(window.bounds.X + (int)indent.X, window.bounds.Y + window.bounds.Height, width, ((int)textSize.Y + 4));

            drawableTextField.color = menuColor;

            if (width == -1)
            {
                bounds.Width = (int)textSize.X + 4;
            }

            drawableTextField.bounds = bounds;
            //
            drawableTextField.labelPos = new Vector2(bounds.X + 2, bounds.Y + 1);

            // Text
            drawableTextField.text = " " + text + (((int)inputLineTimer) % 2 == 0 && drawableTextField.hasFocus ? '|' : ' ');

            // CHARACTER INPUT
            string tempText = text;
            if (drawableTextField.hasFocus)
            {
                if (Input.KeyInput.AnyKeyPressed())
                    inputTimer = GTime.Time;

                switch (Input.KeyInput.InputChar)
                {
                    case '\0':
                    case '\n':
                    case '\t':
                        break;
                    case '\b':
                        if (Input.KeyInput.AnyKeyPressed() || (Input.KeyInput.AnyKeyCheck() && GTime.Time - inputTimer > 0.5f))
                        {
                            if (text.Length > 0)
                                text = text.Remove(text.Length - 1, 1);
                        }
                        break;
                    default:
                        if (Input.KeyInput.AnyKeyPressed() || (Input.KeyInput.AnyKeyCheck() && GTime.Time - inputTimer > 0.5f))
                        {
                            if (Input.KeyInput.InputChar >= minChar && Input.KeyInput.InputChar <= maxChar)
                                text += Input.KeyInput.InputChar;
                        }
                        break;
                }

                if (Input.KeyInput.AnyKeyReleased())
                {
                    inputTimer = null;
                }

            }

            if (width > 0)
            {
                if (font.MeasureString(" " + text + " ").X > width)
                    text = tempText;
            }


            Rectangle fieldBounds = bounds;
            if ((textFieldFlags & IMGUITextFieldFlags.H_ALLIGN_LABEL) > 0)
            {
                fieldBounds.X = bounds.X + bounds.Width + 3;
            }
            else
            {
                fieldBounds.Y += (int)textSize.Y;
            }
            //fieldBounds.Height -= (int)textSize.Y;

            if (fieldBounds.Contains(Input.MouseInput.Position.X, Input.MouseInput.Position.Y))
            {
                if (CanFocus(window))
                {
                    drawableTextField.color = selectedColor;
                    if (Input.MouseInput.Pressed() && CanFocus(window))
                    {
                        drawableTextField.hasFocus = true;
                    }
                }
            }
            else
            {
                if (Input.MouseInput.Pressed())
                {
                    drawableTextField.hasFocus = false;
                }
            }
            drawableTextField.fieldBounds = fieldBounds;
            drawableTextField.bounds = new Rectangle(bounds.X,bounds.Y,bounds.Width + fieldBounds.Width + 3, bounds.Height);
            UpdateMenuBounds(bounds);
            UpdateMenuBounds(fieldBounds);
        }
        #endregion
        #endregion

        #region UTILITY FUNCTIONS

        private static void TryAlign<T>(GUIWindow window, ref T drawable, IMGUIDrawableFlags flags) where T : GUIDrawable
        {
            if ((flags & IMGUIDrawableFlags.HORIZONTAL_ALIGN) > 0 && window.drawables.Count > 0)
            {

                int index = GetDrawableIndex(window, drawable) - 1;
                if (index >= 0)
                {
                    Rectangle pBounds = window.drawables[index].bounds;
                    drawable.bounds.X = pBounds.X + pBounds.Width + (int)(gContext.style.indent.X);
                    drawable.bounds.Y = pBounds.Y;
                }//3548930191
            }
            else
            {
                drawable.bounds.Y += window.bounds.Height;
            }
        }

        private static int GetDrawableIndex(GUIWindow window, GUIDrawable drawable)
        {
            for (int i = 0; i < window.drawables.Count; i++)
            {
                if (window.drawables[i].id == drawable.id)
                    return i;
            }
            return -1;
        }

        // checks to see if there is a window above you
        private static bool CanFocus(GUIWindow window)
        {
            int index = gContext.dwindows.IndexOf(window); // index to check from
            if (guiMenuBar != null)
            {
                if (guiMenuBar.bounds.Contains(MouseInput.Position))
                    return false;
            }
            if (index != gContext.dwindows.Count - 1)
            {
                for (int i = index + 1; i < gContext.dwindows.Count; i++)
                {
                    if (gContext.dwindows[i].fullBounds.Contains(Input.MouseInput.Position))
                        return false;
                }
            }

            if (guiMenuBar != null)
            {
                foreach (var menu in guiMenuBar.menus)
                {
                    if (menu.Value.active)
                    {
                        if (menu.Value.itemBounds.Contains(MouseInput.Position))
                            return false;
                    }
                }
            }

            return true;
        }

        public static Vector2 measureString(string str) { return font.MeasureString(str); }

        // for external use only, to check if any ui element contains the mouse position.
        public static bool ContainsMouse()
        {
            if (guiMenuBar != null)
            {
                if (guiMenuBar.bounds.Contains(MouseInput.Position))
                    return true;


                foreach (var menu in guiMenuBar.menus)
                {
                    if (menu.Value.active)
                    {
                        if (menu.Value.itemBounds.Contains(MouseInput.Position))
                            return true;
                    }
                }
            }

            for (int i = 0; i < gContext.dwindows.Count; i++)
            {
                if (gContext.dwindows[i].fullBounds.Contains(MouseInput.Position) || gContext.dwindows[i].dragging)
                    return true;
            }

            return false;
        }

        // compares the current menu bounds against a newly added GUIDrawables bounds and expands where necessary
        private static void UpdateMenuBounds(Rectangle compareBounds)
        {
            gContext.currentWindow.bounds.Width = Math.Max(gContext.currentWindow.bounds.Width, (compareBounds.X - gContext.currentWindow.bounds.X) + compareBounds.Width + (int)indent.X);
            gContext.currentWindow.bounds.Height = Math.Max(gContext.currentWindow.bounds.Height, (compareBounds.Y - gContext.currentWindow.bounds.Y) + compareBounds.Height + (int)indent.Y);
        }


        public static uint HashStr(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(data);
                uint h = BitConverter.ToUInt32(hash, 0);
                return h;
            }
        }
        #endregion
    }
}