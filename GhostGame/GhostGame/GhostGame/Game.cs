using System;
using System.Collections.Generic;
using System.Linq;
using GhostGame.Actors;
using GhostGame.Enums;
using GhostGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GhostGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static Rectangle backgroundRectangle;
        private Ghost Ghost1;
        private Texture2D ghostTexture;
        private SpriteFont font;
        private GraphicsDeviceManager graphics;
        public static Hunter[] Hunters;
        private Texture2D[] hunterTextures;
        public static Texture2D mapTexture;
        private SpriteBatch spriteBatch;
        private PrimitiveBatch primitiveBatch;
        public static Viewport viewport;
        private TimeSpan gameTimer;
        public static Rectangle[] wallBoundingBoxes;
        private Texture2D boxes;
        private Texture2D boxoutline;
        public static Color[] mapTextureData;
        public static Color[] hunterTextureData;
        public static Color[] ghostTextureData;
        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            viewport = graphics.GraphicsDevice.Viewport;
            hunterTextures = new Texture2D[4];
            Hunters = new Hunter[4];
            backgroundRectangle = new Rectangle(0, 0, (int)(viewport.Width), (int)(viewport.Height));
            base.Initialize();

            gameTimer = TimeSpan.FromMinutes(3);
            Hunters[0] = new Hunter(0, hunterTextures[0], new Vector2(100, 100), 0);
            //Hunters[1] = new Hunter(1, hunterTextures[1], new Vector2(1075, 150), 0);
            //Hunters[2] = new Hunter(2, hunterTextures[2], new Vector2(100, 300), 0);
            //Hunters[3] = new Hunter(3, hunterTextures[3], new Vector2(100, 500), 0);
            Ghost1 = new Ghost(5, ghostTexture, new Vector2(325,300), 0);
            wallBoundingBoxes = MapBoundingBoxGenerator.generateBoundingBoxesArray(boxes);
            mapTextureData = new Color[GhostGame.Game.backgroundRectangle.Width * GhostGame.Game.backgroundRectangle.Height];
            GhostGame.Game.mapTexture.GetData(GhostGame.Game.mapTextureData);
            hunterTextureData = new Color[hunterTextures[0].Width * hunterTextures[0].Height];
            hunterTextures[0].GetData(hunterTextureData);
            ghostTextureData = new Color[ghostTexture.Width * ghostTexture.Height];
            ghostTexture.GetData(ghostTextureData);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            primitiveBatch = new PrimitiveBatch(GraphicsDevice);
            BasicEffect effect = new BasicEffect(GraphicsDevice);
            effect.LightingEnabled = false;
            effect.TextureEnabled = false;
            hunterTextures[0] = Content.Load<Texture2D>("PlayerSprites/redPlayer");
            hunterTextures[1] = Content.Load<Texture2D>("PlayerSprites/bluePlayer");
            hunterTextures[2] = Content.Load<Texture2D>("PlayerSprites/yellowPlayer");
            hunterTextures[3] = Content.Load<Texture2D>("PlayerSprites/greenPlayer");
            ghostTexture = Content.Load<Texture2D>("PlayerSprites/ghostPlayer");
            mapTexture = Content.Load<Texture2D>("StandardLevel");
            font = Content.Load<SpriteFont>("StandardFont");
            boxes = Content.Load<Texture2D>("StandardLevelBounds");
            boxoutline = Content.Load<Texture2D>("boxoutline");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            for (int i = 0; i < 4; i++)
            {
                if (Hunters[i] == null)
                    continue;
                
                Hunters[i].ProcessInput(GamePad.GetState(PlayerIndexMapping.map[i]));
                Hunters[i].ProcessKeyboardArrows(Keyboard.GetState());
                if (Utils.CollisionUtilities.SmartCollisionCheck(Hunters[i], Ghost1))
                {
                    Ghost1.health++;
                }
            }

            Ghost1.ProcessInput(Keyboard.GetState());

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.Draw(mapTexture, backgroundRectangle, Color.White);

            foreach (var hunter in Hunters)
            {
                if (hunter == null)
                {
                    continue;
                }
                hunter.Draw(spriteBatch);
                if (hunter.flashlightActive)
                {
                    var flashvectors = hunter.GetFlashlightTrianglePoints();
                    //primitiveBatch.Begin(PrimitiveType.TriangleList);
                    //primitiveBatch.AddVertex(flashvectors[0], Color.Yellow);
                    //primitiveBatch.AddVertex(flashvectors[1], Color.Yellow);
                    //primitiveBatch.AddVertex(flashvectors[2], Color.Yellow);
                    //primitiveBatch.End();

                    this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, flashvectors, 0, 1);
                }
            }

            Ghost1.Draw(spriteBatch);
            
            spriteBatch.DrawString(font, "Ghost: " + Ghost1.health + "%", new Vector2(0, 0), Color.Black);
            spriteBatch.DrawString(font, "flashlength: " + Hunters[0].FlashlightLength().ToString(), new Vector2(200, 0), Color.Black);
            spriteBatch.DrawString(font, "flashpercent: " + Hunters[0].FlashlightPercentage().ToString(), new Vector2(500, 50), Color.Black);
            //spriteBatch.DrawString(font, "Rotation: " + Hunters[0].Rotation, new Vector2(0, 0), Color.Black);

            //for (int i = 0; i < GhostGame.Game.wallBoundingBoxes.Length; i++)
            //{
            //    spriteBatch.Draw(boxoutline, wallBoundingBoxes[i], Color.White);
            //}
            var currentTime = gameTimer - gameTime.TotalGameTime;
            string secondString= "";
            if (currentTime.Seconds % 60 < 10)
                secondString = "0";
            secondString += (currentTime.Seconds % 60);
            spriteBatch.DrawString(font, currentTime.Minutes + ":" + secondString, new Vector2(600, 75), Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
