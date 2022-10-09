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
        Texture2D textureStone;
        SpriteFont spriteFont;

        bool debugMode = false;

        double timerGame;

        int score = 0;

        // Stone properties
        double timerStoneFrame;
        int intervalStoneFrame;
        int stoneFrame;
        int stoneFrameMax = 16;
        Vector2 positionStone;
        Rectangle stoneSourceRectangle;

        KeyboardState keyboardState;
        KeyboardState keyboardStatePrevious;
        MouseState mouseState;
        MouseState mouseStatePrevious;

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
            intervalStoneFrame = 100;
            timerStoneFrame = intervalStoneFrame;

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
            textureStone = Content.Load<Texture2D>("spritesheet_stone");

            stoneSourceRectangle = new Rectangle(0, 0, textureStone.Width / 4, textureStone.Height / 4);
            positionStone = new Vector2(center.X - (textureStone.Width / 6), 400);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            keyboardStatePrevious = keyboardState;
            keyboardState = Keyboard.GetState();
            UpdateDebugMode();

            mouseStatePrevious = mouseState;
            mouseState = Mouse.GetState();


            if (currentGameState == GameState.Menu)
            {
                if (currentGameState == GameState.Menu)
                {
                    timerStoneFrame -= gameTime.ElapsedGameTime.TotalMilliseconds;
                    AnimateStone();
                }
            }

            StartGame();

            if (currentGameState == GameState.Play)
            {
                // Update moles
                foreach (Mole mole in moles)
                {
                    mole.Update(gameTime);

                    if (mole.GetMoleState() != MoleState.IsHit)
                        OnMouseClick(mole);

                    UpdateMoleTexture(mole);
                }

                // Countdown timer until GameOver
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

            spriteBatch.Draw(textureBackground, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 3f, SpriteEffects.None, 0f);

            if (currentGameState == GameState.Menu)
            {
                DrawStoneAnimation();
                if (positionStone.Y == 800)
                {
                    string menuText = "Press Space to play.";
                    spriteBatch.DrawString(spriteFont, menuText, new Vector2(center.X - spriteFont.MeasureString(menuText).X / 2, center.Y - spriteFont.MeasureString(menuText).Y / 2), Color.White);
                }
            }

            if (currentGameState == GameState.Play)
            {
                foreach (Mole mole in moles)
                {
                    mole.Draw(spriteBatch);

                    // Draw Hitboxes
                    if (debugMode == true)
                    {
                        Texture2D textureMoleHitbox = new Texture2D(GraphicsDevice, 1, 1);
                        textureMoleHitbox.SetData(new[] { Color.White });
                        mole.DrawHitbox(spriteBatch, textureMoleHitbox);
                    }

                    spriteBatch.DrawString(spriteFont, "Score: " + score, Vector2.Zero, Color.White);
                }

                spriteBatch.DrawString(spriteFont, "" + (int)timerGame, new Vector2(center.X - spriteFont.MeasureString("" + (int)timerGame).X/2,0), Color.White);
            }

            if (currentGameState == GameState.GameOver)
            {
                string gameOverMessage = "GAME OVER!\nYou scored " + score + "\nPress Space to play again.";
                spriteBatch.DrawString(spriteFont, gameOverMessage, new Vector2(center.X - spriteFont.MeasureString(gameOverMessage).X/2, center.Y - spriteFont.MeasureString(gameOverMessage).Y/2), Color.White);
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
                    score = 0;
                    PopulateMoles();
                    currentGameState = GameState.Play;
                    timerGame = 31;
                }
            }
        }

        public void PopulateMoles()
        {
            int marginMoles = 200;
            int positionMoleX = (int)center.X - (2 * textureHoleForeground.Width);
            int positionMoleY = (int)center.Y /*- textureHoleForeground.Height*/;

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

        public void AnimateStone()
        {
            // Animates the stone spritesheet
            if (timerStoneFrame <= 0)
            {
                timerStoneFrame = intervalStoneFrame;
                stoneFrame++;

                if (stoneFrame >= stoneFrameMax)
                {
                    stoneFrame = 0;
                }

                int frameX = stoneFrame % 4;
                int frameY = (int)(stoneFrame / 4);

                stoneSourceRectangle.X = frameX * 64;
                stoneSourceRectangle.Y = frameY * 64;
            }

            if (positionStone.Y >= 800)
            {
                positionStone.Y = 800;
            }
            else
            {
                positionStone.Y += 1;
            }
        }

        public void DrawStoneAnimation()
        {
            spriteBatch.Draw(textureHole, new Vector2(center.X - textureHole.Width/2, 700), Color.White);
            spriteBatch.Draw(textureStone, positionStone, stoneSourceRectangle, Color.White);
            spriteBatch.Draw(textureHoleForeground, new Vector2(center.X - textureHoleForeground.Width / 2, 700), Color.White);
        }

        private void OnMouseClick(Mole mole)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton != ButtonState.Pressed)
            {
                if (mole.GetHitbox().Contains(Mouse.GetState().Position))
                {
                    mole.SetMoleState(MoleState.IsHit);
                    score++;
                    Debug.WriteLine("Hit");
                }
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
    }
}