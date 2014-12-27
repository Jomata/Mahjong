using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mahjong;
using Mahjong.Async.Players;
using Microsoft.Xna.Framework.Input;

namespace MahjongXNA.Screens
{
    public class MahjongTableScreen : GameScreen
    {
        struct DrawableStringInfo
        {
            public string Text { get; set; }
            public float Rotation { get; set; }
            public Vector2 Position { get; set; }
            public Color Color { get; set; }
            public SpriteFont Font { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        private Vector2 TopCenter { get { return new Vector2(GraphicsDevice.Viewport.Width / 2, 0); } }
        private Vector2 BotCenter { get { return new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height); } }
        private Vector2 LeftCenter { get { return new Vector2(0, GraphicsDevice.Viewport.Height / 2); } }
        private Vector2 RightCenter { get { return new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2); } }

        private static int Margin = 10;
        private static int TilesPerDiscardRow = 6;
        private ScoreKeeper Score = new ScoreKeeper();

        Mahjong.Async.Game MJGame;
        HmnXNA Player;
        SpriteFont Font_CourierNew;

        Color SelectableTileColor_Highlighted = Color.LightPink;
        Color SelectableTileColor_MouseOver = Color.DeepPink;
        Color SelectableTileColor_RiichiDiscard = Color.HotPink;
        Color SelectableTileColor_Recommended = Color.HotPink;
        bool recommendDiscards = true;
        bool tileToolTips = true;
        Color BoardBGColor = Color.DarkOliveGreen;

        Dictionary<Mahjong.IPlayer, BoardPosition> PlayerPositions;

        IDictionary<string, Texture2D> TileTextures = new Dictionary<string, Texture2D>(9 * 3 + 4 + 3);
        Dictionary<string, List<Tile>> RecommendedTilesCache = new Dictionary<string, List<Tile>>();
        Dictionary<string, int> ShantenCahe = new Dictionary<string, int>();
        Texture2D BackTileTexture;
        Texture2D RiichiStickTexture;
        
        List<TileTextureInfo> TilesToDraw = new List<TileTextureInfo>();
        List<Tile> SelectableTiles = new List<Tile>();
        Tile SelectedTile = null;
        List<Tile> RiichiTiles = new List<Tile>();
        IDictionary<IPlayer, DrawableStringInfo> PlayerNames = new Dictionary<IPlayer, DrawableStringInfo>();

        Vector2 tooltipPosition = Vector2.Zero;
        string tooltipText = String.Empty;
        Texture2D tooltipBG;
        Color tooltipBGColor = Color.Gold;

        string BoardMessage = "";

        ContentManager content;
        ContentManager Content
        {
            get
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                return content;
            }
        }

        private void LoadConfigValues()
        {
            #region Loading Config info
            this.SelectableTileColor_Highlighted = Colors.FromName(GetConfigValue("SelectableTileColor_Highlighted"));
            this.SelectableTileColor_MouseOver = Colors.FromName(GetConfigValue("SelectableTileColor_MouseOver"));
            this.SelectableTileColor_RiichiDiscard = Colors.FromName(GetConfigValue("SelectableTileColor_RiichiDiscard"));
            this.SelectableTileColor_Recommended = Colors.FromName(GetConfigValue("SelectableTileColor_Recommended"));
            bool.TryParse(GetConfigValue("recommendDiscards"), out this.recommendDiscards);
            bool.TryParse(GetConfigValue("tileToolTips"), out this.tileToolTips);
            #endregion
        }

        private void UpdatePlayerPositionsAndWinds()
        {
            foreach (var Player in this.MJGame.Players)
            {
                if (!this.PlayerNames.ContainsKey(Player) && Player != this.Player)
                {
                    var info = new DrawableStringInfo();
                    info.Text = String.Format("{0} ({1})", Player.Name, Player.PlayerWind);
                    info.Font = this.Font_CourierNew;
                    info.Color = Color.Black;
                    var stringWidth = this.Font_CourierNew.MeasureString(info.Text).X;
                    var stringHeight = this.Font_CourierNew.MeasureString(info.Text).Y;
                    info.Width = (int)stringWidth;
                    info.Height = (int)stringHeight;

                    switch (this.PlayerPositions[Player])
                    {
                        case BoardPosition.Top:
                            info.Position = TopCenter + new Vector2(-1 * stringWidth / 2, Margin + this.BackTileTexture.Height + Margin);
                            break;
                        case BoardPosition.Bot:
                            break;
                        case BoardPosition.Left:
                            info.Position = LeftCenter + new Vector2(Margin + this.BackTileTexture.Height + Margin + stringHeight, -1 * stringWidth / 2);
                            info.Rotation = (float)Math.PI / 2.0f;  // 90 degrees
                            break;
                        case BoardPosition.Right:
                            info.Position = RightCenter - new Vector2(Margin + this.BackTileTexture.Height + Margin + stringHeight, -1 * stringWidth / 2);
                            info.Rotation = (float)Math.PI * 1.5f;  // 90 degrees
                            break;
                        default:
                            break;
                    }

                    this.PlayerNames.Add(Player, info);
                }
            }
        }

        private void SetupGame()
        {
            this.Player = new HmnXNA();
            this.Player.Name = "Human";

            var AIDelay = TimeSpan.FromSeconds(0.75);

            this.MJGame = new Mahjong.Async.Game();
            this.MJGame.RegisterPlayer(this.Player);
            this.MJGame.RegisterPlayer(new AIShantenDelayed { Name = "Articuno", Delay = AIDelay });
            this.MJGame.RegisterPlayer(new AIShantenDelayed { Name = "Zapdos", Delay = AIDelay });
            this.MJGame.RegisterPlayer(new AIShantenDelayed { Name = "Moltres", Delay = AIDelay });

            this.MJGame.PlayerWin += new Mahjong.Async.PlayerWinEventHandler(MJGame_PlayerWin);
            this.MJGame.NoTilesLeft += new EventHandler(MJGame_NoTilesLeft);

            this.PlayerPositions = new Dictionary<IPlayer, BoardPosition>();
            this.PlayerPositions.Add(this.Player, BoardPosition.Bot);
            this.Score.RegisterPlayer(this.Player);
            var currPlayer = this.MJGame.GetPlayerAfter(this.Player);
            this.PlayerPositions.Add(currPlayer, BoardPosition.Left);
            this.Score.RegisterPlayer(currPlayer);
            currPlayer = this.MJGame.GetPlayerAfter(currPlayer);
            this.PlayerPositions.Add(currPlayer, BoardPosition.Top);
            this.Score.RegisterPlayer(currPlayer);
            currPlayer = this.MJGame.GetPlayerAfter(currPlayer);
            this.PlayerPositions.Add(currPlayer, BoardPosition.Right);
            this.Score.RegisterPlayer(currPlayer);
            
            this.Player.AskForRiichi += new EventHandler(Player_AskForRiichi);
            this.Player.AskForTsumo += new EventHandler<EventArg<Tile>>(Player_AskForTsumo);
            this.Player.AskForCompleteQuad += new EventHandler<EventArg<Tile>>(Player_AskForCompleteQuad);
            this.MJGame.PlayerRiichi += new Mahjong.Async.PlayerRiichi(Game_PlayerRiichi);

            this.MJGame.Play();
        }

        void Game_PlayerRiichi(Mahjong.Async.Player sender)
        {
            this.RiichiTiles.Add(sender.Game.CurrentDiscardTile);
        }

        private void PostGameHandler(object sender, PlayerIndexEventArgs e)
        {
            GameOverScreen = true;
            this.showScoresScreen();
        }

        private void showScoresScreen()
        {
            ScoreScreen scoreScreen = new ScoreScreen(this.Score);
            ScreenManager.AddScreen(scoreScreen, ControllingPlayer);
            scoreScreen.ScreenClosed += new EventHandler<PlayerIndexEventArgs>(scoreScreen_ScreenClosed);
        }

        bool GameOverScreen = false;
        void scoreScreen_ScreenClosed(object sender, PlayerIndexEventArgs e)
        {
            if (GameOverScreen)
            {
                this.StartGame();
            }
        }

        private void StartGame()
        {
            GameOverScreen = false;
            this.RiichiTiles.Clear();
            this.MJGame.Play();
        }

        void MJGame_NoTilesLeft(object sender, EventArgs e)
        {
            IDictionary<IPlayer, int> Payments = new Dictionary<IPlayer, int>(4);
            foreach (var player in this.MJGame.Players)
            {
                Payments.Add(player, 0);
            }
            this.Score.RegisterRoundPayments(Payments);

            string msg = "Ran out of tiles";
            MessageBoxScreen OutOfTilesScreen = new MessageBoxScreen(msg, false);
            OutOfTilesScreen.Accepted += new EventHandler<PlayerIndexEventArgs>(PostGameHandler);
            OutOfTilesScreen.Cancelled += new EventHandler<PlayerIndexEventArgs>(PostGameHandler);
            ScreenManager.AddScreen(OutOfTilesScreen, ControllingPlayer);
        }

        void MJGame_PlayerWin(Mahjong.Async.Player Winner, List<Meld> Concealed, List<Meld> Exposed, Tile WinningTile, Mahjong.Async.Player PlayerHit, IDictionary<IPlayer, int> Payments)
        {
            Score.RegisterRoundPayments(Payments);

            StringBuilder winMessage = new StringBuilder();
            winMessage.AppendFormat("Player {0} wins!", Winner.Name);
            winMessage.AppendLine();

            if (PlayerHit == null)
            {
                winMessage.AppendLine("Tsumo with " + WinningTile.ToLongString());
            }
            else
            {
                winMessage.AppendFormat("Ron on {0}'s {1}", PlayerHit.Name, WinningTile.ToLongString());
                winMessage.AppendLine();
            }

            var scorer = new Mahjong.ScorerJapanese(Concealed, Exposed, this.MJGame, Winner);
            var hans = scorer.GetHan();
            foreach (var han in hans)
            {
                winMessage.AppendFormat("> {0}", han);
                winMessage.AppendLine();
            }
            var fus = scorer.GetFu();
            foreach (var fu in fus)
            {
                var fuString = fu.ToString();

                if (fuString.Length > 40)
                    fuString = fuString.Substring(0, 40) + String.Empty.PadLeft(3, '.');

                winMessage.AppendFormat(">> {0}", fuString);
                winMessage.AppendLine();
            }

            winMessage.AppendLine();
            //winMessage.AppendLine();

            foreach (var payer in Payments)
            {
                if (payer.Value < 0)
                {
                    winMessage.AppendLine();
                    winMessage.AppendFormat("{0} pays {1}", payer.Key.Name, -1 * payer.Value);
                }
            }

            MessageBoxScreen GameOverMessageBox = new MessageBoxScreen(winMessage.ToString(),false);
            //GameOverMessageBox.IsPopup = true;
            GameOverMessageBox.Accepted += new EventHandler<PlayerIndexEventArgs>(PostGameHandler);
            GameOverMessageBox.Cancelled += new EventHandler<PlayerIndexEventArgs>(PostGameHandler);
            ScreenManager.AddScreen(GameOverMessageBox, ControllingPlayer);
        }

        public MahjongTableScreen() : base()
        {
            LoadConfigValues();
            SetupGame();
        }

        void Player_AskForCompleteQuad(object sender, EventArg<Tile> e)
        {
            var confirmQuadMessageBox = new YesNoDialogScreen(this.Player.Message);
            confirmQuadMessageBox.Accepted += new EventHandler<PlayerIndexEventArgs>(confirmQuadMessageBox_Accepted);
            confirmQuadMessageBox.Cancelled += new EventHandler<PlayerIndexEventArgs>(confirmQuadMessageBox_Cancelled);
            ScreenManager.AddScreen(confirmQuadMessageBox, ControllingPlayer);
        }

        void confirmQuadMessageBox_Accepted(object sender, PlayerIndexEventArgs e)
        { this.Player.AskForCompleteQuadCallback(true); }

        void confirmQuadMessageBox_Cancelled(object sender, PlayerIndexEventArgs e)
        { this.Player.AskForCompleteQuadCallback(false); }

        void Player_AskForTsumo(object sender, EventArg<Tile> e)
        {
            string message = String.Format("Do you want to tsumo with a {0}?",e.Data.ToLongString());
            var confirmTsumoMessageBox = new YesNoDialogScreen(message);
            confirmTsumoMessageBox.Accepted += new EventHandler<PlayerIndexEventArgs>(confirmTsumoMessageBox_Accepted);
            confirmTsumoMessageBox.Cancelled += new EventHandler<PlayerIndexEventArgs>(confirmTsumoMessageBox_Cancelled);
            ScreenManager.AddScreen(confirmTsumoMessageBox, ControllingPlayer);
        }

        void confirmTsumoMessageBox_Accepted(object sender, PlayerIndexEventArgs e)
        { this.Player.AskForTsumoCallback(true); }

        void confirmTsumoMessageBox_Cancelled(object sender, PlayerIndexEventArgs e)
        { this.Player.AskForTsumoCallback(false); }

        void Player_AskForRiichi(object sender, EventArgs e)
        {
            string message = "Do you want to riichi?";
            var confirmRiichiMessageBox = new YesNoDialogScreen(message);
            confirmRiichiMessageBox.Accepted += new EventHandler<PlayerIndexEventArgs>(confirmRiichiMessageBox_Accepted);
            confirmRiichiMessageBox.Cancelled += new EventHandler<PlayerIndexEventArgs>(confirmRiichiMessageBox_Cancelled);
            ScreenManager.AddScreen(confirmRiichiMessageBox, ControllingPlayer);
        }

        void confirmRiichiMessageBox_Accepted(object sender, PlayerIndexEventArgs e)
        {
            this.Player.AskForRiichiCallback(true);
        }

        void confirmRiichiMessageBox_Cancelled(object sender, PlayerIndexEventArgs e)
        {
            this.Player.AskForRiichiCallback(false);
        }

        private string GetConfigValue(string key)
        {
            //return System.Configuration.ConfigurationSettings.AppSettings[key];
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        private Point MousePosition
        {
            get
            {
                var mouseState = Mouse.GetState();
                var mousePosition = new Point(mouseState.X, mouseState.Y);
                return mousePosition;
                //return new Point(0, 0);
            }
        }

        private void LoadTileTextures()
        {
            var allTiles = Tile.GetAllTiles();
            foreach (var tile in allTiles)
            {
                if (!TileTextures.ContainsKey(tile.ToString()))
                    TileTextures.Add(tile.ToString(), Content.Load<Texture2D>(tile.GetTexturePath()));
            }
            BackTileTexture = Content.Load<Texture2D>(Tile.Unknown().GetTexturePath());
        }

        private SpriteBatch spriteBatch
        {
            get { return ScreenManager.SpriteBatch; }
        }

        private void DrawPlayerNames()
        {
            foreach (var playerNameInfo in this.PlayerNames.Values)
            {
                var text = new Texture2D(ScreenManager.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                text.SetData<Color>(new Color[] { BoardBGColor });
                var rect = new Rectangle((int)playerNameInfo.Position.X, (int)playerNameInfo.Position.Y, playerNameInfo.Width, playerNameInfo.Height);
                spriteBatch.Draw(text, rect, null, Color.White, playerNameInfo.Rotation, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.DrawString(playerNameInfo.Font, playerNameInfo.Text, playerNameInfo.Position, playerNameInfo.Color,playerNameInfo.Rotation,Vector2.Zero,1.0f,SpriteEffects.None,0);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.DarkOliveGreen);
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, BoardBGColor, 0, 0);
            spriteBatch.Begin();

            //If SpiteSortMode = Immediate, we have to draw the tiles first, else, we draw them last
            DrawTiles();
            DrawPlayerNames();
            DrawTooltip();
            DrawMessage();
            DrawRiichiSticks();

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void HandleTileSelected(Tile SelectedTile)
        {
            if (this.Player.Status == HmnXNAStatus.AskingIntercept || this.Player.Status == HmnXNAStatus.AskingRon)
            {
                this.Player.TileWasClicked(SelectedTile);
                this.SelectedTile = null;
            }
            else if (this.Player.Status == HmnXNAStatus.ChoosingDiscard && SelectedTile != null)
            {
                this.Player.TileWasClicked(SelectedTile);
                this.SelectedTile = null;
            }
            else if (this.Player.Status == HmnXNAStatus.ChoosingMeld && SelectedTile != null)
            {
                this.Player.AskForMeldCallback(this.getSelectedMeld());
            }
        }

        private void ChangeSelected(int direction)
        {
            if (this.SelectableTiles.Count > 0)
            {
                if (this.SelectedTile == null)
                {
                    this.SelectedTile = this.SelectableTiles.First();
                }
                else if (this.lastTileSelectionChange + this.timeBetweenTileSelectionChanges < DateTime.Now)
                {
                    this.lastTileSelectionChange = DateTime.Now;
                    int selectedIndex = this.SelectableTiles.IndexOf(this.SelectedTile);
                    int newSelectedIndex = (SelectableTiles.Count + selectedIndex + direction) % SelectableTiles.Count;
                    this.SelectedTile = this.SelectableTiles[newSelectedIndex];
                }
            }
        }

        TimeSpan timeBetweenTileSelectionChanges = TimeSpan.FromSeconds(0.1);
        DateTime lastTileSelectionChange = DateTime.Now;
        public override void HandleInput(InputState input)
        {
            PlayerIndex foo;
            int playerIndex = (int)ControllingPlayer.Value;
            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || keyboardState.IsKeyDown(Keys.Escape))
            {
                var pauseMenu = new PauseMenuScreen();
                ScreenManager.AddScreen(pauseMenu, ControllingPlayer);
                pauseMenu.Closed += new EventHandler<PlayerIndexEventArgs>(pauseMenu_Closed);
            }
            else if (input.IsNewLeftMouseClick())
            {
                var mousePosition = new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                var tileClicked = this.TilesToDraw.Find(tti => tti.Tile != null && tti.Rectangle.Contains(mousePosition));
                if(null != tileClicked && SelectableTiles.Contains(tileClicked.Tile))
                    HandleTileSelected(tileClicked.Tile);
                else
                    HandleTileSelected(null);
            }
            else if (input.IsNewKeyPress(Keys.Home, ControllingPlayer, out foo))
            {
                this.SelectedTile = this.SelectableTiles.First();
            }
            else if (input.IsNewKeyPress(Keys.End, ControllingPlayer,out foo))
            {
                this.SelectedTile = this.SelectableTiles.Last();
            }
            else if (input.IsNewKeyPress(Keys.S, ControllingPlayer, out foo))
            {
                //this.Score.RegisterRoundPayments(new Dictionary<IPlayer, int>
                //{
                //    { this.Player , 2000 }
                //    , {this.MJGame.Players[1] , -1000}
                //    , {this.MJGame.Players[2] , -1000}
                //});

                this.showScoresScreen();
            }
            else if (input.IsMenuSelect(ControllingPlayer, out foo))
            {
                this.HandleTileSelected(this.SelectedTile);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out foo))
            {
                this.HandleTileSelected(null);
            }
            else if (input.IsMenuRight(ControllingPlayer) || input.IsMenuUp(ControllingPlayer) || input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Right))
            {
                this.ChangeSelected(+1);
            }
            else if (input.IsMenuLeft(ControllingPlayer) || input.IsMenuDown(ControllingPlayer) || input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Left))
            {
                this.ChangeSelected(-1);
            }
            else if (keyboardState.IsKeyDown(Keys.R))
            {
                Player_AskForRiichi(null, null);
            }
            else if (input.MouseMoved())
            {
                var mousePosition = new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                var tmp = this.TilesToDraw.Find(tti => tti.Tile != null && tti.Rectangle.Contains(mousePosition));
                if (tmp != null && this.SelectableTiles.Contains(tmp.Tile))
                    this.SelectedTile = tmp.Tile;
                else
                    this.SelectedTile = null;
            }

            base.HandleInput(input);
        }

        void pauseMenu_Closed(object sender, PlayerIndexEventArgs e)
        {
            this.LoadConfigValues();
        }

        public override void LoadContent()
        {
            LoadTileTextures();
            this.Font_CourierNew = Content.Load<SpriteFont>("CourierNew");

            this.tooltipBG = new Texture2D(ScreenManager.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this.tooltipBG.SetData<Color>(new Color[] { this.tooltipBGColor });
            this.RiichiStickTexture = Content.Load<Texture2D>("img/riichi_100x10");

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        void UpdateTooltip()
        {
            if (tileToolTips)
            {
                this.tooltipPosition = Vector2.Zero;
                this.tooltipText = String.Empty;

                var hoveredTile = this.TilesToDraw.Find(tti => tti.Tile != null && tti.Rectangle.Contains(MousePosition));
                if (hoveredTile != null)
                {
                    this.tooltipPosition = new Vector2(MousePosition.X + 15, MousePosition.Y);
                    this.tooltipText = hoveredTile.Tile.ToLongString();

                    if (this.MJGame.DoraIndicators.Contains(hoveredTile.Tile))
                    {
                        this.tooltipText += String.Format(" (Dora is {0})", hoveredTile.Tile.IsDoraIndicatorOf().ToLongString());
                    }

                    var tooltipTextSize = Font_CourierNew.MeasureString(this.tooltipText);
                    if (tooltipPosition.X + tooltipTextSize.X > this.GraphicsDevice.Viewport.Width)
                    {
                        tooltipPosition.X -= tooltipTextSize.X + 20; //cursor width?
                    }
                }
            }
        }

        private void DrawRiichiSticks()
        {
            int discardPoolWidth = (TilesPerDiscardRow - 1) * this.BackTileTexture.Width + 1 * this.BackTileTexture.Height; //5 tiles normal, 1 sideway tile for riichi
            int discardPoolHeight = 3 * this.BackTileTexture.Height;
            int paddingUnderDiscardPool = Margin + BackTileTexture.Height + Margin;
            float angle = 0;

            Vector2 riichiCenter = Vector2.Zero;

            foreach (var player in this.MJGame.Players)
            {
                if (player.Riichi)
                {
                    switch (this.PlayerPositions[player])
                    {
                        case BoardPosition.Top:
                            riichiCenter = TopCenter;
                            riichiCenter.Y += paddingUnderDiscardPool + discardPoolHeight;
                            riichiCenter.X -= this.RiichiStickTexture.Width / 2;
                            riichiCenter.Y += this.RiichiStickTexture.Height;
                            angle = 0;
                            break;
                        case BoardPosition.Bot:
                            riichiCenter = BotCenter;
                            riichiCenter.Y -= paddingUnderDiscardPool + discardPoolHeight;
                            riichiCenter.X -= this.RiichiStickTexture.Width / 2;
                            riichiCenter.Y -= 2 * this.RiichiStickTexture.Height;
                            angle = 0;
                            break;
                        case BoardPosition.Left:
                            riichiCenter = LeftCenter;
                            riichiCenter.X += paddingUnderDiscardPool + discardPoolHeight;
                            riichiCenter.Y -= this.RiichiStickTexture.Width / 2;
                            riichiCenter.X += 2 * this.RiichiStickTexture.Height;
                            angle = (float)Math.PI / 2.0f;
                            break;
                        case BoardPosition.Right:
                            riichiCenter = RightCenter;
                            riichiCenter.X -= paddingUnderDiscardPool + discardPoolHeight;
                            riichiCenter.Y -= this.RiichiStickTexture.Width / 2;
                            riichiCenter.X -= this.RiichiStickTexture.Height;
                            angle = (float)Math.PI / 2.0f;
                            break;
                    }

                    spriteBatch.Draw(this.RiichiStickTexture, riichiCenter, null, Color.White, angle, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
                }
            }
        }

        void UpdatePlayersDiscardPools()
        {
            foreach (var player in this.MJGame.Players)
            {
                Vector2 discardPoolStart = Vector2.Zero;
                Vector2 riichiTilePositionMod = Vector2.Zero;
                Vector2 riichiTilePositionOffset = Vector2.Zero;
                int sidewaysTileDiff = (BackTileTexture.Height - BackTileTexture.Width)/2;
                int discardPoolWidth = (TilesPerDiscardRow - 1) * this.BackTileTexture.Width + 1 * this.BackTileTexture.Height; //5 tiles normal, 1 sideway tile for riichi
                int discardPoolHeight = 3 * this.BackTileTexture.Height;
                int paddingUnderDiscardPool = Margin + BackTileTexture.Height + Margin;
                int tileOffsetX = 0, tileOffsetY = 0;
                float angle = 0;
                bool HorizontalTiles = true;

                switch (this.PlayerPositions[player])
                {
                    case BoardPosition.Top:
                        discardPoolStart = TopCenter;
                        discardPoolStart.X += discardPoolWidth / 2;
                        discardPoolStart.Y += paddingUnderDiscardPool + discardPoolHeight;
                        tileOffsetX = -1 * BackTileTexture.Width;
                        tileOffsetY = -1 * BackTileTexture.Height;
                        angle = (float)Math.PI;  // 180 degrees;
                        riichiTilePositionMod.X -= BackTileTexture.Width * 1.5f;
                        riichiTilePositionMod.Y -= sidewaysTileDiff;
                        riichiTilePositionOffset.X -= BackTileTexture.Height - BackTileTexture.Width;
                        break;
                    case BoardPosition.Bot:
                        discardPoolStart = BotCenter;
                        discardPoolStart.X -= discardPoolWidth / 2;
                        discardPoolStart.Y -= paddingUnderDiscardPool + discardPoolHeight;
                        tileOffsetX = +1 * BackTileTexture.Width;
                        tileOffsetY = +1 * BackTileTexture.Height;
                        riichiTilePositionMod.X += BackTileTexture.Width * 1.5f;
                        riichiTilePositionMod.Y += sidewaysTileDiff;
                        riichiTilePositionOffset.X += BackTileTexture.Height - BackTileTexture.Width;
                        break;
                    case BoardPosition.Left:
                        discardPoolStart = LeftCenter;
                        discardPoolStart.Y -= discardPoolWidth / 2;
                        discardPoolStart.X += paddingUnderDiscardPool + discardPoolHeight;
                        //discardPoolStart.Y -= BackTileTexture.Height; //compensating for rotation
                        tileOffsetX = -1 * BackTileTexture.Height;
                        tileOffsetY = +1 * BackTileTexture.Width;
                        HorizontalTiles = false;
                        angle = (float)Math.PI / 2.0f;  // 90 degrees
                        riichiTilePositionMod.X -= sidewaysTileDiff;
                        riichiTilePositionMod.Y += BackTileTexture.Width * 1.5f;
                        riichiTilePositionOffset.Y += BackTileTexture.Height - BackTileTexture.Width;
                        break;
                    case BoardPosition.Right:
                        discardPoolStart = RightCenter;
                        discardPoolStart.Y += discardPoolWidth / 2;
                        discardPoolStart.X -= paddingUnderDiscardPool + discardPoolHeight;
                        tileOffsetX = +1 * BackTileTexture.Height;
                        tileOffsetY = -1 * BackTileTexture.Width;
                        HorizontalTiles = false;
                        angle = (float)Math.PI * 1.5f;  // 270 degrees
                        riichiTilePositionMod.X += sidewaysTileDiff;
                        riichiTilePositionMod.Y -= BackTileTexture.Width * 1.5f;
                        riichiTilePositionOffset.Y -= BackTileTexture.Height - BackTileTexture.Width;
                        break;
                }



                int i = 0;
                Vector2 currentTilePosition = discardPoolStart;
                foreach (var discardedTile in new List<Tile>(player.Discards))
                {
                    var TTI = new TileTextureInfo();
                    TTI.Tile = discardedTile;
                    TTI.Texture = this.TileTextures[discardedTile.ToString()];
                    
                    //if (HorizontalTiles)
                    //    TTI.Position = new Vector2(discardPoolStart.X + (i % TilesPerDiscardRow) * tileOffsetX, discardPoolStart.Y + (i / TilesPerDiscardRow) * tileOffsetY);
                    //else
                    //    TTI.Position = new Vector2(discardPoolStart.X + (i / TilesPerDiscardRow) * tileOffsetX, discardPoolStart.Y + (i % TilesPerDiscardRow) * tileOffsetY);

                    TTI.Position = currentTilePosition;
                    if (HorizontalTiles)
                    {
                        if ((i + 1) % TilesPerDiscardRow == 0)
                        {
                            currentTilePosition.X  = discardPoolStart.X;
                            currentTilePosition.Y += tileOffsetY;
                        }
                        else
                        {
                            currentTilePosition.X += tileOffsetX;
                        }
                    }
                    else
                    {
                        if ((i + 1) % TilesPerDiscardRow == 0)
                        {
                            currentTilePosition.Y  = discardPoolStart.Y;
                            currentTilePosition.X += tileOffsetX;
                        }
                        else
                        {
                            currentTilePosition.Y += tileOffsetY;
                        }
                    }
                    

                    TTI.Rotation = angle;
                    i++;

                    if (RiichiTiles.Contains(TTI.Tile))
                    {
                        TTI.Rotation += (float)Math.PI / 2;
                        TTI.Position += riichiTilePositionMod;
                        currentTilePosition += riichiTilePositionOffset;
                    }

                    this.TilesToDraw.Add(TTI);
                }
                if(
                    (this.Player.Status == HmnXNAStatus.AskingIntercept && this.Player.PlayerDiscardingTile == player)
                    || (this.Player.Status == HmnXNAStatus.AskingRon && this.Player.PlayerDiscardingTile == player)
                )
                {
                    var TTI = new TileTextureInfo();
                    TTI.Tile = this.Player.TileBeingDiscarded;
                    TTI.Texture = this.TileTextures[this.Player.TileBeingDiscarded.ToString()];
                    //if (HorizontalTiles)
                    //    TTI.Position = new Vector2(discardPoolStart.X + (i % TilesPerDiscardRow) * tileOffsetX, discardPoolStart.Y + (i / TilesPerDiscardRow) * tileOffsetY);
                    //else
                    //    TTI.Position = new Vector2(discardPoolStart.X + (i / TilesPerDiscardRow) * tileOffsetX, discardPoolStart.Y + (i % TilesPerDiscardRow) * tileOffsetY);
                    TTI.Position = currentTilePosition;
                    TTI.Rotation = angle;

                    if (RiichiTiles.Contains(TTI.Tile))
                    {
                        TTI.Rotation += (float)Math.PI / 2;
                        TTI.Position += riichiTilePositionMod;
                    }

                    if(this.SelectedTile == TTI.Tile)
                        TTI.ShadeColor = this.SelectableTileColor_MouseOver;
                    else
                        TTI.ShadeColor = this.SelectableTileColor_Highlighted;

                    TTI.Tile = this.Player.TileBeingDiscarded;
                    this.TilesToDraw.Add(TTI);

                    this.SelectableTiles.Clear();
                    this.SelectableTiles.Add(TTI.Tile);
                }
            }
        }

        static string GetTilesKey(IList<Tile> Tiles)
        {
            return String.Join("", Tiles);
        }

        //MouseState currentMouseState;
        //void OnMouseDown(MouseState e)
        //{
        //}

        //void OnMouseUp(MouseState e)
        //{
        //    var mousePosition = new Point(e.X, e.Y);

        //    if (this.Player.Status == HmnXNAStatus.AskingIntercept)
        //    {
        //        var tileClicked = this.TilesToDraw.Find(tti => tti.Tile != null && tti.Rectangle.Contains(mousePosition));
        //        if (tileClicked != null)
        //            this.Player.TileWasClicked(tileClicked.Tile);
        //        else
        //            this.Player.TileWasClicked(null);
        //    }
        //    else if (this.Player.Status == HmnXNAStatus.ChoosingDiscard)
        //    {
        //        foreach (var TTI in this.TilesToDraw)
        //        {
        //            if (TTI.Rectangle.Contains(mousePosition))
        //            {
        //                this.Player.TileWasClicked(TTI.Tile);
        //                break;
        //            }
        //        }
        //    }
        //}

        private enum BoardPosition { Top, Bot, Left, Right }
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this.TilesToDraw.Clear();
            this.SelectableTiles.Clear();
            #region Player Tiles
            UpdatePlayerHandTiles(BoardPosition.Bot, this.Player);
            var currPlayer = this.MJGame.GetPlayerAfter(this.Player);
            UpdatePlayerHandTiles(BoardPosition.Left, currPlayer);
            currPlayer = this.MJGame.GetPlayerAfter(currPlayer);
            UpdatePlayerHandTiles(BoardPosition.Top, currPlayer);
            currPlayer = this.MJGame.GetPlayerAfter(currPlayer);
            UpdatePlayerHandTiles(BoardPosition.Right, currPlayer);
            #endregion

            this.UpdatePlayerPositionsAndWinds();

            UpdatePlayersDiscardPools();
            UpdateDoras();
            UpdateTooltip(); //Must be called after all tiles are calculated
            BoardMessage = Player.Message;
            if (this.Player.Status == HmnXNAStatus.ChoosingDiscard && this.recommendDiscards)
            {
                if (!String.IsNullOrWhiteSpace(BoardMessage))
                    BoardMessage += "\n";
                
                var shantenNumber = this.GetShantenNumber(this.Player.MyTiles);
                if(shantenNumber > 0)
                    BoardMessage += string.Format("Shanten {0}.",shantenNumber);
                else
                    BoardMessage += "Tenpai.";
                BoardMessage += string.Format(" Recommended discards highlighted.");
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private Meld getSelectedMeld()
        {
            return this.Player.discardAvailableMelds.Find(meld => meld.Tiles.Any(meldTile => meldTile == this.SelectedTile));
        }

        private void UpdatePlayerHandTiles(BoardPosition PlayerPosition, Mahjong.Async.Player currPlayer)
        {
            Vector2 tileListStart = Vector2.Zero;
            Vector2 meldsStart = Vector2.Zero;
            //var scale = 1.0f;
            float angle = 0;
            int tileOffsetX = 0;
            int tileOffsetY = 0;

            var mouseState = Mouse.GetState();
            var mousePosition = new Point(mouseState.X, mouseState.Y);
            
            switch (PlayerPosition)
            {
                case BoardPosition.Top:
                    tileListStart = this.TopCenter;
                    tileListStart.Y += Margin + BackTileTexture.Height;
                    tileListStart.X += (14 * BackTileTexture.Width) / 2;
                    tileListStart.X -= BackTileTexture.Width;
                    angle = (float)Math.PI;  // 180 degrees;
                    tileOffsetX = -1 * BackTileTexture.Width;
                    tileOffsetY = 0;
                    meldsStart.X = Margin + BackTileTexture.Width;
                    meldsStart.Y = Margin + BackTileTexture.Height;
                    break;
                case BoardPosition.Bot:
                    tileListStart = this.BotCenter;
                    tileListStart.Y -= this.TileTextures.Values.Max(x => x.Height) + Margin;
                    tileListStart.X -= (14 * this.TileTextures.Values.Max(x => x.Width)) / 2;
                    angle = 0;
                    tileOffsetX = this.BackTileTexture.Width;
                    tileOffsetY = 0;
                    meldsStart.X = GraphicsDevice.Viewport.Width - Margin - this.BackTileTexture.Width;
                    meldsStart.Y = GraphicsDevice.Viewport.Height - Margin - this.BackTileTexture.Height;
                    break;
                case BoardPosition.Left:
                    tileListStart = this.LeftCenter;
                    tileListStart.Y -= (14 * BackTileTexture.Width) / 2;
                    tileListStart.Y += BackTileTexture.Width;
                    tileListStart.X += Margin + BackTileTexture.Height;
                    angle = (float)Math.PI / 2.0f;  // 90 degrees
                    tileOffsetX = 0;
                    tileOffsetY = BackTileTexture.Width;
                    meldsStart.X = tileListStart.X;
                    meldsStart.Y = GraphicsDevice.Viewport.Height - Margin - BackTileTexture.Width;
                    break;
                case BoardPosition.Right:
                    tileListStart = this.RightCenter;
                    tileListStart.Y += (14 * BackTileTexture.Width) / 2;
                    tileListStart.X -= Margin + BackTileTexture.Height;
                    tileListStart.Y -= BackTileTexture.Width;
                    angle = (float)Math.PI * 1.5f;  // 270 degrees
                    tileOffsetX = 0;
                    tileOffsetY = -1 * BackTileTexture.Width;
                    meldsStart.X = tileListStart.X;
                    meldsStart.Y = Margin + BackTileTexture.Width;
                    break;
            }

            Vector2 currentPosition = new Vector2(tileListStart.X, tileListStart.Y);
            List<Tile> recommendedTiles = new List<Tile>();
            if (recommendDiscards && this.Player.Status == HmnXNAStatus.ChoosingDiscard)
            {
                recommendedTiles = GetRecommendedDiscards(this.Player.MyTiles);
            }

            if (currPlayer == this.Player)
            {
                var PlayerTiles = this.Player.MyTiles.OrderBy(t => t.Value());
                var tmpSelectableTiles = new List<Tile>();
                this.SelectableTiles.Clear();

                if (this.Player.Status == HmnXNAStatus.ChoosingDiscard)
                {
                    if (this.Player.Riichi)
                    {
                        if (this.Player.Ippatsu)
                            tmpSelectableTiles = new List<Tile>(this.Player.riichiDiscardTiles);
                        else
                            tmpSelectableTiles = new List<Tile> { this.Player.LastDraw };
                    }
                    else
                    {
                        tmpSelectableTiles = new List<Tile>(this.Player.MyTiles);
                    }
                }
                //else if (this.Player.Status == HmnXNAStatus.ChoosingMeld)
                //{
                //    tmpSelectableTiles = new List<Tile>(this.Player.discardAvailableMelds.GetAllTiles());
                //}

                int i = 0;
                foreach (var tile in PlayerTiles)
                {
                    if (tile != this.Player.LastDraw)
                    {
                        var TTI = new TileTextureInfo();

                        TTI.Tile = tile;

                        TTI.Texture = this.TileTextures[tile.ToString()];
                        TTI.Position = currentPosition;
                        TTI.Rotation = angle;

                        if (this.Player.Status == HmnXNAStatus.ChoosingMeld)
                        {
                            if (TTI.Tile == this.SelectedTile)
                            {
                                TTI.ShadeColor = this.SelectableTileColor_MouseOver;
                                this.SelectableTiles.Add(TTI.Tile);
                            }
                            else if (this.Player.discardAvailableMelds.Any(meld => meld.Tiles.Any(meldTile => meldTile == TTI.Tile)))
                            {
                                this.SelectableTiles.Add(TTI.Tile);
                                //Need to find the meld it belongs to, if that same meld contains the selected tile, color it as selected, if not, as selectable
                                var selectedMeld = this.getSelectedMeld();

                                if (selectedMeld != null && selectedMeld.Tiles != null && selectedMeld.Tiles.Contains(TTI.Tile))
                                {
                                    TTI.ShadeColor = this.SelectableTileColor_MouseOver;
                                }
                                else
                                {
                                    TTI.ShadeColor = this.SelectableTileColor_Highlighted;
                                }
                            }
                            else
                            {
                                TTI.ShadeColor = Color.White;
                            }
                        }
                        else if (this.Player.Status == HmnXNAStatus.ChoosingDiscard)
                        {
                            if (TTI.Tile == this.SelectedTile)
                            {
                                TTI.ShadeColor = this.SelectableTileColor_MouseOver;
                                this.SelectableTiles.Add(TTI.Tile);
                            }
                            else if (tmpSelectableTiles.Contains(TTI.Tile))
                            {
                                this.SelectableTiles.Add(TTI.Tile);

                                if (recommendDiscards && recommendedTiles.Contains(TTI.Tile))
                                {
                                    TTI.ShadeColor = this.SelectableTileColor_Recommended;
                                }
                                else
                                {
                                    TTI.ShadeColor = this.SelectableTileColor_Highlighted;
                                }
                            }
                            else
                            {
                                TTI.ShadeColor = Color.White;
                            }
                        }
                        else if (this.Player.Status == HmnXNAStatus.AskingRiichi)
                        {
                            if (this.Player.riichiDiscardTiles.Contains(TTI.Tile))
                            {
                                TTI.ShadeColor = this.SelectableTileColor_RiichiDiscard;
                            }
                        }

                        currentPosition.X += tileOffsetX;
                        currentPosition.Y += tileOffsetY;

                        i++;

                        this.TilesToDraw.Add(TTI);
                    }
                }

                //most recent tile always at the end
                if (this.Player.LastDraw != null)
                {
                    var TTI = new TileTextureInfo();

                    TTI.Tile = this.Player.LastDraw;

                    TTI.Texture = this.TileTextures[this.Player.LastDraw.ToString()];
                    TTI.Position = currentPosition;
                    TTI.Rotation = angle;

                    if (this.Player.Status == HmnXNAStatus.ChoosingDiscard)
                    {
                        if (TTI.Tile == SelectedTile)
                        {
                            this.SelectableTiles.Add(TTI.Tile);
                            TTI.ShadeColor = this.SelectableTileColor_MouseOver;
                        }
                        else if (tmpSelectableTiles.Contains(TTI.Tile))
                        {
                            this.SelectableTiles.Add(TTI.Tile);
                            TTI.ShadeColor = this.SelectableTileColor_Highlighted;
                        }
                    }

                    //currentPosition.X += tileOffsetX;
                    //currentPosition.Y += tileOffsetY;

                    TTI.Position += new Vector2(tileOffsetX, tileOffsetY) / 2;

                    this.TilesToDraw.Add(TTI);
                }
            }
            else
            {
                if (currPlayer.RevealedHand != null && currPlayer.RevealedHand.Count > 0)
                {
                    int i = 0;
                    foreach (var meld in currPlayer.RevealedHand.ToList())
                    {
                        foreach (var tile in meld.Tiles.ToList())
                        {
                            var TTI = new TileTextureInfo();
                            TTI.Texture = this.TileTextures[tile.ToString()];
                            TTI.Tile = tile;
                            TTI.Position = currentPosition;
                            TTI.Rotation = angle;
                            currentPosition.X += tileOffsetX;
                            currentPosition.Y += tileOffsetY;

                            this.TilesToDraw.Add(TTI);

                            i++;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < currPlayer.HandTilesCount; i++)
                    {
                        var TTI = new TileTextureInfo();
                        TTI.Texture = this.BackTileTexture;
                        TTI.Position = currentPosition;
                        TTI.Rotation = angle;
                        currentPosition.X += tileOffsetX;
                        currentPosition.Y += tileOffsetY;

                        this.TilesToDraw.Add(TTI);
                    }
                }
            }

            currentPosition = meldsStart;
            foreach (var meld in currPlayer.ExposedMelds)
            {
                foreach (var meldTile in meld.Tiles.OrderBy(x => x.Value() * -1))
                {
                    //currentPosition.X -= tileOffsetX;
                    //currentPosition.Y -= tileOffsetY;

                    var TTI = new TileTextureInfo();
                    TTI.Tile = meldTile;
                    TTI.Texture = TileTextures[meldTile.ToString()];
                    TTI.Position = currentPosition;
                    TTI.Rotation = angle;
                    //TTI.Rectangle = new Rectangle(
                    //    (int)currentPosition.X,
                    //    (int)currentPosition.Y,
                    //    BackTileTexture.Width, BackTileTexture.Height);

                    currentPosition.X -= tileOffsetX;
                    currentPosition.Y -= tileOffsetY;

                    this.TilesToDraw.Add(TTI);
                }
            }

            //if (this.PlayerTiles.ContainsKey(PlayerPosition)) this.PlayerTiles[PlayerPosition] = TileTextureInfoList;
            //else this.PlayerTiles.Add(PlayerPosition, TileTextureInfoList);
        }

        private List<Tile> GetRecommendedDiscards(IList<Tile> Hand)
        {
            List<Tile> recommendedTiles = new List<Tile>();
            var cacheKey = GetTilesKey(this.Player.MyTiles);
            if (this.RecommendedTilesCache.ContainsKey(cacheKey))
            {
                recommendedTiles = this.RecommendedTilesCache[cacheKey];
            }
            else
            {
                var shantenInfo = this.Player.GetShantenInfo();
                recommendedTiles = shantenInfo.Value;
                this.RecommendedTilesCache.Add(cacheKey, recommendedTiles);
                this.ShantenCahe.Add(cacheKey, shantenInfo.Key);
            }
            return recommendedTiles;
        }

        private int GetShantenNumber(IList<Tile> Hand)
        {
            List<Tile> recommendedTiles = new List<Tile>();
            var cacheKey = GetTilesKey(this.Player.MyTiles);
            if (!this.ShantenCahe.ContainsKey(cacheKey))
            {
                var shantenInfo = this.Player.GetShantenInfo();
                this.RecommendedTilesCache.Add(cacheKey, shantenInfo.Value);
                this.ShantenCahe.Add(cacheKey, shantenInfo.Key);
            }

            return ShantenCahe[cacheKey];
        }

        private GraphicsDevice GraphicsDevice
        {
            get
            {
                return this.ScreenManager.GraphicsDevice;
            }
        }

        private void UpdateDoras()
        {
            bool overlapDoras = true;

            //Center, 5tiles wide, 2 tiles tall
            Vector2 center = new Vector2(this.GraphicsDevice.Viewport.Width / 2, this.GraphicsDevice.Viewport.Height / 2);

            var wallHeight = 2 * this.BackTileTexture.Height;
            var wallWidth = 5 * this.BackTileTexture.Width;

            Vector2 deadWallPosition = Vector2.Zero;
            deadWallPosition.X = center.X - wallWidth / 2;
            deadWallPosition.Y = center.Y;

            if (overlapDoras)
            {
                deadWallPosition.Y -= this.BackTileTexture.Height * 2 / 5;
            }

            for (int i = 0; i < 5; i++)
            {
                Texture2D top, bot;
                TileTextureInfo ttiBot = new TileTextureInfo();
                TileTextureInfo ttiTop = new TileTextureInfo();

                if (MJGame.DoraIndicators.Count > i)
                {
                    top = this.TileTextures[MJGame.DoraIndicators[i].ToString()];
                    ttiTop.Tile = MJGame.DoraIndicators[i];
                }
                else
                {
                    top = this.BackTileTexture;
                }
                if (MJGame.UradoraIndicators.Count > i)
                {
                    bot = this.TileTextures[MJGame.UradoraIndicators[i].ToString()];
                    ttiBot.Tile = MJGame.UradoraIndicators[i];
                }
                else
                {
                    bot = this.BackTileTexture;
                }

                ttiBot.Texture = bot;
                ttiBot.Position = deadWallPosition;
                deadWallPosition.Y -= bot.Height;
                
                ttiTop.Texture = top;
                ttiTop.Position = deadWallPosition;
                if (overlapDoras)
                {
                    ttiTop.Position = deadWallPosition + new Vector2(0, this.BackTileTexture.Height * 4 / 5);
                }

                deadWallPosition.X += top.Width;
                deadWallPosition.Y += bot.Height;

                TilesToDraw.Add(ttiBot);
                TilesToDraw.Add(ttiTop);
            }
        }

        private void DrawMessage()
        {
            if (!String.IsNullOrWhiteSpace(BoardMessage))
            {
                Vector2 messagePosition = Vector2.Zero;
                var stringSize = this.Font_CourierNew.MeasureString(BoardMessage);
                messagePosition.X = (this.GraphicsDevice.Viewport.Width - stringSize.X) / 2;
                messagePosition.Y = this.GraphicsDevice.Viewport.Height - Margin - this.BackTileTexture.Height - Margin - stringSize.Y;
                spriteBatch.DrawString(this.Font_CourierNew, BoardMessage, messagePosition, Color.Black);
            }
        }

        void DrawTiles()
        {
            Vector2 origin = new Vector2(0, 0);
            foreach (var TTI in this.TilesToDraw)
            {
                //Color TileColor;
                //if (TTI.MouseOvered) TileColor = this.SelectableTileColor_MouseOver;
                //else if (TTI.Highlighted) TileColor = this.SelectableTileColor_Highlighted;
                //else TileColor = Color.White;

                //spriteBatch.Draw(TTI.Texture, TTI.Position, null, TileColor, TTI.Rotation, origin, 1.0f, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(TTI.Texture, TTI.Position, null, TTI.ShadeColor, TTI.Rotation, origin, 1.0f, SpriteEffects.None, 0.0f);
            }
            //spriteBatch.Draw(tileTexture, currentPosition, null, this.SelectableTileColor, angle, new Vector2(0, 0), scale, SpriteEffects.None, 0.0f);
        }

        void DrawTooltip()
        {
            if (tileToolTips && !String.IsNullOrEmpty(this.tooltipText))
            {
                var stringSize = this.Font_CourierNew.MeasureString(tooltipText);
                spriteBatch.Draw(this.tooltipBG, new Rectangle((int)this.tooltipPosition.X - 1, (int)this.tooltipPosition.Y - 1, (int)stringSize.X + 2, (int)stringSize.Y + 2), Color.White);
                spriteBatch.DrawString(Font_CourierNew, this.tooltipText, this.tooltipPosition, Color.DarkBlue);
            }
        }
    }
}
