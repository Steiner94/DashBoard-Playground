
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Data;
using ModernDashboard.Model;
using MySql.Data.MySqlClient;

namespace ModernDashboard.ViewModel
{
    public class MusicViewModel : INotifyPropertyChanged
    {
        private CollectionViewSource MusicItemsCollection;
        public ICollectionView MusicSourceCollection => MusicItemsCollection.View;

        public MusicViewModel()
        {
            ObservableCollection<MusicItems> musicItems = new ObservableCollection<MusicItems>();
            string connectionString = "Server=localhost;Port=3306;Database=mvvm;Uid=root;Pwd=123456;";
            string query = "SELECT * FROM music";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string musicName = reader.GetString("ganere");
                            string musicImage = reader.GetString("image_path");
                            MusicItems musicItem = new MusicItems { MusicName = musicName, MusicImage = musicImage };
                            musicItems.Add(musicItem);
                        }
                    }
                }
            }

            MusicItemsCollection = new CollectionViewSource { Source = musicItems };
            MusicItemsCollection.Filter += MenuItems_Filter;
        }

        private string filterText;
        public string FilterText
        {
            get => filterText;
            set
            {
                filterText = value;
                MusicItemsCollection.View.Refresh();
                OnPropertyChanged("FilterText");
            }
        }

        private void MenuItems_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                e.Accepted = true;
                return;
            }

            MusicItems _item = e.Item as MusicItems;
            if (_item.MusicName.ToUpper().Contains(FilterText.ToUpper()))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}
