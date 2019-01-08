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
            repoUserInfo = new Repository<UserInfo>();
            repoUserInfo = new Repository<UserInfo>();
            repoUserInfo = new Repository<UserInfo>();
            repoUserInfo = new Repository<UserInfo>();
            repoUserInfo = new Repository<UserInfo>();
            repoUserInfo = new Repository<UserInfo>();
            repoUserInfo = new Repository<UserInfo>();
            repoUserInfo = new Repository<UserInfo>();
            repoUserInfo = new Repository<UserInfo>();
            var where = repoUserInfo.Join<Student>((u,s)=>u.Name==s.Id ).And(s=>s.Name.Contains("123"));

        }

        public static void TestSQLServer()
        {
            ConnectionBuilder.ConfigRegist("Data Source=.;Initial Catalog = AdminLTE.Net.DB;User ID=sa;Password =mimashi123", DBType.SqlServer2012);
            var repoUserInfo = new Repository<UserInfo>();
            var getls = repoUserInfo.Where(u => u.Id > 3);
        }
    }
}
