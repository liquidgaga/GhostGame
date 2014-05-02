using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GhostGame.Utils
{
    public static class MapBoundingBoxGenerator
    {
        public static Rectangle[] generateBoundingBoxesArray(Texture2D map)
        {
            var mapTextureData = new Color[map.Width * map.Height];
            List<Rectangle> resultList = new List<Rectangle>();
            map.GetData(mapTextureData);

            for (int yindex = 0; yindex < map.Height; yindex++)
            {
                for (int xindex = 0; xindex < map.Width; xindex++)
                {
                    Rectangle locatedRectangle;

                    // If this is a bounding pixel, and there is no bounding pixel above and to the left, this is top left corner.
                    if (mapTextureData[yindex * map.Width + xindex].pixelIsGreen() //this
                        && (yindex <= 0 || !mapTextureData[(yindex - 1) * map.Width + xindex].pixelIsGreen()) //up
                        && (xindex <= 0 || !mapTextureData[yindex * map.Width + (xindex - 1)].pixelIsGreen())) //left
                    {
                        locatedRectangle = new Rectangle();
                        //We have found an upper left corner
                        locatedRectangle.X = xindex;
                        locatedRectangle.Y = yindex;

                        for (int hfinder = xindex; xindex < map.Width; hfinder++)
                        {
                            if (yindex * map.Width + hfinder > mapTextureData.Length || !mapTextureData[yindex * map.Width + hfinder].pixelIsGreen())
                            {

                                locatedRectangle.Width = hfinder - xindex - 1;
                                break;
                            }
                        }

                        for (int wfinder = yindex; wfinder < map.Height; wfinder++)
                        {
                            if (wfinder * map.Width + xindex > mapTextureData.Length || !mapTextureData[wfinder * map.Width + xindex].pixelIsGreen())
                            {
                                locatedRectangle.Height = wfinder - yindex - 1;
                                break;
                            }
                        }
                        resultList.Add(locatedRectangle);
                    }
                }
            }
            return resultList.ToArray();
        }

        private static bool pixelIsGreen(this Color color){
            return color.B == 76 && color.R == 34 && color.G == 177;
        }
    }
}
