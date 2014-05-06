using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
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
        void ProcessInputs(GameManager gm, ref GameTime gameTime);
    }

    class KeyboardInputs : IKeyboardInputs
    {
        private const int InitialDelay = 250;
        private const int SmallDelays = 100;
        private const int DropDelay = 25;

        private TimeSpan[] LastTimeKeyWasPressed;
        private TimeSpan[] RepetitionDelay;

        private KeyboardState newKbState, oldKbState;

        public KeyboardInputs()
        {
            int NumElements = InputKey.GetNames(typeof(InputKey)).Length;
            
            LastTimeKeyWasPressed = new TimeSpan[NumElements];
            RepetitionDelay = new TimeSpan[NumElements];
            for (int i = 0; i < NumElements; i++) {
                RepetitionDelay[i] = TimeSpan.Zero;
                LastTimeKeyWasPressed[i] = TimeSpan.Zero;
            }
        }

        public void OnKeyDown(GameManager gm, GameTime gameTime, InputKey k)
        {
            int i = (int)k;
            RepetitionDelay[i] = TimeSpan.FromMilliseconds(InitialDelay);
            LastTimeKeyWasPressed[i] = TimeSpan.Zero;

            if (!gm.gravity.Enabled)
            {
                if (k == InputKey.DebugMoveUpKey)
                {
                    gm.CurrentShape.Move(gm.playField, MoveDirection.MoveUp);
                }
                if (k == InputKey.DebugMoveDownKey)
                {
                    gm.CurrentShape.Move(gm.playField, MoveDirection.MoveDown);
                }
            }

            if (k == InputKey.LeftKey)
            {
                if (gm.CurrentShape.Move(gm.playField, MoveDirection.MoveLeft))
                {
                    gm.soundManager.Play(TetrisSoundsFX.MoveSound);

                }
            }
            if (k == InputKey.RightKey)
            {
                if (gm.CurrentShape.Move(gm.playField, MoveDirection.MoveRight))
                {
                    gm.soundManager.Play(TetrisSoundsFX.MoveSound);

                }
            }

            if (k == InputKey.RotateClockwise)
            {
                if (gm.CurrentShape.Rotate(gm.playField, RotateDirection.Clockwise))
                {
                    gm.soundManager.Play(TetrisSoundsFX.MoveSound);
                }
            }
            if (k == InputKey.RotateCounterClockwise)
            {
                if (gm.CurrentShape.Rotate(gm.playField, RotateDirection.CounterClockwise))
                {
                    gm.soundManager.Play(TetrisSoundsFX.MoveSound);
                }
            }

            if (k == InputKey.FallKey)
            {
                gm.gravity.SetGravitySpeed(TimeSpan.FromMilliseconds(DropDelay));
                gm.gravity.GravityTimer = TimeSpan.Zero;
            }

            if (k == InputKey.ResetKey)
            {
                gm.Reset();
            }

            if (k == InputKey.TurnOffGravityKey) {
                gm.gravity.Enabled = !gm.gravity.Enabled;
            }
        }

        public void OnKeyUp(GameManager gm, GameTime gameTime, InputKey k)
        { 
            if (k == InputKey.FallKey)
            {
                gm.gravity.SetGravitySpeed(TimeSpan.FromMilliseconds(1000));
            }
        }

        public void OnKeyPressed(GameManager gm, GameTime gameTime, InputKey k)
        {
            if (k == InputKey.LeftKey || k == InputKey.RightKey)
            {
                int i = (int)k;
                LastTimeKeyWasPressed[i] += gameTime.ElapsedGameTime;
                while (LastTimeKeyWasPressed[i] > RepetitionDelay[i])
                {
                    LastTimeKeyWasPressed[i] -= RepetitionDelay[i];
                    RepetitionDelay[i] = TimeSpan.FromMilliseconds(SmallDelays);

                    if (gm.CurrentShape.Move(gm.playField, k == InputKey.LeftKey ? MoveDirection.MoveLeft : MoveDirection.MoveRight))
                    {
                        gm.soundManager.Play(TetrisSoundsFX.MoveSound);
                    }
                }
            }

            if (k == InputKey.RotateClockwise || k == InputKey.RotateCounterClockwise)
            {
                int i = (int)k;
                LastTimeKeyWasPressed[i] += gameTime.ElapsedGameTime;
                while (LastTimeKeyWasPressed[i] > RepetitionDelay[i])
                {
                    LastTimeKeyWasPressed[i] -= RepetitionDelay[i];
                    RepetitionDelay[i] = TimeSpan.FromMilliseconds(SmallDelays);

                    if (gm.CurrentShape.Rotate(gm.playField, k == InputKey.RotateClockwise ? RotateDirection.Clockwise : RotateDirection.CounterClockwise))
                    {
                        gm.soundManager.Play(TetrisSoundsFX.RotateSound);
                    }
                }
            }
        }

        public void ProcessKey(GameManager gm, GameTime gameTime, Keys k)
        {
            InputKey ik = KeysToInputKey(k);

            if (this.IsKeyPressed(k))
                OnKeyDown(gm, gameTime, ik);
            if (this.IsKeyReleased(k))
                OnKeyUp(gm, gameTime, ik);
            if((oldKbState.IsKeyDown(k) && newKbState.IsKeyDown(k)))
                OnKeyPressed(gm, gameTime, ik);
        }

        public void ProcessInputs(GameManager gm, ref GameTime gameTime)
        {
            newKbState = Keyboard.GetState();

            if (gm.GetGameState() == GameStates.Playing)
            {
                ProcessKey(gm, gameTime, Keys.Up);
                ProcessKey(gm, gameTime, Keys.Down);
                ProcessKey(gm, gameTime, Keys.Left);
                ProcessKey(gm, gameTime, Keys.Right);
                ProcessKey(gm, gameTime, Keys.Space);
                ProcessKey(gm, gameTime, Keys.NumPad3);
                ProcessKey(gm, gameTime, Keys.NumPad1);
            }
            ProcessKey(gm, gameTime, Keys.P);
            ProcessKey(gm, gameTime, Keys.G);
            ProcessKey(gm, gameTime, Keys.R);

            oldKbState = newKbState;
        }

        private InputKey KeysToInputKey(Keys k)
        {
            switch (k)
            {
                //case Keys.Up: return InputKey.DebugMoveUpKey;
                //case Keys.Down: return InputKey.DebugMoveDownKey;
                case Keys.Left: return InputKey.LeftKey;
                case Keys.Right: return InputKey.RightKey;
                case Keys.Down: return InputKey.FallKey;
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
