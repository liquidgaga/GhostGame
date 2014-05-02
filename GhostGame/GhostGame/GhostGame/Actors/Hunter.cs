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
    public class Hunter : BaseActor
    {
        private const float hunterMoveSpeed = 3.0f;
        private const float hunterRotationSpeed = 0.1f;
        private const int batteryCostPerFrame = 1;
        private const int maxBattery = 100 * 30; // 100% times 30 frames per second.

        private const float flashlightAngle = (float) (.25 * Math.PI);
        private const int maxLightLength = 50;

        public bool flashlightActive;
        private int battery;

        public Hunter(int playerNumber, Texture2D sprite, Vector2 position, float rotation) : base(playerNumber, sprite, position, rotation)
        {
            this.MoveSpeed = hunterMoveSpeed;
            this.RotationSpeed = hunterRotationSpeed;
            this.battery = maxBattery;
            this.flashlightActive = false;
        }

        public void ProcessInput(GamePadState padState)
        {
            //Process Movement
            float deltaY = 0;
            float deltaX = 0;

            if (padState.Buttons.A == ButtonState.Pressed)
            {
                ProcessFlashlight(true);
            }
            else
            {
                ProcessFlashlight(false);
            }

            if (padState.DPad.Up == ButtonState.Pressed)
            {
                deltaY -= this.MoveSpeed;
            }

            if (padState.DPad.Down == ButtonState.Pressed)
            {
                deltaY += this.MoveSpeed;
            }

            if (padState.DPad.Right == ButtonState.Pressed)
            {
                deltaX += this.MoveSpeed;
            }

            if (padState.DPad.Left == ButtonState.Pressed)
            {
                deltaX -= this.MoveSpeed;
            }

            ProcessMovementAndRotation(deltaX, deltaY);
        }

        public void ProcessKeyboardArrows(KeyboardState state){
            //Process Movement
            float deltaY = 0;
            float deltaX = 0;

            if (state.IsKeyDown(Keys.Space))
            {
                ProcessFlashlight(true);
            }
            else
            {
                ProcessFlashlight(false);
            }

            if (state.IsKeyDown(Keys.Up))
            {
                deltaY -= this.MoveSpeed;
            }

            if (state.IsKeyDown(Keys.Down))
            {
                deltaY += this.MoveSpeed;
            }

            if (state.IsKeyDown(Keys.Right))
            {
                deltaX += this.MoveSpeed;
            }

            if (state.IsKeyDown(Keys.Left))
            {
                deltaX -= this.MoveSpeed;
            }

            ProcessMovementAndRotation(deltaX, deltaY);
        }

        public void ProcessMovementAndRotation(float deltaX, float deltaY)
        {
            float targetAngle;
            Direction direction = Direction.D;

            // No input.
            if (deltaX == 0 && deltaY == 0)
                return;

            // Find the Direction we are trying to move towards.
            // U
            if (deltaX == 0 && deltaY < 0)
            {
                direction = Direction.U;

            }

            // UR
            if (deltaX > 0 && deltaY < 0)
            {
                direction = Direction.UR;
            }

            // R
            if (deltaX > 0 && deltaY == 0)
            {
                direction = Direction.R;
            }

            // DR
            if (deltaX > 0 && deltaY > 0)
            {
                direction = Direction.DR;
            }

            // D
            if (deltaX == 0 && deltaY > 0)
            {
                direction = Direction.D;
            }

            // DL
            if (deltaX < 0 && deltaY > 0)
            {
                direction = Direction.DL;
            }

            // L
            if (deltaX < 0 && deltaY == 0)
            {
                direction = Direction.L;
            }

            // UL
            if (deltaX < 0 && deltaY < 0)
            {
                direction = Direction.UL;
            }

            // Rotate at max rotate speed towards our target angle;
            targetAngle = DirectionAngleMapping.map[direction];

            if (this.Rotation != targetAngle)
            {
                if (Math.Abs(this.Rotation - targetAngle) % MathHelper.TwoPi < this.RotationSpeed)
                {
                    this.Rotation = targetAngle;
                }

                // 0 is an annoying special case.
                else if (this.Rotation == 0)
                {
                    if (targetAngle >= Math.PI)
                    {
                        this.Rotation -= this.RotationSpeed;
                    }

                    else
                    {
                        this.Rotation += this.RotationSpeed;
                    }
                }

                else
                {
                    if (this.Rotation >= targetAngle)
                    {
                        if (this.Rotation - Math.PI >= targetAngle)
                        {
                            this.Rotation += this.RotationSpeed;
                        }
                        else
                        {
                            this.Rotation -= this.RotationSpeed;
                        }
                    }
                    else
                    {
                        if (this.Rotation + Math.PI >= targetAngle)
                        {
                            this.Rotation += this.RotationSpeed;
                        }
                        else
                        {
                            this.Rotation -= this.RotationSpeed;
                        }
                    }
                }

                // counterclockwise from 0 means we subtract from two pi
                if (this.Rotation < 0f)
                {
                    this.Rotation = MathHelper.TwoPi + this.Rotation;
                }

                // if we have gone above two pi, mod it, two pi is a full rotation.
                this.Rotation = this.Rotation % MathHelper.TwoPi;

                return;
            }

            // If we are facing our target angle, then move that direction.
            if (deltaX != 0 && deltaY != 0)
            {
                deltaX = deltaX / 2;
                deltaY = deltaY / 2;
            }

            int MaxX = Game.viewport.Width - this.Sprite.Width;
            int MinX = 0 + this.Sprite.Width;
            int MaxY = Game.viewport.Height - this.Sprite.Height;
            int MinY = 0 + Sprite.Height;

            this.Position.X += deltaX;
            this.Position.Y += deltaY;

            bool willCollide = Utils.CollisionUtilities.CheckWallCollision(this);
            
            for(int i = 0; i < GhostGame.Game.Hunters.Length; i++){
                if(i == this.PlayerNumber || GhostGame.Game.Hunters[i] == null || willCollide)
                    continue;

                if(CollisionUtilities.SmartCollisionCheck(this, GhostGame.Game.Hunters[i])){
                    willCollide = true;
                    break;
                }
            }

            if (willCollide)
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

        public void ProcessFlashlight(bool active)
        {
            if (!active || battery == 0)
            {
                this.flashlightActive = false;
                return;
            }

            battery -= batteryCostPerFrame;

            if (battery <= 0)
            {
                battery = 0;
                this.flashlightActive = false;
                return;
            }
            this.flashlightActive = true;
        }

        public double FlashlightPercentage()
        {
            return (double)battery / (double)maxBattery;
        }

        public double FlashlightLength()
        {
            return maxLightLength / Math.Cos(flashlightAngle) * this.FlashlightPercentage();
        }

        public VertexPositionColor[] GetFlashlightTrianglePoints()
        {
            var resultVectorArray = new VertexPositionColor[2];
            var originPoint = this.GetFlashlightOriginPoint();
            resultVectorArray[0] = originPoint;
            var point1 = new VertexPositionColor(new Vector3( (float)(originPoint.Position.X + (Math.Cos(flashlightAngle/2) * this.FlashlightLength())), (float)(originPoint.Position.Y + (Math.Sin(flashlightAngle) * this.FlashlightLength())), 0 ), Color.Yellow);
            //var point2 = new VertexPositionColor(new Vector3( (float)(originPoint.Position.X + (Math.Cos(-flashlightAngle/2) * this.FlashlightLength())), (float)(originPoint.Position.Y + (Math.Sin(-flashlightAngle/2) * this.FlashlightLength())), 0), Color.Yellow);
            resultVectorArray[1] = point1;
            //resultVectorArray[2] = point2;
            return resultVectorArray;
        }

        private VertexPositionColor GetFlashlightOriginPoint()
        {
            return new VertexPositionColor(new Vector3((float)(this.Position.X + (Math.Cos(this.Rotation) * this.Sprite.Width / 2)), (float)(this.Position.Y + (Math.Sin(this.Rotation) * this.Sprite.Width / 2)), 0), Color.Yellow);
        }
    }
}
