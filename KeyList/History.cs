using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyList
{
    public class History
    {
        int origin;
        string type, comment, owner;
        long date;
        public History(int origin, string type, string comment, long date, string owner)
        {
            this.origin = origin;
            this.type = type;
            this.comment = comment;
            this.date = date;
            this.owner = owner;
        }

        public int Origin { get => origin; set => origin = value; }
        public string Type { get => type; set => type = value; }
        public string Comment { get => comment; set => comment = value; }
        public long Date { get => date; set => date = value; }
        public string DateString { get => new DateTime(Date).ToString("yyyy-MM-dd_HHmm"); }
        public string Owner { get => owner; set => owner = value; }
    }
}
