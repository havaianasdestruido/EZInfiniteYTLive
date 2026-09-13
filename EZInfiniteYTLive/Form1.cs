using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EZInfiniteYTLive
{
    public partial class Form1 : Form
    {
        private string _videoFolder = string.Empty;
        private VideoStreamer _streamer;
        private bool _shuffle = false;
        private string _ffmpegPath = "ffmpeg.exe"; // Adjust if needed

        public Form1()
        {
            InitializeComponent();
            StatusLabel.Text = "Idle";
            this.Text = "EZ Infinite YT Live - Idle";
            this.FormClosed += (s, e) =>
            {
                if (_streamer != null)
                {
                    _streamer.Dispose();
                }
            };
            AlphabeticOrderRadio.Checked = true;
        }

        private void ChooseVideosButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _videoFolder = fbd.SelectedPath;
                    StatusLabel.Text = $"Selected: {_videoFolder}";
                }
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_videoFolder) || !Directory.Exists(_videoFolder))
            {
                MessageBox.Show("Please select a valid video folder.");
                return;
            }
            string rtmpUrl = RMTPUrl.Text.Trim();
            string streamKey = StreamKey.Text.Trim();
            if (string.IsNullOrEmpty(rtmpUrl) || string.IsNullOrEmpty(streamKey))
            {
                MessageBox.Show("Please enter RTMP URL and Stream Key.");
                return;
            }
            string fullRtmp = rtmpUrl.EndsWith("/") ? rtmpUrl + streamKey : rtmpUrl + "/" + streamKey;
            if (_streamer != null)
            {
                _streamer.Dispose();
            }
            _streamer = new VideoStreamer(_ffmpegPath, _videoFolder, fullRtmp);
            StatusLabel.Text = "Streaming...";
            StartButton.Enabled = false;
            ForceStopButton.Enabled = true;
            //change form1 title
            this.Text = "EZ Infinite YT Live - Streaming";
            _streamer.StartStreaming();
        }

        private void ForceStopButton_Click(object sender, EventArgs e)
        {
            if (_streamer != null)
            {
                _streamer.StopStreaming();
                _streamer.Dispose();
                _streamer = null;
            }
            StatusLabel.Text = "Stopped";
            this.Text = "EZ Infinite YT Live - Stopped";
            StartButton.Enabled = true;
            ForceStopButton.Enabled = false;
        }

        private void ShuffleRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (ShuffleRadio.Checked)
            {
                _shuffle = true;
            }
        }

        private void AlphabeticOrderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (AlphabeticOrderRadio.Checked)
            {
                _shuffle = false;
            }
        }
    }
}
