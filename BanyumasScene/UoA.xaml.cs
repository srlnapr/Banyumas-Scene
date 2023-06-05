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
    /// Interaction logic for UoA.xaml
    /// </summary>
    public partial class UoA : Window
    {
        public UoA()
        {
            InitializeComponent();
        }

        private void btnUser_Click(object sender, RoutedEventArgs e)
        {
            Usign usign = new Usign();
            usign.Show(); // Assuming 'Show' method exists in the 'Usign' class
            this.Close();

        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            admlogn navbar = new admlogn();
            navbar.Show();
            this.Close();
        }
    }
}
