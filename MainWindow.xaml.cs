using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace MojVLC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        int lastMinute;
        ObservableCollection<ListPlay> listsPlays;
        MainList mainList;

        public MainWindow()
        {
            InitializeComponent();
            mainList = new MainList();
            Playlist.ItemsSource = mainList.PlayList;
            
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            mePlayer.Volume = (double)volumeSlider.Value;

            int lastMinute = DateTime.Now.Minute;
            Style style = new Style { TargetType = typeof(Window) };
            style.Setters.Add(new Setter(Window.BackgroundProperty, Brushes.Yellow));
            Application.Current.Resources["WinBackground"] = style;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MainList));

                TextReader reader = new StreamReader("@ShranjniPlaylist.xml");

                MainList listLoaded = (MainList)serializer.Deserialize(reader);
                mainList.ListName = listLoaded.ListName;

                mainList.PlayList.Clear();
                foreach (ListPlay listItem in listLoaded.PlayList)
                {
                    mainList.PlayList.Add(listItem);
                }
                reader.Close();
            }
            catch { }
        }
        #region predvajalnik
        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            mePlayer.Volume = (double)volumeSlider.Value;
        }
        
        private void timer_Tick(object sender, EventArgs e)
        {
            if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = mePlayer.Position.TotalSeconds;
            }
            if (mePlayer.Source != null)
            {
                if (mePlayer.NaturalDuration.HasTimeSpan) TotalTime.Content = mePlayer.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
            }
            if (Properties.Settings.Default.ZamikShranjevanja.ToString() != "0") {
                int currentTime = DateTime.Now.Minute;
                if ((lastMinute + Int32.Parse(Properties.Settings.Default.ZamikShranjevanja.ToString())) <= currentTime)
                {
                    lastMinute = currentTime;


                    XmlSerializer serializer = new XmlSerializer(typeof(MainList));
                    using (TextWriter writer = new StreamWriter("@ShranjniPlaylist.xml"))
                    {
                        serializer.Serialize(writer, mainList);
                    }
                }
            }
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mePlayer != null) && (mePlayer.Source != null);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Play();
            busy.IsBusy = true;
            mediaPlayerIsPlaying = true;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Pause();

            busy.IsBusy = false;
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;

        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Stop();
            busy.IsBusy = false;
            mediaPlayerIsPlaying = false;
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mePlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }

        bool repeat = false;
        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            if (Repeat.Content == FindResource("Repeat"))
            {
                Repeat.Content = FindResource("NoRepeat");
                repeat = true;
            }
            else
            {
                Repeat.Content = FindResource("Repeat");
                repeat = false;
            }
        }


        bool shuffle = false;
        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            if (Shuffle.Content == FindResource("Shuffle"))
            {
                Shuffle.Content = FindResource("NoShuffle");
                shuffle = true;
            }
            else
            {
                Shuffle.Content = FindResource("Shuffle");
                shuffle = false;
            }
        }
        static Random rnd = new Random();
        private void mePlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (repeat)
            {
                mePlayer.Position = new TimeSpan(0, 0, 1);
                mePlayer.Play();
                busy.IsBusy = true;
            }
            else
            {
                if (shuffle)
                {
                    int r = rnd.Next(Playlist.Items.Count);

                    var selectedItem = (dynamic)Playlist.Items.GetItemAt(r);
                    while (predvajanipath == selectedItem.Lokacija.ToString()) { 
                        r = rnd.Next(Playlist.Items.Count);
                    selectedItem = (dynamic)Playlist.Items.GetItemAt(r);
                
                    }
                    predvajanipath = selectedItem.Lokacija;
                    mePlayer.Source = new Uri(predvajanipath);
                    mePlayer.Play();
                    busy.IsBusy = true;

                    for (int i = 0; i < Playlist.Items.Count; i++)
                    {
                        selectedItem = (dynamic)Playlist.Items[i];
                        selectedItem.IsPlaying = false;
                    }
                    selectedItem = (dynamic)Playlist.Items.GetItemAt(r);
                    selectedItem.IsPlaying = true;

                }
                else
                {
                    try
                    {
                        int index = -1;
                        var selectedItem = (dynamic)Playlist.SelectedItems[0];
                        for (int i = 0; i < Playlist.Items.Count; i++)
                        {

                            selectedItem = (dynamic)Playlist.Items[i];
                            if (selectedItem.IsPlaying)
                            { index = i; }
                        }
                        index++;

                        selectedItem = (dynamic)Playlist.Items.GetItemAt(index);
                        predvajanipath = selectedItem.Lokacija;
                        mePlayer.Source = new Uri(predvajanipath);
                        mePlayer.Play();

                        busy.IsBusy = true;

                        for (int i = 0; i < Playlist.Items.Count; i++)
                        {
                            selectedItem = (dynamic)Playlist.Items[i];
                            predvajanipath = selectedItem.Lokacija;
                            selectedItem.IsPlaying = false;
                        }
                        selectedItem = (dynamic)Playlist.Items.GetItemAt(index);
                        selectedItem.IsPlaying = true;
                    }
                    catch
                    {
                        var selectedItem = (dynamic)Playlist.Items.GetItemAt(0);
                        predvajanipath = selectedItem.Lokacija;
                        mePlayer.Source = new Uri(predvajanipath);
                        mePlayer.Play();

                        busy.IsBusy = true;


                        for (int i = 0; i < Playlist.Items.Count; i++)
                        {
                            selectedItem = (dynamic)Playlist.Items[i];
                            predvajanipath = selectedItem.Lokacija;
                            selectedItem.IsPlaying = false;
                        }
                        selectedItem = (dynamic)Playlist.Items.GetItemAt(0);
                        selectedItem.IsPlaying = true;
                    }
                }
            }
        }
        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = -1;
                var selectedItem = (dynamic)Playlist.SelectedItems[0];
                for (int i = 0; i < Playlist.Items.Count; i++)
                {

                    selectedItem = (dynamic)Playlist.Items[i];
                    if (selectedItem.IsPlaying)
                    { index = i; }
                }
                index--;

                selectedItem = (dynamic)Playlist.Items.GetItemAt(index);
                predvajanipath = selectedItem.Lokacija;
                mePlayer.Source = new Uri(predvajanipath);
                mePlayer.Play();

                busy.IsBusy = true;


                for (int i = 0; i < Playlist.Items.Count; i++)
                {
                    selectedItem = (dynamic)Playlist.Items[i];
                    predvajanipath = selectedItem.Lokacija;
                    selectedItem.IsPlaying = false;
                }
                selectedItem = (dynamic)Playlist.Items.GetItemAt(index);
                selectedItem.IsPlaying = true;
            }
            catch
            {
                MessageBox.Show("Prišli ste do zacetka seznama predvajanja.");
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = -1;
                var selectedItem = (dynamic)Playlist.SelectedItems[0];
                for (int i = 0; i < Playlist.Items.Count; i++)
                {

                    selectedItem = (dynamic)Playlist.Items[i];
                    if (selectedItem.IsPlaying)
                    { index = i; }
                }
                index++;

                selectedItem = (dynamic)Playlist.Items.GetItemAt(index);
                predvajanipath = selectedItem.Lokacija;
                mePlayer.Source = new Uri(predvajanipath);
                mePlayer.Play();

                busy.IsBusy = true;


                for (int i = 0; i < Playlist.Items.Count; i++)
                {
                    selectedItem = (dynamic)Playlist.Items[i];
                    predvajanipath = selectedItem.Lokacija;
                    selectedItem.IsPlaying = false;
                }
                selectedItem = (dynamic)Playlist.Items.GetItemAt(index);
                selectedItem.IsPlaying = true;
            }
            catch
            {
                MessageBox.Show("Prišli ste do konca seznama predvajanja.");
            }
        }


        #endregion
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Open_Click(sender, e);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var list1 = mainList.PlayList;
            ListPlay selectedItem = (ListPlay)Playlist.SelectedItem;
            mainList.PlayList.Remove(selectedItem);

            try
            {
                Playlist.SelectAll();
            }
            catch { MessageBox.Show("Vse je izbrisano"); }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (dynamic)Playlist.SelectedItems[0];
            string path = selectedItem.Lokacija;

            Playlist win = new Playlist(path);
            win.ShowDialog();
        }

        string predvajanipath="";
        private void Playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            bool onePlaying = false;
            var selectedItem = (dynamic)Playlist.SelectedItems[0];
            for (int i = 0; i < Playlist.Items.Count; i++)
            {

                selectedItem = (dynamic)Playlist.Items[i];
                if (selectedItem.IsPlaying)
                { onePlaying = true; }
                predvajanipath = selectedItem.Lokacija;
                selectedItem.IsPlaying = false;
            }
            if (!onePlaying)
            {
                selectedItem = (dynamic)Playlist.SelectedItems[0];
                predvajanipath = selectedItem.Lokacija;
                selectedItem.IsPlaying = true;
                mePlayer.Source = new Uri(predvajanipath);
                mePlayer.Play();

                busy.IsBusy = true;
                mediaPlayerIsPlaying = true;
            }
            else
            {
                mePlayer.Stop();
                busy.IsBusy = false;
                mediaPlayerIsPlaying = false;
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings win = new Settings();
            win.ShowDialog();
        }

        private void PlaylistMenu_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Playlist.SelectedItems.Count > 0)
            {
                int index = -1;
                var selectedItem = (dynamic)Playlist.SelectedItems[0];
                for (int i = 0; i < Playlist.Items.Count; i++)
                {

                    selectedItem = (dynamic)Playlist.Items[i];
                    if (selectedItem.IsPlaying)
                    { index = i; }
                }

                int SelectedListViewItem = Int32.Parse(Playlist.SelectedIndex.ToString());


                if ((SelectedListViewItem != index && index != -1)|| (index == -1))
                {
                    DeleteItem.IsEnabled = true;
                    EditItem.IsEnabled = true;
                }
                else
                {
                    DeleteItem.IsEnabled = false;
                    EditItem.IsEnabled = false;
                }
            }
            else
            {
                DeleteItem.IsEnabled = false;
                EditItem.IsEnabled = false;
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            int dolzina1 = Playlist.Items.Count;
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = " Supported| *.mp3; *.asf; *.wma; *.wmv; *.wm; *.asx; *.wax; *.wvx; *.wmx; wpl; *.avi; *.mpg; *.mpeg; *.m1v; *.mp2; *.mp3; *.mpa; *.mpe; *.m3u; *.mid; *.midi; *.rmi; *.wav; *.m4a; *.mp4; *.m4v; *.mp4v; *.3g2; *.3gp2; *.3gp; *.3gpp; *.m4a; *.aac; *.adt; *.adts";
            open.Multiselect = true;
            if (open.ShowDialog() == true)
            {
                ObservableCollection<ListPlay> listPlay = new ObservableCollection<ListPlay>();
                int dolzina = Playlist.Items.Count;
                for (int i = 0; i < dolzina- dolzina1; i++)
                {
                    var selectedItem = (dynamic)Playlist.Items[i];
                    string item = selectedItem.Lokacija;
                    ListPlay lp = new ListPlay();

                    lp.Lokacija = item;
                    lp.DatumSpremenbe = File.GetLastWriteTime(item).ToString();

                    double len = new FileInfo(item).Length;
                    while (len >= 1024)
                    {
                        len = len / 1024;
                    }
                    string result = String.Format(" {0:0.##} {1}", len, "MB");
                    lp.Velikost = result;

                    var file = TagLib.File.Create(item);
                    lp.Leto = file.Tag.Year.ToString();

                    lp.Naslov = System.IO.Path.GetFileNameWithoutExtension(item);
                    lp.Zvrst = file.Tag.FirstGenre;


                    double minute = file.Properties.Duration.Minutes;
                    double sekunde = file.Properties.Duration.Seconds;
                    if (sekunde < 10)
                    {
                        lp.Trajanje = minute.ToString() + ":0" + sekunde;
                    }
                    else
                    {
                        lp.Trajanje = minute.ToString() + ":" + sekunde;

                    }
                    listPlay.Add(lp);

                    //nov list
                    mainList.PlayList.Add(lp);
                }

                    try
                    {
                    foreach (String file in open.FileNames)
                        {
                            string path = file.ToString();
                            ListPlay lp = new ListPlay();

                                lp.Lokacija = path;
                                lp.DatumSpremenbe = File.GetLastWriteTime(path).ToString();

                                double len = new FileInfo(path).Length;
                                while (len >= 1024)
                                {
                                    len = len / 1024;
                                }
                                string result = String.Format(" {0:0.##} {1}", len, "MB");
                                lp.Velikost = result;

                                var file2 = TagLib.File.Create(path);
                                lp.Leto = file2.Tag.Year.ToString();

                                lp.Naslov = System.IO.Path.GetFileNameWithoutExtension(path);
                                lp.Zvrst = file2.Tag.FirstGenre;


                                double minute = file2.Properties.Duration.Minutes;
                                double sekunde = file2.Properties.Duration.Seconds;
                                if (sekunde < 10)
                                {
                                      lp.Trajanje = minute.ToString() + ":0" + sekunde;
                                }
                                else
                                { 
                                        lp.Trajanje = minute.ToString()+":"+ sekunde;
                                }

                        //listPlay.Add(lp);
                        this.mainList.PlayList.Add(lp);
                    }

                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Loading Eror.\n");
                    }
                    listsPlays = listPlay;

            }
        }
 
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "XML-File | *.xml";
                if (sfd.ShowDialog() == true)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MainList));
                    using (TextWriter writer = new StreamWriter(sfd.FileName))
                    {
                        serializer.Serialize(writer, mainList);
                    }
                }
                MessageBox.Show("Export Successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MainList));

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "XML| *.xml";
            if (open.ShowDialog() == true)
            {
                TextReader reader = new StreamReader(open.FileName);

                MainList listLoaded = (MainList)serializer.Deserialize(reader);
                mainList.ListName = listLoaded.ListName;

                mainList.PlayList.Clear();
                foreach(ListPlay listItem in listLoaded.PlayList)
                {
                    mainList.PlayList.Add(listItem);
                }
                reader.Close();
            }
        }
        
        private void Horizontal_Click(object sender, RoutedEventArgs e)
        {
            
            Grid.SetColumnSpan(stackList, 3);
            Grid.SetColumnSpan(stackPlayer,3);
            Grid.SetColumn(stackPlayer, 0);
            Grid.SetRow(stackPlayer, 2);
            var converter = new GridLengthConverter();
            row2.Height = (GridLength)converter.ConvertFromString("auto");
            row3.Height = (GridLength)converter.ConvertFromString("*");
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {

            Grid.SetColumnSpan(stackList, 1);
            Grid.SetColumnSpan(stackPlayer, 1);
            Grid.SetColumn(stackPlayer, 2);
            Grid.SetRow(stackPlayer, 1);
            var converter = new GridLengthConverter();
            row2.Height = (GridLength)converter.ConvertFromString("*");
            row3.Height = (GridLength)converter.ConvertFromString("0");
        }

        private void Playlist_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            int index = -1;
            var selectedItem = (dynamic)Playlist.SelectedItems[0];
            for (int i = 0; i < Playlist.Items.Count; i++)
            {

                selectedItem = (dynamic)Playlist.Items[i];
                if (selectedItem.IsPlaying)
                { index = i; }
            }

            int SelectedListViewItem = Int32.Parse(Playlist.SelectedIndex.ToString());


            if ((SelectedListViewItem != index && index != -1) || (index == -1))
            {
                DeleteItem2.IsEnabled = true;
                EditItem2.IsEnabled = true;
            }
            else
            {
                DeleteItem2.IsEnabled = false;
                EditItem2.IsEnabled = false;
            }
        }

        private void ShowContainingFolder_Click(object sender, RoutedEventArgs e)
        {

            var selectedItem = (dynamic)Playlist.SelectedItems[0];
            string path = selectedItem.Lokacija;
            path = System.IO.Path.GetDirectoryName(path);
            Process.Start(path);

        }

        private void _BlueColor_Click(object sender, RoutedEventArgs e)
        {
           
            var dict = this.Resources.MergedDictionaries.ElementAtOrDefault(0);
            dict.Source = new Uri(@"DictionaryBlue.xaml", UriKind.Relative);
        }

        private void _DefaultColor_Click(object sender, RoutedEventArgs e)
        {
            var dict = this.Resources.MergedDictionaries.ElementAtOrDefault(0);
            dict.Source = new Uri(@"DictionaryDefault.xaml", UriKind.Relative);
        }

        private void _GrayColor_Click(object sender, RoutedEventArgs e)
        {
            var dict = this.Resources.MergedDictionaries.ElementAtOrDefault(0);
            dict.Source = new Uri(@"DictionaryGrey.xaml", UriKind.Relative);

        }
    }
}
