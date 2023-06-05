using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BanyumasScene.Pages
{
    public partial class Review : Page
    {
        private int selectedId; // variabel untuk menyimpan ID data yang dipilih

        public Review()
        {
            InitializeComponent();
            DataGrid_Loaded(null, null);
        }
        MySqlCommand cmd;
        MySqlDataAdapter adapt;

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            string connectionString = "server=localhost;user=root;database=uadb;password=";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand("SELECT * FROM tbllog", connection);

            try
            {
                connection.Open();

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                DataGrid.ItemsSource = dataTable.DefaultView;
            }
            catch (MySqlException ex)
            {
                // Tangani kesalahan koneksi
            }
            finally
            {
                connection.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "server=localhost;user=root;database=uadb;password=";
            string insertQuery = "INSERT INTO tbllog (fullname, username, domisili, age, nohp, email, password, confirmpass, role) " +
                                 "SELECT @fullname, @username, @domisili, @Age, @nohp, @email, @password, @confirmpass, @Role " +
                                 "FROM DUAL " +
                                 "WHERE NOT EXISTS (SELECT 1 FROM tbllog WHERE username = @username OR email = @email)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@fullname", txtFullname.Text);
                    insertCommand.Parameters.AddWithValue("@username", txtUsername.Text);
                    insertCommand.Parameters.AddWithValue("@domisili", txtDomisili.Text);
                    insertCommand.Parameters.AddWithValue("@Age", txtAge.Text);
                    insertCommand.Parameters.AddWithValue("@nohp", txtNoHp.Text);
                    insertCommand.Parameters.AddWithValue("@email", txtEmail.Text);
                    insertCommand.Parameters.AddWithValue("@password", txtPassword.Password);
                    insertCommand.Parameters.AddWithValue("@confirmpass", txtConfirm.Password);
                    insertCommand.Parameters.AddWithValue("@Role", txtRole.Text);
                    string password = txtPassword.Password;
                    string confirmPass = txtConfirm.Password;

                    if (password != confirmPass)
                    {
                        // Konfirmasi kata sandi tidak cocok, lakukan penanganan kesalahan yang sesuai
                        MessageBox.Show("Konfirmasi kata sandi tidak cocok.", "Kesalahan", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data berhasil disimpan.");

                        // Mengambil kembali data dari tabel tbllog
                        string selectQuery = "SELECT * FROM uadb.tbllog";
                        MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectQuery, connection);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        // Mengatur ulang sumber data data grid
                        DataGrid.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        MessageBox.Show("Username atau email sudah digunakan. Silakan gunakan username dan email lain.", "Kesalahan", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            txtFullname.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtDomisili.Text = string.Empty;
            txtAge.Text = string.Empty;
            txtNoHp.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPassword.Password = string.Empty;
            txtConfirm.Password = string.Empty;
            txtRole.Text = string.Empty;
            txtID.Text = string.Empty;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)DataGrid.SelectedItem;
                int selectedId = Convert.ToInt32(selectedRow["id"]);

                string connectionString = "server=localhost;user=root;database=uadb;password=";
                string deleteQuery = "DELETE FROM uadb.tbllog WHERE id = @id";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@id", selectedId);
                        deleteCommand.ExecuteNonQuery();

                        MessageBox.Show("Data berhasil dihapus.");

                        // Memperbarui data grid setelah penghapusan
                        string selectQuery = "SELECT * FROM uadb.tbllog";
                        MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectQuery, connection);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        DataGrid.ItemsSource = dataTable.DefaultView;
                        DataGrid_Loaded(null, null);
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih baris yang akan dihapus.", "Kesalahan", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)DataGrid.SelectedItem;

                int id = Convert.ToInt32(selectedRow["id"]);
                string username = selectedRow["username"].ToString();
                string fullname = selectedRow["fullname"].ToString();
                string domisili = selectedRow["domisili"].ToString();
                string age = selectedRow["age"].ToString();
                string nohp = selectedRow["nohp"].ToString();
                string email = selectedRow["email"].ToString();
                string password = selectedRow["password"].ToString();
                string confirmpass = selectedRow["confirmpass"].ToString();
                string Role = selectedRow["Role"].ToString();


                string newFullname = txtFullname.Text;
                string newUsername = txtUsername.Text;
                string newDomisili = txtDomisili.Text;
                string newAge = txtAge.Text;
                string newNohp = txtNoHp.Text;
                string newEmail = txtEmail.Text;
                string newPassword = txtPassword.Password;
                string newConfirmpass = txtConfirm.Password;
                string newRole = txtRole.Text;

                if (fullname == newFullname && username == newUsername && domisili == newDomisili && age == newAge && nohp == newNohp && email == newEmail && password == newPassword && confirmpass == newConfirmpass && Role == newRole)
                {
                    MessageBox.Show("Tidak ada perubahan pada data.");
                    return;
                }

                string connectionString = "server=localhost;user=root;database=uadb;password=";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        string updateQuery = "UPDATE tbllog SET fullname = @newFullname, username = @newUsername, domisili = @newDomisili, age = @newAge, nohp = @newNohp, email = @newEmail, password = @newPassword, confirmpass = @newConfirmpass, Role = @newRole WHERE id = @id";
                        MySqlCommand command = new MySqlCommand(updateQuery, connection);
                        command.Parameters.AddWithValue("@newFullname", newFullname);
                        command.Parameters.AddWithValue("@newUsername", newUsername);
                        command.Parameters.AddWithValue("@newDomisili", newDomisili);
                        command.Parameters.AddWithValue("@newAge", newAge);
                        command.Parameters.AddWithValue("@newNohp", newNohp);
                        command.Parameters.AddWithValue("@newEmail", newEmail);
                        command.Parameters.AddWithValue("@newPassword", newPassword);
                        command.Parameters.AddWithValue("@newConfirmpass", newConfirmpass);
                        command.Parameters.AddWithValue("@newRole", newRole);

                        command.Parameters.AddWithValue("@id", id);


                        command.ExecuteNonQuery();

                        MessageBox.Show("Data updated!");

                        selectedRow["fullname"] = newFullname;
                        selectedRow["username"] = newUsername;
                        selectedRow["domisili"] = newDomisili;
                        selectedRow["age"] = newAge;
                        selectedRow["nohp"] = newNohp;
                        selectedRow["email"] = newEmail;
                        selectedRow["password"] = newPassword;
                        selectedRow["confirmpass"] = newConfirmpass;
                        selectedRow["Role"] = newRole;


                        string selectQuery = "SELECT * FROM tbllog";
                        MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectQuery, connection);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        DataGrid.ItemsSource = dataTable.DefaultView;
                        DataGrid_Loaded(null, null);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while updating the record: " + ex.Message, "ERROR");
                    }
                }
            }
            else
            {
                MessageBox.Show("Select the record you want to update", "ERROR");
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
                        command.CommandText = "SELECT * FROM tbllog WHERE id LIKE @keyword OR fullname LIKE @keyword OR username LIKE @keyword OR domisili LIKE @keyword OR age LIKE @keyword OR nohp LIKE @keyword OR email LIKE @keyword OR password LIKE @keyword OR confirmpass LIKE @keyword OR role LIKE @keyword";
                        command.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataTable dataTable = new DataTable();

                        adapter.Fill(dataTable);

                        // Update the DataGrid's ItemsSource with the search results
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
