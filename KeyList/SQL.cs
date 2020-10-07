
using System;
using System.CodeDom;
using System.Collections;
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

            new SQLiteCommand("CREATE TABLE IF NOT EXISTS \"history\"(\n" +
            "\t\"id\"\tINTEGER PRIMARY KEY AUTOINCREMENT,\n" +
            "\t\"origin\"\tINTEGER DEFAULT -1,\n" +
            "\t\"owner_id\"\tINTEGER DEFAULT -1,\n" +
            "\t\"type\"\tTEXT,\n" +
            "\t\"comment\"\tTEXT DEFAULT \"\",\n" +
            "\t\"date\"\tINTEGER DEFAULT 0\n" +
            ");", con).ExecuteNonQuery();

            new SQLiteCommand("CREATE TABLE IF NOT EXISTS \"computer\"(" +
                "\"id\"    INTEGER PRIMARY KEY AUTOINCREMENT," +
                "\"brand\" TEXT DEFAULT \"\"," +
                "\"model\" TEXT DEFAULT \"\"," +
                "\"serial\"    TEXT UNIQUE," +
                "\"owner_id\"  INTEGER UNIQUE," +
                "\"status\"    INTEGER DEFAULT 0," +
                "\"comment\"   TEXT DEFAULT \"\"" +
                "); ", con).ExecuteNonQuery();



        }
        //TODO add history
        public void removeComputer(int id)
        {
            //    Pupil p = getPupil(id);
            //    InsertHistory(-1, -1, "removed", p.ToString, DateTime.Now.Ticks);

            using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM computer where id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // TODO add hsitory
        public List<MyItem> getUnAssignedComputerList(string search)
        {
            List<MyItem> list = new List<MyItem>();
            string query = "select * from computer where owner_id=-1 OR owner_id isnull;";
            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {
                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    search = search.ToLower();

                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                    List<History> histories = GetHistory(2, id);

                    string shortHistory = getHistoryShort(histories, 4);

                    Computer c = new Computer(
                        id,
                        reader.GetString(reader.GetOrdinal("brand")),
                        reader.GetString(reader.GetOrdinal("model")),
                        reader.GetString(reader.GetOrdinal("serial")),
                        reader.GetString(reader.GetOrdinal("comment")),
                        reader.GetString(reader.GetOrdinal("smartwater")),
                        reader.GetInt32(reader.GetOrdinal("buy_out")),
                        reader.GetInt32(reader.GetOrdinal("status")),
                        -1,
                        shortHistory);

                    if (c.Serielnumber.ToLower().Contains(search) || c.Model.ToLower().Contains(search) || c.Smartwater.ToLower().Contains(search) || c.Brand.ToLower().Contains(search))
                        list.Add(new MyItem
                        {
                            C = c
                        });
                }
            }

            return list;
        }

        // TODO add history
        public bool addComputer(string brand, string model, string serial, string smartwater)
        {
            //Pupil p = new Pupil(-1, Grade, Class, Year, FirstName, LastName, "");
            //InsertHistory(-1, -1, "added", p.ToString, DateTime.Now.Ticks);

            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO computer (brand,model,serial,smartwater) VALUES (@brand,@model,@serial,@smartwater)", con))
                {
                    cmd.Parameters.AddWithValue("@brand", brand);
                    cmd.Parameters.AddWithValue("@model", model);
                    cmd.Parameters.AddWithValue("@serial", serial);
                    cmd.Parameters.AddWithValue("@smartwater", smartwater);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public long addPupil(String FirstName, String LastName, String Class, String Grade, String Year)
        {
            sem.WaitOne();
            long lastID = -1;

            Pupil p = new Pupil(-1, Grade, Class, Year, FirstName, LastName, "");
            InsertHistory(-1, -1, "added", p.ToString, DateTime.Now.Ticks);

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
        public void InsertHistory(int origin, int owner_id, string type, string comment, long date)
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
        // origin: pupil=0,locker=1,computer=2,general=-1
        public List<History> GetHistory()
        {
            List<History> list = new List<History>();
            using (SQLiteCommand cmd = new SQLiteCommand(
               "SELECT * from history ORDER BY date DESC", con))
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                int cnt = 0;
                while (reader.Read() && cnt < 2000)
                {
                    try
                    {

                        string type, comment, owner = "";
                        int owner_id, origin, id;
                        long date;

                        id = reader.GetInt32(reader.GetOrdinal("id"));
                        origin = reader.GetInt32(reader.GetOrdinal("origin"));
                        type = reader.GetString(reader.GetOrdinal("type"));
                        comment = reader.GetString(reader.GetOrdinal("comment"));
                        date = reader.GetInt64(reader.GetOrdinal("date"));
                        owner_id = reader.GetInt32(reader.GetOrdinal("owner_id"));


                        if (origin == 0)
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
                        else if (origin == 1)
                        {
                            owner = getLockerID(owner_id).Number + "";
                        }
                        else if (origin == 2)
                        {
                            Computer c = getComputer(owner_id);
                            if (c == null)
                            {
                                owner = "null";
                            }
                            else
                            {
                                owner = c.Serielnumber + "";

                            }

                        }
                        History h = new History(id, origin, type, comment, date, owner);
                        list.Add(h);
                        cnt++;
                    }
                    catch { }
                }
            }


            return list;
        }
        public List<History> GetHistory(int origin, int id)
        {
            List<History> list = new List<History>();

            using (SQLiteCommand cmd = new SQLiteCommand(
               "SELECT * from history where origin=@origin AND owner_id=@owner_id ORDER BY date DESC", con))
            {
                cmd.Parameters.AddWithValue("@origin", origin);
                cmd.Parameters.AddWithValue("@owner_id", id);

                SQLiteDataReader reader = cmd.ExecuteReader();

                int cnt = 0;
                while (reader.Read() && cnt < 50)
                {
                    try
                    {
                        string type, comment;
                        int id_history;
                        type = reader.GetString(reader.GetOrdinal("type"));
                        comment = reader.GetString(reader.GetOrdinal("comment"));
                        long date = reader.GetInt64(reader.GetOrdinal("date"));
                        id_history = reader.GetInt32(reader.GetOrdinal("id"));

                        History h = new History(id_history, origin, type, comment, date, "");
                        list.Add(h);
                        cnt++;
                    }
                    catch { }
                }
            }
            //list.Reverse();
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
            InsertHistory(-1, -1, "removed", p.ToString, DateTime.Now.Ticks);

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
                    int id = reader.GetInt32(reader.GetOrdinal("id"));

                    List<History> histories = GetHistory(1, id);

                    string comment = getHistoryShort(histories, 2);

                    l = new Locker(
                       id,
                       reader.GetInt32(reader.GetOrdinal("number")),
                       reader.GetInt32(reader.GetOrdinal("keys")),
                       reader.GetString(reader.GetOrdinal("floor")),
                       reader.GetInt32(reader.GetOrdinal("status")),
                       owner,
                       comment);
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

                    List<History> histories = GetHistory(1, id);
                    string comment = getHistoryShort(histories, 2);

                    l = new Locker(
                       reader.GetInt32(reader.GetOrdinal("id")),
                       reader.GetInt32(reader.GetOrdinal("number")),
                       reader.GetInt32(reader.GetOrdinal("keys")),
                       reader.GetString(reader.GetOrdinal("floor")),
                       reader.GetInt32(reader.GetOrdinal("status")),
                       owner,
                       comment);
                }
            }
            return l;
        }

        private static string getHistoryShort(List<History> histories, int cnt)
        {
            string comment = "";

            for (int i = 0; i < histories.Count && i < cnt; i++)
            {
                comment += "** " + histories[i].DateString + ", " + histories[i].Comment;
                if (i < (cnt - 1) && i < (histories.Count - 1))
                {
                    comment += "\n";
                }
            }

            return comment;
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

                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                    List<History> histories = GetHistory(1, id);

                    string comment = getHistoryShort(histories, 2);

                    Pupil p = null;
                    Locker l = new Locker(
                        id,
                        reader.GetInt32(reader.GetOrdinal("number")),
                        reader.GetInt32(reader.GetOrdinal("keys")),
                        reader.GetString(reader.GetOrdinal("floor")),
                        reader.GetInt32(reader.GetOrdinal("status")),
                        owner,
                        comment);

                    // Console.WriteLine(l.Owner_id);
                    Computer c = new MyItem().C;
                    if (owner == -1)
                    {
                        p = new Pupil(-1, "", "", "", "", "", "");
                    }
                    else
                    {
                        p = getPupil(owner);

                        Computer cTmp = getComputerByOwner(owner);

                        if (cTmp != null)
                            c = cTmp;
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
                        else if (!p.Firstname.ToLower().Contains(parts[i]) && !p.Lastname.ToLower().Contains(parts[i]))
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
                            L = l,
                            C = c
                        });
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
                    List<History> histories = GetHistory(0, id);
                    string comment = getHistoryShort(histories, 5);


                    p = new Pupil(
                        id,
                        reader.GetString(reader.GetOrdinal("grade")),
                        reader.GetString(reader.GetOrdinal("classP")),
                        reader.GetString(reader.GetOrdinal("year")),
                        reader.GetString(reader.GetOrdinal("firstname")),
                        reader.GetString(reader.GetOrdinal("lastname")),
                        comment);
                }
            }
            return p;
        }

        //TODO add history
        public Computer getComputer(int id)
        {
            Computer c = null;

            using (SQLiteCommand cmd = new SQLiteCommand(
                "SELECT * from computer where id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {

                    //string comment = getHistoryShort(histories, 5);
                    int owner_id;

                    if (!int.TryParse(reader.GetValue(reader.GetOrdinal("owner_id")) + "", out owner_id))
                    {

                        owner_id = -1;
                    }
                    List<History> histories = GetHistory(2, id);

                    string shortHistory = getHistoryShort(histories, 4);

                    c = new Computer(
                        id,
                        reader.GetString(reader.GetOrdinal("brand")),
                        reader.GetString(reader.GetOrdinal("model")),
                        reader.GetString(reader.GetOrdinal("serial")),
                        reader.GetString(reader.GetOrdinal("comment")),
                        reader.GetString(reader.GetOrdinal("smartwater")),
                        reader.GetInt32(reader.GetOrdinal("buy_out")),
                        reader.GetInt32(reader.GetOrdinal("status")),
                        owner_id,
                        shortHistory);
                }
            }
            return c;
        }

        public void updateComputer(Computer c)
        {
            string query = "UPDATE computer set brand=@brand,model=@model,serial=@serial,smartwater=@smartwater,comment=@comment,status=@status,buy_out=@buy_out where id=@id";

            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@brand", c.Brand);
                    cmd.Parameters.AddWithValue("@model", c.Model);
                    cmd.Parameters.AddWithValue("@serial", c.Serielnumber);
                    cmd.Parameters.AddWithValue("@smartwater", c.Smartwater);
                    cmd.Parameters.AddWithValue("@comment", c.Comment);
                    cmd.Parameters.AddWithValue("@status", c.Status);
                    cmd.Parameters.AddWithValue("@buy_out", c.Buy_out);
                    cmd.Parameters.AddWithValue("@id", c.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {

            }

        }

        //TODO add history
        public Computer getComputerByOwner(int owner_id)
        {
            Computer c = null;

            using (SQLiteCommand cmd = new SQLiteCommand(
                "SELECT * from computer where owner_id=@owner_id", con))
            {
                cmd.Parameters.AddWithValue("@owner_id", owner_id);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    //List<History> histories = GetHistory(0, id);
                    //string comment = getHistoryShort(histories, 5);

                    if (!int.TryParse(reader.GetValue(reader.GetOrdinal("owner_id")) + "", out owner_id))
                    {

                        owner_id = -1;
                    }

                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                    List<History> histories = GetHistory(2, id);

                    string shortHistory = getHistoryShort(histories, 4);

                    c = new Computer(
                        id,
                        reader.GetString(reader.GetOrdinal("brand")),
                        reader.GetString(reader.GetOrdinal("model")),
                        reader.GetString(reader.GetOrdinal("serial")),
                        reader.GetString(reader.GetOrdinal("comment")),
                        reader.GetString(reader.GetOrdinal("smartwater")),
                        reader.GetInt32(reader.GetOrdinal("buy_out")),
                        reader.GetInt32(reader.GetOrdinal("status")),
                        owner_id,
                        shortHistory);
                }
            }
            return c;
        }
        public int getLastComputerID()
        {
            int lastId = -1;
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT id from computer", con))
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
            string query = "UPDATE locker set keys=@keys,status=@status where id=@id";

            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {
                cmd.Parameters.AddWithValue("@keys", l.Keys);
                cmd.Parameters.AddWithValue("@status", l.Status);
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

            //pupil histopry
            InsertHistory(0, p.Id, "locker", l.Number + "->", DateTime.Now.Ticks);

            //locker history
            InsertHistory(1, l.Id, "comment", p.ToString + "->", DateTime.Now.Ticks);

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

            //pupil history
            InsertHistory(0, p.Id, "locker", "->" + l.Number, DateTime.Now.Ticks);

            //locker history
            InsertHistory(1, l.Id, "comment", "->" + p.ToString, DateTime.Now.Ticks);

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


            string query = "UPDATE pupil set firstname=@firstname,lastname=@lastname,grade=@grade,classP=@classP where id=@id";

            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {
                cmd.Parameters.AddWithValue("@firstname", p.Firstname);
                cmd.Parameters.AddWithValue("@lastname", p.Lastname);
                cmd.Parameters.AddWithValue("@grade", p.Grade);
                cmd.Parameters.AddWithValue("@classP", p.Class);
                cmd.Parameters.AddWithValue("@id", p.Id);
                cmd.ExecuteNonQuery();
            }
        }
        // TODO add history
        public void assignComputerToPupil(string serial, int pupilID)
        {
            string query = "UPDATE computer set owner_id=@owner_id,status=1 where serial=@serial";

            Pupil p = getPupil(pupilID);
            Computer c = getComputerSerial(serial);

            if (p != null)
            {
                InsertHistory(0, p.Id, "computer", "->" + serial, DateTime.Now.Ticks);
            }

            if (c != null)
            {
                InsertHistory(2, c.Id, "comment", "->" + p.ToString, DateTime.Now.Ticks);
            }

            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {
                cmd.Parameters.AddWithValue("@owner_id", pupilID);
                cmd.Parameters.AddWithValue("@serial", serial);
                cmd.ExecuteNonQuery();
            }
        }

        public Computer getComputerSerial(string serial)
        {
            Computer c = null;

            using (SQLiteCommand cmd = new SQLiteCommand(
                "SELECT * from computer where serial=@serial", con))
            {
                cmd.Parameters.AddWithValue("@serial", serial);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    //List<History> histories = GetHistory(0, id);
                    //string comment = getHistoryShort(histories, 5);
                    int owner_id;

                    if (!int.TryParse(reader.GetValue(reader.GetOrdinal("owner_id")) + "", out owner_id))
                    {

                        owner_id = -1;
                    }
                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                    List<History> histories = GetHistory(2, id);

                    string shortHistory = getHistoryShort(histories, 4);

                    c = new Computer(
                        id,
                        reader.GetString(reader.GetOrdinal("brand")),
                        reader.GetString(reader.GetOrdinal("model")),
                        reader.GetString(reader.GetOrdinal("serial")),
                        reader.GetString(reader.GetOrdinal("comment")),
                        reader.GetString(reader.GetOrdinal("smartwater")),
                        reader.GetInt32(reader.GetOrdinal("buy_out")),
                        reader.GetInt32(reader.GetOrdinal("status")),
                        owner_id,
                        shortHistory);
                }
            }
            return c;
        }
        //TODO add history
        public void removePCFromPupil(int id)
        {

            Computer c = getComputer(id);
            Pupil p = getPupil(c.Owner_id);

            if (p != null)
            {
                InsertHistory(0, p.Id, "computer", c.Serielnumber + "->", DateTime.Now.Ticks);
            }

            if (c != null)
            {
                InsertHistory(2, c.Id, "comment", p.ToString + "->", DateTime.Now.Ticks);
            }

            string query = "UPDATE computer set owner_id=null,status=0 where id=@id";

            using (SQLiteCommand cmd = new SQLiteCommand(
                query, con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }


}
