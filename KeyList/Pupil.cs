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
        private string comment;

        public Pupil(int id, string grade, string @class, string year, string firstname, string lastname, string comment)
        {
            this.id = id;
            this.grade = grade;
            _class = @class;
            this.year = year;
            this.firstname = firstname;
            this.lastname = lastname;
            this.comment = comment;
        }

        public int Id { get => id; set => id = value; }
        public string Grade { get => grade; set => grade = value; }
        public string Class { get => _class; set => _class = value; }
        public string Year { get => year; set => year = value; }
        public string Firstname { get => firstname; set => firstname = value; }
        public string Lastname { get => lastname; set => lastname = value; }
        public string Comment { get => comment; set => comment = value; }
    }
}
