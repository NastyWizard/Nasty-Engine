using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NastyEngine
{
    public class Animator : Component
    {
        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        private Animation currentAnimation;

        private SpriteRenderer spriteRenderer;

        public bool IsPlaying { get; private set; }

        private float TimePlaying;
        private float? TimeAtFrame;


        public Animator(SpriteRenderer sr)
        {
            spriteRenderer = sr;
        }

        public override void OnInit()
        {
            base.OnInit();

        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (IsPlaying)
            {
                TimePlaying += GTime.Delta;

                if (!TimeAtFrame.HasValue)
                {
                    TimeAtFrame = TimePlaying;
                }

                if (TimePlaying - TimeAtFrame > currentAnimation.AnimSpeed)
                {
                    TimeAtFrame = null;
                    currentAnimation.SetFrame(currentAnimation.CurrentFrame + 1);
                }
            }
            if(currentAnimation!= null)
                spriteRenderer.SourceRect = currentAnimation.DrawRect;
        }

        public void AddAnimation(string name, Animation an)
        {
            if (animations.Count == 0)
                currentAnimation = an;
            animations.Add(name, an);
        }


        public void PlayAnimation(string name)
        {
            IsPlaying = true;
            currentAnimation = animations[name];
        }

        public void SetFrame(int frame)
        {
            currentAnimation.SetFrame(frame);
        }

        public void StopAnimation()
        {
            IsPlaying = false;
        }

        public void ResetAnimation()
        {
            currentAnimation.SetFrame(0);
        }

    }
}
