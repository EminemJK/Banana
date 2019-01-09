using Banana.Uow;
using Banana.Uow.Lambda;
using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore_TestApp
{
    class ExpressionTests
    {
        public static void TestCreateSQL()
        {
            ConnectionBuilder.ConfigRegist("Data Source=.;Initial Catalog = AdminLTE.Net.DB;User ID=sa;Password =mimashi123", DBType.SqlServer2012);
            var repoUserInfo = new Repository<UserInfo>();
            var ls = repoUserInfo.QueryList(u => u.Id > 0 && u.Name.Contains("E"));

            var data = new List<object> { 1, 2, 3, 4, 5, 6, 7 };
            var whin = repoUserInfo.WhereIsIn(x => x.Id, data);

            var join = repoUserInfo.Create();
            join.Join<Student>((u, s) => u.Name == "4" && u.Id > s.Sex);

        }

        public static void TestSQLServer()
        {
            ConnectionBuilder.ConfigRegist("Data Source=.;Initial Catalog = AdminLTE.Net.DB;User ID=sa;Password =mimashi123", DBType.SqlServer2012);
            var repoUserInfo = new Repository<UserInfo>(); 
        }
    }
}
