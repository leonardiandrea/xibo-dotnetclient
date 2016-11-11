/*
 * Author:      Andrea Leonardi
 * Creation:    2016/09/06
 *
 * Description: 
 * 
 */
using System;
using System.Windows.Forms;
using System.Diagnostics;
using Vlc.DotNet.Core;
using System.IO;
using Vlc.DotNet.Forms;

namespace XiboClient
{
    public partial class VlcVideoPlayer : Form
    {
        private bool _finished;
        private bool _visible = true;

        public delegate void VideoFinished();
        public event VideoFinished VideoEnd;

        public delegate void VideoErrored();
        public event VideoErrored VideoError;

        private bool _looping = false;

        public VlcVideoPlayer()
        {
            InitializeComponent();
            this.TopLevel = false;

            _finished = false;
        }

        public void StartPlayer(string filePath)
        {
            Trace.WriteLine(new LogMessage("VideoPlayer - StartPlayer", "Play from scratch"), LogType.Audit.ToString());
            if (_visible)
            {
                vlcMediaPlayer.Visible = true;
                vlcMediaPlayer.Width = this.Width;
                vlcMediaPlayer.Height = this.Height;
            }
            else
            {
                vlcMediaPlayer.Visible = false;
            }

            vlcMediaPlayer.Location = new System.Drawing.Point(0, 0);

            // Check if we're reproducing stream or file from filesystem
            if (Uri.IsWellFormedUriString(filePath, UriKind.Absolute))
            {
                vlcMediaPlayer.SetMedia(new Uri(filePath, UriKind.Absolute), null);
            }
            else
            {
                vlcMediaPlayer.SetMedia(new System.IO.FileInfo(filePath), null);
            }

            // TODO: Find a way to set this property (if they are needed) in VLC component
            //vlcMediaPlayer.uiMode = "none";
            //vlcMediaPlayer.stretchToFit = true;
            //vlcMediaPlayer.windowlessVideo = false;

            vlcMediaPlayer.Play();
        }

        /// <summary>
        /// Set Loop
        /// </summary>
        /// <param name="looping"></param>
        public void SetLooping(bool looping)
        {
            // TODO: Find a way to setup loop mode...
            //vlcMediaPlayer.settings.setMode("loop", looping);      
            //_looping = looping;
        }

        /// <summary>
        /// Set Mute
        /// </summary>
        /// <param name="mute"></param>
        public void SetMute(bool mute)
        {
            if (mute)
            {
                vlcMediaPlayer.Audio.Volume = 0;
                vlcMediaPlayer.Audio.IsMute = true;
            }
            else
            {
                vlcMediaPlayer.Audio.Volume = 100;
            }
        }

        /// <summary>
        /// Set Volume
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(int volume)
        {
            if (volume == 0)
            {
                SetMute(true);
            }
            else
            {
                vlcMediaPlayer.Audio.Volume = volume;
                vlcMediaPlayer.Audio.IsMute = false;
            }
        }

        /// <summary>
        /// Visible
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            _visible = visible;
        }

        /// <summary>
        /// Stop and Clear everything
        /// </summary>
        public void StopAndClear()
        {
            try
            {
                if (vlcMediaPlayer != null)
                {
                    vlcMediaPlayer.Stop();

                    // Remove the WMP control
                    Controls.Remove(vlcMediaPlayer);

                    // Workaround to remove the event handlers from the cachedLayoutEventArgs
                    PerformLayout();

                    // Close this form
                    Close();
                }

                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (AccessViolationException)
            {

            }
            finally
            {
                // DEBUG
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }


        public void Play()
        {
            if (vlcMediaPlayer.State != Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
            {
                Trace.WriteLine(new LogMessage("VideoPlayer - StartPlayer", "Resuming from pause"), LogType.Audit.ToString());
                vlcMediaPlayer.Play();
            }
        }

        public void SkipToEnd()
        {
            string mediaUrl = string.Empty;
            VlcMedia vlcMedia = vlcMediaPlayer.GetCurrentMedia();

            if (vlcMedia != null && vlcMedia.URL != null) mediaUrl = vlcMedia.URL;

            Trace.WriteLine(new LogMessage("VideoPlayer - SkipToEnd", mediaUrl + "."), LogType.Audit.ToString());
            vlcMediaPlayer.Position = ((float)vlcMediaPlayer.GetCurrentMedia().Duration.TotalSeconds) - 0.01F;
        }

        void vlcMediaPlayer_ErrorEvent(object sender, VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            // Run this code in the right thread
            if (InvokeRequired)
            {
                Invoke(new EventHandler<VlcMediaPlayerEncounteredErrorEventArgs>(vlcMediaPlayer_ErrorEvent), new object[] { sender, e });
            }
            else
            {
                // Get the error for logging
                string error;
                try
                {
                    // TODO: To be tested...
                    error = e.ToString();
                }
                catch
                {
                    error = "Unknown Error";
                }

                string mediaUrl = string.Empty;
                VlcMedia vlcMedia = vlcMediaPlayer.GetCurrentMedia();

                if (vlcMedia != null && vlcMedia.URL != null) mediaUrl = vlcMedia.URL;

                Trace.WriteLine(new LogMessage("VideoPlayer - ErrorEvent", mediaUrl + ". Ex = " + error), LogType.Error.ToString());

                // Raise the event
                if (VideoError == null)
                {
                    Trace.WriteLine(new LogMessage("VideoPlayer - ErrorEvent", "Error event handler is null"), LogType.Audit.ToString());
                }
                else
                {
                    VideoError();
                }
            }
        }

        void vlcMediaPlayer_EndReached(object sender, VlcMediaPlayerEndReachedEventArgs e)
        {
            // Run this code in the right thread
            if (InvokeRequired)
            {
                Invoke(new EventHandler<VlcMediaPlayerEndReachedEventArgs>(vlcMediaPlayer_EndReached), new object[] { sender, e });
            }
            else
            {
                // Media Ended
                // indicate we are stopped
                _finished = true;

                // Raise the event
                if (VideoEnd == null)
                {
                    Trace.WriteLine(new LogMessage("VideoPlayer - Playstate Complete", "Video end handler is null"), LogType.Audit.ToString());
                }
                else
                {
                    VideoEnd();
                }
            }
        }

        void vlcMediaPlayer_OnVlcControlNeedLibDirectory(object sender, VlcLibDirectoryNeededEventArgs e)
        {
            // TODO: Move this in ApplicationSettings
            e.VlcLibDirectory = new DirectoryInfo(@"C:\Program Files (x86)\VideoLAN\VLC");
        }

        /// <summary>
        /// Has this player finished playing
        /// </summary>
        public bool FinishedPlaying
        {
            get
            {
                return _finished;
            }
        }
    }
}