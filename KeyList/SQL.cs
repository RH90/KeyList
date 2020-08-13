
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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


        public SQL()
        {


            con = new SQLiteConnection("Data Source=C:\\Users\\rilhas\\Desktop\\test.db;New=False;");
            con.Open();

            new SQLiteCommand("CREATE TABLE IF NOT EXISTS " +
               "locker(" +
               "id INTEGER PRIMARY KEY AUTOINCREMENT," +
               "keys Integer Default 0, " +
               "number Integer UNIQUE, " +
               "floor TEXT, " +
               "status Integer DEFAULT 6," +
               "owner_id INTEGER UNIQUE," +
               "comment TEXT DEFAULT \"\")", con).ExecuteNonQuery();

            new SQLiteCommand("CREATE TABLE IF NOT EXISTS " +
               "pupil(" +
               "id INTEGER PRIMARY KEY AUTOINCREMENT," +
               "grade TEXT DEFAULT \"\", " +
               "classP TEXT DEFAULT \"\"," +
               "year TEXT DEFAULT \"\"," +
               "firstname TEXT DEFAULT \"\"," +
               "lastname TEXT DEFAULT \"\"," +
               "comment TEXT DEFAULT \"\")", con).ExecuteNonQuery();

        }
        public long addPupil(String FirstName, String LastName, String Class, String Grade, String Year)
        {
            sem.WaitOne();
            long lastID = -1;
            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO pupil (firstname,lastname,class,grade,year) VALUES (@firstname,@lastname,@class,@grade,@year)", con))
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

                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (!p.Firstname.ToLower().Contains(parts[i]) && !p.Lastname.ToLower().Contains(parts[i]) && !l.Comment.ToLower().Contains(parts[i]))
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
    }


}
