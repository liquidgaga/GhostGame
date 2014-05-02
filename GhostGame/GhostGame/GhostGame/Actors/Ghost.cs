using System;
using System.Collections.Generic;
using System.Linq;
using GhostGame.Enums;
using GhostGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GhostGame.Actors
{
    public class Ghost : BaseActor
    {
        private const float ghostMoveSpeed = 3.0f;
        private const float ghostRotationSpeed = 1.0f;
        public int health;

        public Ghost(int playerNumber, Texture2D sprite, Vector2 position, float rotation) : base(playerNumber, sprite, position, rotation)
        {
            this.MoveSpeed = ghostMoveSpeed;
            this.RotationSpeed = ghostRotationSpeed;
            health = 100;
        }

        public void ProcessInput(KeyboardState state)
        {
            this.ProcessMovementAndRotation(state);
        }

        public void ProcessMovementAndRotation(KeyboardState state)
        {
            //Process Movement
            float deltaY = 0;
            float deltaX = 0;

            if (state.IsKeyDown(Keys.W))
            {
                deltaY -= this.MoveSpeed;
            }

            if (state.IsKeyDown(Keys.S))
            {
                deltaY += this.MoveSpeed;
            }

            if (state.IsKeyDown(Keys.D))
            {
                deltaX += this.MoveSpeed;
            }

            if (state.IsKeyDown(Keys.A))
            {
                deltaX -= this.MoveSpeed;
            }

            // No input.
            if (deltaX == 0 && deltaY == 0)
                return;

            if (deltaX != 0 && deltaY != 0)
            {
                deltaX = deltaX / 2;
                deltaY = deltaY / 2;
            }

            int MaxX = Game.viewport.Width - this.Sprite.Width/2;
            int MinX = 0 + this.Sprite.Width/2;
            int MaxY = Game.viewport.Height - this.Sprite.Height/2;
            int MinY = 0 + Sprite.Height/2;

            this.Position.X += deltaX;
            this.Position.Y += deltaY;

            if (Utils.CollisionUtilities.CheckWallCollision(this))
            {
                this.Position.X -= deltaX;
                this.Position.Y -= deltaY;
            }

            if (this.Position.X > MaxX)
            {
                this.Position.X = MaxX;
            }

            else if (this.Position.X < MinX)
            {
                this.Position.X = MinX;
            }

            if (this.Position.Y > MaxY)
            {
                this.Position.Y = MaxY;
            }

            else if (this.Position.Y < MinY)
            {
                this.Position.Y = MinY;
            }
        }
    }
}
