using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace BanyumasScene.Pages
{
    public partial class FavoritPage : Page, INotifyPropertyChanged
    {

        private ObservableCollection<IsFavorite> _favoriteList;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IsFavorite> FavoriteList
        {
            get { return _favoriteList; }
            set
            {
                _favoriteList = value;
                OnPropertyChanged(nameof(FavoriteList));
            }
        }

        public FavoritPage()
        {
            InitializeComponent();
            DataContext = this;
            // Initialize the FavoriteList
            FavoriteList = new ObservableCollection<IsFavorite>();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Bind the FavoriteList property to the datagrid
            myListView.ItemsSource = FavoriteList;
        }


    }
}