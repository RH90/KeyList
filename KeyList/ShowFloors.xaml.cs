using System;
using System.Collections;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ShowFloors : Window
    {
        public ShowFloors()
        {
            Text = new ArrayList();
            for (int i = 0; i < 500; i++)
            {
                Text.Add("");
            }
            List<MyItem> list = MainWindow.sql.getAllLockers("", "", "", "", "");

            for (int i = 0; i < list.Count; i++)
            {
                MyItem m = list[i];
                Text[m.L.Number] = m.L.StatusColor;
            }

            InitializeComponent();
        }
        public ArrayList Text { get; set; }
    }

}
