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

        [Column("UserName")]
        public string UserNameFiel { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public int Sex { get; set; }

        public int Enable { get; set; }

        [ExceptUpdate]
        public DateTime CreateTime { get; set; }

        public override string ToString()
        {
            return $"Id：{Id} UserName：{UserNameFiel} Name：{Name} Phone：{Phone}";
        }
    }
}
