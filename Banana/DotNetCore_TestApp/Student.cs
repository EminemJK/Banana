using Banana.Uow.Models;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore_TestApp
{
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
}
