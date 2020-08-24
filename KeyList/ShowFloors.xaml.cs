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


            addToolTip(tab1);
            addToolTip(tab2);
            addToolTip(tab3);

            //for (int i = 0; i < test.Children.Count; i++)
            //{
            //    if (test.Children[i] is Border)
            //    {
            //        try
            //        {
            //            Border b = (Border)test.Children[i];
            //            TextBlock t = (TextBlock)b.Child;
            //            string bindingText = t.GetBindingExpression(TextBlock.BackgroundProperty).ParentBinding.Path.Path;
            //            if (!bindingText.Contains(t.Text))
            //                Console.WriteLine("Error: " + t.Text);
            //        }
            //        catch
            //        { }

            //    }
            //}

        }

        private void addToolTip(UniformGrid tab)
        {
            for (int i = 0; i < tab.Children.Count; i++)
            {
                if (tab.Children[i] is Border)
                {
                    try
                    {
                        Border b = (Border)tab.Children[i];
                        TextBlock t = (TextBlock)b.Child;
                        Locker l = MainWindow.sql.getLocker(int.Parse(t.Text));

                        //MainWindow.sql.getPupil();
                        if (l != null)
                        {
                            Pupil p = MainWindow.sql.getPupil(l.Owner_id);


                            ToolTipService.SetShowDuration(t, int.MaxValue);
                            ToolTipService.SetInitialShowDelay(t, 0);
                            t.ToolTip =
                              "==Keys==\n" + l.Keys
                            + "\n==Comment==\n" + l.Comment
                            + "\n==Status==\n" + l.StatusText;

                            if (p != null)
                            {
                                t.ToolTip +=
                                    "\n==Pupil==\n" + p.Firstname + " " + p.Lastname + ", " + p.Grade + p.Class + ", " + p.Year;
                            }
                        }

                    }

                    catch
                    { }

                }
            }
        }

        public ArrayList Text { get; set; }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
