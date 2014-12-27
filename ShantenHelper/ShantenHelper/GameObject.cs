using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShantenHelper.Interfaces;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShantenHelper
{
    public abstract class GameObject : DrawableGameComponent
    {
        public GameObject(Game game)
            : base(game)
        {
        }

        public SpriteBatch SpriteBatch { get; private set; }

        protected IDrawingComponent DrawingComp {get;set;}
        public override void Draw(GameTime gameTime)
        {
            if (this.DrawingComp != null)
            {
                this.DrawingComp.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        protected ILoadingComponent LoaderComp { get; set; }
        protected override void LoadContent()
        {
            if (this.LoaderComp != null)
            {
                this.LoaderComp.Load(this.Game.Content);
            }

            this.SpriteBatch = this.Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            if (this.SpriteBatch == null)
                this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);

            base.LoadContent();
        }

        protected IUpdatingComponent UpdatingComp { get; set; }
        public override void Update(GameTime gameTime)
        {
            if (this.UpdatingComp != null)
            {
                this.UpdatingComp.Update(gameTime);
            }
            base.Update(gameTime);
        }
    }
}
