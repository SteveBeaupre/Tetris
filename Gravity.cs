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
        private TimeSpan GravitySpeed;

        public bool Enabled;

        private bool[] LinesCleared;

        public Gravity()
        {
            Enabled = true;
            Reset();
        }

        public void Reset()
        {
            LinesCleared = new bool[PlayField.NumCellsOnYAxis];
            GravityTimer = TimeSpan.Zero;
            SetGravitySpeed(TimeSpan.FromMilliseconds(1000));
        }

        public void Update(GameManager gm, ref GameTime gameTime)
        {
            if (!Enabled)
                return;

            GravityTimer += gameTime.ElapsedGameTime;

            if (GravityTimer > GravitySpeed)
            {
                while (GravityTimer > GravitySpeed)
                    GravityTimer -= GravitySpeed;

                if (gm.GetGameState() == GameStates.Playing)
                {
                    // Can we move the shape down?
                    if (gm.CurrentShape.CanMove(gm.playField, MoveDirection.MoveDown))
                    {
                        // Move it down 
                        gm.CurrentShape.Move(gm.playField, MoveDirection.MoveDown);
                    }
                    else
                    {
                        // Reset the block falling speed
                        gm.soundManager.Play(TetrisSoundsFX.LockSound);

                        // We can't move it down further, so add it to the static array 
                        if (!gm.playField.AddShapeToCellsArray(gm.CurrentShape)) 
                        {
                            gm.SetGameState(GameStates.GameOver);
                            return;
                        }

                        gm.CurrentShape.Hide();

                        // Clear the lines
                        if (gm.playField.ClearLines(ref LinesCleared))
                        {
                            // We have some lines to clear...
                            GravitySpeed = TimeSpan.FromMilliseconds(250);
                            gm.SetGameState(GameStates.Clearing);
                        }
                        else 
                        {
                            // Nothing to clear
                            gm.SpawnRandomShape();
                            GravitySpeed = TimeSpan.FromMilliseconds(1000);
                        }
                    }
                }
                else if (gm.GetGameState() == GameStates.Clearing)
                {
                    DoNaiveGravity(gm);

                    int NumLinesToClearLeft = 0;
                    for (int y = 0; y < PlayField.NumCellsOnYAxis; y++)
                    {
                        if (LinesCleared[y])
                        {
                            NumLinesToClearLeft++;
                        }
                    }

                    if (NumLinesToClearLeft == 0)
                    {
                        gm.SpawnRandomShape();
                        GravitySpeed = TimeSpan.FromMilliseconds(1000);
                        gm.SetGameState(GameStates.Playing);
                    }
                    else
                    {
                        gm.soundManager.Play(TetrisSoundsFX.LockSound);
                    }
                }
            }
        }

        public void DoNaiveGravity(GameManager gm)
        {
            // move each row above the first cleared one by one
            for (int y = PlayField.NumCellsOnYAxis - 1; y >= 0; y--)
            {
                if (LinesCleared[y])
                {
                    gm.playField.MoveRowsDown(y - 1);

                    LinesCleared[y] = false;
                    for (int z = y; z > 0; z--)
                        LinesCleared[z] = LinesCleared[z - 1];

                    break;
                }
            }
        }

        public void DoStickyGravity(GameManager gm)
        {

        
        }

        public TimeSpan GetGravitySpeed()
        {
            return GravitySpeed;
        }

        public void SetGravitySpeed(TimeSpan Speed)
        {
            GravitySpeed = Speed;
        }

    }
}
