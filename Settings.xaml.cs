using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MojVLC.Properties;

namespace MojVLC
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selected = GenreList.SelectedItem.ToString();
                Properties.Settings.Default.Genres.Remove(selected);
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            
                GenreList.Items.Clear();
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
                        GenreList.Items.Add(item);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Select genre first!");
            }

        }
        private void Settings_Load(object sender, RoutedEventArgs e)
        {
            ZamikShranjevanja.Text = Properties.Settings.Default.ZamikShranjevanja.ToString();
            GenreList.Items.Clear();
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
                    GenreList.Items.Add(item);
                }
            }

        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Genres.Add(GenreText.Text);
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            
            GenreList.Items.Clear();
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
                    GenreList.Items.Add(item);
                }
            }
        }
        private void listView_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
               GenreText.Text = GenreList.SelectedItem.ToString();
            }
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                string selected = GenreList.SelectedItem.ToString();
                Properties.Settings.Default.Genres.Remove(selected);
                Properties.Settings.Default.Genres.Add(GenreText.Text);
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();

                GenreList.Items.Clear();
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
                    foreach (var item1 in followedList)
                    {
                        GenreList.Items.Add(item1);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Select genre first!");
            }
        }
        private void Shranjevanje_Click(object sender, RoutedEventArgs e)
        {

            if (int.TryParse(ZamikShranjevanja.Text.ToString(), out int n))
            {
                Properties.Settings.Default.ZamikShranjevanja = Convert.ToInt32(ZamikShranjevanja.Text.ToString());
            }
            else
            {
                MessageBox.Show("Vnesite število!");
            }
        }
    }
}
