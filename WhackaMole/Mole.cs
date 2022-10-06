using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace WhackaMole
{
    enum MoleState { IsDown, IsUp, GoingDown, GoingUp, IsHit }

    internal class Mole
    {
        Random random = new Random();

        MoleState moleState;
        MoleState descending = MoleState.GoingDown | MoleState.IsHit;
        double moleStateTimer;
        int moleStateTimerThreshold;

        Texture2D textureHole;
        Texture2D textureHoleForeground;

        Texture2D textureMole;
        Texture2D textureMoleHit;
        Vector2 positionMoleHole;
        Vector2 positionMole;
        Vector2 velocityMole;
        Rectangle moleHitbox;

        MouseState mouseStateCurrent;
        MouseState mouseStatePrevious;

        // Blank Mole, use only for testing purposes
        public Mole(Texture2D textureMole)
        {
            this.textureMole = textureMole;
            velocityMole = Vector2.Zero;
        }

        // Generate a Mole including the hole and holeForeground bundled into one object
        public Mole(int positionMoleX, int positionMoleY, Texture2D textureMole, Texture2D textureHole, Texture2D textureHoleForeground)
        {
            this.textureMole = textureMole;
            this.textureHole = textureHole;
            this.textureHoleForeground = textureHoleForeground;

            positionMoleHole = new Vector2(positionMoleX, positionMoleY);
            positionMole = positionMoleHole;
            moleState = MoleState.IsDown;

            moleHitbox = new Rectangle(positionMoleX, positionMoleY, textureMole.Width, textureMole.Height);
        }
        
        public void Update(GameTime gameTime)
        {
            // Get user input
            mouseStatePrevious = mouseStateCurrent;
            mouseStateCurrent = Mouse.GetState();

            positionMole += velocityMole;
            moleStateTimer += gameTime.ElapsedGameTime.TotalSeconds;

            // Manage hitbox mousclick collision
            if(moleState != MoleState.IsHit)
                OnMouseClick(); // move to Game1.cs so you can move score there so you can draw it etc

            UpdateMoleHitbox();
            UpdateMoleState();
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureHole, positionMoleHole, Color.White);
            spriteBatch.Draw(textureMole, positionMole, Color.White);
            spriteBatch.Draw(textureHoleForeground, positionMoleHole, Color.White);
        }

        public void UpdateMoleState()
        {
            UpdateMoleStateTimer(); // This resets the timer when the threshold has passed, yet somehow ManageMole still works? It even works better than if UpdateMoleStateTimer was at the bottom?

            switch (moleState)
            {
                case MoleState.IsDown:
                    if (moleStateTimer >= moleStateTimerThreshold)
                    {
                        moleState = MoleState.GoingUp;
                    }
                    break;

                case MoleState.IsUp:
                    if (moleStateTimer >= random.Next(1, 3))
                    {
                        moleState = MoleState.GoingDown;
                    }
                    break;

                case MoleState.GoingDown:
                    velocityMole = new Vector2(0, random.Next(4, 8));

                    if (positionMole.Y >= positionMoleHole.Y)
                    {
                        velocityMole = Vector2.Zero;
                        moleState = MoleState.IsDown;
                    }
                    break;

                case MoleState.GoingUp:
                    velocityMole = new Vector2(0, random.Next(-4, -2));

                    if (positionMole.Y <= positionMoleHole.Y - textureMole.Height / 1.5f)
                    {
                        velocityMole = Vector2.Zero;
                        moleState = MoleState.IsUp;
                    }
                    break;

                case MoleState.IsHit:
                    velocityMole = new Vector2(0, 1);

                    if (positionMole.Y >= positionMoleHole.Y)
                    {
                        velocityMole = Vector2.Zero;
                        moleState = MoleState.IsDown;
                    }
                    break;

                default:
                    Debug.WriteLine("UpdateMoleState() went out of bounds");
                    break;
            }            
        }

        public void UpdateMoleStateTimer()
        {
            if (moleStateTimer >= moleStateTimerThreshold)
            {
                moleStateTimer = 0;
                moleStateTimerThreshold = random.Next(4);
            }
        }

        public void UpdateMoleHitbox()
        {
            moleHitbox.Y = (int)positionMole.Y + 80;
            moleHitbox.Height = (int)(positionMoleHole.Y - positionMole.Y - 30);
        }

        // Show transparent moleHitbox shape
        public void DrawHitbox(SpriteBatch spriteBatch, Texture2D textureMoleHitbox)
        {
                spriteBatch.Draw(textureMoleHitbox, moleHitbox, Color.FromNonPremultiplied(155, 255, 61, 122));
        }

        // Check Mousclick moleHitbox Collision
        public void OnMouseClick()
        {
            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton != ButtonState.Pressed)
            {
                if (moleHitbox.Contains(Mouse.GetState().Position))
                {
                    Debug.WriteLine("Hit");
                    moleState = MoleState.IsHit;
                    //score++
                }
            }
        }

        public MoleState GetMoleState()
        {
            return moleState;
        }

        public void SetMoleTexture(Texture2D textureMole)
        {
            this.textureMole = textureMole;
        }
    }
}
