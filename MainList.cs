using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MojVLC
{
    public class MainList : INotifyPropertyChanged
    {
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
        public MainList()
        {
            ListName = "Trust";
            PlayList = new ObservableCollection<ListPlay>();
        }
        private string _listName;

        [XmlAttribute]
        public string ListName
        {
            get { return _listName; }
            set
            {
                _listName = value;
                OnPropertyChanged(new PropertyChangedEventArgs(""));
            }
        }

        public ObservableCollection<ListPlay> PlayList { get; set; }
    }
}
