using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace NastyEngine
{
    public class TileMap
    {
        public List<List<Tile>> Tiles;

        public List<object> MapData;

        public Vector2 position;

        public TileMap(string url, Vector2 pos)
        {

            position = pos;

            Tiles = new List<List<Tile>>();

            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            PythonLoader.ExecuteFile(path + @"\Content\Python\TileMapLoader.py", new Tuple<string, object>("filename", url));
            MapData = PythonLoader.GetVariable("tileData");
            int layerCount = 0;
            foreach (dynamic data in MapData)
            {
                Tiles.Add(new List<Tile>());
                int c = 0;
                foreach (dynamic t in data.data)
                {
                    if (t.tileType > 0)
                    {
                        Tile tile = new Tile((TileType)t.tileType, this, c++);
                        tile.transform.Position = position - new Vector2((float)t.x,(float)t.y + ((float)t.z * 8.0f));
                        tile.depth = t.z;
                        Tiles[layerCount].Add(tile);
                        tile.Update();
                    }
                }
                layerCount++;
            }

        }

        public void Render()
        {
            for (int j = 0; j < Tiles.Count; j++)
            {
                for (int i = 0; i < Tiles[j].Count; i++)
                {
                    if (i < Tiles[j].Count)
                        Tiles[j][i].Render();
                }
            }

        #if DEBUG
            for (int j = 0; j < Tiles.Count; j++)
            {
                for (int i = 0; i < Tiles[j].Count; i++)
                {
                    Color col = Color.Red;

                    switch (j)
                    {
                        case 0:
                            break;
                        case 1:
                            col = Color.HotPink;
                            break;
                        case 2:
                            col = Color.Blue;
                            break;
                        case 3:
                            col = Color.Green;
                            break;
                    }
                    
                    Draw.Dot(Tiles[j][i].transform.Position, col);
                }
            }
        #endif
        }
    }
}
