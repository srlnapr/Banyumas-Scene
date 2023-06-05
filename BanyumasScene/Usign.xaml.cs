using MySql.Data.MySqlClient;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BanyumasScene
{
    /// <summary>
    /// Interaction logic for Usign.xaml
    /// </summary>
    public partial class Usign : Window
    {
        MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=");
        MySqlCommand command;
        MySqlDataReader mdr;
        public Usign()
        {
            InitializeComponent();
        }

        private void btnSignup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Please fill in all information", "Error");
                return;
            }

            using (var connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password="))
            {
                try
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM uadb.tbllog WHERE Username = @username;";
                    var command = new MySqlCommand(selectQuery, connection);
                    command.Parameters.AddWithValue("@username", txtUsername.Text);
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        MessageBox.Show("Username is not available!");
                        reader.Close();
                    }
                    else
                    {
                        reader.Close();

                        string insertQuery = "INSERT INTO uadb.tbllog (fullname, username, domisili, age, nohp, email, password, confirmpass, Role) " +
                                             "VALUES (@Fullname, @username, @domisili, @Age, @nohp, @email, @password, @confirmpass, @Role)";

                        var insertCommand = new MySqlCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@Fullname", txtFullname.Text);
                        insertCommand.Parameters.AddWithValue("@username", txtUsername.Text);
                        insertCommand.Parameters.AddWithValue("@domisili", txtDomisili.Text);
                        insertCommand.Parameters.AddWithValue("@Age", txtAge.Text);
                        insertCommand.Parameters.AddWithValue("@nohp", txtNoHp.Text);
                        insertCommand.Parameters.AddWithValue("@email", txtEmail.Text);
                        insertCommand.Parameters.AddWithValue("@password", txtPassword.Password);
                        insertCommand.Parameters.AddWithValue("@confirmpass", txtConfirm.Password);
                        insertCommand.Parameters.AddWithValue("@Role", string.IsNullOrEmpty(txtRole.Text) ? "User" : txtRole.Text);

                        string password = txtPassword.Password;
                        string confirmPass = txtConfirm.Password;

                        if (password != confirmPass)
                        {
                            MessageBox.Show("Password confirmation does not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        insertCommand.ExecuteNonQuery();

                        MessageBox.Show("Account successfully created!");
                        uslogn uslogn = new uslogn();
                        uslogn.Show();
                        this.Close();
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("An error occurred while connecting to the database: " + ex.Message);
                }
            }
        }


        private void btnClear_Click(object sender, RoutedEventArgs e)
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
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            uslogn Uslogn = new uslogn();
            Uslogn.Show();
            this.Close();
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {

        }
    }
}
