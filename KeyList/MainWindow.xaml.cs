using KeyList;
using System;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    // ** TO DO **
    // History for owner_id for locker
    // add pupils
    // batch operations (remove,add pupils to lockers)
    // auto up grade on students? 
    // multi satus
    // go to item on letter click sensitive to column

    public partial class MainWindow : Window
    {
        String currentSort = "";
        bool ascending = true;
        int selected = -2;
        ShowFloors showFloorWindow = null;
        public string dir;
        public static SQL sql;

        public MainWindow()
        {


            InitializeComponent();
            // listView.Items.Add(new MyItem {FirstName="Rilind",LastName="Hasanaj safsa s" });
            //'Skåp-info -7$'
            /*
            List<String> list= GetExcelSheetNames(@"C:\Users\rilhas\Desktop\test.xlsx");

            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i]);
            }
            */
            // con = new SQLiteConnection("Data Source=skåp.db;New=False;");

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


            //importXLSX(sql);
            listView.ItemsSource = (sql.getAllLockers("", "", "", "", ""));

            Console.WriteLine(listView.Items.Count);

            listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridViewColumn_Click));
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("L.Number", ListSortDirection.Ascending));

            bSave.IsEnabledChanged += BSave_IsEnabledChanged;



            //for (int i = 0; i < lines.Length; i++)
            //{
            //    string[] parts = lines[i].Split('\t');
            //    string firstname = parts[1];
            //    string lastname = parts[2];
            //    bool check = sql.checkIfPupilExists(firstname, lastname);
            //    Console.WriteLine(firstname + ", " + lastname + ", " + check);
            //}



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

            if (listView.SelectedIndex == -1)
            {
                Console.WriteLine("clear buttons");
                Console.WriteLine(listView.SelectedItem);
                bRemovePupil.IsEnabled = false;
                bAssignPupil.IsEnabled = false;
                bSave.IsEnabled = false;
                spEdit.IsEnabled = false;
                return;
            }
            spEdit.IsEnabled = true;


            Console.WriteLine(listView.SelectedIndex);

            MyItem m = (MyItem)listView.SelectedItem;

            if (m.P.Id == -1)
            {
                bRemovePupil.IsEnabled = false;
                bAssignPupil.IsEnabled = true;
            }
            else
            {
                bRemovePupil.IsEnabled = true;
                bAssignPupil.IsEnabled = false;
            }
            // bSave.IsEnabled = true;

            // selected = listView.SelectedIndex;


            tbLockerNumber.Text = String.Format("Nummer:{0,4}", m.L.Number);
            tbKeys.Text = m.L.Keys + "";
            tbLockerComment.Text = m.L.Comment + "";
            tbPupilFirstname.Text = m.P.Firstname + "";
            tbPupilLastName.Text = m.P.Lastname + "";
            tbPupilGrade.Text = m.P.Grade + "";
            tbPupilClass.Text = m.P.Class + "";
            cbStatus.SelectedIndex = m.L.Status;
            tbPupilComment.Text = m.P.Comment;

            //Console.WriteLine(m.Locker);

            //Console.WriteLine(m.ToString());

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
                    case "Årskurs":
                        col = "P.Grade";
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
                else if (col == "P.Grade")
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
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbsStatus is null || tbSearch is null)
            {
                return;
            }
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

        }

        private void Show_Pupil_Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new ShowPupil();

            window.ShowDialog();

            Console.WriteLine("hallow");

        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            MyItem m = (MyItem)listView.SelectedItem;

            if (m.L.Owner_id != -1)
            {
                //TODO edit pupil
                string firstname, lastname, grade, classP, comment;

                firstname = tbPupilFirstname.Text;
                lastname = tbPupilLastName.Text;
                grade = tbPupilGrade.Text;
                classP = tbPupilClass.Text;
                comment = tbPupilComment.Text;

                if (!(firstname.Equals("") || lastname.Equals("") || classP.Equals("")) && (grade.Equals("7") || grade.Equals("8") || grade.Equals("9")))
                {
                    Console.WriteLine(firstname + "," + lastname + ", " + grade + ", " + classP + ", " + comment);


                    bool commentCheck = false;
                    if (!m.P.Comment.Equals(comment))
                    {
                        commentCheck = true;
                    }

                    m.P.Firstname = firstname;
                    m.P.Lastname = lastname;
                    m.P.Grade = grade;
                    m.P.Class = classP;
                    m.P.Comment = comment;

                    sql.UpdatePupil(m.P, commentCheck);
                }


            }

            int tryNumber, tryKeys;

            if (int.TryParse(tbKeys.Text, out tryKeys))
            {
                m.L.Keys = tryKeys;
            }
            m.L.Status = cbStatus.SelectedIndex;
            m.L.Comment = tbLockerComment.Text;


            sql.updateLocker(m.L);

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

                if (!m.P.Comment.Equals(""))
                {
                    m.P.Comment += "\n";
                }
                m.P.Comment += "** " + DateTime.Now.ToString("yyyy-MM-dd") + ", " + m.L.Number + "->";
                sql.UpdatePupil(m.P, false);

                sql.removePupilFromLocker(m.L.Id);
                m.L.Owner_id = -1;
                m.P = new Pupil(-1, "", "", "", "", "", "");



                m.L.Status = (int)Locker.StatusT.LÅST_AV_SKOLAN;
                cbStatus.SelectedIndex = m.L.Status;

                tbPupilFirstname.Text = "";
                tbPupilLastName.Text = "";
                tbPupilGrade.Text = "";
                tbPupilClass.Text = "";
                tbPupilComment.Text = "";

                ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view.Refresh();
                bRemovePupil.IsEnabled = false;

                bAssignPupil.IsEnabled = true;

                ExportCSV();

            }

        }

        private void bAssignPupil_Click(object sender, RoutedEventArgs e)
        {
            MyItem m = (MyItem)listView.SelectedItem;

            var window = new AssignPupil(m.L.Number);

            Nullable<bool> res = window.ShowDialog();



            if (m != null && res.Value)
            {
                sql.assignPupilToLocker(m.L.Id, window.pupilID);
                m.L.Owner_id = window.pupilID;
                m.P = sql.getPupil(window.pupilID);
                m.L.Status = (int)Locker.StatusT.ELEVE_HAR_SLÅPET;
                if (!m.P.Comment.Equals(""))
                {
                    m.P.Comment += "\n";
                }
                m.P.Comment += "** " + DateTime.Now.ToString("yyyy-MM-dd") + ", ->" + m.L.Number;

                sql.UpdatePupil(m.P, false);

                tbPupilFirstname.Text = m.P.Firstname;
                tbPupilLastName.Text = m.P.Lastname;
                tbPupilGrade.Text = m.P.Grade;
                tbPupilClass.Text = m.P.Class;
                tbPupilComment.Text = m.P.Comment;

                cbStatus.SelectedIndex = m.L.Status;
                ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view.Refresh();
                bRemovePupil.IsEnabled = true;
                bAssignPupil.IsEnabled = false;

                ExportCSV();
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
                string comment = n.L.Comment.Replace("\"", "'");
                text += "\"" + n.L.Number + "\",\"" + n.L.Keys + "\",\"" + n.L.StatusText + "\",\"" + n.P.Firstname + "\",\"" + n.P.Lastname + "\",\"" + n.P.Grade + "\",\"" + n.P.Class + "\",\"" + n.P.Year + "\",\"" + comment + "\"\n";
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
            History history = new History();

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
                text += m.L.Number + "\t" + m.L.Keys + "\t" + m.L.StatusText + "\t" + m.P.Firstname + "\t" + m.P.Lastname + "\t" + m.P.Grade + "\t" + m.P.Class + "\t" + m.P.Year + "\t" + m.L.Comment.Replace("\r\n", "").Replace("\n", "") + "\n";
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
                int keys = int.Parse(tbKeys.Text);

                keys++;
                tbKeys.Text = keys + "";
                bSave.IsEnabled = true;

            }
        }

        private void bSubKey_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex != -1)
            {
                int keys = int.Parse(tbKeys.Text);
                if (keys > 0)
                {
                    keys--;
                    tbKeys.Text = keys + "";
                    bSave.IsEnabled = true;
                }
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
