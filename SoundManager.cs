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
    enum TetrisSoundsFX { MoveSound, RotateSound, LockSound }

    class SoundManager
    {
        private SoundEffect[] Effects;

        public SoundManager()
        {
            int NumItems = TetrisSoundsFX.GetNames(typeof(TetrisSoundsFX)).Length;
            Effects = new SoundEffect[NumItems];
        }

        public void Load(ContentManager cm)
        {
            Effects[(int)TetrisSoundsFX.MoveSound] = cm.Load<SoundEffect>("Move");
            Effects[(int)TetrisSoundsFX.RotateSound] = cm.Load<SoundEffect>("Rotate");
            Effects[(int)TetrisSoundsFX.LockSound] = cm.Load<SoundEffect>("Lock");
        }

        public void Play(TetrisSoundsFX SndFX)
        {
            Effects[(int)SndFX].Play();
        }
    }
}
