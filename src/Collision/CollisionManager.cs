using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NastyEngine
{
    public class CollisionManager
    {
        private class QuadTree
        {
            public QuadTree m_parent;
            public Rectangle m_bounds;
            private QuadTree[] m_children;
            private Color m_col;

            public QuadTree(Rectangle bounds)
            {
                m_parent = null;
                m_children = null;
                m_bounds = bounds;
                m_col = Color.LawnGreen;
            }

            public void Clear()
            {
                m_children = null;
            }

            public void Render()
            {
                if (m_children != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        m_children[i].Render();
                    }
                }
                Draw.RectOutline(m_bounds, m_col);
            }

            public void Split(List<Collider> colliders)
            {
                if (colliders.Count >= 2)
                {
                    List<Collider> containedColliders = new List<Collider>();
                    if (m_children == null)
                    {
                        int halfWidth = m_bounds.Width / 2;
                        int halfHeight = m_bounds.Height / 2;

                        if (halfHeight < CollisionManager.GetMinQuadSize() || halfWidth < CollisionManager.GetMinQuadSize())
                            return;

                        m_children = new QuadTree[4];

                        // TL
                        m_children[0] = new QuadTree(new Rectangle(m_bounds.X, m_bounds.Y, halfWidth, halfHeight));
                        // TR
                        m_children[1] = new QuadTree(new Rectangle(m_bounds.X + halfWidth, m_bounds.Y, halfWidth, halfHeight));
                        // BL
                        m_children[2] = new QuadTree(new Rectangle(m_bounds.X, m_bounds.Y + halfHeight, halfWidth, halfHeight));
                        // BR
                        m_children[3] = new QuadTree(new Rectangle(m_bounds.X + halfWidth, m_bounds.Y + halfHeight, halfWidth, halfHeight));

                        int numStatic = 0;
                        int numDynamic = 0;

                        for (int k = 0; k < colliders.Count; k++)
                        {
                            if (m_bounds.Intersects(colliders[k].m_bounds))
                            {
                                containedColliders.Add(colliders[k]);
                                if (colliders[k].IsStatic())
                                    numStatic++;
                                else
                                    numDynamic++;
                            }
                        }
                        if (numDynamic == 0 || (numDynamic == 1 && numStatic == 0))
                        {
                            m_children = null;
                        }
                        else
                        {
                            int numContained = 0;
                            for (int i = 0; i < 4; i++)
                            {
                                m_children[i].m_parent = this;
                                m_children[i].Split(containedColliders);
                                numContained++;
                            }
                        }
                    }

                    if (containedColliders.Count < 2)
                    {
                        m_col = Color.MonoGameOrange;
                    }
                    else
                    {
                        m_col = Color.LawnGreen;
                        HandleOverlap(containedColliders);
                    }
                }

            } // Split end

            private void HandleOverlap(List<Collider> colliders)
            {
                HashSet<Tuple<Guid, Guid>> pairedColliders = new HashSet<Tuple<Guid, Guid>>();
                for (int i = 0; i < colliders.Count; i++)
                {
                    for (int k = colliders.Count - 1; k >= 0; k--)
                    {
                        if (k == i) continue;
                        if (colliders[i].IsStatic()) continue;
                        //var forwardPair = new Tuple<Guid, Guid>(colliders[i].Parent.ID, colliders[k].Parent.ID);
                        //var reversePair = new Tuple<Guid, Guid>(colliders[k].Parent.ID, colliders[i].Parent.ID);
                        //if (pairedColliders.Contains(reversePair))
                        //    continue;

                        if (colliders[i].CheckOverlap(colliders[k]))
                        {
                            //pairedColliders.Add(forwardPair);
                            if(!colliders[i].m_others.Contains(colliders[k]))
                                colliders[i].OnOverlapEnter(colliders[k]);
                            if (colliders[k].IsStatic())
                                colliders[k].OnOverlapEnter(colliders[i]);
                            //colliders[k].OnOverlapEnter(colliders[i]);
                        } else if(colliders[i].m_others.Contains(colliders[k]))
                        { 
                            //pairedColliders.Add(forwardPair);
                            colliders[i].OnOverlapExit(colliders[k]);
                            if (colliders[k].IsStatic())
                                colliders[k].OnOverlapExit(colliders[i]);
                            //colliders[k].OnOverlapExit(colliders[i]);
                        }
                    }
                }
            }


        } // QuadTree end

        public static CollisionManager Instance_;

        public static bool DebugRender = false;

        private QuadTree m_quadTree;
        private int m_minQuadSize = 16;

        private List<Collider> m_colliders;

        private int m_worldWidth;
        private int m_worldHeight;

        public CollisionManager(int worldWidth, int worldHeight)
        {
            if (Instance_ != null)
            {
                Debug.Fail("Error, can't have 2 collision manager instances.");
                return;
            }

            Instance_ = this;
            int maxVal = Math.Max(worldWidth, worldHeight);
            m_quadTree = new QuadTree(new Rectangle(0, 0, maxVal, maxVal));
            m_colliders = new List<Collider>(); 
            m_worldWidth = worldWidth;
            m_worldHeight = worldHeight;
        }

        public static void Update()
        {
            Instance_.m_quadTree.Clear();
            Instance_.m_quadTree.Split(Instance_.m_colliders);
        }

        public static void Register(Collider collider)
        {
            Instance_.m_colliders.Add(collider);
        }

        public static void DrawQuadTree()
        {
            Instance_.m_quadTree.Render();
        }

        public static void SetMinQuadSize(int minSize)
        {
            Instance_.m_minQuadSize = minSize;
        }

        public static int GetMinQuadSize()
        {
            return Instance_.m_minQuadSize;
        }

    }
}
