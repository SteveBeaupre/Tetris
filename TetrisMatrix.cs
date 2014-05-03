using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetris
{
    class TetrisMatrix
    {
        public Vec2[] Mat;

        public TetrisMatrix()
        {
            Mat = new Vec2[4] { new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0) };
        }

        public TetrisMatrix(Vec2 v1, Vec2 v2, Vec2 v3, Vec2 v4)
        {
            Mat = new Vec2[4] { v1, v2, v3, v4 };
        }

        public TetrisMatrix(Vec2 v)
        {
            Mat = new Vec2[4] { v, v, v, v };
        }

        public void set(Vec2 v1, Vec2 v2, Vec2 v3, Vec2 v4)
        {
            Mat[0] = v1;
            Mat[1] = v2;
            Mat[2] = v3;
            Mat[3] = v4;
        }

        public static TetrisMatrix operator +(TetrisMatrix m1, TetrisMatrix m2)
        {
            TetrisMatrix Tmp = new TetrisMatrix();

            for (int i = 0; i < 4; i++)
                Tmp.Mat[i] = m1.Mat[i] + m2.Mat[i];

            return Tmp;
        }

        public static TetrisMatrix operator +(TetrisMatrix m, Vec2 v)
        {
            TetrisMatrix Tmp = new TetrisMatrix();

            for (int i = 0; i < 4; i++)
                Tmp.Mat[i] = m.Mat[i] + v;

            return Tmp;
        }
        public static TetrisMatrix operator +(Vec2 v, TetrisMatrix m)
        {
            TetrisMatrix Tmp = new TetrisMatrix();

            for (int i = 0; i < 4; i++)
                Tmp.Mat[i] = m.Mat[i] + v;

            return Tmp;
        }
    }
}
