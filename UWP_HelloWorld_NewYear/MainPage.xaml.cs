using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWP_HelloWorld_NewYear.Data;
using UWP_HelloWorld_NewYear.Data.Helpers;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP_HelloWorld_NewYear
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Files Files { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.Loaded += MainPage_Loaded;

            Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(SWatchStackPanel);

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Color.FromArgb(1, 30, 33, 42);
            titleBar.ButtonForegroundColor = Colors.WhiteSmoke;

            Files = new Files();
            MainContentList.ItemsSource = Files; 
            MainContentList.IsItemClickEnabled = true;
            MainContentList.ItemClick += MainContentList_ItemClick;
            MainContentList.SelectionChanged += MainContentList_SelectionChanged;
            MainContentList.PointerEntered += MainContentList_PointerEntered;

            AddContentButton.Click += AddContentButton_Click;
            RemoveContentButton.Click += RemoveContentButton_Click;
            OpenPlayerButton.Click += OpenPlayerButton_Click;
            UserInfoButton.Click += UserInfoButton_Click;

            SortContentByDateModified.Click += SortContentByDateModified_Click;
            SortContentByName.Click += SortContentByName_Click;
            SortContentBySize.Click += SortContentBySize_Click;

            PlayContentWhenNotInPlayerCheckBox.Click += PlayContentWhenNotInPlayerCheckBox_Click;

            SetUpPageAnimation();
        }

        private void MainContentList_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Hover on to display video preview of the content using MediaPlayer, BUT this event does not do the trick. Find another.
        }

        private void PlayContentWhenNotInPlayerCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["playContentWhenNotInPlayer"] = PlayContentWhenNotInPlayerCheckBox.IsChecked;
            // Do this when Files are modified. => SettingsModified() that's called each time a Files have been modified
            //ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            //localSettings.Values["Files"] = Files;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            object value = false;

            if (localSettings.Values.TryGetValue("playContentWhenNotInPlayer", out value))
            {
                PlayContentWhenNotInPlayerCheckBox.IsChecked = (bool?)value == null ? false /* <= default value */ : (bool)value ;
            }
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Move SWatch logo under "back button", to prevent overlapping
            if (this.Frame.CanGoBack)
                SWatchStackPanel.Margin = new Thickness(0, 20, 0, 0);
            else
                SWatchStackPanel.Margin = new Thickness(0, 0, 0, 0);
        }

        private void UserInfoButton_Click(object sender, RoutedEventArgs e)
        {
            //(SWatchStackPanel.Children[1] as TextBlock).Text = "Clicked";
        }

        private void OpenPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PlayerPage), null, new DrillInNavigationTransitionInfo());
        }

        private void RemoveContentButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainContentList == null || MainContentList.SelectedIndex == -1)
                return;

            Files.RemoveAt(MainContentList.SelectedIndex);
            UpdateStatusBar("Content was successfully removed");
        }

        private async void AddContentButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".mp3");
            //picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.VideosView, 100);
                BasicProperties basicProperties = await file.GetBasicPropertiesAsync();

                // if (!_storageFiles.Contains(file)) // Bug occurs (playing a wrong file), investigate ?

                SWatchFile swatchFile = new SWatchFile()
                {
                    Name = file.Name.Remove(file.Name.Length - 4),
                    Path = file.Path,
                    Size = basicProperties.Size,
                    Thumbnail = thumbnail,
                    ThumbnailBitmapImage = await StorageItemThumbnailToBitmapImage(thumbnail),
                    DateCreated = file.DateCreated.Date,
                    DateModified = basicProperties.DateModified.Date,
                    Type = SWatchFile.GetFileTypeFromFileContentType(file.ContentType),
                };

                swatchFile.StorageFile = file;

                //if (!Files.Contains(swatchFile)) // Bug occurs (playing a wrong file), investigate ?
                Files.Add(swatchFile);
                MainContentList.SelectedItem = Files[Files.Count - 1]; // Select last added file
            }
            else
            {
                Console.WriteLine("[ERROR] Error when adding a file to GUI");
                UpdateStatusBar("Error while adding content");
            }
            UpdateStatusBar("Content was successfully added");
        }

        private void MainContentList_ItemClick(object sender, ItemClickEventArgs e)
        {
            GridView gridView = sender as GridView;

            if (gridView == null || gridView.SelectedIndex == -1)
                return;

            SWatchFile selectedFile = gridView?.SelectedItem as SWatchFile;
            SWatchFile clickedFile = e.ClickedItem as SWatchFile;

            if (!selectedFile.Equals(clickedFile)) 
                return; // When selection changes do not immediately navigate to player

            IStorageFile storageFile = clickedFile.StorageFile;

            this.Frame.Navigate(typeof(PlayerPage), new object[] { storageFile, clickedFile }, new ContinuumNavigationTransitionInfo());
        }

        private void MainContentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView gridView = sender as GridView;
            UpdateStatusBar("Selected content changed. " + (gridView?.SelectedValue as SWatchFile)?.Name);
        }

        private void SortContentBySize_Click(object sender, RoutedEventArgs e)
        {
            List<SWatchFile> sortableList = new List<SWatchFile>(Files);
            sortableList.Sort((SWatchFile f1, SWatchFile f2) => { return f1.Size > f2.Size ? 1 : (f1.Size < f2.Size ? -1 : 0); });

            for (int i = 0; i < sortableList.Count; i++)
            {
                Files.Move(Files.IndexOf(sortableList[i]), i);
            }

            UpdateStatusBar("Content is sorted by file size in ascending order");
        }

        private void SortContentByName_Click(object sender, RoutedEventArgs e)
        {
            List<SWatchFile> sortableList = new List<SWatchFile>(Files);
            sortableList.Sort((SWatchFile f1, SWatchFile f2) => { return f1.Name.CompareTo(f2.Name); });

            for (int i = 0; i < sortableList.Count; i++)
            {
                Files.Move(Files.IndexOf(sortableList[i]), i);
            }

            UpdateStatusBar("Content is sorted by file name in ascending order");
        }

        private void SortContentByDateModified_Click(object sender, RoutedEventArgs e)
        {
            List<SWatchFile> sortableList = new List<SWatchFile>(Files);
            sortableList.Sort(new DateModifiedBasedComparer());

            for (int i = 0; i < sortableList.Count; i++)
            {
                Files.Move(Files.IndexOf(sortableList[i]), i);
            }

            UpdateStatusBar("Content is sorted by file date modified in ascending order");
        }

        public static async Task<BitmapImage> StorageItemThumbnailToBitmapImage(StorageItemThumbnail thumbnail)
        {
            using (IRandomAccessStream fileStream = thumbnail.CloneStream())
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.DecodePixelHeight = 100;
                bitmapImage.DecodePixelWidth = 100;
                await bitmapImage.SetSourceAsync(fileStream);
                return bitmapImage;
            }
        }

        public void UpdateStatusBar(string text, bool errorOccured = false)
        {
            if (errorOccured)
                StatusBar.Foreground = new SolidColorBrush(Colors.Red);
            else
                StatusBar.Foreground = new SolidColorBrush(Colors.WhiteSmoke);

            StatusBar.Text = text;
        }
    }
}
