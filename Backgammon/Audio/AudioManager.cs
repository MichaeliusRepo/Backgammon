using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Audio
{
    internal class AudioManager
    {
        private Dictionary<string, SoundEffect> SoundFX = new Dictionary<string, SoundEffect>();
        private ContentManager Content;
        private Song Music;

        private static AudioManager instance;

        internal static AudioManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AudioManager();
                return instance;
            }
        }

        private AudioManager() { MediaPlayer.Stop(); } // Remove default public constructor

        internal void ToggleAudio()
        {
            if (MediaPlayer.State == MediaState.Stopped)
                MediaPlayer.Play(Music);
            else
                MediaPlayer.Stop();
        }

        internal bool AudioMuted()
        {
            return MediaPlayer.State == MediaState.Stopped;
        }

        internal void PlaySound(string name)
        { // Overload parameters at Play() are Volume, Pitch, Pan
            if (!AudioMuted())
                SoundFX[name].Play();
        }

        private void LoadSound(string name)
        {
            SoundFX.Add(name, Content.Load<SoundEffect>("Audio/" + name));
        }

        public void LoadContent(ContentManager Content)
        {
            this.Content = new ContentManager(Content.ServiceProvider, "Content");
            Music = Content.Load<Song>("Audio/Music");
            MediaPlayer.IsRepeating = true;

            LoadSound("Checker");
            LoadSound("MenuClick");
        }

        public void UnloadContent()
        {
            Music.Dispose();
            foreach (SoundEffect sfx in SoundFX.Values)
                sfx.Dispose();
        }

    }
}
