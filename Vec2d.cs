using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetris
{
    struct Vec2
    {
        public int x, y;

        public Vec2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vec2 operator +(Vec2 v1, Vec2 v2)
        {
            Vec2 Tmp;
            Tmp.x = v1.x + v2.x;
            Tmp.y = v1.y + v2.y;
            return Tmp;
        }
    }
}
