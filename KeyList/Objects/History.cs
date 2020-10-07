using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyList
{
    public class History
    {

        public enum OriginT
        {
            GENERAL = -1,
            PUPIL = 0,
            LOCKER = 1,
            COMPUTER = 2

        }

        int origin, id;
        string type, comment, owner;
        long date;
        public History(int id, int origin, string type, string comment, long date, string owner)
        {
            this.origin = origin;
            this.type = type;
            this.comment = comment;
            this.date = date;
            this.owner = owner;
            this.id = id;
        }

        public int Origin { get => origin; set => origin = value; }
        public string OriginText { get => Enum.GetName(typeof(OriginT), Origin); }
        public string Type { get => type; set => type = value; }
        public string Comment { get => comment; set => comment = value; }
        public long Date { get => date; set => date = value; }
        public string DateString { get => new DateTime(Date).ToString("yyyy-MM-dd_HHmm"); }
        public string Owner { get => owner; set => owner = value; }
        public int Id { get => id; set => id = value; }
    }
}
