using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

/*** Code by Rajiv Puvvada and Ara Yessayan **/
/*** Modification code to work with MP3's by Sam Dannemiller **/


namespace Lumen
{
    public class AudioManager
    {

        private Dictionary<MusicSelection, Song> songs;              //Dictionaries are useful for keying a selection enum to a particular audio element.
        private Dictionary<SFXSelection, SoundEffect> soundEffects;
        private float SFXVolume;     //Value between 0.0 and 1.0.
        //private MediaPlayer player;

        /*NOTE: Volume for music is internal to the MediaPlayer static class.  
                So we don't need a variable for it here.*/



        //This is an enum that speeds up coding and reduces errors.
        //Specifying SFX by enum takes advantage of Intellisense and reduces 
        //the chance of a spelling mistake.
        public enum SFXSelection
        {
            Bump,
            RollingWheels,
            RotateLight,
            GirlRunning,
            SlideCrate,
            SqueakyWheels,
            CryingGirl,
            ElectricChum,
            Meep,
            SwitchOff,
            SwitchOn
        }

        //This is an enum that speeds up coding and reduces errors.
        //Specifying songs by enum takes advantage of Intellisense and reduces 
        //the chance of a spelling mistake.
        public enum MusicSelection
        {
            CreepyOverworld,
            MenuScreen
        }

        public void Initialize()
        {
            //Initializes the dictionaries.
            soundEffects = new Dictionary<SFXSelection, SoundEffect>();
            songs = new Dictionary<MusicSelection, Song>();
            SFXVolume = 1.0f;
            MediaPlayer.Volume = 0.6f;
            MediaPlayer.IsRepeating = true;
        }

        public void LoadContent(ContentManager content)
        {
            //Uses relative pathfiles from your project (i.e. your mp3 files 
            //should be located in the Music folder for this demo).


            //Music
            //****ADD CODE HERE TO LOAD IN YOUR SONG
            //Hint: Follow the example for adding sound effects.
            //songs.Add(MusicSelection.Bluh, content.Load<Song>("Songs/Bluh"));
            songs.Add(MusicSelection.CreepyOverworld, content.Load<Song>("music/overworld"));
            songs.Add(MusicSelection.MenuScreen, content.Load<Song>("music/final"));

            //Sound Effects
            //soundEffects.Add(SFXSelection.VictoryTrumpets, content.Load<SoundEffect>("SFX/VictoryTrumpets"));
            soundEffects.Add(SFXSelection.Bump, content.Load<SoundEffect>("sfx/bump"));
            soundEffects.Add(SFXSelection.CryingGirl, content.Load<SoundEffect>("sfx/crying"));
            soundEffects.Add(SFXSelection.ElectricChum, content.Load<SoundEffect>("sfx/electrichum001"));
            soundEffects.Add(SFXSelection.Meep, content.Load<SoundEffect>("sfx/meep"));
            soundEffects.Add(SFXSelection.SwitchOff, content.Load<SoundEffect>("sfx/switchOFF"));
            soundEffects.Add(SFXSelection.SwitchOn, content.Load<SoundEffect>("sfx/switchON"));
            soundEffects.Add(SFXSelection.RollingWheels, content.Load<SoundEffect>("sfx/boringwheesl"));
            soundEffects.Add(SFXSelection.RotateLight, content.Load<SoundEffect>("sfx/rotatelight"));
            soundEffects.Add(SFXSelection.SlideCrate, content.Load<SoundEffect>("sfx/slidecrate"));
            soundEffects.Add(SFXSelection.SqueakyWheels, content.Load<SoundEffect>("sfx/squeakywheels_002"));
            soundEffects.Add(SFXSelection.GirlRunning, content.Load<SoundEffect>("sfx/running"));

            //*********TODO: Uncomment out the above line once VictoryTrumpets.mp3 is put in the correct directory.

            /*NOTE: This sound effect file is not included initially in the correct directory.
                    We want you to move this file to the correct location. If you do not do so, and uncomment the above line you will get
                    an error.  Make sure you add VictoryTrumpets.mp3 file to the SFX directory in the Content folder.
                    Once you add the file, make sure you right click it in the solution explorer (on the right hand side of 
                    your screen) and select properties.  Under the "Content Processor" field select "SoundEffect - XNA Framework"
                    Not doing this second step will result in XNA treating you SoundEffect as a song, which causes an error.
             */

        }


        //Plays music.  This should be done through using the MediaPlayer static class.
        /* It also stops the previous song if it was playing or paused.
         * 
         * You typically don't want multiple songs to be playing at once, so make
         * sure to stop the previous song.
         *
         * Trying to play a song already playing may crash your game.
         * 
         */

        public void Play(MusicSelection selection)
        {
            Song song;
            if (songs.TryGetValue(selection, out song))
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(song);
            }
            else
            {
                Console.WriteLine("Could not find song: " + selection.ToString());
            }
        }



        //Plays a sound effect.  I have done this one for you.
        public SoundEffectInstance Play(SFXSelection selection)
        {
            SoundEffect sfx;
            if (soundEffects.TryGetValue(selection, out sfx))
            {
                //Console.WriteLine(selection);
                SoundEffectInstance sound = sfx.CreateInstance();
                sound.IsLooped = true;
                sound.Volume = SFXVolume;
                sound.Play();
                return sound;
            }
            else
            {
                Console.WriteLine("Could not find sound effect: " + selection.ToString());
                return null;
            }
        }

        public void PlayCry(SFXSelection selection)
        {
            SoundEffect sfx;
            if (soundEffects.TryGetValue(selection, out sfx))
            {
                //Console.WriteLine(selection);
                SoundEffectInstance sound = sfx.CreateInstance();
                sound.Volume = SFXVolume;
                sound.Play();
            }
            else
            {
                Console.WriteLine("Could not find sound effect: " + selection.ToString());
            }
        }


        //This function will pause the currently playing song, and resume it if it is playing.
        //Be sure to check that it is valid to try and pause/resume the song at a given time.
        public void Pause()
        {
            //TODO: Your code here
            MediaPlayer.Pause();
        }

        //Stops the song. Be sure to check that it is valid to stop the song here.
        public void Stop()
        {
            MediaPlayer.Stop();
        }


        //Make sure volume does not go above 1.0.  Hint: Look at MathHelper.Clamp for a quick function to use.
        public void IncreaseSFXVolume(float increment)
        {
            if (SFXVolume + increment <= 1.0f)
            {
                SFXVolume += increment;
            }
            else
            {
                SFXVolume = 1.0f;
            }
        }

        //Make sure volume does not go below 0.0.
        public void DecreaseSFXVolume(float decrement)
        {
            if (SFXVolume - decrement >= 0.0f)
            {
                SFXVolume -= decrement;
            }
            else
            {
                SFXVolume = 0.0f;
            }
        }

        //Make sure volume does not go above 1.0.  
        public void IncreaseMusicVolume(float increment)
        {
            if (MediaPlayer.Volume + increment <= 1.0f)
            {
                MediaPlayer.Volume += increment;
            }
            else
            {
                MediaPlayer.Volume = 1.0f;
            }
        }

        //Make sure volume does not go below 0.0.  
        public void DecreaseMusicVolume(float decrement)
        {
            if (MediaPlayer.Volume - decrement >= 1.0f)
            {
                MediaPlayer.Volume -= decrement;
            }
            else
            {
                MediaPlayer.Volume = 0.0f;
            }
        }

        //*** END YOUR CODE ***//

        //Don't touch these functions
        protected void Update(GameTime gameTime)
        {
            //Nothing to update.
        }

        protected void Draw(GameTime gameTime)
        {
            //Nothing to draw..
        }

    }
}
