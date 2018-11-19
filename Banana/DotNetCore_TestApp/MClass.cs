using Banana.Uow.Models;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore_TestApp
{
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
