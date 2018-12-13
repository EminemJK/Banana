using Banana.Uow.Models;
using System;

namespace DotNetCore_TestApp
{
    [Table("T_User")]
    public class UserModel_Oracle : IEntity
    {
        [Key("user_sequence")]
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public int Sex { get; set; }

        public int Enable { get; set; }

        public DateTime CreateTime { get; set; }

        public override string ToString()
        {
            return $"Id：{Id} UserName：{UserName} Name：{Name} Phone：{Phone}";
        }
    }
}
