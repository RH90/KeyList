
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KeyList
{
    public class SQL
    {
        private SQLiteConnection con;
        private Semaphore sem = new Semaphore(1, 1);


        public SQL(string dbPath)
        {

            if (File.Exists(dbPath))
            {
                con = new SQLiteConnection("Data Source=" + dbPath + ";New=False;");
            }
            else
            {
                con = new SQLiteConnection("Data Source=skåp.db;New=True;");
            }

            con.Open();

            new SQLiteCommand("CREATE TABLE IF NOT EXISTS locker(" +
               "id INTEGER PRIMARY KEY AUTOINCREMENT," +
               "keys Integer Default 0, " +
               "number Integer UNIQUE, " +
               "floor TEXT, " +
               "status Integer DEFAULT 6," +
               "owner_id INTEGER UNIQUE," +
               "comment TEXT DEFAULT \"\")", con).ExecuteNonQuery();

            new SQLiteCommand("CREATE TABLE IF NOT EXISTS pupil(" +
               "id INTEGER PRIMARY KEY AUTOINCREMENT," +
               "grade TEXT DEFAULT \"\", " +
               "classP TEXT DEFAULT \"\"," +
               "year TEXT DEFAULT \"\"," +
               "firstname TEXT DEFAULT \"\"," +
               "lastname TEXT DEFAULT \"\"," +
               "comment TEXT DEFAULT \"\"," +
               "inschool INTEGER DEFAULT 1)", con).ExecuteNonQuery();

            new SQLiteCommand("CREATE TABLE IF NOT EXISTS history(" +
               "id INTEGER PRIMARY KEY AUTOINCREMENT," +
               "comment TEXT DEFAULT \"\")", con).ExecuteNonQuery();
        }

        public long addPupil(String FirstName, String LastName, String Class, String Grade, String Year)
        {
            sem.WaitOne();
            long lastID = -1;

            Pupil p = new Pupil(-1, Grade, Class, Year, FirstName, LastName, "");
            InsertHistory("general", -1, "added", p.ToString, DateTime.Now.ToString("yyyy-MM-dd_HHmm"));

            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO pupil (firstname,lastname,classP,grade,year) VALUES (@firstname,@lastname,@class,@grade,@year)", con))
            {
                cmd.Parameters.Add("@firstname", DbType.String).Value = FirstName;
                cmd.Parameters.Add("@lastname", DbType.String).Value = LastName;
                cmd.Parameters.Add("@class", DbType.String).Value = Class;
                cmd.Parameters.Add("@grade", DbType.String).Value = Grade;
                cmd.Parameters.Add("@year", DbType.String).Value = Year;
                cmd.ExecuteNonQuery();
                lastID = con.LastInsertRowId;
            }
            sem.Release();

            return lastID;
        }

        public int GetNumberOfHistory()
        {
            string query = "select count(*) from history";
            int cnt = -1;

            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {

                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    cnt = reader.GetInt32(reader.GetOrdinal("count(*)"));
                }
            }
            return cnt;
        }
        public void InsertHistory(string origin, int owner_id, string type, string comment, string date)
        {
            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO history (origin,owner_id,type,comment,date) VALUES (@origin,@owner_id,@type,@comment,@date)", con))
            {
                Console.WriteLine(origin + "," + owner_id + "," + type + "," + comment + "," + date);
                cmd.Parameters.AddWithValue("@origin", origin);
                cmd.Parameters.AddWithValue("@owner_id", owner_id);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@comment", comment);
                cmd.Parameters.AddWithValue("@date", date);

                cmd.ExecuteNonQuery();
            }
        }
        public List<History> GetHistory()
        {
            List<History> list = new List<History>();

            int cnt = GetNumberOfHistory();
            using (SQLiteCommand cmd = new SQLiteCommand(
               "SELECT * from history", con))
            {
                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (cnt > 2000)
                    {
                        cnt--;
                        continue;
                    }
                    try
                    {
                        string origin, type, comment, date, owner = "";
                        int owner_id;

                        origin = reader.GetString(reader.GetOrdinal("origin"));
                        type = reader.GetString(reader.GetOrdinal("type"));
                        comment = reader.GetString(reader.GetOrdinal("comment"));
                        date = reader.GetString(reader.GetOrdinal("date"));
                        Console.WriteLine("TEst");
                        owner_id = reader.GetInt32(reader.GetOrdinal("owner_id"));



                        if (origin.Equals("pupil"))
                        {
                            Pupil p = getPupil(owner_id);
                            if (p == null)
                            {
                                owner = "null";
                            }
                            else
                            {
                                owner = getPupil(owner_id).ToString;
                            }

                        }
                        else if (origin.Equals("locker"))
                        {
                            owner = getLocker(owner_id).Number + "";
                        }
                        else if (origin.Equals("computer"))
                        {
                            //TODO
                        }


                        History h = new History(origin, type, comment, date, owner);
                        list.Add(h);
                    }
                    catch { }
                }
            }


            return list;
        }

        public bool checkIfPupilExists(string firstname, string lastname)
        {
            bool check = false;
            using (SQLiteCommand cmd = new SQLiteCommand(
                "SELECT * from pupil where firstname=@firstname AND lastname=@lastname", con))
            {
                cmd.Parameters.AddWithValue("@firstname", firstname);
                cmd.Parameters.AddWithValue("@lastname", lastname);

                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    check = true;
                }
            }
            return check;
        }
        public void removePupil(int id)
        {
            Pupil p = getPupil(id);
            InsertHistory("general", -1, "removed", p.ToString, DateTime.Now.ToString("yyyy-MM-dd_HHmm"));

            using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM pupil where id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

        }

        public Locker getLocker(int number)
        {
            Locker l = null;

            using (SQLiteCommand cmd = new SQLiteCommand(
                "SELECT * from locker where number=@number", con))
            {
                cmd.Parameters.AddWithValue("@number", number);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int owner;

                    if (!int.TryParse(reader.GetValue(reader.GetOrdinal("owner_id")) + "", out owner))
                    {

                        owner = -1;
                    }

                    l = new Locker(
                       reader.GetInt32(reader.GetOrdinal("id")),
                       reader.GetInt32(reader.GetOrdinal("number")),
                       reader.GetInt32(reader.GetOrdinal("keys")),
                       reader.GetString(reader.GetOrdinal("floor")),
                       reader.GetInt32(reader.GetOrdinal("status")),
                       owner,
                       reader.GetString(reader.GetOrdinal("comment")));
                }
            }
            return l;
        }
        public Locker getLockerID(int id)
        {
            Locker l = null;

            using (SQLiteCommand cmd = new SQLiteCommand(
                "SELECT * from locker where id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int owner;

                    if (!int.TryParse(reader.GetValue(reader.GetOrdinal("owner_id")) + "", out owner))
                    {

                        owner = -1;
                    }

                    l = new Locker(
                       reader.GetInt32(reader.GetOrdinal("id")),
                       reader.GetInt32(reader.GetOrdinal("number")),
                       reader.GetInt32(reader.GetOrdinal("keys")),
                       reader.GetString(reader.GetOrdinal("floor")),
                       reader.GetInt32(reader.GetOrdinal("status")),
                       owner,
                       reader.GetString(reader.GetOrdinal("comment")));
                }
            }
            return l;
        }

        public long addLocker(String Locker, string Status, string Keys, string CommentLocker, long OwnerID)
        {
            sem.WaitOne();
            long lastID = -1;
            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO locker (keys,number,status,comment,owner_id) VALUES (@keys,@number,@status,@comment,@owner_id)", con))
            {
                cmd.Parameters.Add("@number", DbType.String).Value = Locker;
                cmd.Parameters.Add("@keys", DbType.String).Value = Keys;
                cmd.Parameters.Add("@status", DbType.String).Value = Status;
                cmd.Parameters.Add("@comment", DbType.String).Value = CommentLocker;

                if (OwnerID == -1)
                {
                    cmd.Parameters.Add("@owner_id", DbType.Object).Value = null;
                }
                else
                {
                    cmd.Parameters.Add("@owner_id", DbType.Int64).Value = OwnerID;
                }
                Console.WriteLine(Locker);
                cmd.ExecuteNonQuery();
                lastID = con.LastInsertRowId;
            }
            sem.Release();

            return lastID;
        }
        public List<MyItem> getAllLockers(String search, string grade, string classP, string floor, string status)
        {
            List<MyItem> list = new List<MyItem>();

            string lockerQuery = "";

            if (floor != "" || status != "")
            {
                lockerQuery += " where ";
            }

            bool addAND = false; ;
            if (floor != "")
            {
                lockerQuery += " floor=\"" + floor + "\" ";
                addAND = true;
            }

            if (status != "")
            {
                if (addAND)
                    lockerQuery += " and ";
                addAND = true;

                lockerQuery += " status=" + status + " ";

            }
            //if (grade != "")
            //{
            //    if (addAND)
            //        lockerQuery += " and ";
            //    addAND = true;
            //    lockerQuery += " grade=\"" + grade + "\"";
            //}
            //if (classP != "")
            //{
            //    if (addAND)
            //        lockerQuery += " and ";
            //    addAND = true;
            //    lockerQuery += " classP=\"" + classP + "\" ";
            //}

            string query = "SELECT * from locker " + lockerQuery;
            Console.WriteLine(query);
            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {
                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int owner;

                    if (!int.TryParse(reader.GetValue(reader.GetOrdinal("owner_id")) + "", out owner))
                    {

                        owner = -1;
                    }

                    Pupil p = null;
                    Locker l = new Locker(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetInt32(reader.GetOrdinal("number")),
                        reader.GetInt32(reader.GetOrdinal("keys")),
                        reader.GetString(reader.GetOrdinal("floor")),
                        reader.GetInt32(reader.GetOrdinal("status")),
                        owner,
                        reader.GetString(reader.GetOrdinal("comment")));

                    // Console.WriteLine(l.Owner_id);

                    if (owner == -1)
                    {
                        p = new Pupil(-1, "", "", "", "", "", "");
                    }
                    else
                    {
                        p = getPupil(owner);
                    }

                    //Console.WriteLine(reader.GetOrdinal("locker.owner_id"));
                    search = search.ToLower();

                    string[] parts = search.Split(' ');

                    bool check = true;
                    int tmp;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (int.TryParse(parts[i], out tmp))
                        {
                            if (!l.Number.ToString().Equals(parts[i]))
                                check = false;
                        }
                        else if (!p.Comment.ToLower().Contains(parts[i]) && !p.Firstname.ToLower().Contains(parts[i]) && !p.Lastname.ToLower().Contains(parts[i]) && !l.Comment.ToLower().Contains(parts[i]))
                        {
                            check = false;
                        }

                    }
                    if (classP != "" && !classP.Equals(p.Class))
                    {
                        check = false;
                    }

                    // Console.WriteLine(grade + "  " + p.Grade);
                    if (grade != "" && !grade.Equals(p.Grade))
                    {
                        check = false;
                    }

                    if ((search == "" && grade == "" && classP == "") || check)
                    {

                        list.Add(new MyItem
                        {
                            P = p,
                            L = l
                        }); ;
                    }

                }
            }
            return list;
        }
        public Pupil getPupil(int id)
        {
            Pupil p = null;

            using (SQLiteCommand cmd = new SQLiteCommand(
                "SELECT * from pupil where id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    p = new Pupil(
                        id,
                        reader.GetString(reader.GetOrdinal("grade")),
                        reader.GetString(reader.GetOrdinal("classP")),
                        reader.GetString(reader.GetOrdinal("year")),
                        reader.GetString(reader.GetOrdinal("firstname")),
                        reader.GetString(reader.GetOrdinal("lastname")),
                        reader.GetString(reader.GetOrdinal("comment")));
                }
            }
            return p;
        }
        public int getLastPupilID()
        {
            int lastId = -1;

            using (SQLiteCommand cmd = new SQLiteCommand("SELECT id from pupil", con))
            {
                //cmd.Parameters.AddWithValue("@id", lastID);
                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lastId = reader.GetInt32(reader.GetOrdinal("id"));
                }
            }
            return lastId;
        }
        public void updateLocker(Locker l)
        {
            string query = "UPDATE locker set keys=@keys,status=@status,comment=@comment where id=@id";

            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {
                cmd.Parameters.AddWithValue("@keys", l.Keys);
                cmd.Parameters.AddWithValue("@status", l.Status);
                cmd.Parameters.AddWithValue("@comment", l.Comment);
                cmd.Parameters.AddWithValue("@id", l.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<MyItem> getUnAssignedPupulList()
        {
            List<MyItem> list = new List<MyItem>();
            string query = "select * from pupil where not EXISTS(select * from locker where owner_id=pupil.id);";
            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {
                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    Pupil p = new Pupil(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("grade")),
                        reader.GetString(reader.GetOrdinal("classP")),
                        reader.GetString(reader.GetOrdinal("year")),
                        reader.GetString(reader.GetOrdinal("firstname")),
                        reader.GetString(reader.GetOrdinal("lastname")),
                        reader.GetString(reader.GetOrdinal("comment")));

                    Locker l = null;

                    list.Add(new MyItem
                    {
                        P = p,
                        L = l
                    }); ;
                }
            }

            return list;
        }

        public void removePupilFromLocker(int id)
        {
            Locker l = getLockerID(id);
            Pupil p = getPupil(l.Owner_id);
            Console.WriteLine(id);
            Console.WriteLine(l.Number);

            InsertHistory("pupil", p.Id, "locker", l.Number + "->", DateTime.Now.ToString("yyyy-MM-dd_HHmm"));

            string query = "UPDATE locker set owner_id=null,status=1 where id=@id";

            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void assignPupilToLocker(int id, int pupilID)
        {
            Locker l = getLockerID(id);
            Pupil p = getPupil(pupilID);
            InsertHistory("pupil", p.Id, "locker", "->" + l.Number, DateTime.Now.ToString("yyyy-MM-dd_HHmm"));

            string query = "UPDATE locker set owner_id=@owner_id,status=0 where id=@id";

            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {
                cmd.Parameters.AddWithValue("@owner_id", pupilID);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdatePupil(Pupil p, bool comment)
        {
            if (comment)
            {
                //string[] com = p.Comment.Split('\n');

                // String history = String.Format("{0} | {1}, Komment: {2}", DateTime.Now.ToString("yyyy-MM-dd_HHmm"), p.ToString, com[com.Length - 1]);
                //InsertHistory("pupil", p.Id, "comment", com[com.Length - 1], DateTime.Now.ToString("yyyy-MM-dd_HHmm"));
            }


            string query = "UPDATE pupil set firstname=@firstname,lastname=@lastname,grade=@grade,classP=@classP,comment=@comment where id=@id";

            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {
                cmd.Parameters.AddWithValue("@firstname", p.Firstname);
                cmd.Parameters.AddWithValue("@lastname", p.Lastname);
                cmd.Parameters.AddWithValue("@grade", p.Grade);
                cmd.Parameters.AddWithValue("@classP", p.Class);
                cmd.Parameters.AddWithValue("@comment", p.Comment);
                cmd.Parameters.AddWithValue("@id", p.Id);
                cmd.ExecuteNonQuery();
            }
        }
    }


}
