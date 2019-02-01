using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace MojVLC
{
    /// <summary>
    /// Interaction logic for Playlist.xaml
    /// </summary>
    public partial class Playlist : Window
    {

        string path;
        public Playlist(string _path)
        {
            InitializeComponent();
            path = _path;
        }
        
        private void Playlist_Loaded(object sender, RoutedEventArgs e)
        {
            //Lokacija
            TextBlockURL.Text = TextBlockURL.Text+path;

            //Datum Spremenbe
            TextBlockDatum.Text = TextBlockDatum.Text + " " + File.GetLastWriteTime(path);

            //velikost
            double len = new FileInfo(path).Length;
            while (len >= 1024)
            {
                len = len / 1024;
            }
            string result = String.Format(" {0:0.##} {1}", len, "MB");
            TextBlockSize.Text = TextBlockSize.Text + " " + result;

            //leto
            var file = TagLib.File.Create(path); // Change file path accordingly.
            TextBoxYear.Text = TextBoxYear.Text + " " + file.Tag.Year;
            file.Save();

            //dolzina
            double minute = file.Properties.Duration.Minutes;
            double sekunde = file.Properties.Duration.Seconds;
            if (sekunde < 10)
            {
                TextBlockDuration.Text = TextBlockDuration.Text + " " +minute.ToString() + ":0" + sekunde;
            }
            else
            {
                TextBlockDuration.Text = TextBlockDuration.Text + " " +minute.ToString() + ":" + sekunde;
            }
            file.Save();

            //naslov
            PlaylistWin.Title = System.IO.Path.GetFileNameWithoutExtension(path); 

            #region zvrst
            ComboGenre.Items.Clear();
            //check for null (first run)
            if (Properties.Settings.Default.Genres != null)
            {
                //create a new collection again
                StringCollection collection = new StringCollection();
                //set the collection from the settings variable
                collection = Properties.Settings.Default.Genres;
                //convert the collection back to a list
                List<string> followedList = collection.Cast<string>().ToList();
                //populate the listview again from the new list
                foreach (var item in followedList)
                {
                    ComboGenre.Items.Add(item);
                }

                //if (file.Tag.Genres==empty)
                {
                    string[] FileGenre = file.Tag.Genres;

                    string selectedItem = "";
                    bool obstaja = false;
                    foreach (var item in FileGenre)
                    {
                        foreach (var item2 in followedList)
                        {
                            if (item == item2)
                            {
                                obstaja = true;
                                selectedItem = item;
                            }
                        }
                    }
                    if (!obstaja) {
                        foreach (var item in FileGenre)
                        {
                            Properties.Settings.Default.Genres.Add(item.ToString());

                            selectedItem = item;
                        }
                        Properties.Settings.Default.Save();
                        Properties.Settings.Default.Reload();
                    }
                    ComboGenre.Items.Clear();
                    //check for null (first run)
                    if (Properties.Settings.Default.Genres != null)
                    {
                        //create a new collection again
                        collection = new StringCollection();
                        //set the collection from the settings variable
                        collection = Properties.Settings.Default.Genres;
                        //convert the collection back to a list
                        followedList = collection.Cast<string>().ToList();
                        //populate the listview again from the new list
                        foreach (var item in followedList)
                        {
                            ComboGenre.Items.Add(item);
                        }
                    }
                    ComboGenre.SelectedValue=selectedItem;
                }
            }
            #endregion


        }
        private void Cencel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var file = TagLib.File.Create(path);
            if (int.TryParse(TextBoxYear.Text.ToString(), out int n))
            {
                file.Tag.Year = Convert.ToUInt32(TextBoxYear.Text.ToString());
            }
            else
            {
                MessageBox.Show("Napačna letnica!");
            }
            if (ComboGenre.SelectedIndex > -1)
            { 
                string[] gen = new string[1];
                gen[0] = ComboGenre.SelectedValue.ToString();
                file.Tag.Genres = gen;
            }
            file.Save();

            MessageBox.Show("Shranjeno!");
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = " Supported|  *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (open.ShowDialog() == true)
            {
                this.image.Source = new BitmapImage(new Uri(open.FileName.ToString()));
            }
        }
    }
}
