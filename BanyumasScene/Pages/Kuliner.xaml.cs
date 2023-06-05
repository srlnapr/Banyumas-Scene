using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BanyumasScene.Pages
{
    /// <summary>
    /// Interaction logic for Kuliner.xaml
    /// </summary>
    public partial class Kuliner : Page
    {
        private byte[] imageData;
        public Kuliner()
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

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var duplicateColumns = dataGrid.Columns.Where(column => column is DataGridTemplateColumn && column.Header.ToString() == "Foto")
       .ToList();

            foreach (var column in duplicateColumns)
            {
                dataGrid.Columns.Remove(column);
            }

            DataGridTemplateColumn imageColumn = new DataGridTemplateColumn();
            imageColumn.Header = "Foto";

            // Membuat template data untuk kolom gambar
            DataTemplate imageTemplate = new DataTemplate();

            // Menambahkan elemen Image ke dalam template data
            FrameworkElementFactory imageFactory = new FrameworkElementFactory(typeof(Image));
            imageFactory.SetBinding(Image.SourceProperty, new Binding("Foto") { Converter = new ByteArrayToImageSourceConverter() });
            imageFactory.SetValue(Image.MaxHeightProperty, 100.0);
            imageFactory.SetValue(Image.MaxWidthProperty, 100.0);
            imageTemplate.VisualTree = imageFactory;

            // Mengatur template data ke kolom gambar
            imageColumn.CellTemplate = imageTemplate;

            // Menambahkan kolom gambar ke DataGrid
            dataGrid.Columns.Add(imageColumn);

            // Melakukan query untuk mengambil data dari database
            string connectionString = "datasource=localhost;port=3306;username=root;database=uadb";
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Query SQL untuk mendapatkan data dari database,
                    string selectQuery = "SELECT id, nama AS Nama, deksripsi AS Deksripsi, harga AS Harga, tempat AS Tempat, jenis AS Jenis, foto AS Foto, lastup AS Lastup FROM tblkuliner";
                    MySqlCommand command = new MySqlCommand(selectQuery, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Mengkonversi data BLOB ke byte[]
                    foreach (DataRow row in dataTable.Rows)
                    {
                        row["Foto"] = (byte[])row["Foto"];
                    }

                    // Mengaitkan DataTable dengan DataGrid
                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan saat memuat data dari database: " + ex.Message);
                }
            }


        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string nama = txtNama.Text;
            string tempat = txtTempat.Text;
            string deksripsi = txtDeksripsi.Text;
            int harga = int.Parse(txtHarga.Text);
            string jenis = txtJenis.Text;
            string lastup = DateTime.Now.ToString("yyyy-MM-dd");

            // Koneksi ke database
            string connectionString = "datasource=localhost;port=3306;username=root;database=uadb";
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Query SQL untuk menyimpan data gambar ke database berserta caption dan place
                    string insertQuery = "INSERT INTO tblkuliner (nama, deksripsi, tempat, harga, jenis, foto, lastup) VALUES (@nama, @deksripsi, @tempat, @harga, @jenis, @foto, @lastup)";
                    MySqlCommand command = new MySqlCommand(insertQuery, connection);
                    command.Parameters.AddWithValue("@foto", imageData);
                    command.Parameters.AddWithValue("@nama", txtNama.Text);
                    command.Parameters.AddWithValue("@deksripsi", txtDeksripsi.Text);
                    command.Parameters.AddWithValue("@tempat", txtTempat.Text);
                    command.Parameters.AddWithValue("@harga", txtHarga.Text);
                    command.Parameters.AddWithValue("@jenis", txtJenis.Text);
                    command.Parameters.AddWithValue("@lastup", lastup);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Data gambar berhasil disimpan ke database.");
                    dataGrid_Loaded(null, null);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan saat menyimpan data gambar: " + ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            if (dataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                int selectedId = Convert.ToInt32(selectedRow["id"]);

                string connectionString = "server=localhost;user=root;database=uadb;password=";
                string deleteQuery = "DELETE FROM tblkuliner WHERE id = @id";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@id", selectedId);
                        deleteCommand.ExecuteNonQuery();

                        MessageBox.Show("Data berhasil dihapus.");

                        string selectQuery = "SELECT * FROM tblkuliner";
                        MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectQuery, connection);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        dataGrid.ItemsSource = dataTable.DefaultView;
                        dataGrid_Loaded(null, null);

                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                int id = Convert.ToInt32(selectedRow["id"]); // Ubah "id" sesuai dengan nama kolom ID di tabel

                string nama = txtNama.Text;
                string deksripsi = txtDeksripsi.Text;
                string tempat = txtTempat.Text;
                string harga = txtHarga.Text;
                string jenis = txtJenis.Text;

                if (nama == selectedRow["nama"].ToString() &&
                    deksripsi == selectedRow["deksripsi"].ToString() &&
                    tempat == selectedRow["tempat"].ToString() &&
                    harga == selectedRow["harga"].ToString() &&
                    jenis == selectedRow["jenis"].ToString())
                {
                    MessageBox.Show("No changes have been made to the data", "No Changes");
                    return;
                }

                if (nama != "" && deksripsi != "" && tempat != "" && harga != "" && jenis != "")
                {
                    // Validasi apakah harga hanya berisi angka
                    if (int.TryParse(harga, out int hargaValue))
                    {
                        string connectionString = "server=localhost;user=root;database=uadb;password=";
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            try
                            {
                                connection.Open();
                                string updateQuery = "UPDATE tblkuliner SET nama = @nama, tempat = @tempat, deksripsi = @deksripsi, harga = @harga, jenis = @jenis, foto = @foto WHERE id = @id";

                                using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                                {
                                    command.Parameters.AddWithValue("@id", id);
                                    command.Parameters.AddWithValue("@foto", imageData);
                                    command.Parameters.AddWithValue("@nama", nama);
                                    command.Parameters.AddWithValue("@deksripsi", deksripsi);
                                    command.Parameters.AddWithValue("@tempat", tempat);
                                    command.Parameters.AddWithValue("@harga", hargaValue);
                                    command.Parameters.AddWithValue("@jenis", jenis);
                                    command.ExecuteNonQuery();

                                    MessageBox.Show("Record Successfully Updated", "UPDATE");

                                    string selectQuery = "SELECT * FROM tblkuliner"; // Ubah query sesuai dengan tabel yang digunakan
                                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectQuery, connection);
                                    DataTable dataTable = new DataTable();
                                    dataAdapter.Fill(dataTable);

                                    dataGrid.ItemsSource = dataTable.DefaultView;
                                    dataGrid_Loaded(null, null);
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
                        MessageBox.Show("Input must be a number for 'Harga'", "ERROR");
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


        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtNama.Text = string.Empty;
            txtDeksripsi.Text = string.Empty;
            txtHarga.Text = string.Empty;
            txtJenis.Text = string.Empty;
            txtTempat.Text = string.Empty;
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
                            command.CommandText = "SELECT id, nama AS Nama, deksripsi AS Deksripsi, harga AS Harga, tempat AS Tempat, jenis AS Jenis, foto AS Foto, lastup AS Lastup FROM tblkuliner";
                        }
                        else
                        {
                            // Jika TextBox pencarian diisi, lakukan pencarian sesuai dengan keyword
                            command.CommandText = "SELECT id, nama AS Nama, deksripsi AS Deksripsi, harga AS Harga, tempat AS Tempat, jenis AS Jenis, foto AS Foto, lastup AS Lastup FROM tblkuliner WHERE id LIKE @keyword OR nama LIKE @keyword OR tempat LIKE @keyword OR deksripsi LIKE @keyword  OR harga LIKE @keyword OR jenis LIKE @keyword OR foto LIKE @keyword";
                            command.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                        }

                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        adapter.Fill(dataTable);

                        // Menghapus kolom gambar yang ada sebelumnya
                        var duplicateColumns = dataGrid.Columns.Where(column => column.Header.ToString() == "Foto").ToList();
                        foreach (var column in duplicateColumns)
                        {
                            dataGrid.Columns.Remove(column);
                        }

                        // Membuat kolom gambar baru
                        DataGridTemplateColumn imageColumn = new DataGridTemplateColumn();
                        imageColumn.Header = "Foto";

                        // Membuat template data untuk kolom gambar
                        DataTemplate imageTemplate = new DataTemplate();

                        // Menambahkan elemen Image ke dalam template data
                        FrameworkElementFactory imageFactory = new FrameworkElementFactory(typeof(Image));
                        imageFactory.SetBinding(Image.SourceProperty, new Binding("Foto") { Converter = new ByteArrayToImageSourceConverter() });
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (User.type == "Admin")
            {
                btnClear.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
                btnUpload.Visibility = Visibility.Visible;
                txtDeksripsi.Visibility = Visibility.Visible;
                txtHarga.Visibility = Visibility.Visible;
                txtJenis.Visibility = Visibility.Visible;
                txtNama.Visibility = Visibility.Visible;
                txtTempat.Visibility = Visibility.Visible;
                pictureBox.Visibility = Visibility.Visible;
            }
            else if (User.type != "Admin")
            {
                btnClear.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Collapsed;
                btnUpload.Visibility = Visibility.Collapsed;
                txtTempat.Visibility = Visibility.Collapsed;
                txtNama.Visibility = Visibility.Collapsed;
                txtJenis.Visibility = Visibility.Collapsed;
                txtHarga.Visibility = Visibility.Collapsed;
                txtDeksripsi.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                pictureBox.Visibility = Visibility.Collapsed;

            }
        }
    }
}
