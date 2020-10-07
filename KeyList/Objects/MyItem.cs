using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyList
{
    public class MyItem
    {

        private Pupil p = new Pupil(-1, "", "", "", "", "", "");
        private Locker l = new Locker(-1, -1, -1, "", -1, -1, "");
        private Computer c = new Computer(-1, "", "", "", "", "", -1, -1, -1, "");
        public Pupil P { get => p; set => p = value; }
        public Locker L { get => l; set => l = value; }
        public Computer C { get => c; set => c = value; }

    }
}
