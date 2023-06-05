﻿using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace BanyumasScene
{
    /// <summary>
    /// Interaction logic for uslogn.xaml
    /// </summary>
    public partial class uslogn : Window
    {
        public uslogn()
        {
            InitializeComponent();
            
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
           
        }


        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Please input Username and Password", "Error");
                return;
            }

            if (txtPassword.Password != txtConfirmpass.Password)
            {
                MessageBox.Show("Password and Confirm Password do not match", "Error");
                return;
            }

            using (var connection = new MySqlConnection("datasource=localhost;port=3306;username=root;password="))
            {
                try
                {
                    connection.Open();
                    string selectQuery = "SELECT * FROM uadb.tbllog WHERE Username = @username AND Password = @password;";
                    var command = new MySqlCommand(selectQuery, connection);
                    command.Parameters.AddWithValue("@username", txtUsername.Text);
                    command.Parameters.AddWithValue("@password", txtPassword.Password);
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string Role = reader["Role"].ToString();

                        if (Role != "Admin")
                        {
                            User.type = "User";

                            User.Username = txtUsername.Text;
                            MessageBox.Show("Login Successful!");

                            reader.Close();

                            string updateLastLoginQuery = "UPDATE uadb.tbllog SET lastlogin = CURRENT_TIMESTAMP WHERE username = @username";
                            MySqlCommand updateCommand = new MySqlCommand(updateLastLoginQuery, connection);
                            updateCommand.Parameters.AddWithValue("@username", txtUsername.Text);
                            updateCommand.ExecuteNonQuery();

                            string selectFullnameQuery = "SELECT fullname FROM uadb.tbllog WHERE username = @username";
                            MySqlCommand fullnameCommand = new MySqlCommand(selectFullnameQuery, connection);
                            fullnameCommand.Parameters.AddWithValue("@username", txtUsername.Text);
                            var fullnameReader = fullnameCommand.ExecuteReader();

                            string fullname = string.Empty;
                            if (fullnameReader.Read())
                            {
                                fullname = fullnameReader.GetString("fullname");
                            }

                            fullnameReader.Close();

                            Navbar navbar = new Navbar(fullname);
                            navbar.Show();
                            this.Close();
                        }
                        else if (Role == "Admin")
                        {
                            User.type = "Admin";

                            MessageBox.Show("Login failed! Admin login is not allowed.", "Error");
                        }
                        else
                        {
                            MessageBox.Show("Incorrect Login Information! Try again.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect Login Information! Try again.");
                    }
                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("An error occurred while connecting to the database: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }


        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Usign usign = new Usign();
            usign.Show();
            this.Close();
        }
    }

    
}