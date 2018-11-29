using System;
using Banana.Uow;
using Dapper.Contrib.Extensions;
using Banana.Uow.Models;
using System.Collections.Generic;
using Banana.Uow.Extension;
using Dapper;
using System.Linq;

/***********************************
 * Coder：EminemJK
 * Date：2018-11-16
 **********************************/

namespace DotNetCore_TestApp
{
    class Program
    {
        static string strConn = @"Data Source=.;Initial Catalog = AdminLTE.Net.DB;User ID=sa;Password =mimashi123";
        static void Main(string[] args)
        {
            ConnectionBuilder.ConfigRegist(strConn, Banana.Uow.Models.DBType.SqlServer);
            //Dos();

            var repoUserInfo = new Repository<UserInfo>();
            var page1 = repoUserInfo.QueryList(1, 10, "sex=@sex", new { sex = 1 });
            var page2 = repoUserInfo.QueryList(2, 10, "sex=@sex", new { sex = 1 });
            var page3 = repoUserInfo.QueryList(3, 10, "sex=@sex", new { sex = 1 });


            var info = repoUserInfo.QueryList("UserName=@userName and Password =@psw", new { userName = "admin", psw= "25d55ad283aa400af464c76d713c07ad" }).FirstOrDefault();

            var repo = new Repository<Category>();
                
            var list = repo.QueryList("where ParentNamePath like @ParentNamePath", new { ParentNamePath = "%,电气设备,%" }); 

           

            //var model = repo.Query(2);

            //var modelAll = repo.QueryAll();

            //var id = repo.Insert(new Student("EminemJK", 1, 2));

            //var ok = repo.Delete(model);

            //var model3 = repo.Query(3);
            //model.Name = "Banana" + DateTime.Now.ToShortTimeString();
            //var upOk = repo.Update(model);

            //var ls = new List<Student>();
            //ls.Add(new Student("张三", 1, 1));
            //ls.Add(new Student("王二", 0, 2));
            //ls.Add(new Student("刘五", 1, 3));

            //var inb = repo.InsertBatch("insert into " + repo.TableName + " values(@Name,@Sex,@ClassId)", ls);

            using (UnitOfWork uow = new UnitOfWork())
            {
                var studentRepo = uow.Repository<Student>();
                var model = new Student("啊啊", 1, 1);
                var sid = studentRepo.Insert(model);

                var classRepo = uow.Repository<MClass>();
                var cid = classRepo.Insert(new MClass("五年级"));
                if (sid > 0 && cid < 0)
                {
                    uow.Commit();
                }
                else
                {
                    uow.Rollback();
                }
            }
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }

        static void Dos()
        {
            var Repository = new Repository<UserInfo>();
            List<UserInfo> ls = new List<UserInfo>();
            ls.Add(new UserInfo() { Name = "Monkey D. Luffy", Phone = "15878451111", Password = "12345678", Sex = 1, UserName = "Luffy", CreateTime = DateTime.Now, Enable = 1 });
            ls.Add(new UserInfo() { Name = "索隆", Phone = "13355526663", Password = "12345678", Sex = 1, UserName = "Zoro", CreateTime = DateTime.Now, Enable =1 });
            ls.Add(new UserInfo() { Name = "娜美", Phone = "15878451111", Password = "12345678", Sex = 0, UserName = "Nami", CreateTime = DateTime.Now, Enable = 1 });
            ls.Add(new UserInfo() { Name = "山治", Phone = "17755602229", Password = "12345678", Sex = 1, UserName = "Sanji", CreateTime = DateTime.Now, Enable = 1 });
            ls.Add(new UserInfo() { Name = "乌索普", Phone = "14799995555", Password = "12345678", Sex = 1, UserName = "Usopp", CreateTime = DateTime.Now, Enable = 1 });
            ls.Add(new UserInfo() { Name = "乔巴", Phone = "18966660000", Password = "12345678", Sex = 1, UserName = "Chopper", CreateTime = DateTime.Now, Enable = 1 });
            ls.Add(new UserInfo() { Name = "罗宾", Phone = "13122227878", Password = "12345678", Sex = 0, UserName = "Robin", CreateTime = DateTime.Now, Enable = 1 });
            ls.Add(new UserInfo() { Name = "弗兰奇", Phone = "15962354412", Password = "12345678", Sex = 1, UserName = "Franky", CreateTime = DateTime.Now, Enable = 1 });
            ls.Add(new UserInfo() { Name = "布鲁克", Phone = "14322221111", Password = "12345678", Sex = 1, UserName = "Brook", CreateTime = DateTime.Now, Enable = 1 });
            ls.Add(new UserInfo() { Name = "甚平", Phone = "15655479960", Password = "12345678", Sex = 1, UserName = "Jinbe", CreateTime = DateTime.Now, Enable = 1 });
            Repository.InsertBatch("  INSERT INTO dbo.T_User( UserName ,Password ,Name ,Sex,Phone ,Enable ,CreateTime) VALUES  ( @UserName ,@Password ,@Name ,@Sex ,@Phone ,@Enable ,@CreateTime)", ls);
        }
    }

    [Table("T_User")]
    public class UserInfo : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public int Sex { get; set; }

        public int Enable { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
