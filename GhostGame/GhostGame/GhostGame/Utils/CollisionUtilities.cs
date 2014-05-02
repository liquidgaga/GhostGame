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
using GhostGame;
using GhostGame.Actors;

namespace GhostGame.Utils
{
    class CollisionUtilities
    {

        public static bool CheckWallCollision(BaseActor actor)
        {


            var actorOrigin = new Vector2(actor.Sprite.Width/2,actor.Sprite.Height/2);

            Matrix actorTransform =
                Matrix.CreateTranslation(new Vector3(-actorOrigin, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(actor.Position, 0.0f));

            var actorRectangle = CalculateBoundingRectangle(
                new Rectangle(0, 0, actor.Sprite.Width, actor.Sprite.Height), actorTransform);

            for (int i = 0; i < GhostGame.Game.wallBoundingBoxes.Length; i++)
            {
                var currentRectangle = GhostGame.Game.wallBoundingBoxes[i];
                bool collisionfound = false;
                if (TestBoundingBoxCollision(actorRectangle, currentRectangle))
                {
                    Color[] imagePiece = GetImageData(GhostGame.Game.mapTextureData, GhostGame.Game.mapTexture.Width, currentRectangle);
                    var subImageOrigin = new Vector2(currentRectangle.Width / 2, currentRectangle.Height / 2);
                    Matrix subImageTransform =
                        Matrix.CreateTranslation(new Vector3(-subImageOrigin, 0.0f)) *
                        Matrix.CreateTranslation(new Vector3(new Vector2(currentRectangle.X, currentRectangle.Y), 0.0f));

                    Color[] actorTexturedata;
                    // Extract collision data
                    if (actor.GetType() == typeof(Hunter))
                        actorTexturedata = GhostGame.Game.hunterTextureData;
                    else
                        actorTexturedata = GhostGame.Game.ghostTextureData;
                    collisionfound = IntersectPixels(actorRectangle, actorTexturedata, currentRectangle, imagePiece);
                }
                if (collisionfound)
                    return true;
            }
            return false;
        }

        private static Color[] GetImageData(Color[] colorData, int width, Rectangle rectangle)
        {
            Color[] color = new Color[rectangle.Width * rectangle.Height];
            for (int x = 0; x < rectangle.Width; x++)
                for (int y = 0; y < rectangle.Height; y++)
                    color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * width];
            return color;
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(
                            Matrix transformA, int widthA, int heightA, Color[] dataA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        private static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                           Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }

        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public static bool SmartCollisionCheck(BaseActor first, BaseActor other)
        {
            // Extract collision data
            var otherTextureData =
                new Color[other.Sprite.Width * other.Sprite.Height];
            other.Sprite.GetData(otherTextureData);

            var firstTextureData =
                new Color[first.Sprite.Width * first.Sprite.Height];
            first.Sprite.GetData(firstTextureData);

            var otherOrigin = new Vector2(other.Sprite.Width / 2, other.Sprite.Height / 2);

            var firstOrigin = new Vector2(first.Sprite.Width / 2, first.Sprite.Height / 2);

            Matrix firstTransform =
                Matrix.CreateTranslation(new Vector3(-firstOrigin, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(first.Position, 0.0f));

            // Build the block's transform
            Matrix otherTransform =
                Matrix.CreateTranslation(new Vector3(-otherOrigin, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(other.Position, 0.0f));

            // Get the bounding rectangle of the person
            Rectangle firstRectangle = CalculateBoundingRectangle(
                     new Rectangle(0, 0, first.Sprite.Width, first.Sprite.Height), firstTransform);

            // Calculate the bounding rectangle of this block in world space
            Rectangle otherRectangle = CalculateBoundingRectangle(
                     new Rectangle(0, 0, other.Sprite.Width, other.Sprite.Height), otherTransform);

            // The per-pixel check is expensive, so check the bounding rectangles
            // first to prevent testing pixels when collisions are impossible.
            if (firstRectangle.Intersects(otherRectangle))
            {
                // Check collision with person
                if (IntersectPixels(firstTransform, first.Sprite.Width,
                                    first.Sprite.Height, firstTextureData,
                                    otherTransform, other.Sprite.Width,
                                    other.Sprite.Height, otherTextureData))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TestBoundingBoxCollision(BaseActor first, BaseActor other)
        {
            return first.BoundingBox().Intersects(other.BoundingBox());
        }

        public static bool TestBoundingBoxCollision(Rectangle first, Rectangle other)
        {
            return first.Intersects(other);
        }
    }
}
