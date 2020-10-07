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

namespace KeyList
{
    /// <summary>
    /// Interaction logic for AddComputer.xaml
    /// </summary>
    public partial class AddComputer : Window
    {
        public string s = "";
        public AddComputer()
        {
            InitializeComponent();
            Computer c = MainWindow.sql.getComputer(MainWindow.sql.getLastComputerID());
            if (c != null)
            {
                tbBrand.Text = c.Brand;
                tbModel.Text = c.Model;
            }
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Close();
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            string brand = tbBrand.Text;
            string model = tbModel.Text;
            string serial = tbSerielnumber.Text;
            string smartwater = tbSmartwater.Text;


            bool check = MainWindow.sql.addComputer(brand, model, serial, smartwater);
            if (check)
                s = serial;
            Close();
        }
    }
}
