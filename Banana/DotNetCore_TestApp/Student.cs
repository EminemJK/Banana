using System;
using Banana.Uow.Models;

namespace DotNetCore_TestApp
{
    [Table("T_Student")]
    public class Student : IEntity
    {
        [ExplicitKey]
        [Column("uuid")]
        public string Id { get; set; } 
        public string Name { get; set; }

        [Column("phone")]
        public string LinkPhone { get; set; }

        public int Sex { get; set; }
 
        public DateTime CreateTime { get; set; }

        public override string ToString()
        {
            return $"Id：{Id} Name：{Name} Phone：{LinkPhone}";
        }
    }
}

/*
 CREATE TABLE T_Student
 (
	uuid VARCHAR(32) PRIMARY KEY, --guid
	NAME NVARCHAR(50),
	Sex INT,
	Phone VARCHAR(11),
	CreateTime DATE 
 ) 
 */
