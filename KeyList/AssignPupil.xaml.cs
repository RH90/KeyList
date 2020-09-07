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
    /// Interaction logic for AssignPupil.xaml
    /// </summary>
    public partial class AssignPupil : Window
    {
        public int pupilID = -1;
        public AssignPupil(int locker)
        {

            InitializeComponent();
            Title = "Ge skåp nr." + locker;
            listView.ItemsSource = MainWindow.sql.getUnAssignedPupulList();

            if (listView.Items.Count > 0)
            {
                listView.SelectedIndex = 0;
            }
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedIndex != -1)
            {
                MyItem m = (MyItem)listView.SelectedItem;
                pupilID = m.P.Id;
            }
        }
        public void listView_MouseClick(object sender, MouseButtonEventArgs e)
        {



        }
        private void bAccept_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex != -1 && pupilID != -1)
            {
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
    }
}
