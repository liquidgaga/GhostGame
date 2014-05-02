using System;
using System.Collections.Generic;
using System.Linq;
using GhostGame.Actors;
using Microsoft.Xna.Framework;

namespace GhostGame.Enums
{
    public static class PlayerIndexMapping
    {
        public static Dictionary<int, PlayerIndex> map = new Dictionary<int, PlayerIndex>()
        {
            {0, PlayerIndex.One},
            {1, PlayerIndex.Two},
            {2, PlayerIndex.Three},
            {3, PlayerIndex.Four},
        };
    }
}
