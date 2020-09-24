using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyList
{
    public class Pupil
    {
        private int id;
        private string grade;
        private string _class;
        private string year;
        private string firstname;
        private string lastname;
        private string historyShort;

        public Pupil(int id, string grade, string @class, string year, string firstname, string lastname, string historyShort)
        {
            this.id = id;
            this.grade = grade;
            _class = @class;
            this.year = year;
            this.firstname = firstname;
            this.lastname = lastname;
            this.historyShort = historyShort;
        }

        public int Id { get => id; set => id = value; }
        public string Grade { get => grade; set => grade = value; }
        public string Class { get => _class; set => _class = value; }
        public string Year { get => year; set => year = value; }
        public string Firstname { get => firstname; set => firstname = value; }
        public string Lastname { get => lastname; set => lastname = value; }
        //public string Comment { get => comment; set => comment = value; }
        public string GradeClass { get => Grade + Class; }

        public string ToString
        {
            get
            {
                return Firstname + " " + Lastname + ", " + Grade + Class;
            }
        }

        public string HistoryShort { get => historyShort; set => historyShort = value; }

        //public string CommentShort
        //{
        //    get
        //    {
        //        string commentShort = "";
        //        string[] parts = comment.Split('\n');
        //        int cnt = 5;
        //        if (parts.Length < 5)
        //        {
        //            cnt = parts.Length;
        //        }

        //        for (int i = parts.Length - 1; i >= 0 && i >= (parts.Length - cnt); i--)
        //        {
        //            commentShort += parts[i];
        //            if (i > (parts.Length - cnt))
        //            {
        //                commentShort += "\n";
        //            }
        //        }
        //        return commentShort;
        //    }
        //}
    }
}
