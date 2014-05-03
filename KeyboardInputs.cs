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
    public enum InputKey { UnknowKey, LeftKey, RightKey, FallKey, RotateClockwise, RotateCounterClockwise, PauseKey, ResetKey, TurnOffGravityKey , DebugMoveDownKey , DebugMoveUpKey};

    public enum MoveDirection { MoveDown, MoveUp, MoveLeft, MoveRight };
    public enum RotateDirection { Clockwise, CounterClockwise };

    interface IKeyboardInputs
    {
        void ProcessInputs(GameManager gm, ref CellsGrid Grid, ref BaseTetrisShape CurrentShape, ref Gravity GameGravity);
    }

    class KeyboardInputs : IKeyboardInputs
    {
        private KeyboardState newKbState, oldKbState;

        public void OnKeyDown(GameManager gm, CellsGrid Grid, BaseTetrisShape CurrentShape, Gravity GameGravity, InputKey k)
        {
            if (!GameGravity.Enabled) {
                if (k == InputKey.DebugMoveUpKey)
                {
                    CurrentShape.Move(Grid, MoveDirection.MoveUp);
                }
                if (k == InputKey.DebugMoveDownKey)
                {
                    CurrentShape.Move(Grid, MoveDirection.MoveDown);
                }
            }

            if (k == InputKey.LeftKey)
            {
                CurrentShape.Move(Grid, MoveDirection.MoveLeft);
            }
            if (k == InputKey.RightKey)
            {
                CurrentShape.Move(Grid, MoveDirection.MoveRight);
            }

            if (k == InputKey.RotateClockwise)
            {
                CurrentShape.Rotate(Grid, RotateDirection.Clockwise);
            }
            if (k == InputKey.RotateCounterClockwise)
            {
                CurrentShape.Rotate(Grid, RotateDirection.CounterClockwise);
            }

            if (k == InputKey.FallKey)
            {
                GameGravity.SetBlockFallingSpeed(TimeSpan.FromMilliseconds(50));
            }

            if (k == InputKey.ResetKey)
            {
                gm.Reset();
            }

            if (k == InputKey.TurnOffGravityKey) {
                GameGravity.Enabled = !GameGravity.Enabled;
            }
        }

        public void OnKeyUp(GameManager gm, CellsGrid Grid, BaseTetrisShape CurrentShape, Gravity GameGravity, InputKey k)
        { 
            if (k == InputKey.FallKey)
            {
                GameGravity.SetBlockFallingSpeed(TimeSpan.FromMilliseconds(1000));
            }

        }

        public void ProcessKey(GameManager gm, CellsGrid Grid, BaseTetrisShape CurrentShape, Gravity GameGravity, Keys k)
        {
            InputKey ik = KeysToInputKey(k);

            if (this.IsKeyPressed(k))
                OnKeyDown(gm, Grid, CurrentShape, GameGravity, ik);
            if (this.IsKeyReleased(k))
                OnKeyUp(gm, Grid, CurrentShape, GameGravity, ik);
        }

        public void ProcessInputs(GameManager gm, ref CellsGrid Grid, ref BaseTetrisShape CurrentShape, ref Gravity GameGravity)
        {
            newKbState = Keyboard.GetState();

            ProcessKey(gm, Grid, CurrentShape, GameGravity, Keys.Up);
            ProcessKey(gm, Grid, CurrentShape, GameGravity, Keys.Down);
            ProcessKey(gm, Grid, CurrentShape, GameGravity, Keys.Left);
            ProcessKey(gm, Grid, CurrentShape, GameGravity, Keys.Right);
            ProcessKey(gm, Grid, CurrentShape, GameGravity, Keys.Space);
            ProcessKey(gm, Grid, CurrentShape, GameGravity, Keys.NumPad3);
            ProcessKey(gm, Grid, CurrentShape, GameGravity, Keys.NumPad1);
            ProcessKey(gm, Grid, CurrentShape, GameGravity, Keys.P);
            ProcessKey(gm, Grid, CurrentShape, GameGravity, Keys.G);
            ProcessKey(gm, Grid, CurrentShape, GameGravity, Keys.R);

            oldKbState = newKbState;
        }

        private InputKey KeysToInputKey(Keys k)
        {
            switch (k)
            {
                case Keys.Down: return InputKey.DebugMoveDownKey;
                case Keys.Up: return InputKey.DebugMoveUpKey;
                case Keys.Left: return InputKey.LeftKey;
                case Keys.Right: return InputKey.RightKey;
                case Keys.Space: return InputKey.FallKey;
                case Keys.NumPad1: return InputKey.RotateCounterClockwise;
                case Keys.NumPad3: return InputKey.RotateClockwise;
                case Keys.P: return InputKey.PauseKey;
                case Keys.G: return InputKey.TurnOffGravityKey;
                case Keys.R: return InputKey.ResetKey;
            }

            return InputKey.UnknowKey;
        }

        private bool IsKeyPressed(Keys k)
        {
            return (oldKbState.IsKeyUp(k) && newKbState.IsKeyDown(k));
        }

        private bool IsKeyReleased(Keys k)
        {
            return (oldKbState.IsKeyDown(k) && newKbState.IsKeyUp(k));
        }
   }
}
