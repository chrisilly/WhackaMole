using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace WhackaMole
{
    enum GameState
    {
        Start,
        Play,
        Win,
        Lose,
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        GameState currentGameState;
        GameState GameOver = GameState.Win | GameState.Lose;

        Texture2D textureBackground;
        Texture2D textureMole;
        Texture2D textureMoleHit;
        Texture2D textureHole;
        Texture2D textureHoleForeground;
        Texture2D textureMallet;

        Mole[,] moles = new Mole[3, 3];
        Hole[,] moleHoles = new Hole[3, 3];
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            textureMole = Content.Load<Texture2D>("mole");
            textureHole = Content.Load<Texture2D>("hole");
            textureHoleForeground = Content.Load<Texture2D>("hole_foreground");
            textureMallet = Content.Load<Texture2D>("mallet");
            
            PopulateMoles();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LawnGreen); // HoleForeground color is (111, 209, 72)

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            foreach (Mole mole in moles)
            {
                mole.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void PopulateMoles()
        {
            int marginMoles = 250;

            for (int i = 0; i < moles.GetLength(0); i++)
            {
                for (int j = 0; j < moles.GetLength(1); j++)
                {
                    moles[i, j] = new Mole(i*marginMoles, j*marginMoles, textureMole);
                }
            }
        }
    }
}