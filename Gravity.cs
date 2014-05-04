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
        public  TimeSpan GravityTimer;
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
            GravityTimer = TimeSpan.Zero;
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

        public void Update(GameManager gm, ref GameTime gameTime)
        {
            if (!Enabled)
                return;

            GravityTimer += gameTime.ElapsedGameTime;

            if (GravityTimer > BlocksFallSpeed)
            {
                while (GravityTimer > BlocksFallSpeed)
                    GravityTimer -= BlocksFallSpeed;

                // Can we move the shape down?
                if (gm.CurrentShape.CanMove(gm.Grid, MoveDirection.MoveDown))
                {
                    // Move it down 
                    gm.CurrentShape.Move(gm.Grid, MoveDirection.MoveDown);
                }
                else
                {
                    // We can't move it down further, so add it to the static array 
                    gm.Grid.AddShapeToCellsArray(gm.CurrentShape);
                    gm.Grid.ClearLines(gm.CurrentShape);

                    // Create a new shape
                    gm.SpawnRandomShape();
                    // Reset the block falling speed
                    BlocksFallSpeed = TimeSpan.FromMilliseconds(1000);
                    gm.soundManager.Play(TetrisSoundsFX.StickSound);
                }
            }
        }
    }
}
