/***********************************
 * Coder：EminemJK
 * Date：2018-12-04
 **********************************/

using System;
using Banana.Uow;
using Banana.Uow.Models;
using System.Collections.Generic;
using Banana.Uow.Extension;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using Banana.Utility.Common;
using Banana.Utility.Redis;

namespace DotNetCore_TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TestExplicitKey();
            TestSQLServer();

            TestMySQL();

            //TestPostgres(); 

            //TestSQLite();

            //TestOracle();

            TestBCPStore();
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        } 

        #region Show
        static void Show(IEnumerable<UserInfo> infos)
        {
            foreach (var info in infos)
            {
                Show(info);
            }
        }

        static void Show(UserInfo info)
        {
            Show(info.ToString());
        }

        static void Show(string info)
        {
            Console.WriteLine(info);
        } 
        #endregion


        static List<UserInfo> TestData()
        { 
            List<UserInfo> data = new List<UserInfo>();
            string password = Banana.Utility.Encryption.MD5.Encrypt("mimashi123");
            
            data.Add(new UserInfo() { Name = "Monkey D. Luffy", Phone = "15878451111", Password = password, Sex = 1, UserNameFiel = "Luffy", CreateTime = DateTime.Now, Enable = 1 });
            data.Add(new UserInfo() { Name = "Zoro", Phone = "13355526663", Password = password, Sex = 1, UserNameFiel = "Zoro", CreateTime = DateTime.Now, Enable =1 });
            data.Add(new UserInfo() { Name = "Nami", Phone = "15878451111", Password = password, Sex = 0, UserNameFiel = "Nami", CreateTime = DateTime.Now, Enable = 1 });
            data.Add(new UserInfo() { Name = "Sanji", Phone = "17755602229", Password = password, Sex = 1, UserNameFiel = "Sanji", CreateTime = DateTime.Now, Enable = 1 });
            data.Add(new UserInfo() { Name = "Usopp", Phone = "14799995555", Password = password, Sex = 1, UserNameFiel = "Usopp", CreateTime = DateTime.Now, Enable = 1 });

            data.Add(new UserInfo() { Name = "Chopper", Phone = "18966660000", Password = password, Sex = 1, UserNameFiel = "Chopper", CreateTime = DateTime.Now, Enable = 1 });
            data.Add(new UserInfo() { Name = "Robin", Phone = "13122227878", Password = password, Sex = 0, UserNameFiel = "Robin", CreateTime = DateTime.Now, Enable = 1 });
            data.Add(new UserInfo() { Name = "Franky", Phone = "15962354412", Password = password, Sex = 1, UserNameFiel = "Franky", CreateTime = DateTime.Now, Enable = 1 });
            data.Add(new UserInfo() { Name = "Brook", Phone = "14322221111", Password = password, Sex = 1, UserNameFiel = "Brook", CreateTime = DateTime.Now, Enable = 1 });
            data.Add(new UserInfo() { Name = "Jinbe", Phone = "15655479960", Password = password, Sex = 1, UserNameFiel = "Jinbe", CreateTime = DateTime.Now, Enable = 1 });

            data.Add(new UserInfo() { Name = "Li", Phone = "18966661220", Password = password, Sex = 1, UserNameFiel = "Li", CreateTime = DateTime.Now, Enable = 1 });
            data.Add(new UserInfo() { Name = "Papi", Phone = "13122221378", Password = password, Sex = 0, UserNameFiel = "Papi", CreateTime = DateTime.Now, Enable = 1 });
            data.Add(new UserInfo() { Name = "Hacy", Phone = "15962354512", Password = password, Sex = 1, UserNameFiel = "Hacy", CreateTime = DateTime.Now, Enable = 1 });
            data.Add(new UserInfo() { Name = "Hook", Phone = "14322221411", Password = password, Sex = 1, UserNameFiel = "Hook", CreateTime = DateTime.Now, Enable = 1 });
            data.Add(new UserInfo() { Name = "Yami", Phone = "15655479960", Password = password, Sex = 1, UserNameFiel = "Yami", CreateTime = DateTime.Now, Enable = 1 });
            Random r = new Random();
            foreach (var d in data)
            {
                d.CreateTime = d.CreateTime.AddDays(-r.Next(0,360));
            }
            return data;
        }

        /// <summary>
        /// SQLServer
        /// </summary>
        static void TestSQLServer()
        {
            ConnectionBuilder.ConfigRegist("Data Source=.;Initial Catalog = AdminLTE.Net.DB;User ID=sa;Password =mimashi123", DBType.SqlServer);
            var repoUserInfo = new Repository<UserInfo>();

            //repoUserInfo.Execute(@"CREATE TABLE T_User
            //                         (
            //                         Id INT PRIMARY KEY IDENTITY(1,1),
            //                         UserName VARCHAR(50),
            //                         Password VARCHAR(50),
            //                         Name NVARCHAR(50),
            //                         Sex INT,
            //                         Phone VARCHAR(20),
            //                         Enable INT,
            //                         CreateTime DATETIME
            //                         )", null);
            //var datas = TestData();
            //repoUserInfo.Insert(datas);

            var list = repoUserInfo.QueryList();

            var page1 = repoUserInfo.QueryList(1, 5);
            var page2 = repoUserInfo.QueryList(2, 5);
            var page3 = repoUserInfo.QueryList(3, 5);

            var model = repoUserInfo.Query(list[0].Id);
            bool b = repoUserInfo.Delete(model);
            list = repoUserInfo.QueryList();

            model = repoUserInfo.Query(list[0].Id);
            model.Phone = "1234567";
            bool ub = repoUserInfo.Update(model);
            list = repoUserInfo.QueryList(order: "order by id");
            UserInfo newUser = new UserInfo()
            {
                Name = "eminemjk",
                UserNameFiel = "eminemjk",
                Phone = "12346578",
                Enable = 1,
                Password = "mimashi123",
                Sex = 1,
                CreateTime = DateTime.Now
            };
            int id = (int)repoUserInfo.Insert(newUser);
        } 

        /// <summary>
        /// MySQL
        /// </summary>
        static void TestMySQL()
        {
            ConnectionBuilder.ConfigRegist("server=192.168.23.129;port=3306;database=tempdb;user=root;password=mimashi123;", DBType.MySQL);

            var repoUserInfo = new Repository<UserInfo>();
            //repoUserInfo.Execute(@"CREATE TABLE IF NOT EXISTS `T_User`(
            //                           `Id` int auto_increment primary key ,
            //                           `UserName` VARCHAR(100) NOT NULL,
            //                           `Password` VARCHAR(40) NOT NULL,
            //                           `Name` VARCHAR(20),
            //                           `Sex` int,
            //                           `Phone` VARCHAR(20),
            //                           `Enable` INT,
            //                           `CreateTime` DATETIME
            //                        )  CHARSET=utf8;", null);
            //var datas = TestData();
            //repoUserInfo.Insert(datas);
            var list = repoUserInfo.QueryList();

            var page1 = repoUserInfo.QueryList(1, 5, "Phone is not null", order: "ID", asc: true);
            var page2 = repoUserInfo.QueryList(2, 5);
            var page3 = repoUserInfo.QueryList(3, 5);

            var model = repoUserInfo.Query(2);
            bool b = repoUserInfo.Delete(model);
            list = repoUserInfo.QueryList();

            model = repoUserInfo.Query(3);
            model.Phone = "1234567";
            bool ub = repoUserInfo.Update(model);
            list = repoUserInfo.QueryList(order: "order by id");
            UserInfo newUser = new UserInfo()
            {
                Name = "eminemjk",
                UserNameFiel = "eminemjk",
                Phone = "12346578",
                Enable = 1,
                Password = "mimashi123",
                Sex = 1,
                CreateTime = DateTime.Now
            };
            int id = (int)repoUserInfo.Insert(newUser);
        }

        /// <summary>
        /// Postgres
        /// </summary>
        static void TestPostgres()
        {
            ConnectionBuilder.ConfigRegist("PORT=5432;DATABASE=postgres;HOST=192.168.23.129;PASSWORD=mimashi123;USER ID=postgres", DBType.Postgres);
            var repoUserInfo = new Repository<UserModel>();
            //repoUserInfo.Execute(@"CREATE TABLE t_user( 
            //                            id         SERIAL      PRIMARY KEY,
            //                            username    CHAR(50)    NOT NULL,
            //                            password    CHAR(50)    NOT NULL,
            //                            name        CHAR(50), 
            //                            phone       CHAR(11),
            //                            sex int,enable int,
            //                            createtime date); ", null);
            //var datas = ModelConvertUtil<UserInfo, UserModel>.ModelCopy(TestData());
            //repoUserInfo.Insert(datas);

            var list = repoUserInfo.QueryList();

            var page1 = repoUserInfo.QueryList(1, 5);
            var page2 = repoUserInfo.QueryList(2, 5);
            var page3 = repoUserInfo.QueryList(3, 5);

            var model = repoUserInfo.Query(2);
            //bool b = repoUserInfo.Delete(model);
            //list = repoUserInfo.QueryList();

            model = repoUserInfo.Query(3);
            model.phone = "1234567";
            bool ub = repoUserInfo.Update(model);
            list = repoUserInfo.QueryList(order: "order by id");
        }

        /// <summary>
        /// SQLite
        /// </summary>
        static void TestSQLite()
        {
            //System.Data.SQLite.SQLiteConnection.CreateFile("SqliteDemo");
            ConnectionBuilder.ConfigRegist("Data Source=SqliteDemo;Version=3;", DBType.SQLite);

            var repoUserInfo = new Repository<UserInfo>();

            //repoUserInfo.Execute(@"CREATE TABLE T_User
            //                         (
            //                         Id Integer Primary Key Autoincrement,
            //                         UserName VARCHAR(50),
            //                         Password VARCHAR(50),
            //                         Name NVARCHAR(50),
            //                         Sex Integer,
            //                         Phone VARCHAR(20),
            //                         Enable Integer,
            //                         CreateTime DATETIME
            //                         )", null);
            //var datas = TestData();
            //repoUserInfo.Insert(datas);
             
            var list = repoUserInfo.QueryList();

            var page1 = repoUserInfo.QueryList(1, 5);
            var page2 = repoUserInfo.QueryList(2, 5);
            var page3 = repoUserInfo.QueryList(3, 5);

            var model = repoUserInfo.Query(2);
            bool b = repoUserInfo.Delete(model);
            list = repoUserInfo.QueryList();

            model = repoUserInfo.Query(3);
            model.Phone = "1234567";
            bool ub = repoUserInfo.Update(model);
            list = repoUserInfo.QueryList(order: "order by id");
            UserInfo newUser = new UserInfo()
            {
                Name = "eminemjk",
                UserNameFiel = "eminemjk",
                Phone = "12346578",
                Enable = 1,
                Password = "mimashi123",
                Sex = 1,
                CreateTime = DateTime.Now
            };
            int id = (int)repoUserInfo.Insert(newUser);
        }

        /// <summary>
        /// UnitOfWork
        /// </summary>
        static void TestUow()
        {
            //ConnectionBuilder.ConfigRegist(strConn, Banana.Uow.Models.DBType.SqlServer);
            //using (UnitOfWork uow = new UnitOfWork())
            //{
            //    var studentRepo = uow.Repository<Student>();
            //    var model = new Student("啊啊", 1, 1);
            //    var sid = studentRepo.Insert(model);

            //    var classRepo = uow.Repository<MClass>();
            //    var cid = classRepo.Insert(new MClass("五年级"));
            //    if (sid > 0 && cid < 0)
            //    {
            //        uow.Commit();
            //    }
            //    else
            //    {
            //        uow.Rollback();
            //    }
            //}
        }

        /// <summary>
        /// Async
        /// </summary>
        static async void TestAsync()
        {
            var repoUserInfo = new Repository<UserInfo>();

            var page = await repoUserInfo.QueryAsync(1);
            var page1 = await repoUserInfo.QueryListAsync(1, 10, "sex=@sex", new { sex = 1 }, order: "createTime", asc: false);
            var page2 = await repoUserInfo.QueryListAsync(2, 10, "sex=@sex", new { sex = 1 });
            var info = await repoUserInfo.QueryListAsync("UserName=@userName and Password =@psw", new { userName = "admin", psw = "25d55ad283aa400af464c76d713c07ad" });

            var count = await repoUserInfo.QueryCountAsync();
            Show("async");
            Show(DateTime.Now.ToString());
            Show(page);
            Show(DateTime.Now.ToString());
            Show(page1.data);
            Show(DateTime.Now.ToString());
            Show(page2.data);
            Show(DateTime.Now.ToString());
            Show(info);
            Show(DateTime.Now.ToString());
            Show("Count：" + count);

            var deleteAsync = await repoUserInfo.DeleteAsync("HeaderImg is Null", null);
        }

        /// <summary>
        /// Oracle
        /// </summary>
        static void TestOracle()
        {
            string conn = string.Concat(
            @"Data Source=",
            @"    (DESCRIPTION=",
            @"        (ADDRESS_LIST=",
            @"            (ADDRESS=",
            @"                (PROTOCOL=TCP)",
            @"                (HOST=172.16.3.62)",
            @"                (PORT=1521)",
            @"            )",
            @"        )",
            @"        (CONNECT_DATA=",
            @"            (SERVICE_NAME=orcl.oracle.com)",
            @"        )",
            @"    );",
            @"Persist Security Info=True;",
            @"User Id=system;",
            @"Password=manager;"
            );

            ConnectionBuilder.ConfigRegist(conn, DBType.Oracle);

            var repoUserInfo = new Repository<UserModel_Oracle>();
            //create table
            //repoUserInfo.Execute(@"CREATE TABLE T_User
            //                         (
            //                         Id number primary key,
            //                         UserName varchar2(50),
            //                         Password varchar2(50),
            //                         Name varchar2(50),
            //                         Sex INTEGER,
            //                         Phone varchar2(20),
            //                         Enable INTEGER,
            //                         CreateTime DATE
            //                         )", null);
            //create sequence
            //repoUserInfo.Execute(@"CREATE SEQUENCE user_sequence
            //                        INCREMENT BY 1
            //                        START WITH 1
            //                        NOMAXVALUE
            //                        NOCYCLE
            //                        NOCACHE", null);

            //datas  
            //var datas = ModelConvertUtil<UserInfo, UserModel_Oracle>.ModelCopy(TestData());
            //repoUserInfo.Insert(datas);

            var list = repoUserInfo.QueryList();

            var page1 = repoUserInfo.QueryList(1, 5);
            var page2 = repoUserInfo.QueryList(2, 5);
            var page3 = repoUserInfo.QueryList(3, 5);

            var model = repoUserInfo.Query(2);
            bool b = repoUserInfo.Delete(model);
            list = repoUserInfo.QueryList();

            model = repoUserInfo.Query(3);
            model.Phone = "1234567";
            bool ub = repoUserInfo.Update(model);
            list = repoUserInfo.QueryList(order: "order by id");
            UserModel_Oracle newUser = new UserModel_Oracle()
            {
                Name = "eminemjk",
                UserName = "eminemjk",
                Phone = "12346578",
                Enable = 1,
                Password = "mimashi123",
                Sex = 1,
                CreateTime = DateTime.Now
            };
            int id = (int)repoUserInfo.Insert(newUser);
            list = repoUserInfo.QueryList();
        }

        /// <summary>
        /// Bcp
        /// </summary>
        static void TestBCPStore()
        {
            ConnectionBuilder.ConfigRegist("Data Source=.;Initial Catalog = AdminLTE.Net.DB;User ID=sa;Password =mimashi123", DBType.SqlServer);
            var repoUserInfo = new Repository<UserInfo>();

            var datas = TestData();

            var conn = repoUserInfo.DBConnection;

            BCPStore bcp = new BCPStore();
            bcp.Init(repoUserInfo);
            List<object> row = new List<object>();
            foreach (var data in datas)
            {
                //对应select top 0 * from tableName 的行数据
                row.Add(data.Id);
                row.Add(data.UserNameFiel);
                row.Add(data.Password);
                row.Add(data.Name);
                row.Add(data.Sex);
                row.Add(data.Phone);
                row.Add(data.Enable);
                row.Add(data.CreateTime);

                bcp.AddData(row.ToArray());
                row.Clear();
            }
            bcp.Flush(conn as System.Data.SqlClient.SqlConnection, true);
        }

        static void TestRedis()
        {
            int dbIdx = 4;
            string key = "testKey";
            Task.Run(()=> {
                while (true)
                {
                    Task.Run(() =>
                    {
                        RedisUtils.StringSet(dbIdx, key, DateTime.Now.ToString(), TimeSpan.FromSeconds(30));
                    });
                    Task.Run(() =>
                    {
                        string v = RedisUtils.StringGet(dbIdx, key);
                        Console.WriteLine("1：" + v);
                    });
                    System.Threading.Thread.Sleep(100);
                }
            });
            Task.Run(() => {
                while (true)
                {
                    Task.Run(() =>
                    {
                        RedisUtils.StringSet(dbIdx, key, DateTime.Now.ToString(), TimeSpan.FromSeconds(30));
                    });
                    Task.Run(() =>
                    {
                        string v = RedisUtils.StringGet(dbIdx, key);
                        Console.WriteLine("2：" + v);
                    });
                    System.Threading.Thread.Sleep(100);
                }
            });
            Task.Run(() => {
                while (true)
                {
                    Task.Run(() =>
                    {
                        RedisUtils.StringSet(dbIdx, key, DateTime.Now.ToString(), TimeSpan.FromSeconds(30));
                    });
                    Task.Run(() =>
                    {
                        string v = RedisUtils.StringGet(dbIdx, key);
                        Console.WriteLine("3：" + v);
                    });
                    System.Threading.Thread.Sleep(100);
                }
            });

        }

        static void TestExplicitKey()
        {
            ConnectionBuilder.ConfigRegist("Data Source=.;Initial Catalog = AdminLTE.Net.DB;User ID=sa;Password =mimashi123", DBType.SqlServer);
            var repoStudent = new Repository<Student>();
            var s1 = new Student()
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "EminemJK",
                LinkPhone = "15522223333",
                Sex = 1,
                CreateTime = Convert.ToDateTime("2019-01-03")
            };
            var bIn = repoStudent.Insert(s1);

            var model = repoStudent.Query(s1.Id);

            var list = repoStudent.QueryList();

            list[0].Name = "Banana";
           var b = repoStudent.Update(list[0]);
        }
    }
}
