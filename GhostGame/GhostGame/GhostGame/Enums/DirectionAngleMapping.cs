using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GhostGame.Enums
{
    public static class DirectionAngleMapping
    {
        public static Dictionary<Direction, float> map = new Dictionary<Direction, float>()
        {
            {Direction.D, (float)(Math.PI * 1.5)},
            {Direction.DL, (float) (Math.PI * 1.25)},
            {Direction.DR, (float)(Math.PI * 1.75)},
            {Direction.L, (float) Math.PI},
            {Direction.R, 0},
            {Direction.U, (float)(Math.PI * .5)},
            {Direction.UL, (float) (Math.PI * .75)},
            {Direction.UR, (float)(Math.PI * .25)},
        };
    }
}
