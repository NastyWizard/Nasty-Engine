using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace NastyEngine
{
    public class PhysBody : Component
    {
        public bool IsSleeping = false;
        private Transform transform;

        public float hspeed = 0, vspeed = 0;
        private float deltaSpeedH;
        private float deltaSpeedV;

        public float dragX = 0, dragY = 0;

        public float gravity = 0.0f;//5.0f;

        public Rectangle BoundingBox = new Rectangle(0, 0, 32, 32);
        public Vector2 BoundingBoxOffset = Vector2.Zero;

        //public Dictionary<SceneNode.TagTypes, PhysBody> Overlaps = new Dictionary<SceneNode.TagTypes, PhysBody>(); // <Name, Body> TODO: maybe change this to Type of gameobject instead of physbody

        private List<PhysBody> collisions = new List<PhysBody>();

        public override void OnInit()
        {
            base.OnInit();
            transform = Parent.transform;
            UpdateRect();
        }

        public override void OnUpdate()
        {

            base.OnUpdate();


            if (!IsSleeping)
            {
                //if (!Overlaps.ContainsKey(SceneNode.TagTypes.FLOOR))
                //    vspeed += gravity;

                deltaSpeedH = hspeed * GTime.Delta;
                deltaSpeedV = vspeed * GTime.Delta;

                handleCollisions();
                transform.LocalPosition.X += deltaSpeedH;
                transform.LocalPosition.Y += deltaSpeedV;
            }
            UpdateRect();

            hspeed *= 1 - Math.Min(1, dragX);
            vspeed *= 1 - Math.Min(1, dragY);

        }

        private void UpdateRect()
        {
            BoundingBox.X = (int)transform.Position.X + (int)BoundingBoxOffset.X;
            BoundingBox.Y = (int)transform.Position.Y + (int)BoundingBoxOffset.Y;
        }

        private void handleCollisions()
        {
            foreach (var other in collisions)
            {
                if (CheckOverlap(other, (int)deltaSpeedH, 1, BoundingBox.Width, BoundingBox.Height - 2))
                {
                    int breakCount = 0;
                    int maxBreak = (int)Math.Abs(deltaSpeedH);
                    while (!CheckOverlap(other, Math.Sign(deltaSpeedH), 1, BoundingBox.Width, BoundingBox.Height - 2))
                    {
                        transform.LocalPosition.X += Math.Sign(deltaSpeedH);
                        UpdateRect();
                        breakCount++;
                        if (breakCount > maxBreak)
                            break;
                    }
                    deltaSpeedH = 0;
                    hspeed = 0;
                }

                if (CheckOverlap(other, 1, (int)deltaSpeedV, BoundingBox.Width - 2, BoundingBox.Height))
                {
                    int breakCount = 0;
                    int maxBreak = (int)Math.Abs(deltaSpeedV);
                    while (!CheckOverlap(other, 1, Math.Sign(deltaSpeedV), BoundingBox.Width - 2, BoundingBox.Height - 1))
                    {
                        transform.LocalPosition.Y += Math.Sign(deltaSpeedV);
                        UpdateRect();
                        breakCount++;
                        if (breakCount > maxBreak)
                            break;
                    }
                    if (Math.Sign(deltaSpeedV) == 1)
                    {
                        deltaSpeedV = 0;
                        vspeed = 0;
                    }
                    else
                    {
                        deltaSpeedV = 0;
                        vspeed = 1;
                    }
                }


                int breakCountPush = 0;
                int maxBreakPush = 8;
                while (CheckOverlap(other, 1, 1, BoundingBox.Width - 2, BoundingBox.Height - 2))//
                {
                    breakCountPush++;
                    if (breakCountPush > maxBreakPush) break;
                    Vector2 moveDir = transform.LocalPosition - other.transform.LocalPosition;
                    moveDir.Normalize();
                    moveDir.X = (float)Math.Round(moveDir.X);
                    moveDir.Y = (float)Math.Round(moveDir.Y);
                    transform.LocalPosition += moveDir;
                }

            }
            collisions.Clear();
        }

        public void CollideWith(PhysBody other)
        {
            if (Enabled && other.Enabled)
                collisions.Add(other);
        }

        public bool CheckOverlap(PhysBody body, Vector2 offsetSelf)
        {
            Rectangle newRect = new Rectangle(BoundingBox.X + (int)offsetSelf.X, BoundingBox.Y + (int)offsetSelf.Y, BoundingBox.Width, BoundingBox.Height);
            bool overlaps = body.BoundingBox.Intersects(newRect);

            AddOverlap(overlaps, body);

            return overlaps;
        }

        public bool CheckOverlap(PhysBody body, int X, int Y)
        {
            Rectangle newRect = new Rectangle(BoundingBox.X + X, BoundingBox.Y + Y, BoundingBox.Width, BoundingBox.Height);
            bool overlaps = body.BoundingBox.Intersects(newRect);

            AddOverlap(overlaps, body);

            return overlaps;
        }


        public bool CheckOverlap(PhysBody body, int X, int Y, int width, int height)
        {
            Rectangle newRect = new Rectangle(BoundingBox.X + X, BoundingBox.Y + Y, width, height);
            bool overlaps = body.BoundingBox.Intersects(newRect);

            AddOverlap(overlaps, body);

            return overlaps;
        }

        public bool CheckOverlap(PhysBody body)
        {
            bool overlaps = CheckOverlap(body.BoundingBox);

            AddOverlap(overlaps, body);

            return overlaps;
        }

        public bool CheckOverlap(Rectangle rect)
        {
            bool overlaps = rect.Intersects(BoundingBox);

            return overlaps;
        }

        public void AddOverlap(bool overlaps, PhysBody body)
        {
            //if (overlaps && !Overlaps.ContainsKey(body.Parent.Tags)) // TODO: maybe change this to ID, but gotta improve ID system
            //    Overlaps.Add(body.Parent.Tags, body);
            //else if (Overlaps.ContainsKey(body.Parent.Tags))
            //{
            //    if (Overlaps[body.Parent.Tags] == body && !overlaps)
            //        Overlaps.Remove(body.Parent.Tags);
            //}
        }

    }
}
