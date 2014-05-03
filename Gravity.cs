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
    class Gravity
    {
        private TimeSpan ElapsedTime;
        private TimeSpan BlocksFallSpeed;
        private TimeSpan ClearRowSpeed;

        public bool Enabled;

        public Gravity()
        {
            Enabled = true;
            Reset();
        }

        public void Reset()
        {
            ElapsedTime = TimeSpan.Zero;
            BlocksFallSpeed = TimeSpan.FromMilliseconds(1000);
            ClearRowSpeed = TimeSpan.FromMilliseconds(200);
        }

        public TimeSpan GetBlockFallingSpeed()
        {
            return BlocksFallSpeed;
        }

        public void SetBlockFallingSpeed(TimeSpan Speed)
        {
            BlocksFallSpeed = Speed;
        }

        public TimeSpan GetElapsedTime()
        {
            return ElapsedTime;
        }

        public void ResetElapsedTime(TimeSpan Elapsed)
        {
            ElapsedTime = TimeSpan.Zero;
        }

        public void SetElapsedTime(TimeSpan Elapsed)
        {
            ElapsedTime = Elapsed;
        }

        public void IncElapsedTime(TimeSpan Elapsed)
        {
            ElapsedTime += Elapsed;
        }

        public void Update(GameManager gm, ref GameTime gameTime, ref CellsGrid Grid, ref BaseTetrisShape CurrentShape)
        {
            if (!Enabled)
                return;

            ElapsedTime += gameTime.ElapsedGameTime;

            if (ElapsedTime > BlocksFallSpeed)
            {
                ElapsedTime -= BlocksFallSpeed;

                // Can we move the shape down?
                if (CurrentShape.CanMove(Grid, MoveDirection.MoveDown))
                {
                    // Move it down 
                    CurrentShape.Move(Grid, MoveDirection.MoveDown);
                }
                else
                {
                    // We can't move it down further, so add it to the static array 
                    Grid.AddShapeToCellsArray(CurrentShape);
                    Grid.ClearLines(CurrentShape);

                    // Create a new shape
                    gm.SpawnRandomShape();

                    // Reset the block falling speed
                    BlocksFallSpeed = TimeSpan.FromMilliseconds(1000);
                }
            }
        }
    }
}
