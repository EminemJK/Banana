using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore_TestApp
{
    [Table("T_Category")]
    public class Category:BaseModel
    {
        [Key]
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }

        public int Sort { get; set; }

        public string ParentIdPath { get; set; }

        public string ParentNamePath { get; set; }

        [Computed]
        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
