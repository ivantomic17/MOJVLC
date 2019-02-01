using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;

namespace MojVLC
{
    public class ListPlay : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public ListPlay()
        {
             
        }

        private string _naslov = String.Empty;
        private string _velikost = String.Empty;
        private string _leto = String.Empty;
        private string _datumSpremenbe = String.Empty;
        private string _zvrst = String.Empty;
        private string _lokacija = String.Empty;
        private string _trajanje = String.Empty;
        private bool _isPlaying = false;

        [XmlAttribute]
        public string Naslov
        {
            get { return _naslov; }
            set
            {
                _naslov = value;
                OnPropertyChanged(new PropertyChangedEventArgs(""));
            }
        }

        [XmlAttribute]
        public string Velikost
        {
            get { return _velikost; }
            set
            {
                _velikost = value;
                OnPropertyChanged(new PropertyChangedEventArgs(""));
            }
        }

        [XmlAttribute]
        public string Leto
        {
            get { return _leto; }
            set
            {
                _leto = value;
                OnPropertyChanged(new PropertyChangedEventArgs(""));
            }
        }

        [XmlAttribute]
        public string DatumSpremenbe
        {
            get { return _datumSpremenbe; }
            set
            {
                _datumSpremenbe = value;
                OnPropertyChanged(new PropertyChangedEventArgs(""));
            }
        }

        [XmlAttribute]
        public string Zvrst
        {
            get { return _zvrst; }
            set
            {
                _zvrst = value;
                OnPropertyChanged(new PropertyChangedEventArgs(""));
            }
        }

        [XmlAttribute]
        public string Lokacija
        {
            get { return _lokacija; }
            set
            {
                _lokacija = value;
                OnPropertyChanged(new PropertyChangedEventArgs(""));
            }
        }

        [XmlAttribute]
        public string Trajanje
        {
            get { return _trajanje; }
            set
            {
                _trajanje = value;
                OnPropertyChanged(new PropertyChangedEventArgs(""));
            }
        }

        [XmlAttribute]
        public bool IsPlaying
        {
            get { return _isPlaying;}
            set
            {
                _isPlaying = value;
                OnPropertyChanged(new PropertyChangedEventArgs(""));

            }
        }
    }
}
