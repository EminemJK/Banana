using Banana.Uow.Models;
using System;

namespace DotNetCore_TestApp
{
    [Table("t_user")]
    public class UserModel : IEntity
    {
        [Key]
        public int id { get; set; }

        public string username { get; set; }
        public string password { get; set; }

        public string name { get; set; }

        public string phone { get; set; }

        public int sex { get; set; }

        public int enable { get; set; }

        public DateTime createtime { get; set; }

        public override string ToString()
        {
            return $"Id：{id} UserName：{username} Name：{name} Phone：{phone}";
        }
    }
}
