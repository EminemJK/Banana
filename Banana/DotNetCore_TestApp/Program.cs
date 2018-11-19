using System;
using Banana.Uow;
using Dapper.Contrib.Extensions;
using Banana.Uow.Models;
using System.Collections.Generic;

namespace DotNetCore_TestApp
{
    class Program
    {
        static string strConn = @"Data Source=.;Initial Catalog = LoadDB;User ID=sa;Password =mimashi123";
        static void Main(string[] args)
        {
            ConnectionBuilder.ConfigRegist(strConn, Banana.Uow.Models.DBType.SqlServer);

            //var repo = new Repository<Student>();
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
    }


    [Table("T_Student")]
    public class Student : BaseModel
    {
        [Key]
        public int Id { get; set; }


        public string Name { get; set; }

        public int Sex { get; set; }

        public int ClassId { get; set; }

        public Student()
        { }

        public Student(string name, int i, int classId)
        {
            this.Name = name;
            this.Sex = i;
            this.ClassId = classId;
        }

    }

    [Table("T_Class")]
    public class MClass : BaseModel
    {
        [Key]
        public int Id { get; set; }


        public string Name { get; set; }
        public MClass() { }

        public MClass(string name)
        {
            this.Name = name;
        }
    }
}
