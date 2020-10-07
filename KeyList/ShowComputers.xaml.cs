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
    /// Interaction logic for ShowComputers.xaml
    /// </summary>
    public partial class ShowComputers : Window
    {
        public ShowComputers()
        {
            InitializeComponent();
            listView.ItemsSource = MainWindow.sql.getUnAssignedComputerList("");
        }

        private void ADD_Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddComputer();

            window.ShowDialog();

            listView.ItemsSource = MainWindow.sql.getUnAssignedComputerList("");
            ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Refresh();
        }

        private void REMOVE_Button_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex != -1)
            {
                MyItem m = (MyItem)listView.SelectedItem;
                MessageBoxResult result = MessageBox.Show("Ta bort dator: " + m.C.Model + ", " + m.C.Serielnumber + "?", "Ta Bort", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {

                    MainWindow.sql.removeComputer(m.C.Id);

                    listView.ItemsSource = MainWindow.sql.getUnAssignedComputerList("");
                    ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                    view.Refresh();
                }
            }
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
