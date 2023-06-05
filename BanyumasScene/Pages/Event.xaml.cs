using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BanyumasScene.Pages
{
    /// <summary>
    /// Interaction logic for Event.xaml
    /// </summary>
    public partial class Event : Page
    {
        private byte[] imageData; 
        private string loggedInUsername; 

        public Event()
        {
            InitializeComponent();
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

                // Membaca data gambar dalam bentuk byte array
                imageData = File.ReadAllBytes(imagePath);

                // Menampilkan gambar di PictureBox
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
            string caption = txtCaption.Text;
            string place = txtPlace.Text;
            string loggedInUsername = User.Username; // Menggunakan username dari objek User
            string dateup = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Koneksi ke database
            string connectionString = "datasource=localhost;port=3306;username=root;database=uadb";
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Query SQL untuk menyimpan data gambar ke database berserta caption dan place
                    string insertQuery = "INSERT INTO tblimage (username, caption, place, image, dateup) VALUES (@username, @caption, @place, @image, @dateup)";
                    MySqlCommand command = new MySqlCommand(insertQuery, connection);
                    command.Parameters.AddWithValue("@username", loggedInUsername);
                    command.Parameters.AddWithValue("@image", imageData);
                    command.Parameters.AddWithValue("@caption", txtCaption.Text);
                    command.Parameters.AddWithValue("@place", txtPlace.Text);
                    command.Parameters.AddWithValue("@dateup", dateup);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Data gambar berhasil disimpan ke database.");
                    Page_Loaded(null, null);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan saat menyimpan data gambar: " + ex.Message);
                }
            }
        }


        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (User.type == "Admin")
            {
                // Admin dapat melakukan update pada semua data
                if (DataGrid.SelectedItem != null)
                {
                    DataRowView selectedRow = (DataRowView)DataGrid.SelectedItem;

                    // Mengambil nilai kolom-kolom dari baris terpilih
                    int id = Convert.ToInt32(selectedRow["id"]); // Ubah "id" sesuai dengan nama kolom id di tabel
                    string username = selectedRow["username"].ToString(); // Ubah "username" sesuai dengan nama kolom username di tabel
                    string caption = selectedRow["caption"].ToString(); // Ubah "caption" sesuai dengan nama kolom caption di tabel
                    string place = selectedRow["place"].ToString(); // Ubah "place" sesuai dengan nama kolom place di tabel

                    // Membaca nilai baru dari input pengguna
                    string newCaption = txtCaption.Text;
                    string newPlace = txtPlace.Text;

                    // Memeriksa apakah ada perubahan pada caption dan place
                    if (caption == newCaption && place == newPlace)
                    {
                        MessageBox.Show("Tidak ada perubahan pada data.");
                        return;
                    }

                    // Melakukan operasi update sesuai dengan nilai-nilai yang diambil dari baris terpilih
                    string connectionString = "server=localhost;user=root;database=uadb;password=";
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();

                            // Query SQL untuk melakukan update data di database
                            string updateQuery = "UPDATE tblimage SET caption = @newCaption, place = @newPlace, image = @imageData WHERE id = @id";
                            MySqlCommand command = new MySqlCommand(updateQuery, connection);
                            command.Parameters.AddWithValue("@newCaption", newCaption);
                            command.Parameters.AddWithValue("@newPlace", newPlace);
                            command.Parameters.AddWithValue("@id", id);
                            command.Parameters.AddWithValue("@imageData", imageData);

                            command.ExecuteNonQuery();

                            // Menampilkan pesan atau melakukan tindakan lain setelah update berhasil
                            MessageBox.Show("Data updated!");

                            // Memperbarui baris terpilih dalam DataGrid dengan data terbaru
                            selectedRow["caption"] = newCaption;
                            selectedRow["place"] = newPlace;
                            Page_Loaded(null, null);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Terjadi kesalahan saat memperbarui data: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No row selected!");
                }
            }
            else
            {
                // User hanya dapat mengupdate data yang dimilikinya
                if (DataGrid.SelectedItem != null)
                {
                    DataRowView selectedRow = (DataRowView)DataGrid.SelectedItem;

                    // Mengambil nilai kolom-kolom dari baris terpilih
                    int id = Convert.ToInt32(selectedRow["id"]); // Ubah "id" sesuai dengan nama kolom id di tabel
                    string username = selectedRow["username"].ToString(); // Ubah "username" sesuai dengan nama kolom username di tabel
                    string caption = selectedRow["caption"].ToString(); // Ubah "caption" sesuai dengan nama kolom caption di tabel
                    string place = selectedRow["place"].ToString(); // Ubah "place" sesuai dengan nama kolom place di tabel

                    // Memeriksa apakah data yang akan diupdate adalah milik pengguna
                    if (User.type != "Admin" && username != User.Username)
                        {
                            MessageBox.Show("Anda hanya bisa menghapus data yang Anda masukkan.");
                            return;
                        }

                    // Membaca nilai baru dari input pengguna
                    string newCaption = txtCaption.Text;
                    string newPlace = txtPlace.Text;

                    // Memeriksa apakah ada perubahan pada caption dan place
                    if (caption == newCaption && place == newPlace)
                    {
                        MessageBox.Show("Tidak ada perubahan pada data.");
                        return;
                    }

                    // Melakukan operasi update sesuai dengan nilai-nilai yang diambil dari baris terpilih
                    string connectionString = "server=localhost;user=root;database=uadb;password=";
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();

                            // Query SQL untuk melakukan update data di database
                            string updateQuery = "UPDATE tblimage SET caption = @newCaption, place = @newPlace, image = @imageData WHERE id = @id AND username = @username ";
                            MySqlCommand command = new MySqlCommand(updateQuery, connection);
                            command.Parameters.AddWithValue("@newCaption", newCaption);
                            command.Parameters.AddWithValue("@newPlace", newPlace);
                            command.Parameters.AddWithValue("@id", id);
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@imageData", imageData);

                            command.ExecuteNonQuery();

                            // Menampilkan pesan atau melakukan tindakan lain setelah update berhasil
                            MessageBox.Show("Data updated!");
                            Page_Loaded(null, null);

                            // Memperbarui baris terpilih dalam DataGrid dengan data terbaru
                            selectedRow["caption"] = newCaption;
                            selectedRow["place"] = newPlace;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Terjadi kesalahan saat memperbarui data: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No row selected!");
                }
            }
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Periksa apakah pengguna memiliki role yang diperlukan (misalnya role "Admin") sebelum melanjutkan
            if (User.type == "Admin" || User.type != "Admin")
            {
                if (DataGrid.SelectedItem != null)
                {
                    DataRowView selectedRow = (DataRowView)DataGrid.SelectedItem;

                    int id = Convert.ToInt32(selectedRow["id"]); // Mengambil nilai ID dari DataRowView

                    string username = selectedRow["username"].ToString(); // Ubah "username" sesuai dengan nama kolom username di tabel

                    // Periksa apakah pengguna non-admin adalah pemilik data yang akan dihapus
                    if (User.type != "Admin" && username != User.Username)
                    {
                        MessageBox.Show("Anda hanya bisa menghapus data yang Anda masukkan.");
                        return;
                    }

                    MessageBoxResult result = MessageBox.Show("Apakah Anda yakin ingin menghapus data ini?", "Konfirmasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        string connectionString = "datasource=localhost;port=3306;username=root;database=uadb";
                        using (var connection = new MySqlConnection(connectionString))
                        {
                            try
                            {
                                connection.Open();

                                // Tambahkan kondisi WHERE untuk membatasi penghapusan hanya pada data yang sesuai dengan pengguna yang sedang masuk
                                string deleteQuery = "DELETE FROM tblimage WHERE username = @username AND id = @id";
                                MySqlCommand command = new MySqlCommand(deleteQuery, connection);
                                command.Parameters.AddWithValue("@username", username);
                                command.Parameters.AddWithValue("@id", id);
                                command.ExecuteNonQuery();

                                MessageBox.Show("Data deleted!");

                                Page_Loaded(null, null);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Terjadi kesalahan saat menghapus data: " + ex.Message);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No row selected!");
                }
            }
            else
            {
                MessageBox.Show("Anda tidak memiliki izin untuk menghapus data.");
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           
            var duplicateColumns = DataGrid.Columns .Where(column => column is DataGridTemplateColumn && column.Header.ToString() == "Image")
       .ToList();

            foreach (var column in duplicateColumns)
            {
                DataGrid.Columns.Remove(column);
            }

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
                DataGrid.Columns.Add(imageColumn);

            // Melakukan query untuk mengambil data dari database
            string connectionString = "datasource=localhost;port=3306;username=root;database=uadb";
                using (var connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // Query SQL untuk mendapatkan data dari database
                        string selectQuery = "SELECT id, username AS Username, caption AS Caption, place AS Place, image AS Image, dateup AS dateup FROM tblimage";
                        MySqlCommand command = new MySqlCommand(selectQuery, connection);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Mengkonversi data BLOB ke byte[]
                        foreach (DataRow row in dataTable.Rows)
                        {
                            row["Image"] = (byte[])row["Image"];
                        }

                        // Mengaitkan DataTable dengan DataGrid
                        DataGrid.ItemsSource = dataTable.DefaultView;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Terjadi kesalahan saat memuat data dari database: " + ex.Message);
                    }
                }

      

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
                        command.CommandText = "SELECT id, username AS Username, caption AS Caption, place AS Place, image AS Image, dateup AS dateup FROM tblimage";
                    }
                    else
                    {
                        // Jika TextBox pencarian diisi, lakukan pencarian sesuai dengan keyword
                        command.CommandText = "SELECT id, username AS Username, caption AS Caption, place AS Place, image AS Image, dateup AS dateup FROM tblimage WHERE id LIKE @keyword OR username LIKE @keyword OR place LIKE @keyword OR caption LIKE @keyword";
                        command.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    }

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    // Menghapus kolom gambar yang ada sebelumnya
                    var duplicateColumns = DataGrid.Columns.Where(column => column.Header.ToString() == "Image").ToList();
                    foreach (var column in duplicateColumns)
                    {
                        DataGrid.Columns.Remove(column);
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
                    DataGrid.Columns.Add(imageColumn);

                    // Mengaitkan DataTable dengan DataGrid
                    DataGrid.ItemsSource = dataTable.DefaultView;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Kesalahan", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }






    }
}