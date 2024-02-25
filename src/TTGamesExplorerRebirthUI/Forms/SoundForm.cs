using DarkUI.Forms;
using NAudio.Vorbis;
using NAudio.Wave;
using System.Windows.Forms;
using TTGamesExplorerRebirthLib.Formats;

namespace TTGamesExplorerRebirthUI.Forms
{
    public enum SoundFormat
    {
        WaveformAudio,
        MPEGAudioLayerIII,
        OggVorbis,
        ChatterBoX,
    }

    public partial class SoundForm : DarkForm
    {
        private readonly string _filePath;
        private readonly byte[] _soundBuffer;
        private readonly SoundFormat _soundFormat;

        private WaveOutEvent _outputDevice;
        private IWaveProvider _audioFile;

        private readonly string _titleText;

        public SoundForm(string filePath, byte[] fileBuffer)
        {
            InitializeComponent();

            _filePath = filePath;
            _soundBuffer = fileBuffer ?? File.ReadAllBytes(filePath);

            _soundFormat = Path.GetExtension(_filePath).ToLowerInvariant() switch
            {
                ".adp" or ".wav" => SoundFormat.WaveformAudio,
                ".mp3" => SoundFormat.MPEGAudioLayerIII,
                ".ogg" => SoundFormat.OggVorbis,
                ".cbx" => SoundFormat.ChatterBoX,
                _ => throw new NotSupportedException(),
            };

            toolStripStatusLabel1.Text = $"{Path.GetFileName(_filePath)} ({_soundFormat})";

            _titleText = Text;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            Helper.EnableDarkModeTitle(Handle);
        }

        private void SoundForm_Load(object sender, EventArgs e)
        {
            _outputDevice = new WaveOutEvent();

            try
            {
                switch (_soundFormat)
                {
                    case SoundFormat.WaveformAudio:
                        {
                            _audioFile = new WaveFileReader(new MemoryStream(_soundBuffer));

                            _outputDevice.Init(WaveFormatConversionStream.CreatePcmStream((WaveStream)_audioFile));

                            break;
                        }

                    case SoundFormat.MPEGAudioLayerIII:
                        {
                            _audioFile = new Mp3FileReader(new MemoryStream(_soundBuffer));

                            _outputDevice.Init(_audioFile);

                            break;
                        }

                    case SoundFormat.OggVorbis:
                        {
                            _audioFile = new VorbisWaveReader(new MemoryStream(_soundBuffer));

                            _outputDevice.Init(_audioFile);

                            break;
                        }

                    case SoundFormat.ChatterBoX:
                        {
                            _audioFile = new WaveFileReader(new MemoryStream(new CBX(_soundBuffer).WaveBuffer));

                            _outputDevice.Init(WaveFormatConversionStream.CreatePcmStream((WaveStream)_audioFile));

                            darkButton1.Enabled = true;

                            break;
                        }
                }
            }
            catch
            {
                string message = $"Not supported audio format:\n\n" +
                    $"Format: {_audioFile.WaveFormat.BitsPerSample}bits {_audioFile.WaveFormat.Encoding}\n" +
                    $"Channels: {_audioFile.WaveFormat.Channels}\n" +
                    $"Sample Rate: {_audioFile.WaveFormat.SampleRate}Hz\n\n" +
                    $"You can try to play it using VLC.";

                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Close();

                return;
            }

            button2.PerformClick();
        }

        private void SoundForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _outputDevice?.Stop();
            _outputDevice.Dispose();
            _outputDevice = null;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            _outputDevice ??= new WaveOutEvent();

            switch (_outputDevice.PlaybackState)
            {
                case PlaybackState.Stopped:
                    {
                        button2.Image = new Bitmap(Properties.Resources.pause_green);

                        trackBar1.Value = 0;
                        trackBar1.Maximum = (int)((WaveStream)_audioFile).TotalTime.TotalMilliseconds;

                        _outputDevice.Play();
                        timer1.Start();

                        break;
                    }

                case PlaybackState.Playing:
                    {
                        _outputDevice.Pause();
                        timer1.Stop();
                        button2.Image = new Bitmap(Properties.Resources.play_green);

                        break;
                    }

                case PlaybackState.Paused:
                    {
                        _outputDevice.Play();
                        timer1.Start();
                        button2.Image = new Bitmap(Properties.Resources.pause_green);

                        break;
                    }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _outputDevice.Stop();
            trackBar1.Value = 0;
            timer1.Stop();

            button2.Image = new Bitmap(Properties.Resources.play_green);
            Text = _titleText + $" - {TimeSpan.FromMilliseconds(trackBar1.Value):mm\\:ss\\.fff}/{TimeSpan.FromMilliseconds(trackBar1.Maximum):mm\\:ss\\.fff}";
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            trackBar1.Value = (int)((WaveStream)_audioFile).CurrentTime.TotalMilliseconds;
            Text = _titleText + $" - {TimeSpan.FromMilliseconds(trackBar1.Value):mm\\:ss\\.fff}/{TimeSpan.FromMilliseconds(trackBar1.Maximum):mm\\:ss\\.fff}";
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            ((WaveStream)_audioFile).CurrentTime = TimeSpan.FromMilliseconds(trackBar1.Value);

            if (trackBar1.Value == trackBar1.Maximum)
            {
                Thread.Sleep(100);
                button1.PerformClick();
            }
        }

        private void darkButton1_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                WaveFileWriter.CreateWaveFile(saveFileDialog1.FileName, _audioFile);
            }
        }
    }
}