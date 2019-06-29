using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace NastyEngine
{
    public enum TileType { GRASS = 1, DIRT, SAND, WATER, STONE};

    public class Tile : GameObject
    {
        public TileType type;

        public List<int> MapData;

        public int depth;

        public bool walkable;
        private TileType unWalkables = TileType.WATER; // WHEN ADDING UNWALKABLE TILE TYPES, & IT ONTO THIS VALUE

        // components
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private TileMap tileMap;
        private int mapIndex;

        public Tile(TileType type, TileMap map, int index)
        {
            this.type = type;
            walkable = (type & unWalkables) == 0;

            tileMap = map;
            mapIndex = index;
            //
            animator = new Animator(spriteRenderer = new SpriteRenderer("Tiles"));
            spriteRenderer.Offset = new Vector2(16, 9);

            Animation anim = new Animation();
            anim.MakeAnim(32, 32, 0, 4, (int)type-1);

            animator.AddAnimation("anim",anim);
            animator.PlayAnimation("anim");

            AddComponent(spriteRenderer);
            AddComponent(animator);

            base.Init();
        }
    }
}
