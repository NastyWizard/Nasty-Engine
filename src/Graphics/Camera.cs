using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NastyEngine
{
    public class Camera
    {

        public Matrix Matrix
        {
            get
            {
                if (changed)
                    UpdateMatrices();
                return matrix;
            }
        }

        public Matrix InverseMatrix
        {
            get
            {
                if (changed)
                    UpdateMatrices();
                return inverseMatrix;
            }
        }

        public Viewport Viewport;
        private Matrix matrix;
        private Matrix inverseMatrix;
        private bool changed;

        private Vector2 position = Vector2.Zero; public Vector2 Position { get { return position; } set { changed = true; position = value; } }
        private Vector2 zoom = Vector2.One; public Vector2 Zoom { get { return zoom; } set { changed = true; zoom = value; } }
        private Vector2 origin = Vector2.Zero; public Vector2 Origin { get { return origin; } set { changed = true; origin = value; } }
        private float angle = 0; public float Angle { get { return angle; } set { changed = true; angle = value; } }

        public Camera()
        {
            Viewport = new Viewport
            {
                Width = Engine.GameWidth,
                Height = Engine.GameHeight
            };
            UpdateMatrices();
        }

        public enum CameraFollowTechnique { INSTANT, LERP, CONSTANT_SPEED }
        public void Follow(Vector2 target, float speed = 0, CameraFollowTechnique technique = CameraFollowTechnique.CONSTANT_SPEED)
        {
            Vector2 gameSize = new Vector2(Engine.GameWidth / 2, Engine.GameHeight / 2) / (Engine.PixelPerfect ? 1 : matrix.M11);
            Vector2 targetPos = new Vector2(target.X - gameSize.X, target.Y - gameSize.Y);

            if (speed <= 0 || technique == CameraFollowTechnique.INSTANT)
            {
                Position = targetPos;
            }
            else if (technique == CameraFollowTechnique.LERP)
            {
                Position = Vector2.Lerp(Position, targetPos, speed);
            }
            else if (technique == CameraFollowTechnique.CONSTANT_SPEED)
            {
                Vector2 dir = targetPos - Position;
                dir.Normalize();
                Position = Position + dir * speed;
            }
        }

        public void UpdateMatrices()
        {
            matrix = Matrix.Identity *
                Matrix.CreateTranslation(new Vector3(-new Vector2((int)Math.Floor(position.X), (int)Math.Floor(position.Y)), 0)) *
                Matrix.CreateRotationZ(angle) *
                Matrix.CreateScale(new Vector3(zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(-new Vector2((int)Math.Floor(origin.X), (int)Math.Floor(origin.Y)), 0));
            inverseMatrix = Matrix.Invert(matrix);
            changed = false;
        }

    }
}
