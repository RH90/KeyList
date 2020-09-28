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
    /// Interaction logic for ShowComputers.xaml
    /// </summary>
    public partial class ShowComputers : Window
    {
        public ShowComputers()
        {
            InitializeComponent();
        }

        private void ADD_Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddComputer();
            window.ShowDialog();
        }

        private void REMOVE_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
