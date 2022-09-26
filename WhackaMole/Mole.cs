using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WhackaMole
{
    enum MoleState { IsDown, IsUp, GoingDown, GoingUp, IsHit }

    internal class Mole
    {
        Random random = new Random();

        MoleState moleState;
        double moleStateTimer;
        int moleStateTimerThreshold;

        Texture2D textureHole;
        Texture2D textureHoleForeground;

        Texture2D textureMole;
        Texture2D textureMoleHit;
        Vector2 positionMoleHole;
        Vector2 positionMole;
        Vector2 velocityMole;

        public Mole(Texture2D textureMole)
        {
            this.textureMole = textureMole;
            velocityMole = Vector2.Zero;
        }

        // Generate a Mole including the hole and holeForeground textures bundled into one object
        public Mole(int positionMoleX, int positionMoleY, Texture2D textureMole, Texture2D textureHole, Texture2D textureHoleForeground)
        {
            this.textureMole = textureMole;
            this.textureHole = textureHole;
            this.textureHoleForeground = textureHoleForeground;
            positionMoleHole = new Vector2(positionMoleX, positionMoleY);
            positionMole = positionMoleHole;
            moleState = MoleState.IsDown;
        }
        
        public void Update(GameTime gameTime)
        {
            positionMole += velocityMole;

            moleStateTimer += gameTime.ElapsedGameTime.TotalSeconds;

            ManageMole();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureHole, positionMoleHole, Color.White);
            spriteBatch.Draw(textureMole, positionMole, Color.White);
            spriteBatch.Draw(textureHoleForeground, positionMoleHole, Color.White);
        }

        public void ManageMole()
        {
            UpdateMoleStateTimer(); // This resets the timer when the threshold has passed, yet somehow ManageMole still works? It works better than if UpdateMoleStateTimer was at the bottom????

            if (moleState == MoleState.IsDown)
            {
                if (moleStateTimer >= moleStateTimerThreshold)
                {
                    velocityMole = new Vector2(0, random.Next(-4,-2));
                    moleState = MoleState.GoingUp;
                }
            }
            else if (moleState == MoleState.GoingUp && positionMole.Y <= positionMoleHole.Y - textureMole.Height/1.5f)
            {
                velocityMole = Vector2.Zero;
                moleState = MoleState.IsUp;
            }
            else if (moleState == MoleState.IsUp)
            {
                if (moleStateTimer >= random.Next(1,3))
                {
                    velocityMole = new Vector2(0, random.Next(4,8));
                    moleState = MoleState.GoingDown;
                }
            }
            else if (moleState == MoleState.GoingDown && positionMole.Y >= positionMoleHole.Y)
            {
                velocityMole = Vector2.Zero;
                moleState = MoleState.IsDown;
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
    }
}
