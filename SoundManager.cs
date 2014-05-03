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
    enum TetrisSoundsFX { MoveSound, StickSound/*, PauseSound*/ }

    class SoundManager
    {
        private SoundEffect[] Effects;

        public SoundManager()
        { 
            int NumItems =  TetrisSoundsFX.GetNames(typeof(Shapes)).Length;
            Effects = new SoundEffect[NumItems];
        }

        public void Load(ContentManager cm)
        {
            Effects[(int)TetrisSoundsFX.MoveSound] = cm.Load<SoundEffect>("Blip");
            Effects[(int)TetrisSoundsFX.StickSound] = cm.Load<SoundEffect>("Stick");
        }

        public void Play(TetrisSoundsFX SndFX)
        {
            Effects[(int)SndFX].Play();
        }
    }
}
