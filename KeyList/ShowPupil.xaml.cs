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
    /// Interaction logic for ShowPupil.xaml
    /// </summary>
    public partial class ShowPupil : Window
    {
        public ShowPupil()
        {
            InitializeComponent();
            InitializeComponent();
            listView.ItemsSource = MainWindow.sql.getUnAssignedPupulList();
            // sort newest up
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ADD_Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddPupil();

            window.ShowDialog();

            listView.ItemsSource = MainWindow.sql.getUnAssignedPupulList();
            ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Refresh();
            //refresh list

        }
        private void REMOVE_Button_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex != -1)
            {
                MyItem m = (MyItem)listView.SelectedItem;
                MessageBoxResult result = MessageBox.Show("Remove Pupil " + m.P.Firstname + " " + m.P.Lastname + "?", "My App", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {

                    MainWindow.sql.removePupil(m.P.Id);

                    listView.ItemsSource = MainWindow.sql.getUnAssignedPupulList();
                    ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                    view.Refresh();
                }
            }

        }
    }
}
