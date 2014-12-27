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
using Mahjong;
using ShantenHelper.Tiles;
using System.Collections.ObjectModel;

namespace ShantenHelper
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont shantenFont;
        
        Vector2 shantenTextPosition;
        string shantenText = "Shanten #: {0}";
        string shantenWarning = "You need to have 14 tiles in hand to get the shanten number";
        Vector2 shantenWarningPosition;

        private bool tenpai = false;
        private string tenpaiText = "Tenpai";

        string handText = "Your hand:";
        string handTextWarning = "Your hand is empty, click on a tile to add it";
        Vector2 handTextPosition;

        ICollection<TileGameObject> SampleTiles;
        ObservableCollection<TileGameObject> PlayerHandTiles;

        int? ShantenNumber = null;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 700;
        }

        private const int TILE_WIDTH = 42;
        private const int TILE_HEIGHT = 64;

        private const int MARGIN_LEFT = 20;
        private const int MARGIN_TOP = 20;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            //Setting up one of each tile
            this.SampleTiles = new List<TileGameObject>(34);
            var Tiles = new List<Tile>(34);
            Tiles.AddRange(SuitedTile.GetAll());
            Tiles.AddRange(HonorTile.GetAll());
            Tiles.AddRange(DragonTile.GetAll());

            int x = 0;
            int y = 0;
            Vector2 TopLeft = new Vector2(MARGIN_LEFT, MARGIN_TOP);
            foreach (var tile in Tiles)
            {
                var TileComp = new TileGameObject(this);
                TileComp.Tile = tile;
                TileComp.Position = TopLeft + new Vector2(x * TILE_WIDTH, y * TILE_HEIGHT);

                this.Components.Add(TileComp);
                this.SampleTiles.Add(TileComp);

                if (x == 8)
                {
                    x = 0;
                    y++;
                }
                else
                {
                    x++;
                }
            }

            //Setting up player's hand
            this.PlayerHandTiles = new ObservableCollection<TileGameObject>();
            this.PlayerHandTiles.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PlayerHandTiles_CollectionChanged);
            //Text
            this.shantenTextPosition = new Vector2(MARGIN_LEFT, 350);
            this.shantenWarningPosition = shantenTextPosition + new Vector2(0, -25);
            this.handTextPosition = shantenTextPosition + new Vector2(0, +25);
            base.Initialize();
        }

        void PlayerHandTiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:

                    IDictionary<Tile,float> ShantenTiles;
                    float BaseIntensity = 0;

                    if (this.PlayerHandTiles.Count == 14)
                    {
                        var hand = this.PlayerHandTiles.Select(t => t.Tile).ToList();
                        //var foo = Mahjong.Hand.GetShantenTiles(hand, new List<Mahjong.Meld>());
                        //ShantenTiles = foo.Value;
                        //ShantenNumber = foo.Key;
                        ShantenTiles = Mahjong.Async.Players.AIShantenPlus.GetUndesirability(hand, out ShantenNumber);

                        this.tenpai = ShantenNumber == 0;

                        if (ShantenTiles.Count > 0)
                        {
                            BaseIntensity = -1f * ShantenTiles.Values.Min();

                            //Mark the "sample" tiles as non highlighteable
                            foreach (var item in this.SampleTiles)
                            {
                                item.Highlighteable = false;
                            }
                        }
                        else if (this.tenpai)
                        {
                            var riichiDiscards = Hand.GetDiscardsForTenpai(typeof(ScorerJapanese), hand, new List<Mahjong.Meld>(), null, null);
                        }
                    }
                    else
                    {
                        ShantenTiles = new Dictionary<Tile,float>();
                        ShantenNumber = null;
                        this.tenpai = false;

                        //Mark the "sample" tiles as highlighteable
                        foreach (var item in this.SampleTiles)
                        {
                            item.Highlighteable = true;
                        }
                    }

                    int i = 0;
                    foreach (var item in this.PlayerHandTiles.OrderBy(t => t.Tile.Value()))
                    {
                        item.Position = new Vector2(MARGIN_LEFT + i * TILE_WIDTH, 400);
                        item.RecommendedDiscard = ShantenTiles.Keys.Any(shanten => shanten.Value() == item.Tile.Value());
                        if (item.RecommendedDiscard)
                        {
                            item.RecommendedDiscardIntensity = ShantenTiles[item.Tile] + BaseIntensity;
                        }
                        else
                        {
                            item.RecommendedDiscardIntensity = null;
                        }
                        i++;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);

            // TODO: use this.Content to load your game content here
            this.shantenFont = Content.Load<SpriteFont>("CourierNew");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        MouseState lastMouseState;
        MouseState currentMouseState;

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

            // TODO: Add your update logic here
            #region Input
            if (this.IsActive) //Handle Input
            {
                lastMouseState = currentMouseState;
                currentMouseState = Mouse.GetState();
                
                // Recognize a single click of the left mouse button
                if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    var SelectedSampleTile = this.SampleTiles.LastOrDefault(t => t.Highlighted);
                    if (SelectedSampleTile != null && this.PlayerHandTiles.Count < 14)
                    {
                        var newTileComp = new TileGameObject(this);
                        newTileComp.Tile = SelectedSampleTile.Tile;
                        //newTileComp.Position = new Vector2(MARGIN_LEFT + this.PlayerHandTiles.Count * TILE_WIDTH , 400);
                        this.PlayerHandTiles.Add(newTileComp);
                        this.Components.Add(newTileComp);
                    }
                    else
                    {
                        var SelectedHandTile = this.PlayerHandTiles.LastOrDefault(t => t.Highlighted);
                        this.Components.Remove(SelectedHandTile);
                        this.PlayerHandTiles.Remove(SelectedHandTile);
                    }
                }
            }
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            this.spriteBatch.Begin();

            if (this.tenpai)
            {
                this.spriteBatch.DrawString(this.shantenFont, this.tenpaiText, shantenTextPosition, Color.DarkRed);
            }
            else
            {
                this.spriteBatch.DrawString(this.shantenFont, string.Format(shantenText, this.ShantenNumber.HasValue ? this.ShantenNumber.Value.ToString() : "N/A"), shantenTextPosition, Color.Black);
            }

            if (!this.ShantenNumber.HasValue)
            {
                this.spriteBatch.DrawString(this.shantenFont, shantenWarning, shantenWarningPosition, Color.DarkRed);
            }
            if (this.PlayerHandTiles.Count == 0)
            {
                this.spriteBatch.DrawString(this.shantenFont, handTextWarning, handTextPosition, Color.DarkRed);
            }
            else
            {
                this.spriteBatch.DrawString(this.shantenFont, handText, handTextPosition, Color.Black);
            }
            this.spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
