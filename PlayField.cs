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

using Tetris;

namespace Tetris
{
    class PlayField
    {
        public const int CellSize = 32;
        public const int NumCellsOnXAxis = 10;
        public const int NumCellsOnYAxis = 15;

        public struct CellData
        {
            public Color CellColor;
            public Vec2 CellPosition;
            public bool IsCellFilled;
        }

        private Rectangle GridRect;

        private CellData[,] Cells = new CellData[NumCellsOnYAxis, NumCellsOnXAxis];

        public PlayField(int ScreenWidth, int ScreenHeight)
        {
            CalculateGridSize(ScreenWidth, ScreenHeight);
            Reset();
        }

        public int GetCellSize()
        {
            return CellSize;
        }

        public CellData GetCell(int x, int y)
        {
            if (IsCellCoordValid(x, y))
            {
                return Cells[y, x];
            }

            CellData tmp;
            tmp.CellColor = Color.White;
            tmp.IsCellFilled = false;
            tmp.CellPosition.x = 0;
            tmp.CellPosition.y = 0;

            return tmp;
        }

        // Use this to test valid block position
        public bool IsBlockCoordValid(int x, int y)
        {
            return x >= 0 && x < NumCellsOnXAxis && y < NumCellsOnYAxis;
        }

        // Use this before accessing the cells array
        public bool IsCellCoordValid(int x, int y)
        {
            return IsBlockCoordValid(x, y) && y >= 0;
        }

        public void Reset()
        {
            for (int y = 0; y < NumCellsOnYAxis; y++)
            {
                for (int x = 0; x < NumCellsOnXAxis; x++)
                {
                    Cells[y, x].CellColor = Color.White;
                    Cells[y, x].IsCellFilled = false;
                }
            }
        }

        public void CreateDebugPattern()
        {
            FillRow(NumCellsOnYAxis - 1, 0, 3);
            FillRow(NumCellsOnYAxis - 1, 5, NumCellsOnXAxis - 1);

            FillRow(NumCellsOnYAxis - 2, 0, 3);
            FillRow(NumCellsOnYAxis - 2, 5, NumCellsOnXAxis - 1);
            Cells[NumCellsOnYAxis - 2, 0].IsCellFilled = false;

            FillRow(NumCellsOnYAxis - 3, 0, 2);
            FillRow(NumCellsOnYAxis - 3, 5, NumCellsOnXAxis - 1);
        }

        // Calculate the grid size
        private void CalculateGridSize(int ScreenWidth, int ScreenHeight)
        {
            // Calculate the grid size
            int GridWidth = CellSize * NumCellsOnXAxis;
            int GridHeight = CellSize * NumCellsOnYAxis;
            int GridLeft = (ScreenWidth - GridWidth) / 2;
            int GridTop = (ScreenHeight - GridHeight) / 2;
            GridRect = new Rectangle(GridLeft, GridTop, GridWidth, GridHeight);

            // Initialize the grid Cells
            for (int y = 0; y < NumCellsOnYAxis; y++)
            {
                for (int x = 0; x < NumCellsOnXAxis; x++)
                {
                    Cells[y, x].CellPosition.x = (GridLeft + (CellSize * x));
                    Cells[y, x].CellPosition.y = (GridTop + (CellSize * y));
                }
            }
        }

        public void AddShapeToCellsArray(BaseTetrisShape CurrentShape) // This goes in game manager
        {
            for (int i = 0; i < 4; i++)
            {
                Vec2 v = CurrentShape.GetBlockPosition(i);
                int x = v.x;
                int y = v.y;

                if (IsCellCoordValid(x, y))
                {
                    Cells[y, x].IsCellFilled = true;
                    Cells[y, x].CellColor = CurrentShape.ShapeColor;
                }
            }

            //ClearLines(CurrentShape);
        }

        public bool IsRowFilled(int y)
        {
            if (y < 0 || y >= NumCellsOnYAxis)
                return false;

            int NumFilled = 0;

            for (int x = 0; x < NumCellsOnXAxis; x++)
            {
                if (Cells[y, x].IsCellFilled)
                    NumFilled++;
            }

            return NumFilled == NumCellsOnXAxis;
        }

        public void FillRow(int y, int x1, int x2)
        {
            if (x1 < 0 || x1 >= NumCellsOnXAxis || x2 < 0 || x2 >= NumCellsOnXAxis || y < 0 || y >= NumCellsOnYAxis)
                return;

            for (int x = x1; x <= x2; x++)
            {
                Cells[y, x].IsCellFilled = true;
            }
        }

        public void ClearRow(int y)
        {
            if (y < 0 || y >= NumCellsOnYAxis)
                return;

            for (int x = 0; x < NumCellsOnXAxis; x++)
            {
                Cells[y, x].IsCellFilled = false;
                Cells[y, x].CellColor = Color.Black;
            }
        }

        public void MoveRowDown(int y)
        {
            if (y < 0 || y >= NumCellsOnYAxis - 1)
                return;

            // Move the upper rows down
            for (int x = 0; x < NumCellsOnXAxis; x++)
            {
                Cells[y + 1, x].CellColor = Cells[y, x].CellColor;
                Cells[y + 1, x].IsCellFilled = Cells[y, x].IsCellFilled;
            }
        }

        public void MoveRowsDown(int y)
        {
            // Move the upper rows down
            while (y > 0)
            {
                MoveRowDown(y--);
            }
        }

        public bool ClearLines(ref bool[] ClearedLines)
        {
            bool res = false;
            int y = NumCellsOnYAxis-1;
            while(y >= 0){

                bool IsFilled = IsRowFilled(y);

                // if all cells of row y are filled
                if (IsFilled)
                {
                    // Clear them
                    ClearRow(y);

                    ClearedLines[y] = true;

                    res = true;

                    // Move the upper rows down
                    //MoveRowsDown(y);
                }/* else {
                    y--;
                }*/
                y--;
            }

            return res;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D Block)
        {
            for (int y = 0; y < NumCellsOnYAxis; y++)
            {
                for (int x = 0; x < NumCellsOnXAxis; x++)
                {
                    bool visible = Cells[y, x].IsCellFilled;

                    if (visible)
                    {
                        CellData Cell = GetCell(x, y);

                        Color BlockColor = Cell.CellColor;
                        Vec2 CellPos = new Vec2(Cell.CellPosition.x, Cell.CellPosition.y);

                        spriteBatch.Draw(Block, new Rectangle(CellPos.x, CellPos.y, CellSize, CellSize), BlockColor);
                    }
                }
            }
        }
    }
}
