using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP_HelloWorld_NewYear.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_HelloWorld_NewYear
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LayoutPage : Page
    {
        public Files SwatchFiles = new Files();
        public ImageSource SwatchImage = new BitmapImage(new Uri("ms-appx:///Assets/music_icon.png"));
        public ImageSource TempImage = new BitmapImage(new Uri("ms-appx:///Assets/user2.png"));
        
        public LayoutPage()
        {
            this.InitializeComponent();
            SwatchFiles.Add(
                new SWatchFile()
                {
                    Name = "kdla",
                    Path = "C:/ekld/dkala;dk",
                    Type = FileType.Audio
                });
            SwatchFiles.Add(
                new SWatchFile()
                {
                    Name = "kdla",
                    Path = "C:/ekld/dkala;dk",
                    Type = FileType.Audio
                });
            SwatchFiles.Add(
                new SWatchFile()
                {
                    Name = "kdla",
                    Path = "C:/ekld/dkala;dk",
                    Type = FileType.Audio
                });

            this.OpacityStackPanel.PointerEntered += OpacityStackPanel_PointerEntered;
            this.OpacityStackPanel.PointerExited += OpacityStackPanel_PointerExited;
        }

        private void OpacityStackPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            myStoryboardFadeOut.Begin();
        }

        private void OpacityStackPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            myStoryboardFadeIn.Begin();
        }
    }
}
