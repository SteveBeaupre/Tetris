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

    interface IGameManager
    {
        void Reset();

        void UpdateInputs();
        void UpdateGravity(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch, Texture2D Block, Texture2D Background);
    }

    class GameManager : IGameManager
    {
        KeyboardInputs KbInputs;
        
        CellsGrid Grid;
        BaseTetrisShape CurrentShape;
        Gravity GameGravity;

        public GameManager(int ScreenWidth, int ScreenHeight)
        {
            Grid = new CellsGrid(ScreenWidth, ScreenHeight);
            KbInputs = new KeyboardInputs();
            GameGravity = new Gravity();

            Reset();
        }

        public void Reset()
        {
            Grid.Reset();
            //Grid.CreateDebugPattern();
            GameGravity.Reset();
            SpawnRandomShape((int)Shapes.I);
            //GameGravity.Enabled = false;
        }

        public void SpawnRandomShape(int i = -1)
        {
            int r;
            if (i < 0)
            {
                Random random = new Random();
                r = random.Next(0, 7);
            }
            else 
            {
                r = i > 7 ? 7 : i;
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
        }

        public void UpdateInputs()
        {
            KbInputs.ProcessInputs(this, ref Grid, ref CurrentShape, ref GameGravity);
        }

        public void UpdateGravity(GameTime gameTime)
        {
            GameGravity.Update(this, ref gameTime, ref Grid, ref CurrentShape);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D Block, Texture2D Background)
        {
            spriteBatch.Begin(); 
            
            spriteBatch.Draw(Background, new Vector2(0, 0), Color.White);

            Grid.Draw(spriteBatch, Block);
            CurrentShape.Draw(spriteBatch, Block, Grid);

            spriteBatch.End();

        }
    }
}
