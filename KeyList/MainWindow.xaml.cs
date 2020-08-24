using KeyList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
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
    // History for locker number for pupil
    // Show list of pupils
    // add pupils
    // batch operations (remove,add pupils to lockers)
    // auto up grade on students? 
    // multi satus
    // add column for pupil if they have ever never lost a key
    // go to item on letter click sensitive to column

    public partial class MainWindow : Window
    {
        String currentSort = "";
        bool ascending = true;
        int selected = -2;
        ShowFloors showFloorWindow = null;

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
            sql = new SQL();
            //importXLSX(sql);
            listView.ItemsSource = (sql.getAllLockers("", "", "", "", ""));

            Console.WriteLine(listView.Items.Count);

            listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridViewColumn_Click));
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("L.Number", ListSortDirection.Ascending));

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
            }
            //listView.SelectedIndex == selected
            if (listView.SelectedIndex == -1)
            {

                return;
            }


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

            tbLockerNumber.Text = "Locker: " + m.L.Number;
            tbLockerKeys.Text = m.L.Keys + "";
            tbLockerComment.Text = m.L.Comment + "";
            tbPupilFirstname.Text = m.P.Firstname + "";
            tbPupilLastName.Text = m.P.Lastname + "";
            tbPupilGrade.Text = m.P.Grade + "";
            tbPupilClass.Text = m.P.Class + "";
            cbStatus.SelectedIndex = m.L.Status;

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
                    case "Locker":
                        col = "L.Number";
                        break;
                    case "Keys":
                        col = "L.Keys";
                        break;
                    case "Floor":
                        col = "L.Floor";
                        break;
                    case "Status":
                        col = "L.Status";
                        break;
                    case "FirstName":
                        col = "P.Firstname";
                        break;
                    case "LastName":
                        col = "P.Lastname";
                        break;
                    case "Grade":
                        col = "P.Grade";
                        break;
                    default:
                        return;

                }


                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view.Culture = new CultureInfo("sv-SE");

                view.SortDescriptions.Clear();
                if (currentSort == col)
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
                if (col == "P.Grade")
                {
                    view.SortDescriptions.Add(new SortDescription("P.Class", ListSortDirection.Ascending));
                    view.SortDescriptions.Add(new SortDescription("P.Lastname", ListSortDirection.Ascending));
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
            Console.WriteLine(cbsStatus.Items[2]);
            Console.WriteLine();

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
            }

            //TODO add plus minus button to key textbox

            int tryNumber, tryKeys;

            if (int.TryParse(tbLockerKeys.Text, out tryKeys))
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

        private void bRemovePupil_Click(object sender, RoutedEventArgs e)
        {
            //TODO create alert window with accept or cancel, only if there is a owners
            MessageBoxResult result = MessageBox.Show("Remove Pupil?", "My App", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                MyItem m = (MyItem)listView.SelectedItem;
                Console.WriteLine("Remove pupil from: " + m.L.Id);
                sql.removePupilFromLocker(m.L.Id);
                m.L.Owner_id = -1;
                m.P = new Pupil(-1, "", "", "", "", "", "");

                m.L.Status = (int)Locker.StatusT.LÅST_AV_SKOLAN;
                cbStatus.SelectedIndex = m.L.Status;

                ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view.Refresh();
                bRemovePupil.IsEnabled = false;
                bAssignPupil.IsEnabled = true;

            }

        }

        private void bAssignPupil_Click(object sender, RoutedEventArgs e)
        {
            var window = new AssignPupil();

            Nullable<bool> res = window.ShowDialog();

            MyItem m = (MyItem)listView.SelectedItem;

            if (m != null && res.Value)
            {
                sql.assignPupilToLocker(m.L.Id, window.pupilID);
                m.L.Owner_id = window.pupilID;
                m.P = sql.getPupil(window.pupilID);
                m.L.Status = (int)Locker.StatusT.ELEVE_HAR_SLÅPET;
                cbStatus.SelectedIndex = m.L.Status;
                ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view.Refresh();
                bRemovePupil.IsEnabled = true;
                bAssignPupil.IsEnabled = false;
            }
            // sql.assignPupilToLocker(m.L.Id,);
            Console.WriteLine(res.Value);
            Console.WriteLine();
            // TODO only if there is no owner
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
