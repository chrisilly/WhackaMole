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
    internal class Mole
    {
        Texture2D textureHole;
        Texture2D textureHoleForeground;

        Texture2D textureMole;
        Texture2D textureMoleHit;
        Vector2 positionMole;
        Vector2 velocityMole;

        // Lonesome mole, do not use
        public Mole(int positionMoleX, int positionMoleY, Texture2D textureMole)
        {
            this.textureMole = textureMole;
            this.positionMole = new Vector2(positionMoleX, positionMoleY);
        }

        // Generate a Mole including the hole and holeForeground textures bundled into one object
        public Mole(int positionMoleX, int positionMoleY, Texture2D textureMole, Texture2D textureHole, Texture2D textureHoleForeground)
        {
            this.textureMole = textureMole;
            this.textureHole = textureHole;
            this.textureHoleForeground = textureHoleForeground;
            this.positionMole = new Vector2(positionMoleX, positionMoleY);

        }

        public Mole(Texture2D textureMole)
        {
            this.textureMole = textureMole;
            velocityMole = Vector2.Zero;
        }
        

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureMole, positionMole, Color.White);
        }
    }
}
