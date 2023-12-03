using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer mediaPlayer;
        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sound", "music.mp3")));
            mediaPlayer.MediaEnded += (object o, EventArgs e) =>
            {
                mediaPlayer.Position = TimeSpan.Zero;
                mediaPlayer.Play();
            };
            mediaPlayer.Play(); 
        }
    }
}
