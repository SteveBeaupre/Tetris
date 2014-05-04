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

using Tetris;

namespace Tetris
{
    enum GameStates { Playing, Paused, Clearing, GameOver};

    interface IGameManager
    {
        void Reset();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, Texture2D Block, Texture2D Background);
    }

    class GameManager : IGameManager
    {
        GameStates gameState;

        KeyboardInputs KbInputs;

        public PlayField playField;
        public BaseTetrisShape CurrentShape;
        public Gravity gravity;
        public TimeSpan ElapsedTime;

        public SoundManager soundManager;

        public GameManager(int ScreenWidth, int ScreenHeight)
        {
            soundManager = new SoundManager();
            playField = new PlayField(ScreenWidth, ScreenHeight);
            KbInputs = new KeyboardInputs();
            gravity = new Gravity();

            Reset();
        }

        public GameStates GetGameState()
        {
            return gameState;
        }

        public void SetGameState(GameStates state)
        {
            gameState = state;
        }

        public void Reset()
        {
            playField.Reset();
            playField.CreateDebugPattern();
            gravity.Reset();
            SpawnRandomShape((int)Shapes.L);
            //GameGravity.Enabled = false;
            ElapsedTime = TimeSpan.Zero;
            gameState = GameStates.Playing;
        }

        public void SpawnRandomShape(int i = -1)
        {
            int NumShapes = Shapes.GetNames(typeof(Shapes)).Length;

            int r;
            if (i < 0)
            {
                Random random = new Random();
                r = random.Next(0, NumShapes);
            }
            else 
            {
                r = i > NumShapes ? NumShapes : i;
            }
            
            switch(r)
            {
                case (int)Shapes.I: CurrentShape = new Shape_I(); break;
                case (int)Shapes.J: CurrentShape = new Shape_J(); break;
                case (int)Shapes.L: CurrentShape = new Shape_L(); break;
                case (int)Shapes.O: CurrentShape = new Shape_O(); break;
                case (int)Shapes.S: CurrentShape = new Shape_S(); break;
                case (int)Shapes.T: CurrentShape = new Shape_T(); break;
                case (int)Shapes.Z: CurrentShape = new Shape_Z(); break;
            }
            
            CurrentShape.Show();
        }

        public void Update(GameTime gameTime)
        {
            KbInputs.ProcessInputs(this, ref gameTime);
            gravity.Update(this, ref gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D Block, Texture2D Background)
        {
            spriteBatch.Begin(); 
            
            spriteBatch.Draw(Background, new Vector2(0, 0), Color.White);

            playField.Draw(spriteBatch, Block);
            CurrentShape.Draw(spriteBatch, Block, playField);

            spriteBatch.End();

        }
    }
}
