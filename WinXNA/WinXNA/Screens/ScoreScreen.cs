using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mahjong;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MahjongXNA.Screens
{
    class ScoreScreen : GameScreen
    {
        Texture2D gradientTexture;
        ScoreKeeper score;
        SpriteFont numbersFont;

        public ScoreScreen(ScoreKeeper score)
        {
            this.score = score;
            this.IsPopup = true;
            this.Opacity = 0.5f;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            gradientTexture = content.Load<Texture2D>("gradient");
            this.numbersFont = content.Load<SpriteFont>("clockfont");
        }

        public event EventHandler<PlayerIndexEventArgs> ScreenClosed;

        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex) 
                || input.IsMenuCancel(ControllingPlayer, out playerIndex) 
                || input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.S, ControllingPlayer, out playerIndex)
                || input.IsNewLeftMouseClick())
            {
                // Raise the accepted event, then exit the message box.
                if (ScreenClosed != null)
                    ScreenClosed(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
        }

        private void DrawScoreRow(IList<string> cells, float Top, float Left, int FirstColWidth, int OtherColsWidth, int hPad, SpriteFont font)
        {
            var cellColors = Enumerable.Range(0, cells.Count).Select(i => Color.White).ToList();
            DrawScoreRow(cells, Top, Left, FirstColWidth, OtherColsWidth, hPad, font, cellColors);
        }
        private void DrawScoreRow(IList<string> cells, float Top, float Left, int FirstColWidth, int OtherColsWidth, int hPad, SpriteFont font, IList<Color> cellColors)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            //SpriteFont font = ScreenManager.Font;
            //SpriteFont font = this.numbersFont;
            
            var textPosition = new Vector2(Left, Top);
            textPosition.X += hPad;
            spriteBatch.DrawString(font, cells[0], textPosition, cellColors.ElementAtOrDefault(0) * TransitionAlpha);
            textPosition.X += FirstColWidth + hPad;
            for (int i = 1; i < cells.Count; i++)
            {
                Color color = cellColors.ElementAtOrDefault(i) * TransitionAlpha;
                var myWidth = font.MeasureString(cells[i]).X;
                spriteBatch.DrawString(font, cells[i], textPosition + new Vector2((OtherColsWidth - myWidth) / 2, 0), color);
                textPosition.X += OtherColsWidth;
            }
        }

        public float Opacity { get; set; }

        public override void Draw(GameTime gameTime)
        {
            //Add transparency to see the board behind it?

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = viewportSize * 3 / 4;
            Vector2 textPosition = Vector2.Zero + viewportSize * 1 / 8;
            
            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X,
                                                          (int)textPosition.Y,
                                                          (int)textSize.X,
                                                          (int)textSize.Y);

            List<string> Headers = new List<string>(5);
            List<List<string>> Rows = new List<List<string>>();
            List<List<Color>> RowColors = new List<List<Color>>();
            List<string> Footers = new List<string>(5);

            #region Headers
            Headers.Add("Hand");
            foreach (var player in score.Players)
                Headers.Add(player.Name);
            #endregion

            #region Footers
            Footers.Add("Total");
            foreach (var player in score.Players)
                Footers.Add(this.score.GetPlayerCurrentTotal(player).ToString("#,#"));
            #endregion

            #region Rows
            foreach (var player in score.Players)
            {
                var history = this.score.GetPlayerPaymentsHistory(player);
                int n = 0;
                foreach (var payment in history)
                {
                    if (Rows.Count < n + 1)
                    {
                        Rows.Add(new List<string>());
                        Rows[n].Add((n + 1).ToString("D3"));

                        RowColors.Add(new List<Color>());
                        RowColors[n].Add(Color.White);
                    }

                    Rows[n].Add(payment.ToString("+#,#;-#,#;----"));
                    if(payment > 0)
                        RowColors[n].Add(Color.GreenYellow);
                    else if (payment < 0)
                        RowColors[n].Add(Color.Red);
                    else RowColors[n].Add(Color.White);

                    n++;
                }
            }
            #endregion

            Color color = Color.White * TransitionAlpha;
            int hPad = 8;
            int vPad = 8;

            int colHeight = (int)this.numbersFont.MeasureString(String.Join(String.Empty, Headers)).Y;
            int colHandsWidth = (int)this.numbersFont.MeasureString(Headers[0]).X;
            int colPlayersWidth = (backgroundRectangle.Width - colHandsWidth - 3 * hPad) / this.score.Players.Count;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(gradientTexture, backgroundRectangle, color * Opacity);

            #region Headers
            DrawScoreRow(Headers, textPosition.Y, textPosition.X, colHandsWidth, colPlayersWidth, hPad, this.numbersFont);
            #endregion

            #region Rows
            int i = 0;
            foreach (var row in Rows.Last(8))
            {
                textPosition.Y += vPad + colHeight;
                DrawScoreRow(row, textPosition.Y, textPosition.X, colHandsWidth, colPlayersWidth, hPad, this.numbersFont, RowColors[i]);
                i++;
            }
            #endregion

            #region Footers
            textPosition.Y = backgroundRectangle.Y + backgroundRectangle.Height - colHeight - vPad;
            DrawScoreRow(Footers, textPosition.Y, textPosition.X, colHandsWidth, colPlayersWidth, hPad, this.numbersFont);
            #endregion

            spriteBatch.End();
        }
    }
}
