using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
                Text.Add("Cyan");
            }
            List<MyItem> list = MainWindow.sql.getAllLockers("", "", "", "", "");
            for (int i = 0; i < list.Count; i++)
            {
                MyItem m = list[i];
                Text[m.L.Number] = m.L.StatusColor;
            }

            InitializeComponent();

            InitTabs(tab1);
            InitTabs(tab2);
            InitTabs(tab3);

        }

        private void InitTabs(UniformGrid ug)
        {
            for (int i = 0; i < ug.Children.Count; i++)
            {
                if (ug.Children[i].GetType() == typeof(Border))
                {
                    try
                    {
                        Border b = (Border)ug.Children[i];
                        TextBlock tb = (TextBlock)b.Child;

                        tb.FontWeight = FontWeights.Bold;
                        Locker l = MainWindow.sql.getLocker(int.Parse(tb.Text));
                        if (l != null)
                        {
                            if (l.Keys > 2)
                            {
                                tb.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xAA));
                            }
                            else if (l.Keys <= 1)
                            {
                                tb.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0x00, 0x00));
                            }


                            //Tooltip
                            Pupil p = MainWindow.sql.getPupil(l.Owner_id);
                            ToolTipService.SetShowDuration(tb, int.MaxValue);
                            ToolTipService.SetInitialShowDelay(tb, 0);
                            tb.ToolTip =
                              "==Nycklar==\n" + l.Keys
                            + "\n==Historia==\n" + l.HistoryShort
                            + "\n==Status==\n" + l.StatusText;

                            if (p != null)
                            {
                                tb.ToolTip +=
                                    "\n==Elev==\n" + p.Firstname + " " + p.Lastname + ", " + p.Grade + p.Class + ", " + p.Year;
                            }
                        }
                    }
                    catch { }


                }
            }
        }

        public ArrayList Text { get; set; }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
