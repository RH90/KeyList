using KeyList;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace KeyList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    // ** TO DO **
    // global add student(buttons above tabControl)
    // tab item for just pupils?
    // History for owner_id for locker
    // batch operations (remove,add pupils to lockers)
    // auto up grade on students? 
    // multi satus
    // go to item on letter click sensitive to column

    // Datorer
    // list of students 
    // columns: serial, type, computer comment, pupil comment(history of computers)
    // Add computers

    // give student temp removed status when graduated or removed early
    // remove history
    //replace comment with history

    public partial class MainWindow : Window
    {
        String currentSort = "";
        bool ascending = true;
        int selected = -2;
        ShowFloors showFloorWindow = null;
        public string dir;
        public static SQL sql;
        public List<GridViewColumn> listGVC = new List<GridViewColumn>();

        public MainWindow()
        {
            InitializeComponent();

            //gridView.Columns.Clear();
            //gridView.Columns.Add(GetColumn("Firstname", new SolidColorBrush(Color.FromArgb(0x25, 0x25, 0xDD, 0x55)), "P.Firstname", "", ""));
            //gridView.Columns.Add(GetColumn("Lastname", new SolidColorBrush(Color.FromArgb(0x25, 0x25, 0xDD, 0x55)), "P.Lastname", "", ""));
            //gridView.Columns.Add(GetColumn("Klass", new SolidColorBrush(Color.FromArgb(0x25, 0x25, 0xDD, 0x55)), "P.GradeClass", "", ""));
            //gridView.Columns.Add(GetColumn("Start", new SolidColorBrush(Color.FromArgb(0x25, 0x25, 0xDD, 0x55)), "P.Year", "", ""));

            //gridView.Columns.Add(GetColumn("Skåp", new SolidColorBrush(Color.FromArgb(0x25, 0xDD, 0x25, 0x25)), "L.Number", "", ""));
            //gridView.Columns.Add(GetColumn("Plan", new SolidColorBrush(Color.FromArgb(0x25, 0xDD, 0x25, 0x25)), "L.Floor", "", ""));
            //gridView.Columns.Add(GetColumn("Status", new SolidColorBrush(Color.FromArgb(0x25, 0xDD, 0x25, 0x25)), "L.StatusText", "", "L.StatusColor"));
            //gridView.Columns.Add(GetColumn("Nycklar", new SolidColorBrush(Color.FromArgb(0x25, 0xDD, 0x25, 0x25)), "L.Keys", "", ""));

            //gridView.Columns.Add(GetColumn("Kommentar skåp", new SolidColorBrush(Color.FromArgb(0x25, 0x25, 0x25, 0xDD)), "L.CommentShort", "359", ""));
            //gridView.Columns.Add(GetColumn("Kommentar Elev", new SolidColorBrush(Color.FromArgb(0x25, 0x55, 0x25, 0xFF)), "P.CommentShort", "275", ""));

            //gridView.Columns.Move(3, 0);

            //var path = @"C:\Users\rilhas\Desktop\chrome.csv"; // Habeeb, "Dubai Media City, Dubai"
            //                                                  //using (TextFieldParser csvParser = new TextFieldParser(path))
            //                                                  //{

            //string text = File.ReadAllText(path);

            //int a = 0;
            //for (int i = 0; i < text.Length; i++)
            //{

            //}
            //    csvParser.CommentTokens = new string[] { "\"" };
            //    csvParser.SetDelimiters(new string[] { "," });
            //    csvParser.HasFieldsEnclosedInQuotes = false;


            //    // Skip the row with the column names
            //    csvParser.ReadLine();

            //    while (!csvParser.EndOfData)
            //    {
            //        // Read current line fields, pointer moves to the next line.
            //        string[] fields = csvParser.ReadFields();
            //        for (int i = 0; i < fields.Length; i++)
            //        {
            //            if (i < 5)
            //                Console.Write(fields[i] + "\t\t");

            //        }
            //        Console.WriteLine();
            //    }
            //}




            string[] args = Environment.GetCommandLineArgs();
            Console.WriteLine(args.Length);


            if (args.Length > 1 && Directory.Exists(args[1]))
            {
                Console.WriteLine(args[1]);
                sql = new SQL(args[1] + "\\skåp.db");
                dir = (args[1] + "\\");
            }
            else
            {
                sql = new SQL("skåp.db");
                dir = "";
            }

            listView.ItemsSource = (sql.getAllLockers("", "", "", "", ""));

            Console.WriteLine(listView.Items.Count);

            listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridViewColumn_Click));
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("L.Number", ListSortDirection.Ascending));

            bSave.IsEnabledChanged += BSave_IsEnabledChanged;
            view.Refresh();

            gbPupil.IsEnabledChanged += Is_GbPupil_Enabled;

        }

        public void InitLockerColumns()
        {

        }
        public GridViewColumn GetColumn(string header, SolidColorBrush background, string binding, string width, string foregroundBinding, TextAlignment textAlignment)
        {
            GridViewColumn gvc = new GridViewColumn();
            gvc.Header = header;

            var tb = new FrameworkElementFactory(typeof(TextBlock));
            tb.SetValue(TextBlock.MarginProperty, new Thickness(-6, 0, -6, 0));
            tb.SetValue(TextBlock.PaddingProperty, new Thickness(5, 0, 5, 0));
            tb.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            tb.SetValue(TextBlock.BackgroundProperty, background);
            tb.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            tb.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            tb.SetValue(TextBlock.TextAlignmentProperty, textAlignment);

            if (width != "")
                gvc.SetValue(WidthProperty, Double.Parse(width));

            Binding bText = new Binding();
            bText.Path = new PropertyPath(binding);

            if (foregroundBinding != "")
            {
                Binding bForeground = new Binding();
                bForeground.Path = new PropertyPath(foregroundBinding);
                tb.SetBinding(TextBlock.ForegroundProperty, bForeground);
            }


            Binding bHeight = new Binding();
            bHeight.Path = new PropertyPath("ActualHeight");
            RelativeSource rsH = new RelativeSource();
            rsH.Mode = RelativeSourceMode.FindAncestor;
            rsH.AncestorType = typeof(ListViewItem);
            rsH.AncestorLevel = 1;
            bHeight.RelativeSource = rsH;

            tb.SetBinding(TextBlock.TextProperty, bText);
            tb.SetBinding(TextBlock.HeightProperty, bHeight);


            DataTemplate dt = new DataTemplate() { VisualTree = tb };

            gvc.CellTemplate = dt;


            return gvc;
        }

        public void Diff_Button_Click(object sender, RoutedEventArgs e)
        {
            string path = dir + "diff.txt";
            if (File.Exists(path))
            {
                List<MyItem> list = new List<MyItem>();
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    MyItem m = (MyItem)listView.Items[i];
                    if (m.L.Owner_id != -1)
                    {
                        bool check = checkPupil(m.P.Firstname, m.P.Lastname, lines);
                        if (!check)
                        {
                            list.Add(m);
                        }
                    }
                }
                listView.ItemsSource = list;
            }
        }
        //compare pupil on current list with pupil in diff.txt
        public bool checkPupil(string firstname, string lastname, string[] lines)
        {
            bool check = false;

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace(", ", ",");
                string[] parts = lines[i].Split(',');
                if (parts.Length < 2)
                {
                    continue;
                }
                string firstnameL = parts[1];
                string lastnameL = parts[0];

                if (firstname.ToLower().Equals(firstnameL.ToLower()) && lastname.ToLower().Equals(lastnameL.ToLower()))
                {
                    return true;
                }

            }
            return check;

        }
        private void BSave_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            if (bSave.IsEnabled == true)
            {
                bSave.Background = Brushes.LightCoral;
            }
            else
            {
                bSave.Background = Brushes.Transparent;
            }
        }

        private void importXLSX(SQL sql)
        {
            string filePath = @"C:\Users\rilhas\Desktop\test.xlsx";
            OleDbConnectionStringBuilder sbConnection = new OleDbConnectionStringBuilder();
            String strExtendedProperties = String.Empty;
            sbConnection.DataSource = filePath;

            sbConnection.Provider = "Microsoft.ACE.OLEDB.12.0";
            strExtendedProperties = "Excel 12.0;HDR=Yes;IMEX=1";

            sbConnection.Add("Extended Properties", strExtendedProperties);


            var adapter = new OleDbDataAdapter("SELECT * FROM [Skåp-info -7$]", sbConnection.ToString());
            var ds = new DataSet();

            adapter.Fill(ds, "test");

            DataTable data = ds.Tables["test"];
            foreach (DataColumn column in data.Columns)
            {

                Console.WriteLine(column.ColumnName);

            }
            Console.WriteLine(data.Rows.Count);

            for (int i = 0; i < data.Rows.Count; i++)
            {


                var FirstName = data.Rows[i].Field<object>("Förnamn");
                var LastName = data.Rows[i].Field<object>("Efternamn");
                var Year = data.Rows[i].Field<object>("ÅR");
                var Keys = data.Rows[i].Field<object>("ANTAL _NYCKLAR");
                var Status = data.Rows[i].Field<object>("STATUS");
                var Locker = data.Rows[i].Field<object>("SKÅP");
                var Grade = data.Rows[i].Field<object>("ÅK");
                var Class = data.Rows[i].Field<object>("Klass");
                var CommentLocker = data.Rows[i].Field<object>("Kommentar");
                // Console.WriteLine(Class
                if (Class != null)
                {
                    Class = Regex.Replace(Class.ToString(), "[0-9]", "");
                }
                long id = -1;
                if (FirstName != null)
                {
                    id = sql.addPupil(FirstName.ToString(), LastName.ToString(), Class.ToString(), Grade.ToString(), Year.ToString());
                }
                if (Status == null)
                {
                    Status = "";
                }

                if (Keys == null)
                {
                    Keys = "";
                }
                if (CommentLocker == null)
                {
                    CommentLocker = "";
                }
                if (Locker != null)
                {
                    sql.addLocker(Locker.ToString(), Status.ToString(), Keys.ToString(), CommentLocker.ToString(), id);
                }


                //listView.Items.Add(new MyItem { FirstName = FirstName, LastName = LastName, Year = Year, Keys = Keys, Status = Status, Locker = Locker, Grade = Grade, Class = Class, CommentLocker = CommentLocker });
            }


        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("test");
            bSave.IsEnabled = false;
            if (listView.SelectedIndex == -1)
            {
                spEdit.IsEnabled = false;
                return;
            }
            spEdit.IsEnabled = true;

            Console.WriteLine(listView.SelectedIndex);

            MyItem m = (MyItem)listView.SelectedItem;

            if (m.P.Id == -1)
            {
                gbPupil.IsEnabled = false;
            }
            else
            {
                gbPupil.IsEnabled = true;
            }

            UpdateTextBoxes(m);

        }

        private void UpdateTextBoxes(MyItem m)
        {
            tbLockerNumber.Text = String.Format("Nummer:{0,4}", m.L.Number);
            tbKeys.Text = "Nycklar:      " + m.L.Keys;
            //tbLockerComment.Text = m.L.Comment + "";
            cbStatus.SelectedIndex = m.L.Status;

            dateLocker.SelectedDate = DateTime.Now;
            tbLockerHistory.Text = "";

            listLockerHistory.ItemsSource = sql.GetHistory(1, m.L.Id);
            CollectionView viewLocker = (CollectionView)CollectionViewSource.GetDefaultView(listLockerHistory.ItemsSource);
            viewLocker.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            viewLocker.Refresh();

            tbPupilFirstname.Text = m.P.Firstname + "";
            tbPupilLastName.Text = m.P.Lastname + "";
            tbPupilGrade.Text = m.P.Grade + "";
            tbPupilClass.Text = m.P.Class + "";
            // tbPupilComment.Text = m.P.Comment;

            tbPupilHistory.Text = "";

            if (m.P.Id != -1)
            {
                listPupilHistory.ItemsSource = sql.GetHistory(0, m.P.Id);
                datePupil.SelectedDate = DateTime.Now;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listPupilHistory.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
                view.Refresh();
            }


        }

        private List<String> GetExcelSheetNames(string filePath)
        {
            OleDbConnectionStringBuilder sbConnection = new OleDbConnectionStringBuilder();
            String strExtendedProperties = String.Empty;
            sbConnection.DataSource = filePath;

            sbConnection.Provider = "Microsoft.ACE.OLEDB.12.0";
            strExtendedProperties = "Excel 12.0;HDR=Yes;IMEX=1";

            sbConnection.Add("Extended Properties", strExtendedProperties);
            List<string> listSheet = new List<string>();
            using (OleDbConnection conn = new OleDbConnection(sbConnection.ToString()))
            {
                conn.Open();
                DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow drSheet in dtSheet.Rows)
                {
                    if (drSheet["TABLE_NAME"].ToString().Contains("$"))//checks whether row contains '_xlnm#_FilterDatabase' or sheet name(i.e. sheet name always ends with $ sign)
                    {
                        listSheet.Add(drSheet["TABLE_NAME"].ToString());
                    }
                }
            }
            return listSheet;
        }

        private void GridViewColumn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GridViewColumnHeader gc = (GridViewColumnHeader)e.OriginalSource;
                Console.WriteLine(gc.Content);

                string col = "";
                switch (gc.Content + "")
                {
                    case "Skåp":
                        col = "L.Number";
                        break;
                    case "Nycklar":
                        col = "L.Keys";
                        break;
                    case "Plan":
                        col = "L.Floor";
                        break;
                    case "Status":
                        col = "L.Status";
                        break;
                    case "Förnamn":
                        col = "P.Firstname";
                        break;
                    case "Efternamn":
                        col = "P.Lastname";
                        break;
                    case "Klass":
                        col = "P.GradeClass";
                        break;
                    default:
                        return;

                }


                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view.Culture = new CultureInfo("sv-SE");

                view.SortDescriptions.Clear();

                if (col == "P.Lastname")
                {
                    view.SortDescriptions.Add(new SortDescription("P.Lastname", ListSortDirection.Ascending));
                    view.SortDescriptions.Add(new SortDescription("P.Firstname", ListSortDirection.Ascending));
                }
                else if (col == "P.GradeClass")
                {
                    view.SortDescriptions.Add(new SortDescription("P.Grade", ListSortDirection.Ascending));
                    view.SortDescriptions.Add(new SortDescription("P.Class", ListSortDirection.Ascending));
                    view.SortDescriptions.Add(new SortDescription("P.Lastname", ListSortDirection.Ascending));
                    view.SortDescriptions.Add(new SortDescription("P.Firstname", ListSortDirection.Ascending));
                    view.SortDescriptions.Add(new SortDescription("L.Status", ListSortDirection.Ascending));
                    view.SortDescriptions.Add(new SortDescription("L.Number", ListSortDirection.Ascending));
                }
                else if (currentSort == col)
                {
                    if (ascending)
                    {
                        view.SortDescriptions.Add(new SortDescription(col, ListSortDirection.Descending));
                    }
                    else
                    {
                        view.SortDescriptions.Add(new SortDescription(col, ListSortDirection.Ascending));
                    }
                    ascending = !ascending;

                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription(col, ListSortDirection.Ascending));
                }


                currentSort = col;

            }
            catch
            {

            }

        }

        public void listView_MouseClick(object sender, MouseButtonEventArgs e)
        {

            try
            {

                ListViewItem m = (ListViewItem)sender;
                if (m.IsSelected)
                {
                    return;
                }
                Console.WriteLine("geggegcc");
                listView.SelectedItems.Clear();
                //listView.SelectedItem = -1;

                Console.WriteLine(m.IsSelected);

                Console.WriteLine(sender);

                m.IsSelected = true;
            }
            catch
            {

            }

        }

        private void TextBox_MouseClick(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine("eggee");
            //TextBox tb = (TextBox)sender;
            //Console.WriteLine(tb.TemplatedParent);

            //Console.WriteLine(tb.Parent);
        }

        private void bSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            string status = "", grade = "", floor = "", classP = "";

            if (cbsGrade.SelectedIndex != 0)
            {
                grade = ((ComboBoxItem)cbsGrade.SelectedItem).Content + "";
            }
            if (cbsFloor.SelectedIndex != 0)
            {
                floor = ((ComboBoxItem)cbsFloor.SelectedItem).Content + "";
            }
            if (cbsClass.SelectedIndex != 0)
            {
                classP = ((ComboBoxItem)cbsClass.SelectedItem).Content + "";
            }
            if (cbsStatus.SelectedIndex != 0)
            {
                status = (cbsStatus.SelectedIndex - 1) + "";
            }
            listView.ItemsSource = (sql.getAllLockers(tbSearch.Text, grade, classP, floor, status));
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Culture = new CultureInfo("sv-SE");
            view.SortDescriptions.Add(new SortDescription("L.Number", ListSortDirection.Ascending));
            currentSort = "L.Number";

            //listView.UpdateLayout();

            //System.GC.Collect();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbsStatus is null || tbSearch is null)
            {
                return;
            }
            Search();

        }

        private void Show_Pupil_Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new ShowPupil();
            window.ShowDialog();
            Console.WriteLine("hallow");
        }
        private void Is_GbPupil_Enabled(object sender, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine("Is_GbPupil_Enabled: " + e.NewValue);
            if (((bool)e.NewValue) == false)
            {
                bRemovePupil.IsEnabled = false;
                bAssignPupil.IsEnabled = true;
            }
            else
            {
                bRemovePupil.IsEnabled = true;
                bAssignPupil.IsEnabled = false;
            }

        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            MyItem m = (MyItem)listView.SelectedItem;


            if (m.L.Owner_id != -1)
            {
                //TODO edit pupil
                string firstname, lastname, grade, classP;

                firstname = tbPupilFirstname.Text;
                lastname = tbPupilLastName.Text;
                grade = tbPupilGrade.Text;
                classP = tbPupilClass.Text;
                //comment = tbPupilComment.Text;

                if (!(firstname.Equals("") || lastname.Equals("") || classP.Equals("")) && (grade.Equals("7") || grade.Equals("8") || grade.Equals("9")))
                {
                    Console.WriteLine(firstname + "," + lastname + ", " + grade + ", " + classP);


                    bool commentCheck = false;


                    m.P.Firstname = firstname;
                    m.P.Lastname = lastname;
                    m.P.Grade = grade;
                    m.P.Class = classP;
                    // m.P.Comment = comment;

                    sql.UpdatePupil(m.P, commentCheck);


                    m.P = sql.getPupil(m.P.Id);
                }


            }
            if (m.L.Id != -1)
            {
                m.L.Status = cbStatus.SelectedIndex;
                sql.updateLocker(m.L);
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Refresh();

            bSave.IsEnabled = false;


        }
        // Remove pupil from locker
        private void bRemovePupil_Click(object sender, RoutedEventArgs e)
        {
            //TODO create alert window with accept or cancel, only if there is a owners
            MessageBoxResult result = MessageBox.Show("Ta bort elev från skåp?", "Ta Bort", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {



                MyItem m = (MyItem)listView.SelectedItem;
                Console.WriteLine("Remove pupil from: " + m.L.Id);

                sql.UpdatePupil(m.P, false);

                sql.removePupilFromLocker(m.L.Id);
                m.L.Owner_id = -1;
                m.P = new Pupil(-1, "", "", "", "", "", "");



                m.L.Status = (int)Locker.StatusT.LÅST_AV_SKOLAN;

                UpdateTextBoxes(m);

                m.L = sql.getLockerID(m.L.Id);

                ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view.Refresh();


                gbPupil.IsEnabled = false;

                ExportCSV();

                ListViewItem item = listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem) as ListViewItem;
                if (item != null)
                {
                    item.Focus();
                }
                else
                {
                    Console.WriteLine("item null");
                }





            }

        }

        private void bAssignPupil_Click(object sender, RoutedEventArgs e)
        {
            MyItem m = (MyItem)listView.SelectedItem;

            var window = new AssignPupil(m.L.Number);

            Nullable<bool> res = window.ShowDialog();



            if (m != null && res.Value)
            {
                gbPupil.IsEnabled = true;


                sql.assignPupilToLocker(m.L.Id, window.pupilID);

                m.L.Owner_id = window.pupilID;
                m.P = sql.getPupil(window.pupilID);
                m.L.Status = (int)Locker.StatusT.ELEVE_HAR_SLÅPET;
                sql.UpdatePupil(m.P, false);



                UpdateTextBoxes(m);

                m.L = sql.getLockerID(m.L.Id);

                ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view.Refresh();

                ExportCSV();

                ListViewItem item = listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem) as ListViewItem;
                if (item != null)
                {
                    item.Focus();
                }
                else
                {
                    Console.WriteLine("item null");
                }



                //Clipboard.SetText(text);

            }
            // sql.assignPupilToLocker(m.L.Id,);
            Console.WriteLine(res.Value);
            Console.WriteLine();
            // TODO only if there is no owner
        }

        private void ExportCSV()
        {
            String text = "";
            text = "\"Skåp\",\"Nycklar\",\"Status\",\"Förnamn\",\"Efternamn\",\"Åk\",\"Klass\",\"År\",\"Commentar\"\n";


            List<MyItem> list = sql.getAllLockers("", "", "", "", "");
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(list);
            view.Culture = new CultureInfo("sv-SE");

            view.SortDescriptions.Clear();

            view.SortDescriptions.Add(new SortDescription("P.Grade", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("P.Class", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("P.Lastname", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("P.Firstname", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("L.Status", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("L.Number", ListSortDirection.Ascending));
            list = view.Cast<MyItem>().ToList();
            for (int i = 0; i < list.Count; i++)
            {
                MyItem n = list[i];
                // Console.WriteLine("JJ" + m.L.Comment.Replace("\n", " ") + "JJ");
                string history = n.L.HistoryShort.Replace("\"", "'");
                text += "\"" + n.L.Number + "\",\"" + n.L.Keys + "\",\"" + n.L.StatusText + "\",\"" + n.P.Firstname + "\",\"" + n.P.Lastname + "\",\"" + n.P.Grade + "\",\"" + n.P.Class + "\",\"" + n.P.Year + "\",\"" + history + "\"\n";
            }
            try
            {
                File.WriteAllText(dir + "skåp.csv", text, Encoding.UTF8);
            }
            catch (Exception)
            {

            }

        }

        private void TextBlock_KeyDown(object sender, EventArgs e)
        {
            if (listView.SelectedIndex != -1)
                bSave.IsEnabled = true;
        }

        private void ShowFloors_Button_Click(object sender, RoutedEventArgs e)
        {

            if (showFloorWindow == null || !showFloorWindow.IsVisible)
                showFloorWindow = new ShowFloors();

            showFloorWindow.Show();
        }
        private void History_Button_Click(object sender, RoutedEventArgs e)
        {
            ShowHistory history = new ShowHistory();

            history.ShowDialog();
        }

        private void Export_List_Button_Click(object sender, RoutedEventArgs e)
        {
            // List<MyItem> list = (List<MyItem>);

            String text = "";
            text = "Skåp\tNycklar\tStatus\tFörnamn\tEfternamn\tÅk\tKlass\tÅr\tCommentar\n";
            for (int i = 0; i < listView.Items.Count; i++)
            {

                MyItem m = (MyItem)listView.Items[i];
                // Console.WriteLine("JJ" + m.L.Comment.Replace("\n", " ") + "JJ");
                text += m.L.Number + "\t" + m.L.Keys + "\t" + m.L.StatusText + "\t" + m.P.Firstname + "\t" + m.P.Lastname + "\t" + m.P.Grade + "\t" + m.P.Class + "\t" + m.P.Year + "\t" + m.L.HistoryShort.Replace("\r\n", "").Replace("\n", "") + "\n";
            }

            Clipboard.SetText(text);

        }
        private void tbSearch_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search();
            }
        }

        private void bAddKey_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex != -1)
            {
                MyItem m = (MyItem)listView.SelectedItem;
                tbKeys.Text = "Nycklar:      " + (++m.L.Keys);
                bSave.IsEnabled = true;

            }
        }

        private void bSubKey_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex != -1)
            {
                MyItem m = (MyItem)listView.SelectedItem;
                if (m.L.Keys > 0)
                {
                    tbKeys.Text = "Nycklar:      " + (--m.L.Keys);
                    bSave.IsEnabled = true;
                }
            }
        }

        private void RadioButton_PC_Checked(object sender, RoutedEventArgs e)
        {

            if (gridView != null && gridView.Columns.Count > 11)
            {
                //Console.WriteLine("PC checked");
                //Console.WriteLine();
                //gridView.Columns.RemoveAt(11);
                //gridView.Columns.Add(GetColumn("PC", new SolidColorBrush(Color.FromArgb(0x25, 0xAA, 0x00, 0x00)), "P.Year"));

                //ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                //view.Refresh();
                //System.GC.Collect();
            }
        }
        private void RadioButton_Locker_Checked(object sender, RoutedEventArgs e)
        {

            if (gridView != null && gridView.Columns.Count > 11)
            {
                //Console.WriteLine("Locker checked");
                //Console.WriteLine(gridView.Columns.Count);
                //gridView.Columns.RemoveAt(11);
                //gridView.Columns.Add(GetColumn("locker", new SolidColorBrush(Color.FromArgb(0x25, 0x00, 0xAA, 0x00)), "P.Firstname"));

                //ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                //view.Refresh();
                //System.GC.Collect();
            }

        }
        private void RadioButton_Books_Checked(object sender, RoutedEventArgs e)
        {

            if (gridView != null && gridView.Columns.Count > 11)
            {
                //Console.WriteLine("Books checked");
                //Console.WriteLine(gridView.Columns.Count);
                //gridView.Columns.RemoveAt(11);
                //gridView.Columns.Add(GetColumn("book", new SolidColorBrush(Color.FromArgb(0x25, 0x00, 0x00, 0xAA)), "P.Lastname"));

                //ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                //view.Refresh();
                //System.GC.Collect();
            }
        }

        private void Add_Pupil_History_Button_Click(object sender, RoutedEventArgs e)
        {
            if (tbPupilHistory.Text.Equals(""))
                return;

            string origin = "pupil",
                type = cbPupiHistory.Text,
                comment = tbPupilHistory.Text;

            MyItem m = (MyItem)listView.SelectedItem;
            int owner_id = m.P.Id;
            long date = datePupil.SelectedDate.Value.Ticks;

            try
            {
                sql.InsertHistory(0, owner_id, type, comment, date);

                listPupilHistory.ItemsSource = sql.GetHistory(0, m.P.Id);


                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listPupilHistory.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
                view.Refresh();

                datePupil.SelectedDate = DateTime.Now;
                tbPupilHistory.Text = "";

                m.P = sql.getPupil(m.P.Id);
                CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view1.Refresh();

            }
            catch
            {

            }
        }
        private void Add_Locker_History_Button_Click(object sender, RoutedEventArgs e)
        {
            if (tbLockerHistory.Text.Equals(""))
                return;

            string comment = tbLockerHistory.Text;

            MyItem m = (MyItem)listView.SelectedItem;
            int id = m.L.Id;
            long date = dateLocker.SelectedDate.Value.Ticks;

            try
            {
                sql.InsertHistory(1, id, "comment", comment, date);

                listLockerHistory.ItemsSource = sql.GetHistory(1, m.L.Id);

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listLockerHistory.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
                view.Refresh();

                dateLocker.SelectedDate = DateTime.Now;
                tbLockerHistory.Text = "";

                m.L = sql.getLockerID(m.L.Id);
                CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view1.Refresh();

            }
            catch
            {

            }
        }
    }

}

public class MyItem
{

    /*

                    <GridViewColumn Header="CommentLocker" DisplayMemberBinding="{Binding CommentLocker}"/>
                    <GridViewColumn Header="PupilComment" DisplayMemberBinding="{Binding PupilComment}"/>
     */
    public Pupil P { get; set; }
    public Locker L { get; set; }

}
