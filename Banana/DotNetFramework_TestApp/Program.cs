using Banana.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetFramework_TestApp
{
   
    class Program
    {
        private static string strConn = @"Data Source=.;Initial Catalog = LoadDB;User ID=sa;Password =mimashi123";
        static void Main(string[] args)
        {
            ConnectionBuilder.ConfigRegist(strConn, Banana.Uow.Models.DBType.SqlServer);

            var repo = new Repository<Category>();
            var data = repo.QueryList(" ParentNamePath like @ParentNamePath", new { ParentNamePath = "%,电气设备,%" });
        }
    }
}
