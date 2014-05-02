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

namespace GhostGame.Actors
{
    public class BaseActor
    {
        public const int standardIframeMax = 150;

        public int PlayerNumber;
        public Texture2D Sprite;
        public Vector2 Position;
        public float Rotation;
        public float MoveSpeed;
        public float RotationSpeed;
        public int iFrameCount;
        public bool isInvincible;
        public Vector2 Origin;

        public BaseActor(int playerNumber, Texture2D sprite, Vector2 position, float rotation)
        {
            this.PlayerNumber = playerNumber;
            this.Sprite = sprite;
            this.Position = position;
            this.Rotation = rotation;
            this.isInvincible = false;
            this.iFrameCount = 0;
            this.Origin = new Vector2(this.Sprite.Width / 2, this.Sprite.Height / 2);
        }
        
        public Rectangle BoundingBox()
        {
            var boundingRectangle = new Rectangle();
            boundingRectangle.Width = this.Sprite.Width;
            boundingRectangle.Height = this.Sprite.Height;

            boundingRectangle.Location = new Point((int)this.Position.X, (int)this.Position.Y);
            return boundingRectangle;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(
                this.Sprite, 
                this.Position, 
                null, 
                Color.White, 
                -this.Rotation, 
                this.Origin, 
                1.0f,
                SpriteEffects.None, 
                0);
        }
    }
}
