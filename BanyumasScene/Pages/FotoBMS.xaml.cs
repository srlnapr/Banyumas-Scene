using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BanyumasScene; 
using MySql.Data.MySqlClient;

namespace BanyumasScene.Pages
{
    public partial class FotoBMS : Page
    {
        public FotoBMS()
        {
            InitializeComponent();
            LoadRatingsFromDatabase();

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (User.type == "Admin")
            {
                Rating selectedRating = DataGrid.SelectedItem as Rating;

                if (selectedRating != null)
                {
                    DeleteRatingFromDatabase(selectedRating.Id);

                    LoadRatingsFromDatabase();
                }
                else
                {
                    MessageBox.Show("Please select a rating to delete.");
                }
            }
            else
            {
                MessageBox.Show("Only admin can delete ratings.");
            }
        }

        private const string ConnectionString = "server=localhost;user=root;database=uadb;password=";

        private void LoadRatingsFromDatabase()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    string query = "SELECT id, comment, rating FROM tblrating";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    connection.Open();

                    List<Rating> ratings = new List<Rating>();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string comment = reader.GetString("comment");
                            int rating = reader.GetInt32("rating");

                            ratings.Add(new Rating { Id = id, Comment = comment, RatingValue = rating });
                        }
                    }

                    DataGrid.ItemsSource = ratings;
                }

                // Setelah memuat data dari database, perbarui rata-rata rating
                UpdateAverageRating();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }



        private void UpdateAverageRating()
        {
            double averageRating = CalculateAverageRating();
            txtAverage.Text = averageRating.ToString();
        }

        private double CalculateAverageRating()
        {
            double totalRating = 0;
            int count = 0;

            foreach (var item in DataGrid.Items)
            {
                if (item is Rating rating)
                {
                    totalRating += rating.RatingValue;
                    count++;
                }
            }

            if (count > 0)
            {
                return totalRating / count;
            }
            else
            {
                return 0;
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    string query = "INSERT INTO tblrating (comment, rating) VALUES (@comment, @rating)";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@comment", txtComment.Text);
                    command.Parameters.AddWithValue("@rating", Convert.ToInt32(txtRating.Text));

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                LoadRatingsFromDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting data: " + ex.Message);
            }
        }

     
        private void DeleteRatingFromDatabase(int ratingId)
        {
            string connectionString = "server=localhost;user=root;database=uadb;password=";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "DELETE FROM tblrating WHERE id = @id";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", ratingId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void txtAverage_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }

        private void txtRating_PreviewTextInput_1(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

            // Hanya izinkan karakter angka
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }

            // Batasi maksimal nilai 10
            int rating;
            if (int.TryParse(txtRating.Text + e.Text, out rating) && rating > 10)
            {
                e.Handled = true;
            }
            
        }
    }
}