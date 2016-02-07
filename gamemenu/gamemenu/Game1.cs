using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using WindowsGameLibrary1;

namespace gamemenu
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int displaywidth;
        int displayheight;

        const int numberofhighscores = 10;
        int[] highscores = new int[numberofhighscores];
        string[] highscorename = new string[numberofhighscores];

        Boolean gameover = false;
        float gameruntime = 0;

        graphics2d background;
        Random randomiser = new Random();

        int gamestate = -1;

        GamePadState[] pad = new GamePadState[4];
        KeyboardState keys;
        MouseState mouse;

        SpriteFont mainfont;

        sprites2d mousepointer1, mousepointer2;

        const int numberofoptions = 4;
        sprites2d[,] menuoptions = new sprites2d[numberofoptions, 2];
        int optionselected = 0;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            displaywidth = graphics.GraphicsDevice.Viewport.Width;
            displayheight = graphics.GraphicsDevice.Viewport.Height;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (File.Exists(@"highscore.txt"))
            {
                String line;
                StreamReader sr = new StreamReader(@"highscore.txt");
                line = sr.ReadLine();
             
                line = line.Trim();
                for (int i = 0; i < numberofhighscores &! sr.EndOfStream; i++)
                    highscores[i] = Convert.ToInt32(line);

                sr.Close();
            }

            background = new graphics2d(Content, "Background for Menus", displaywidth, displayheight);
            mousepointer1 = new sprites2d(Content, "X-Games-Cursor", 0, 0, 0.15f, Color.White, true, randomiser);
            mousepointer2 = new sprites2d(Content, "X-Games-Cursor-Highlight", 0, 0, 0.15f, Color.White, true,randomiser);

            menuoptions[0, 0] = new sprites2d(Content, "Start-Normal", displaywidth / 2, 200, 1, Color.White, true,randomiser);
            menuoptions[0, 1] = new sprites2d(Content, "Start-Selected", displaywidth / 2, 200, 1,Color.White, true,randomiser);
            menuoptions[1, 0] = new sprites2d(Content, "Options-Normal", displaywidth / 2, 300, 1, Color.White, true,randomiser);
            menuoptions[1, 1] = new sprites2d(Content, "options-Selected", displaywidth / 2, 300, 1, Color.White, true,randomiser);
            menuoptions[2, 0] = new sprites2d(Content, "High-Score-Normal", displaywidth / 2, 400, 1, Color.White, true,randomiser);
            menuoptions[2, 1] = new sprites2d(Content, "High-Score-Selected", displaywidth / 2, 400, 1, Color.White, true,randomiser);
            menuoptions[3, 0] = new sprites2d(Content, "Exit-Normal", displaywidth / 2, 500, 1, Color.White, true,randomiser);
            menuoptions[3, 1] = new sprites2d(Content, "Exit-Selected", displaywidth / 2, 500, 1, Color.White, true,randomiser);

            for (int i = 0; i < numberofoptions; i++)
                menuoptions[i, 0].updateobject();

            mainfont = Content.Load<SpriteFont>("mainfont");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

            StreamWriter sw = new StreamWriter(@"highscore.txt");
            for (int i = 0; i < numberofhighscores; i++)
            sw.WriteLine(highscores[i].ToString());
            sw.Close();
      
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            pad[0] = GamePad.GetState(PlayerIndex.One);
            pad[1] = GamePad.GetState(PlayerIndex.Two);
            pad[2] = GamePad.GetState(PlayerIndex.Three);
            pad[3] = GamePad.GetState(PlayerIndex.Four);
            keys = Keyboard.GetState();
            mouse = Mouse.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float timebetweenupdates = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            gameruntime += timebetweenupdates;

            mousepointer1.position.X = mouse.X;
            mousepointer1.position.Y = mouse.Y;
            mousepointer1.updateobject();

            mousepointer1.bsphere = new BoundingSphere(mousepointer1.position, 2);

            switch (gamestate)
            {
                case -1:
                    updatemenu();
                    break;
                case 0:
                    updategame(timebetweenupdates);
                    break;
                case 1:
                    updateoptions();
                    break;

                case 2:
                    updatehighscore();
                    break;
                default:
                    this.Exit();
                    break;

            }
          

            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        public void updatemenu()
        {
            optionselected = -1;

            for (int i = 0; i < numberofoptions; i++)
            {
                if (mousepointer1.bsphere.Intersects(menuoptions[i, 0].bbox))
                {
                    optionselected = i;
                    if (mouse.LeftButton == ButtonState.Pressed)
                        gamestate = optionselected;
                }
            }
        }
        public void drawmenu()
        {
            spriteBatch.Begin();

            for (int i = 0; i < numberofoptions; i++)
            {
                if (optionselected == i)
                    menuoptions[i, 1].drawme(ref spriteBatch);
                else
                    menuoptions[i, 0].drawme(ref spriteBatch);
            }
            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);

            spriteBatch.End();
        }

        public void updategame(float gtime)
        {
            if (!gameover)
            {
                gameover = true;
            }
            else
                if (keys.IsKeyDown(Keys.Escape)) gamestate = -1;
        }
        public void drawgame()
        {

        }

        public void updateoptions()
        {
            if (keys.IsKeyDown(Keys.Escape)) gamestate = -1;
        }
        public void drawoptions()
        {
            spriteBatch.Begin();

            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);

            spriteBatch.End();
        }

        public void updatehighscore()
        {
            Array.Sort(highscores);
            Array.Reverse(highscores);

            if (keys.IsKeyDown(Keys.Escape)) gamestate = -1;
        }
        public void drawhighscore()
        {
            spriteBatch.Begin();

            for (int i = 0; i < numberofhighscores; i++)
                spriteBatch.DrawString(mainfont, (i+1).ToString("0") +".  " + highscores[i].ToString("0"), new Vector2(displaywidth / 2 -30, 100 +(i * 30)), Color.White, 0,
                new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);

            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);

            spriteBatch.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            background.drawme(ref spriteBatch);

            spriteBatch.End();

            switch (gamestate)
            {
                case -1:
                    drawmenu();
                    break;
                case 0:
                    drawgame();
                    break;
                case 1:
                    drawoptions();
                    break;

                case 2:
                    drawhighscore();
                    break;
                default:
                    this.Exit();
                    break;

            }

            base.Draw(gameTime);
        }
    }
}
