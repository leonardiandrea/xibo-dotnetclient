using System;
using Vlc.DotNet.Core;
using Vlc.DotNet.Forms;

namespace XiboClient
{
    partial class VlcVideoPlayer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing && (vlcMediaPlayer != null))
            {
                VlcMedia vlcMedia = vlcMediaPlayer.GetCurrentMedia();

                if (vlcMedia != null) vlcMedia.Dispose();

                // Unbind events
                vlcMediaPlayer.VlcLibDirectoryNeeded -= vlcMediaPlayer_OnVlcControlNeedLibDirectory;
                vlcMediaPlayer.EndReached -= vlcMediaPlayer_EndReached;
                vlcMediaPlayer.EncounteredError -= vlcMediaPlayer_ErrorEvent;

                vlcMediaPlayer.Dispose();

                vlcMediaPlayer = null;
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OAVideoPlayer));
            this.vlcMediaPlayer = new Vlc.DotNet.Forms.VlcControl();
            ((System.ComponentModel.ISupportInitialize)(this.vlcMediaPlayer)).BeginInit();
            this.SuspendLayout();
            // 
            // vlcMediaPlayer
            // 
            this.vlcMediaPlayer.Enabled = true;
            this.vlcMediaPlayer.Location = new System.Drawing.Point(0, 0);
            this.vlcMediaPlayer.Name = "vlcMediaPlayer";            

            this.vlcMediaPlayer.Size = new System.Drawing.Size(291, 269);
            this.vlcMediaPlayer.TabIndex = 0;
            // 
            // VideoPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.vlcMediaPlayer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "VideoPlayer";
            this.Text = "VideoPlayer";

            vlcMediaPlayer.VlcLibDirectory = null;
            vlcMediaPlayer.VlcLibDirectoryNeeded += new EventHandler<VlcLibDirectoryNeededEventArgs>(vlcMediaPlayer_OnVlcControlNeedLibDirectory);
            vlcMediaPlayer.EncounteredError += new EventHandler<VlcMediaPlayerEncounteredErrorEventArgs>(vlcMediaPlayer_ErrorEvent);
            vlcMediaPlayer.EndReached += new EventHandler<VlcMediaPlayerEndReachedEventArgs>(vlcMediaPlayer_EndReached);

            ((System.ComponentModel.ISupportInitialize)(this.vlcMediaPlayer)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Vlc.DotNet.Forms.VlcControl vlcMediaPlayer;
    }
}