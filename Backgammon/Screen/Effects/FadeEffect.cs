using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backgammon.Screen;
using Microsoft.Xna.Framework;

namespace Backgammon.Screen.Effects
{
    public class FadeEffect : ImageEffect
    {
        public float FadeSpeed, MinAlpha, MaxAlpha;
        public bool Increase;

        public FadeEffect()
        {
            FadeSpeed = 0.2f;
            Increase = false;
            MinAlpha = 0.2f;
            MaxAlpha = 0.8f;
        }

        public override void LoadContent(ref Image Image)
        {
            base.LoadContent(ref Image);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (image.IsActive)
            {
                if (!Increase)
                    image.Alpha -= FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                    image.Alpha += FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (image.Alpha < MinAlpha)
                {
                    Increase = true;
                    image.Alpha = MinAlpha;
                }
                else if (image.Alpha > MaxAlpha)
                {
                    Increase = false;
                    image.Alpha = MaxAlpha;
                }

            }
            else
                image.Alpha = 1.0f;

        }
    }
}
