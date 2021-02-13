using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RoMoRDuP.UserControls
{
    /// <summary>
    /// Interaktionslogik für UCPlaylistUpdates.xaml
    /// </summary>
    public partial class UCPlaylistUpdates : UserControl
    {
        public PlaylistUpdates.PlaylistUpdates playlistUpdates { get; set; }

        public UCPlaylistUpdates(PlaylistUpdates.PlaylistUpdates playlistUpdates)
        {
            this.playlistUpdates = playlistUpdates;

            InitializeComponent();
        }

        private void MouseDoubleClick_PlaylistUpdates(object sender, MouseButtonEventArgs e)
        {
            PlaylistUpdates.PlayListUpdatesViewModel track = ((ListViewItem)sender).Content as PlaylistUpdates.PlayListUpdatesViewModel; //Casting back to the binded Track

            //MessageBox.Show(track.PathOld);
            if (track != null)
            {
                System.Diagnostics.Process P = new System.Diagnostics.Process();
                P.StartInfo.FileName = "WinMergeU";
                P.StartInfo.Arguments = "\"" + track.PathOriginal + "\"" + " " + "\"" + track.PathTemp + "\"";

                try
                {
                    P.Start();
                }
                catch (Exception)
                {
                    MessageBox.Show("WinMerge cannot be opened.\nDownload and install from http://winmerge.org/downloads/");
                }
            }

        }
    }
}
