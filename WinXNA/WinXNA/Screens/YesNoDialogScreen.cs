#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion


namespace MahjongXNA.Screens
{
    class YesNoDialogScreen : GameScreen
    {
        private struct Button
        {
            public string Text { get; set; }
            public Rectangle Box;
            public bool Selected { get; set; }
            public Vector2 TextPosition { get; set; }
        }

        #region Fields

        string message;
        Texture2D gradientTexture;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        public string TextYes
        {
            get { return this.Yes.Text; }
            set { this.Yes.Text = value; }
        }

        public string TextNo
        {
            get { return this.No.Text; }
            set { this.No.Text = value; }
        }

        const int hPad = 32;
        const int vPad = 16;

        Button Yes, No;
        SpriteBatch spriteBatch;
        SpriteFont font;
        public YesNoDialogScreen(string message) : this(message, false) { }
        public YesNoDialogScreen(string message, bool includeUsageText)
        {
            this.Yes.Selected = true;
            this.TextYes = "Yes";
            this.TextNo = "No";

            //const string usageText = "\nA button, Space, Enter = ok" + "\nB button, Esc = cancel";
            string usageText = String.Format("\nA button, Space, Enter = {0}\nB button, Esc = {1}",this.TextYes,this.TextNo);

            if (includeUsageText)
                this.message = message + usageText;
            else
                this.message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        private bool _CalcBoxSizesDone = false;
        private void CalcBoxSizes()
        {
            if (_CalcBoxSizesDone) return;

            var backgroundRectangle = getBackgroundRectangle();
            Vector2 yesTextSize = font.MeasureString(this.Yes.Text);
            Vector2 noTextSize = font.MeasureString(this.No.Text);
            var textPosition = getTextPosition();
            Vector2 textSize = font.MeasureString(message);

            this.Yes.Box.Width = backgroundRectangle.Width / 2 - 2 * hPad;
            this.Yes.Box.Height = (int)Math.Max(yesTextSize.Y, noTextSize.Y);
            this.Yes.Box.Y = (int)(textPosition.Y + textSize.Y + vPad);
            this.Yes.Box.X = backgroundRectangle.X + hPad;
            this.Yes.TextPosition = new Vector2
            (
                this.Yes.Box.X + this.Yes.Box.Width / 2 - yesTextSize.X / 2
                ,this.Yes.Box.Y
            );

            this.No.Box.Width = this.Yes.Box.Width;
            this.No.Box.Height = this.Yes.Box.Height;
            this.No.Box.Y = this.Yes.Box.Y;
            this.No.Box.X = backgroundRectangle.X + hPad + this.Yes.Box.Width + hPad + hPad;
            this.No.TextPosition = new Vector2
            (
                this.No.Box.X + this.No.Box.Width / 2 - noTextSize.X / 2
                ,this.No.Box.Y
            );
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            gradientTexture = content.Load<Texture2D>("gradient");
        }


        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (input.IsMenuLeft(ControllingPlayer) || input.IsMenuRight(ControllingPlayer))
            {
                if (!this.Yes.Selected && !this.No.Selected)
                {
                    this.Yes.Selected = true;
                    this.No.Selected = false;
                }
                else
                {
                    this.Yes.Selected = !this.Yes.Selected;
                    this.No.Selected = !this.No.Selected;
                }
            }
            else if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                // Raise the selected event, then exit the message box.
                if (this.Yes.Selected && Accepted != null)
                {
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));
                }
                else if (this.No.Selected && Cancelled != null)
                {
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));
                }

                ExitScreen();
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
#if WINDOWS
            else if (input.MouseMoved())
            {
                var mousePosition = new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                this.Yes.Selected = false;
                this.No.Selected = false;
                if (this.Yes.Box.Contains(mousePosition))
                    this.Yes.Selected = true;
                else if (this.No.Box.Contains(mousePosition))
                    this.No.Selected = true;
            }
            else if (input.IsNewLeftMouseClick())
            {
                var mousePosition = new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                if (this.Yes.Box.Contains(mousePosition))
                {
                    if(Accepted != null)
                        Accepted(this, new PlayerIndexEventArgs(playerIndex));
                    ExitScreen();
                }
                else if (this.No.Box.Contains(mousePosition))
                {
                    if(Cancelled != null)
                        Cancelled(this, new PlayerIndexEventArgs(playerIndex));
                    ExitScreen();
                }
            }
#endif
        }


        #endregion

        // Center the message text in the viewport.
        private Vector2 getTextPosition()
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            return (viewportSize - textSize) / 2;
        }

        

        private Rectangle getBackgroundRectangle()
        {
            Vector2 textPosition = getTextPosition();
            Vector2 yesTextSize = font.MeasureString(this.Yes.Text);
            Vector2 noTextSize = font.MeasureString(this.No.Text);
            Vector2 textSize = font.MeasureString(message);

            return new Rectangle((int)textPosition.X - hPad,
                                (int)textPosition.Y - vPad,
                                (int)textSize.X + hPad * 2,
                                (int)(textSize.Y + vPad * 3 + Math.Max(yesTextSize.Y, noTextSize.Y)));
        }

        #region Draw
        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch = ScreenManager.SpriteBatch;
            font = ScreenManager.Font;
            CalcBoxSizes();

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            var textPosition = getTextPosition();
            
            Rectangle backgroundRectangle = getBackgroundRectangle();

            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);

            // Draw the message box text.
            spriteBatch.DrawString(font, message, textPosition, color);

            var yesColor = Yes.Selected ? Color.Gold : Color.White;
            spriteBatch.Draw(gradientTexture, this.Yes.Box, Color.DarkBlue);
            spriteBatch.DrawString(font, this.Yes.Text, this.Yes.TextPosition, yesColor);

            var noColor = No.Selected ? Color.Gold : Color.White;
            spriteBatch.Draw(gradientTexture, this.No.Box, Color.DarkBlue);
            spriteBatch.DrawString(font, this.No.Text, this.No.TextPosition, noColor);

            spriteBatch.End();
        }


        #endregion
    }
}
