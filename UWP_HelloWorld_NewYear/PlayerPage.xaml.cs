using System;
using UWP_HelloWorld_NewYear.Data;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_HelloWorld_NewYear
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayerPage : Page
    {
        private bool _playContentWhenNotInPlayer = false;

        public PlayerPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.Loaded += PlayerPage_Loaded;
            BackToContentPageButton.Click += BackToContentPageButton_Click;
            BrowseFileButton.Click += BrowseFileButton_Click; ;

            OpacityAnimatedStackPanel.PointerEntered += OpacityAnimatedStackPanel_PointerEntered;
            OpacityAnimatedStackPanel.PointerExited += OpacityAnimatedStackPanel_PointerExited;

            SetUpPageAnimation();
        }

        private async void BrowseFileButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".mp3");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    MediaPlayer.MediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
                    MediaPlayer.MediaPlayer.Play();
                }
                catch (Exception ex)
                {
                    DisplayDialog("Error while choosing file", "Please choose a file with proper extension");
                    Console.WriteLine("[ERROR] Error when loading media poster source from file " + ex.Message);
                }
            }
        }
        
        private void OpacityAnimatedStackPanel_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            StoryboardFadeOut.Begin();
        }

        private void OpacityAnimatedStackPanel_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            StoryboardFadeIn.Begin();
        }

        private void PlayerPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            object value = false;

            if (localSettings.Values.TryGetValue("playContentWhenNotInPlayer", out value))
            {
                _playContentWhenNotInPlayer = (bool?)value == null ? false /* <= default value */ : (bool)value;
            }

            OpacityAnimatedStackPanel_PointerExited(null, null); // Automatically hide at load
        }

        private void SetUpPageAnimation()
        {
            TransitionCollection collection = new TransitionCollection();
            NavigationThemeTransition theme = new NavigationThemeTransition();

            var info = new EntranceNavigationTransitionInfo();

            theme.DefaultNavigationTransitionInfo = info;
            collection.Add(theme);
            this.Transitions = collection;
        }

        private void BackToContentPageButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // No need to handle PlayContentWhenNotInPlayer check because OnNavigatedFrom will be called after Frame.Navigate
            this.Frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter == null && MediaPlayer.MediaPlayer.Source != null)
                MediaPlayer.MediaPlayer.Play(); // Page is already cached, just continue playing
            else
            {
                try
                {
                    StorageFile storageFile = ((object[])e.Parameter)?[0] as StorageFile;
                    SWatchFile sWatchFile = ((object[])e.Parameter)?[1] as SWatchFile;
                    MediaPlayer.Source = MediaSource.CreateFromStorageFile(storageFile);

                    // TODO: Choose approriate PosterSource
                    if (sWatchFile.Type == FileType.Audio)
                        MediaPlayer.PosterSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/mickey_playing.gif"));
                    else
                        MediaPlayer.PosterSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/loading.gif"));

                    //MediaPlayer.PosterSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/loading.gif"));
                    //MediaPlayer.PosterSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/rainbow-loading.gif"));
                    //MediaPlayer.PosterSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/loading gear icon.gif"));
                    //MediaPlayer.PosterSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/music_icon.png"));

                    MediaPlayer.MediaPlayer.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Error when setting media source\n" + ex.Message);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (MediaPlayer.MediaPlayer.Source != null && !_playContentWhenNotInPlayer)
                MediaPlayer.MediaPlayer.Pause();
        }

        private async void DisplayDialog(string title, string content)
        {
            ContentDialog noWifiDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok",
                DefaultButton = ContentDialogButton.Close
            };

            ContentDialogResult result = await noWifiDialog.ShowAsync();
        }
    }
}
