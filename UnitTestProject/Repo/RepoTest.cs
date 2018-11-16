using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Banana.Uow; 
using Dapper.Contrib.Extensions;
using Banana.Uow.Models;

namespace UnitTestProject.Repo
{
    [TestClass]
    public class RepoTest
    {
        static string strConn = @"Data Source=.;Initial Catalog = LoadDB;User ID=sa;Password =mimashi123";

        [TestMethod]
        public void TestQuery()
        {
            reg();
            var repo = new Repository<Student>();
            var model = repo.Query(2);
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void TestInsert()
        {
            reg();
            var repo = new Repository<Student>();
            var id = repo.Insert(new Student("EminemJK",1));
            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        public void TestDelete()
        {
            reg();
            var repo = new Repository<Student>();
            var model = repo.Query(2);
            var id = repo.Delete(model);
            Assert.IsTrue(id);
        }

        [TestMethod]
        public void TestUpdate()
        {
            reg();
            var repo = new Repository<Student>();
            var model = repo.Query(3);
            model.Name = "Banana"+DateTime.Now.ToShortTimeString();
            var id = repo.Update(model);
            Assert.IsTrue(id);
        }

        private static void reg()
        {
            ConnectionBuilder.ConfigRegist(strConn, Banana.Uow.Models.DBType.SqlServer);
        }
    }




    [Table("T_Student")]
    public class Student:BaseModel
    {
        [Key]
        public int Id { get; set; }


        public string Name { get; set; }

        public int Sex { get; set; }

        public Student()
        { }

        public Student(string name, int i)
        {
            this.Name = name;
            this.Sex = i;
        }

    }
}
