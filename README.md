<div align=center><img width = '150' height ='150' src ="https://raw.githubusercontent.com/EminemJK/Banana/master/Banana/Doc/banana_logo.png"/></div>

> Developer: Lio.Huang

| Package | NuGet Stable | NuGet Pre-release | Downloads |
| ------- | ------------ | ----------------- | --------- |
| [Banana.Uow](https://www.nuget.org/packages/Banana.Uow/) | [![Banana.Uow](https://img.shields.io/nuget/v/Banana.Uow.svg)](https://www.nuget.org/packages/Banana.Uow/)  | [![Banana.Uow](https://img.shields.io/nuget/vpre/Banana.Uow.svg)](https://www.nuget.org/packages/Banana.Uow/) | [![Banana.Uow](https://img.shields.io/nuget/dt/Banana.Uow.svg)](https://www.nuget.org/packages/Banana.Uow/) |
| [Banana.Utility](https://www.nuget.org/packages/Banana.Utility/) | [![Banana.Utility](https://img.shields.io/nuget/v/Banana.Utility.svg)](https://www.nuget.org/packages/Banana.Utility/)  | [![Banana.Utility](https://img.shields.io/nuget/vpre/Banana.Utility.svg)](https://www.nuget.org/packages/Banana.Utility/) | [![Banana.Utility](https://img.shields.io/nuget/dt/Banana.Utility.svg)](https://www.nuget.org/packages/Banana.Utility/) |

# Banana.Uow
### 项目介绍
👉[English documentation](https://github.com/EminemJK/Banana/wiki)

基于Dapper封装的仓储、工作单元，支持SQL Server, MySQL, Sqlite，Postgresql，Oracle...

### 一、使用说明
#### 1. 注册链接
``` csharp
 ConnectionBuilder.ConfigRegist("strConn", Banana.Uow.Models.DBType.SqlServer);
```
#### 2. 模型
引入命名空间：
``` csharp
using Banana.Uow.Models;
```
创建模型：
``` csharp
   [Table("T_Student")]
   public class Student : IEntity
   {
      // if Oracle db
     //[Key("user_sequence")]
       [Key]
       public int Id { get; set; }
       public string Name { get; set; }
       public int Sex { get; set; }
       public int ClassId { get; set; }
       [Computed]
       public DateTime Createtime { get; set; }
       [Column("UserNameFiel")]
       public string UserName { get; set; }
   }
```
特性说明：
* Table：指定实体对应地数据库表名，如果类名和数据库表名不同，需要设置
* Key：指定此列为自动增长主键（oracle设置序列名称即可）
* ExplicitKey：指定此列为非自动增长主键（例如guid，字符串列）
* Computed：计算属性，此列不作为更新
* Write：指定列是否可写
* Column：指定列名
#### 3. 仓储使用
#### 3.2 增删改查
``` csharp
    var repo = new Repository<Student>();
    //查询单个
    var model = repo.Query(7);

    //查询列表
    var list = repo.QueryList("Name like @Name", new { Name = "%EminemJK%" });

    //分页查询
    var page1 = repo.QueryList(1,5);
    var page2 = repo.QueryList(2,5);
    … …
    var page0 = repo.QueryList(1, 10, "ID>@Id", new { Id = 2 }, "id", false);

    //删除
    boo b = repo.Delete(model);
    boo b = repo.Delete(new Category(){ Id =7 });

    //更新
    bool model = repo.Update(model);

    //插入
    int id = (int)repo.Insert(new Student() { Name = "EminemJK",… … });

    //批量插入
    bool b = repo.InsertBatch(sql,List);

    //执行语句
    int res = repo.Execute(sql,param);
```
#### 3.2 多数据库时
``` csharp
    var dbA = "dbStoreA";
    var dbB = "dbStoreA";
    ConnectionBuilder.ConfigRegist("strConnA", DBType.SqlServer2012, dbA);
    ConnectionBuilder.ConfigRegist("strConnB", DBType.SqlServer2012, dbB);
    
    var userInfo_ReadRepo = new Repository<Student>(dbA); 
    var userInfo_WirteRepo = new Repository<Student>(dbA);
    Your code ……
```
### 二、工作单元
``` csharp
using (UnitOfWork uow = new UnitOfWork())
{
       var studentRepo = uow.GetRepository<Student>();
       var model = new Student("EminemJK");
       var sid = studentRepo.Insert(model);

       var classRepo = uow.GetRepository<MClass>();
       var cid = classRepo.Insert(new MClass("Fifth Grade"));
       if (sid > 0 && cid > 0)
       {
            uow.Commit();
       }
       else
       {
            uow.Rollback();
       }
}
```
多数据库时
``` csharp
using (UnitOfWork uow = new UnitOfWork(IDbConnection context))
{
      Your code
}
```
OR
``` csharp
using (UnitOfWork uow = new UnitOfWork("dbKey"))
{
      Your code
}
```
# Banana.Utility
### 公用库 Utility

| Name| Use |
| ------- | ------- |
| RedisUtils | 基于StackExchange.Redis封装 |
| PinYin  | 拼音帮助类|
| JavaDate | 时间戳 |
| ModelConvertUtil | 模型拷贝 |
| PagingUtil | 分页 |
| HttpHelper | easy Get & Post |
| EnumDescription | 枚举特性说明 |

## To Be Continued
👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍

-------
[Apache License](http://www.apache.org/licenses/)