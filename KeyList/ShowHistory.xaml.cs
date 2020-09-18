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
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class ShowHistory : Window
    {
        public ShowHistory()
        {
            InitializeComponent();

            listView.ItemsSource = MainWindow.sql.GetHistory();

            ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Refresh();

        }
    }
}
