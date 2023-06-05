using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace BanyumasScene.Pages
{
    /// <summary>
    /// Interaction logic for Wisata.xaml
    /// </summary>
    /// 

    public partial class Wisata : Page
    {
        private byte[] imageData;
        private MySqlConnection connection;
        private string connectionString = "datasource=localhost;port=3306;username=root;password=";

        public Wisata()
        {
            InitializeComponent();
            string connectionString = "datasource=localhost;port=3306;username=root;password=";
            DataGrid_Loaded_1(null, null);
            connection = new MySqlConnection(connectionString);
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            IsFavorite favorite = (IsFavorite)button.DataContext;

            FavoritPage favoritPage = new FavoritPage();

            favoritPage.DataContext = this.DataContext;

            favoritPage.FavoriteList.Add(favorite);

            NavigationService.Navigate(favoritPage);
        }

        private void AddToFavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            DataRowView dataRowView = (DataRowView)button.DataContext;

            DataRow dataRow = dataRowView.Row;

            IsFavorite favorite = new IsFavorite
            {
                id = (int)dataRow["ID"],
                name = (string)dataRow["Nama"],
                place = (string)dataRow["Lokasi"],
                price = (decimal)dataRow["Harga Tiket"],
                jam = (string)dataRow["Jam Operasional"],
                jenis = (string)dataRow["Jenis"],
                dateup = (DateTime)dataRow["Update Terakhir"],
                image = (byte[])dataRow["Image"],
            };

            FavoritPage favoritPage = Application.Current.Windows.OfType<FavoritPage>().FirstOrDefault();

            if (favoritPage == null)
            {
                favoritPage = new FavoritPage();
                favoritPage.FavoriteList = new ObservableCollection<IsFavorite>();

                favoritPage.FavoriteList.Add(favorite);

                NavigationService.Navigate(favoritPage);
            }
            else
            {
                if (favoritPage.FavoriteList == null)
                {
                    favoritPage.FavoriteList = new ObservableCollection<IsFavorite>();
                }

                favoritPage.FavoriteList.Add(favorite);
            }

            MessageBox.Show("Wisata ditambahkan ke favorit!");
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public class ByteArrayToImageSourceConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                byte[] imageData = value as byte[];
                if (imageData != null && imageData.Length > 0)
                {
                    using (MemoryStream stream = new MemoryStream(imageData))
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                        return image;
                    }
                }
                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }


        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                imageData = File.ReadAllBytes(imagePath);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(imageData);
                bitmap.EndInit();
                pictureBox.Source = bitmap;

                MessageBox.Show("Gambar berhasil diunggah.");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string namaWisata = txtNamaWisata.Text;
            string lokasi = txtLokasi.Text;
            string hargaTiket = txtHargaTiket.Text;
            string jamBuka = txtJamBuka.Text;
            string jenisWisata = txtJenisWisata.Text;
            string dateup = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                connection.Open();

                string query = "INSERT INTO uadb.tblwisata (name, place, price, jam, jenis, image,dateup) " +
                               "VALUES (@namaWisata, @lokasi, @hargaTiket, @jamBuka, @jenisWisata, @image, @dateup)";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@namaWisata", namaWisata);
                command.Parameters.AddWithValue("@lokasi", lokasi);
                command.Parameters.AddWithValue("@hargaTiket", hargaTiket);
                command.Parameters.AddWithValue("@jamBuka", jamBuka);
                command.Parameters.AddWithValue("@jenisWisata", jenisWisata);
                command.Parameters.AddWithValue("@image", imageData);
                command.Parameters.AddWithValue("@dateup", dateup);

                command.ExecuteNonQuery();

                MessageBox.Show("Data berhasil disimpan.");

                ClearTextBoxes();


                DataGrid_Loaded_1(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void ClearTextBoxes()
        {
            txtNamaWisata.Text = "";
            txtLokasi.Text = "";
            txtHargaTiket.Text = "";
            txtJamBuka.Text = "";
            txtJenisWisata.Text = "";
            pictureBox.Source = null;
        }
    

        private void DataGrid_Loaded_1(object sender, RoutedEventArgs e)
        {
            var duplicateFavoriteColumn = dataGrid.Columns
        .FirstOrDefault(column => column.Header.ToString() == "Favorite");
            var duplicateImageColumn = dataGrid.Columns
                .FirstOrDefault(column => column.Header.ToString() == "Image");

            if (duplicateFavoriteColumn != null)
            {
                dataGrid.Columns.Remove(duplicateFavoriteColumn);
            }

            if (duplicateImageColumn != null)
            {
                dataGrid.Columns.Remove(duplicateImageColumn);
            }

            DataGridTemplateColumn favoritColumn = new DataGridTemplateColumn();
            favoritColumn.Header = "Favorite";

            DataTemplate favoritTemplate = new DataTemplate();

            FrameworkElementFactory favoritFactory = new FrameworkElementFactory(typeof(Button));
            favoritFactory.SetBinding(Button.ContentProperty, new Binding("FavoriteButtonText"));
            favoritFactory.SetBinding(Button.CommandProperty, new Binding("AddToFavoritesCommand"));

            favoritFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(AddToFavoritesButton_Click));

            favoritTemplate.VisualTree = favoritFactory;
            favoritColumn.CellTemplate = favoritTemplate;

            dataGrid.Columns.Add(favoritColumn);

            DataGridTemplateColumn imageColumn = new DataGridTemplateColumn();
            imageColumn.Header = "Image";

            DataTemplate imageTemplate = new DataTemplate();

            FrameworkElementFactory imageFactory = new FrameworkElementFactory(typeof(Image));
            imageFactory.SetBinding(Image.SourceProperty, new Binding("Image") { Converter = new ByteArrayToImageSourceConverter() });
            imageFactory.SetValue(Image.MaxHeightProperty, 100.0);
            imageFactory.SetValue(Image.MaxWidthProperty, 100.0);
            imageTemplate.VisualTree = imageFactory;

            imageColumn.CellTemplate = imageTemplate;

            dataGrid.Columns.Add(imageColumn);

            string connectionString = "datasource=localhost;port=3306;username=root;database=uadb";
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string selectQuery = "SELECT id AS Id, name AS Nama, place AS Lokasi, price AS 'Harga Tiket', jam AS 'Jam Operasional', jenis AS Jenis, dateup AS 'Update Terakhir', image AS Image FROM tblwisata;";
                    MySqlCommand command = new MySqlCommand(selectQuery, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        row["Image"] = (byte[])row["Image"];
                    }

                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading data from the database: " + ex.Message);
                }
            }
        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (User.type == "Admin")
            {
                btnNew.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
                btnUpload.Visibility = Visibility.Visible;
                txtNamaWisata.Visibility = Visibility.Visible;
                txtLokasi.Visibility = Visibility.Visible;
                txtJenisWisata.Visibility = Visibility.Visible;
                txtJamBuka.Visibility = Visibility.Visible;
                txtHargaTiket.Visibility = Visibility.Visible;
                pictureBox.Visibility = Visibility.Visible;
            }
            else if (User.type != "Admin")
            {
                btnNew.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Collapsed;
                btnUpload.Visibility = Visibility.Collapsed;
                txtHargaTiket.Visibility = Visibility.Collapsed;
                txtJamBuka.Visibility = Visibility.Collapsed;
                txtJenisWisata.Visibility = Visibility.Collapsed;
                txtLokasi.Visibility = Visibility.Collapsed;
                txtNamaWisata.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                pictureBox.Visibility = Visibility.Collapsed;

            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                int id = Convert.ToInt32(selectedRow["id"]); // Ubah "ID" sesuai dengan nama kolom ID di tabel

                if (txtNamaWisata.Text != "" && txtLokasi.Text != "" && txtJenisWisata.Text != "" && txtHargaTiket.Text != "" && txtJamBuka.Text != "")
                {
                    string connectionString = "server=localhost;user=root;database=uadb;password=";
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            string updateQuery = "UPDATE tblwisata SET name = @name, place = @place, price = @price, jam = @jam, jenis = @jenis, image = @imageData WHERE ID = @id";

                            using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@name", txtNamaWisata.Text);
                                command.Parameters.AddWithValue("@place", txtLokasi.Text);
                                command.Parameters.AddWithValue("@jenis", txtJenisWisata.Text);
                                command.Parameters.AddWithValue("@jam", txtJamBuka.Text);
                                command.Parameters.AddWithValue("@price", txtHargaTiket.Text);
                                command.Parameters.AddWithValue("@id", id);
                                command.Parameters.AddWithValue("@imageData", imageData);

                                command.ExecuteNonQuery();

                                MessageBox.Show("Record Successfully Updated", "UPDATE");

                                string selectQuery = "SELECT * FROM tblwisata"; // Ubah query sesuai dengan tabel yang digunakan
                                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectQuery, connection);
                                DataTable dataTable = new DataTable();
                                dataAdapter.Fill(dataTable);

                                dataGrid.ItemsSource = dataTable.DefaultView;
                                DataGrid_Loaded_1(null, null);
                            }



                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("An error occurred while updating the record: " + ex.Message, "ERROR");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please fill in all the fields", "ERROR");
                }
            }
            else
            {
                MessageBox.Show("Select the record you want to Update", "ERROR");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                int selectedId = Convert.ToInt32(selectedRow["id"]);

                string connectionString = "server=localhost;user=root;database=uadb;password=";
                string deleteQuery = "DELETE FROM tblwisata WHERE id = @id";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@id", selectedId);
                        deleteCommand.ExecuteNonQuery();

                        MessageBox.Show("Data berhasil dihapus.");

                        string selectQuery = "SELECT * FROM tblwisata";
                        MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectQuery, connection);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        dataGrid.ItemsSource = dataTable.DefaultView;
                        DataGrid_Loaded_1(null, null);

                    }
                }
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            txtHargaTiket.Text = string.Empty;
            txtJamBuka.Text = string.Empty;
            txtJenisWisata.Text = string.Empty;
            txtLokasi.Text = string.Empty;
            txtNamaWisata.Text = string.Empty;
            pictureBox.Source = null;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            // Connection string
            string connectionString = "server=localhost;user=root;database=uadb;password=";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = connection.CreateCommand();

                    if (string.IsNullOrEmpty(keyword))
                    {
                        // Jika TextBox pencarian kosong, tampilkan semua data
                        command.CommandText =  "SELECT id AS Id, name AS Nama, place AS Lokasi, price AS 'Harga Tiket', jam AS 'Jam Operasional', jenis AS Jenis, dateup AS 'Update Terakhir', image AS Image FROM tblwisata;";
                        ;
                    }
                    else
                    {
                        // Jika TextBox pencarian diisi, lakukan pencarian sesuai dengan keyword
                       
                        command.CommandText = "SELECT id, name AS Nama, place AS Lokasi, price AS 'Harga Tiket', jam AS 'Jam Operasional', jenis AS Jenis, dateup AS 'Update Terakhir', image AS Image FROM tblwisata WHERE id LIKE @keyword OR name LIKE @keyword OR place LIKE @keyword OR price LIKE @keyword OR jam LIKE @keyword OR jenis LIKE @keyword OR dateup LIKE @keyword OR image LIKE @keyword";
                        command.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                    }

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    // Menghapus kolom gambar yang ada sebelumnya
                    var duplicateColumns = dataGrid.Columns.Where(column => column.Header.ToString() == "Image").ToList();
                    foreach (var column in duplicateColumns)
                    {
                        dataGrid.Columns.Remove(column);
                    }

                    // Membuat kolom gambar baru
                    DataGridTemplateColumn imageColumn = new DataGridTemplateColumn();
                    imageColumn.Header = "Image";

                    // Membuat template data untuk kolom gambar
                    DataTemplate imageTemplate = new DataTemplate();

                    // Menambahkan elemen Image ke dalam template data
                    FrameworkElementFactory imageFactory = new FrameworkElementFactory(typeof(Image));
                    imageFactory.SetBinding(Image.SourceProperty, new Binding("Image") { Converter = new ByteArrayToImageSourceConverter() });
                    imageFactory.SetValue(Image.MaxHeightProperty, 100.0);
                    imageFactory.SetValue(Image.MaxWidthProperty, 100.0);
                    imageTemplate.VisualTree = imageFactory;

                    // Mengatur template data ke kolom gambar
                    imageColumn.CellTemplate = imageTemplate;

                    // Menambahkan kolom gambar ke DataGrid
                    dataGrid.Columns.Add(imageColumn);

                    // Mengaitkan DataTable dengan DataGrid
                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Kesalahan", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }


}

