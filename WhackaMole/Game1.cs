using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace WhackaMole
{
    enum GameState { Menu, Play, GameOver }

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        Random random = new Random();
        Vector2 center = new Vector2();

        Texture2D textureBackground;
        Texture2D textureMole;
        Texture2D textureMoleHit;
        Texture2D textureHole;
        Texture2D textureHoleForeground;
        Texture2D textureMallet;
        SpriteFont spriteFont;

        bool debugMode = false;

        double timerGame;
        int score;

        KeyboardState keyboardState;
        KeyboardState keyboardStatePrevious;

        GameState currentGameState;

        Mole[,] moles = new Mole[3, 3];

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

            currentGameState = GameState.Menu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            center = new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2);

            spriteFont = Content.Load<SpriteFont>("spritefont");
            textureBackground = Content.Load<Texture2D>("background");
            textureMole = Content.Load<Texture2D>("mole");
            textureMoleHit = Content.Load<Texture2D>("mole_KO");
            textureHole = Content.Load<Texture2D>("hole");
            textureHoleForeground = Content.Load<Texture2D>("hole_foreground");
            textureMallet = Content.Load<Texture2D>("mallet");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            keyboardStatePrevious = keyboardState;
            keyboardState = Keyboard.GetState();
            UpdateDebugMode();

            StartGame();

            if (currentGameState == GameState.Play)
            {
                foreach (Mole mole in moles)
                {
                    mole.Update(gameTime);
                    UpdateMoleTexture(mole);
                }

                timerGame -= gameTime.ElapsedGameTime.TotalSeconds;
                if (timerGame < 0)
                {
                    currentGameState = GameState.GameOver;
                }
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(111,209,72,255)); // HoleForeground color is (111, 209, 72)

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (currentGameState == GameState.Menu)
            {
                spriteBatch.DrawString(spriteFont, "Press Space to play.", new Vector2(center.X, center.Y), Color.White);
            }

            if (currentGameState == GameState.Play)
            {
                foreach (Mole mole in moles)
                {
                    mole.Draw(spriteBatch);
                    if (debugMode == true)
                    {
                        Texture2D textureMoleHitbox = new Texture2D(GraphicsDevice, 1, 1);
                        textureMoleHitbox.SetData(new[] { Color.White });
                        mole.DrawHitbox(spriteBatch, textureMoleHitbox);
                    }
                }

                spriteBatch.DrawString(spriteFont, "" + (int)timerGame, new Vector2(center.X,0), Color.White);
            }

            if (currentGameState == GameState.GameOver)
            {
                string gameOverMessage = "GAME OVER!\nPress Space to play again.";
                spriteBatch.DrawString(spriteFont, gameOverMessage, new Vector2(center.X, center.Y), Color.White);
            }
           
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void StartGame()
        {
            if (currentGameState != GameState.Play)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && keyboardStatePrevious.IsKeyUp(Keys.Space))
                {
                    PopulateMoles();
                    currentGameState = GameState.Play;
                    timerGame = 30;
                }
            }
        }

        public void PopulateMoles()
        {
            int marginMoles = 250;
            int positionMoleX = (int)center.X - (2 * textureHoleForeground.Width);
            int positionMoleY = (int)center.Y - textureHoleForeground.Height;

            for (int i = 0; i < moles.GetLength(0); i++)
            {
                for (int j = 0; j < moles.GetLength(1); j++)
                {
                    moles[i, j] = new Mole(positionMoleX + i * marginMoles, positionMoleY + j * marginMoles, textureMole, textureHole, textureHoleForeground);
                }
            }
        }

        private void UpdateMoleTexture(Mole mole)
        {
            if (mole.GetMoleState() == MoleState.IsHit)
            {
                mole.SetMoleTexture(textureMoleHit);
            }
            else
            {
                mole.SetMoleTexture(textureMole);
            }
        }

        // Toggle debug mode
        public void UpdateDebugMode()
        {
            if (keyboardState.IsKeyDown(Keys.H) && keyboardStatePrevious.IsKeyUp(Keys.H))
            {
                debugMode = !debugMode;
                Debug.WriteLine("Debug mode toggled.");
            }
        }

        //public int SetScore()
        //{
        //    score++;
        //    return score;
        //}
    }
}