using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for AssignComputer.xaml
    /// </summary>
    public partial class AssignComputer : Window
    {
        public string s = "";
        public AssignComputer()
        {
            InitializeComponent();
            listView.ItemsSource = MainWindow.sql.getUnAssignedComputerList("");
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex != -1 )
            {
                MyItem m = (MyItem)listView.SelectedItem;
                s = m.C.Serielnumber;
                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
            }

            this.Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void bSearch_Click(object sender, RoutedEventArgs e)
        {
            listView.ItemsSource = MainWindow.sql.getUnAssignedComputerList(tbSearch.Text);
            ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Refresh();
        }
    }
}
