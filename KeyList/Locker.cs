using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeyList
{
    public class Locker
    {
        public enum StatusT
        {
            ELEVE_HAR_SLÅPET,
            LÅST_AV_SKOLAN,
            ELEV_MED_EGET_LÅS,
            SKÅPET_REPARERAS,
            LÅST_M_MULTILÅS,
            ELEV_MED_DED_SKÅP,
            ANVÄNDS_EJ,
            UTAN_NYCKEL_ELEVINNEHÅLL
        }
        public enum StatusC
        {
            GREEN,
            PINK,
            YELLOW,
            ORANGE,
            BLUE,
            GRAY,
            LIGHTGRAY,
            DARKGRAY
        }

        private int id;
        private int number;
        private int keys;
        private string floor;
        private int status;
        private int owner_id;
        private string comment;

        public Locker(int id, int number, int keys, string floor, int status, int owner_id, string comment)
        {
            this.id = id;
            this.number = number;
            this.keys = keys;
            this.floor = floor;
            this.status = status;
            this.owner_id = owner_id;
            this.comment = comment;
        }

        public int Id { get => id; set => id = value; }
        public int Number { get => number; set => number = value; }
        public int Keys { get => keys; set => keys = value; }
        public string Floor { get => floor; set => floor = value; }
        public int Status { get => status; set => status = value; }
        public int Owner_id { get => owner_id; set => owner_id = value; }
        public string Comment { get => comment; set => comment = value; }

        public string CommentShort
        {
            get
            {
                string commentShort = "";
                string[] parts = comment.Split('\n');

                int cnt = 3;
                for (int i = 0; i < parts.Length; i++)
                {
                    if (i >= (parts.Length - cnt))
                    {
                        commentShort += parts[i];
                        if (i < (parts.Length - 1))
                        {
                            commentShort += "\n";
                        }
                    }

                }

                return commentShort;
            }
        }
        public string StatusText { get => Enum.GetName(typeof(StatusT), status); }
        public string StatusColor { get => Enum.GetName(typeof(StatusC), status); }
    }
}
