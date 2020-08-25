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
    /// Interaction logic for AddPupil.xaml
    /// </summary>
    public partial class AddPupil : Window
    {
        public AddPupil()
        {
            InitializeComponent();
            Pupil p = MainWindow.sql.getPupil(MainWindow.sql.getLastPupilID());
            tbPupilGrade.Text = p.Grade;
            tbPupilClass.Text = p.Class;
            tbPupilYear.Text = p.Year;

        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            string firstname, lastname, grade, classP, year, comment;


            firstname = tbPupilFirstname.Text;
            lastname = tbPupilLastName.Text;
            grade = tbPupilGrade.Text;
            classP = tbPupilClass.Text;
            comment = tbPupilComment.Text;
            year = tbPupilYear.Text;

            if (!(firstname.Equals("") || year.Equals("") || lastname.Equals("") || classP.Equals("")) && (grade.Equals("7") || grade.Equals("8") || grade.Equals("9")))
            {

                MainWindow.sql.addPupil(firstname, lastname, classP, grade, year);
            }
            Close();

        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
