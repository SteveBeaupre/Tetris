using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Tetris
{
    public enum Shapes { I, J, L, O, S, T, Z };
    public enum ShapeOrientation { _0_degree, _90_degree, _180_degree, _270_degree };

    class BaseTetrisShape
    {
        //public const int NumShapesType = 7;
        public const int NumBlockPerShapes = 4;

        private bool Visible;

        public Shapes ShapeType;
        public Color ShapeColor;

        protected TetrisMatrix TranslationMatrix;
        protected TetrisMatrix[] RotationMatrix;

        protected TetrisMatrix InitialBlockPosition;
        protected TetrisMatrix InitialTranslationMatrix;

        protected TetrisMatrix ShapePositionMatrix;
        protected TetrisMatrix ShapeRotationMatrix;
        protected ShapeOrientation ShapeOrientation;

        public BaseTetrisShape()
        {
            TranslationMatrix = new TetrisMatrix(new Vec2(0, 1), new Vec2(0, -1), new Vec2(-1, 0), new Vec2(1, 0));

            RotationMatrix = new TetrisMatrix[4];
            for (int i = 0; i < 4; i++)
                RotationMatrix[i] = new TetrisMatrix();

            ShapePositionMatrix = new TetrisMatrix(new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0));
            ShapeRotationMatrix = new TetrisMatrix(new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0));
            ShapeOrientation = ShapeOrientation._0_degree;
            
            Initialize();
        }

        public virtual void Initialize()
        {
            ShapeType = Shapes.I;
            ShapeColor = Color.Black;

            InitialBlockPosition = new TetrisMatrix();
            InitialTranslationMatrix = new TetrisMatrix();
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        public bool IsCellPositionValid(PlayField playField, PlayField.CellData Cell, int x, int y)
        {
            return playField.IsBlockCoordValid(x, y) && !Cell.IsCellFilled;
        }

        private TetrisMatrix GetMultipliedMatrix(bool MultiplyRotationMatrix = true)
        {
            // This return the real block location, with or without rotation 
            TetrisMatrix mat = InitialBlockPosition;
            mat += InitialTranslationMatrix;
            mat += ShapePositionMatrix;
            if (MultiplyRotationMatrix)
                mat += ShapeRotationMatrix;
            return mat;
        }

        public Vec2 GetBlockPosition(int i)
        {
            TetrisMatrix mat = GetMultipliedMatrix();
            return mat.Mat[i];
        }

        private ShapeOrientation ChangeOrientation(RotateDirection Dir)
        {
            int i = (int)ShapeOrientation;

            switch(Dir)
            {
                case RotateDirection.Clockwise: i++; if (i > 3){i = 0;} break;
                case RotateDirection.CounterClockwise: i--; if (i < 0){i = 3;} break;
            }

            return (ShapeOrientation)i;
        }

        public bool Rotate(PlayField playField, RotateDirection Dir) 
        {
            if (!CanRotate(playField, Dir))
                return false;


            // Change the orientation flag and convert it to an int
            ShapeOrientation = ChangeOrientation(Dir);
            int i = (int)ShapeOrientation;

            // Set matrix to last matrix
            TetrisMatrix RotMatrix = new TetrisMatrix();
            RotMatrix = RotationMatrix[i];

            // Add previous matrix, if any (only affect the two last matrix)
            while (i > 0) {
                RotMatrix += RotationMatrix[i - 1];
                i--;
            } 
                
            // Update the shape rotation matrix
            ShapeRotationMatrix = RotMatrix;

            return true;
        }

        public bool CanRotate(PlayField playField, RotateDirection Dir) 
        {
            // Change the orientation flag and convert it to an int
            ShapeOrientation TmpShapeOrientation = ChangeOrientation(Dir);
            int i = (int)TmpShapeOrientation;

            // Set matrix to last matrix
            TetrisMatrix RotMatrix = new TetrisMatrix();
            RotMatrix = RotationMatrix[i];

            // Add previous matrix, if any (only affect the two last matrix)
            while (i > 0)
            {
                RotMatrix += RotationMatrix[i - 1];
                i--;
            }

            // Get the current blocks position matrix
            TetrisMatrix BlockPos = GetMultipliedMatrix(false);

            // Add the temp. rotation matrix to the block position
            BlockPos += RotMatrix;

            // For each block
            for (i = 0; i < NumBlockPerShapes; i++)
            {
                // Simplify...
                int x = BlockPos.Mat[i].x;
                int y = BlockPos.Mat[i].y;

                // return false if one of the position is invalid
                if (!IsCellPositionValid(playField, playField.GetCell(x, y), x, y))
                    return false;
            }


            // return true if we can rotate there
            return true;
        }

        public bool Move(PlayField playField, MoveDirection Dir)
        {
            if (!CanMove(playField, Dir))
                return false;

            int d = (int)Dir;
            // Update the shape position matrix
            ShapePositionMatrix += TranslationMatrix.Mat[d];

            return true;
        }

        public bool CanMove(PlayField playField, MoveDirection Dir)
        {
            // Get the current blocks position matrix
            TetrisMatrix BlockPos = GetMultipliedMatrix();

            int d = (int)Dir;
            BlockPos += TranslationMatrix.Mat[d];

            // For each block
            for (int i = 0; i < NumBlockPerShapes; i++)
            {
                // Add the move direction vector

                // Simplify...
                int x = BlockPos.Mat[i].x;
                int y = BlockPos.Mat[i].y;

                // return false if one of the position is invalid
                if (!IsCellPositionValid(playField, playField.GetCell(x, y), x, y))
                    return false;
            }

            // return true if we can move there
            return true;
        }

        public bool CanSpawn(PlayField playField)
        {
            // Get the current blocks position matrix
            TetrisMatrix BlockPos = GetMultipliedMatrix();

            // For each block
            for (int i = 0; i < NumBlockPerShapes; i++)
            {
                // Add the move direction vector

                // Simplify...
                int x = BlockPos.Mat[i].x;
                int y = BlockPos.Mat[i].y;

                // return false if the shape can't spawn here
                if (!IsCellPositionValid(playField, playField.GetCell(x, y), x, y))
                    return false;
            }

            return true;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D Block, PlayField playField)
        {
            if (!Visible)
                return;

            TetrisMatrix BlockPos = GetMultipliedMatrix();

            for (int i = 0; i < NumBlockPerShapes; i++)
            {
                int x = BlockPos.Mat[i].x;
                int y = BlockPos.Mat[i].y;

                if (playField.IsCellCoordValid(x, y))
                {
                    PlayField.CellData Cell = playField.GetCell(x, y);

                    int CellSize = playField.GetCellSize();
                    Vec2 CellPos = new Vec2(Cell.CellPosition.x, Cell.CellPosition.y);

                    spriteBatch.Draw(Block, new Rectangle(CellPos.x, CellPos.y, CellSize, CellSize), ShapeColor);
                }
            }
        }
    }

    class Shape_I : BaseTetrisShape
    {
        public override void Initialize()
        {
            ShapeType = Shapes.I;
            ShapeColor = Color.Cyan;

            RotationMatrix[0].set(new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0));
            RotationMatrix[1].set(new Vec2(2, -2), new Vec2(1, -1), new Vec2(0, 0), new Vec2(-1, 1));
            RotationMatrix[2].set(new Vec2(1, 2), new Vec2(0, 1), new Vec2(-1, 0), new Vec2(-2, -1));
            RotationMatrix[3].set(new Vec2(-2, 1), new Vec2(-1, 0), new Vec2(0, -1), new Vec2(1, -2));

            InitialBlockPosition = new TetrisMatrix(new Vec2(0, 2), new Vec2(1, 2), new Vec2(2, 2), new Vec2(3, 2));
            InitialTranslationMatrix = new TetrisMatrix(new Vec2(3, -2));
        }
    }

    class Shape_J : BaseTetrisShape
    {
        public override void Initialize()
        {
            ShapeType = Shapes.J;
            ShapeColor = Color.Blue;

            RotationMatrix[0].set(new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0));
            RotationMatrix[1].set(new Vec2(1, -1), new Vec2(0, 0), new Vec2(-1, 1), new Vec2(-2, 0));
            RotationMatrix[2].set(new Vec2(1, 1), new Vec2(0, 0), new Vec2(-1, -1), new Vec2(0, -2));
            RotationMatrix[3].set(new Vec2(-1, 1), new Vec2(0, 0), new Vec2(1, -1), new Vec2(2, 0));

            InitialBlockPosition = new TetrisMatrix(new Vec2(0, 1), new Vec2(1, 1), new Vec2(2, 1), new Vec2(2, 2));
            InitialTranslationMatrix = new TetrisMatrix(new Vec2(3, -1));
        }
    }

    class Shape_L : BaseTetrisShape
    {
        public override void Initialize()
        {
            ShapeType = Shapes.L;
            ShapeColor = Color.Orange;

            RotationMatrix[0].set(new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0));
            RotationMatrix[1].set(new Vec2(0, -2), new Vec2(1, -1), new Vec2(0, 0), new Vec2(-1, 1));
            RotationMatrix[2].set(new Vec2(2, 1), new Vec2(1, 2), new Vec2(0, 1), new Vec2(-1, 0));
            RotationMatrix[3].set(new Vec2(-1, 1), new Vec2(-2, 0), new Vec2(-1, -1), new Vec2(0, -2));

            InitialBlockPosition = new TetrisMatrix(new Vec2(0, 2), new Vec2(0, 1), new Vec2(1, 1), new Vec2(2, 1));
            InitialTranslationMatrix = new TetrisMatrix(new Vec2(3, -1));
        }
    }

    class Shape_O : BaseTetrisShape
    {
        public override void Initialize()
        {
            ShapeType = Shapes.O;
            ShapeColor = Color.Yellow;
 
            InitialBlockPosition = new TetrisMatrix(new Vec2(0, 0), new Vec2(1, 0), new Vec2(1, 1), new Vec2(0, 1));
            InitialTranslationMatrix = new TetrisMatrix(new Vec2(3, 0));
       }
    }

    class Shape_S : BaseTetrisShape
    {
        public override void Initialize()
        {
            ShapeType = Shapes.S;
            ShapeColor = Color.Lime;

            RotationMatrix[0].set(new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0));
            RotationMatrix[1].set(new Vec2(1, -2), new Vec2(0, -1), new Vec2(1, 0), new Vec2(0, 1));
            RotationMatrix[2].set(new Vec2(1, 1), new Vec2(0, 0), new Vec2(-1, 1), new Vec2(-2, 0));
            RotationMatrix[3].set(new Vec2(-1, 1), new Vec2(0, 0), new Vec2(-1, -1), new Vec2(0, -2));

            InitialBlockPosition = new TetrisMatrix(new Vec2(0, 2), new Vec2(1, 2), new Vec2(1, 1), new Vec2(2, 1));
            InitialTranslationMatrix = new TetrisMatrix(new Vec2(3, -1));
        }
    }

    class Shape_T : BaseTetrisShape
    {
        public override void Initialize()
        {
            ShapeType = Shapes.T;
            ShapeColor = Color.DarkMagenta;

            RotationMatrix[0].set(new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0));
            RotationMatrix[1].set(new Vec2(1, -1), new Vec2(-1, 1), new Vec2(-1, -1), new Vec2(0, 0));
            RotationMatrix[2].set(new Vec2(1, 1), new Vec2(-1, -1), new Vec2(1, -1), new Vec2(0, 0));
            RotationMatrix[3].set(new Vec2(-1, 1), new Vec2(1, -1), new Vec2(1, 1), new Vec2(0, 0));

            InitialBlockPosition = new TetrisMatrix(new Vec2(0, 1), new Vec2(2, 1), new Vec2(1, 2), new Vec2(1, 1));
            InitialTranslationMatrix = new TetrisMatrix(new Vec2(3, -1));
        }
    }

    class Shape_Z : BaseTetrisShape
    {
        public override void Initialize()
        {
            ShapeType = Shapes.Z;
            ShapeColor = Color.Red;

            RotationMatrix[0].set(new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0), new Vec2(0, 0));
            RotationMatrix[1].set(new Vec2(2, -1), new Vec2(1, 0), new Vec2(0, -1), new Vec2(-1, 0));
            RotationMatrix[2].set(new Vec2(0, 2), new Vec2(-1, 1), new Vec2(0, 0), new Vec2(-1, -1));
            RotationMatrix[3].set(new Vec2(-2, 0), new Vec2(-1, -1), new Vec2(0, 0), new Vec2(1, -1));

            InitialBlockPosition = new TetrisMatrix(new Vec2(0, 1), new Vec2(1, 1), new Vec2(1, 2), new Vec2(2, 2));
            InitialTranslationMatrix = new TetrisMatrix(new Vec2(3, -1));
        }
    }

}

