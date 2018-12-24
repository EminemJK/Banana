using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore_TestApp
{
    [Table("T_User")]
    public class UserInfo : IEntity
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

        public string HeaderImg { get; set; }

        public override string ToString()
        {
            return $"Id：{Id} UserName：{UserName} Name：{Name} Phone：{Phone}";
        }
    }
}
