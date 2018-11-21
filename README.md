# Banana.Uow
![image](https://github.com/EminemJK/Banana/blob/master/Banana/Doc/banana_logo.ico)

Coder：EminemJK

Nuget：https://www.nuget.org/packages/Banana.Uow/

### 项目介绍
基于Dapper二次封装的仓储、工作单元

### 使用说明
#### 注册链接
``` csharp
 ConnectionBuilder.ConfigRegist("strConn", Banana.Uow.Models.DBType.SqlServer);
```
#### 模型
引入命名空间：
``` csharp
using Banana.Uow.Models;
using Dapper.Contrib.Extensions;
```
创建模型：
``` csharp
  [Table("T_Student")]
   public class Student : BaseModel
   {
       [Key]
       public int Id { get; set; }
       public string Name { get; set; }
       public int Sex { get; set; }
       public int ClassId { get; set; }
        [Computed]
       public DateTime Createtime { get; set; }
   }

特性说明：
· Table：指定实体对应地数据库表名，如果类名和数据库表名不同，需要设置
· Key：指定此列为自动增长主键
· ExplicitKey：指定此列为非自动增长主键（例如guid，字符串列）
· Computed：计算属性，此列不作为更新
· Write：指定列是否可写
```
#### 仓储使用
``` csharp
   var repo = new Repository<Category>();
   //查询单个
   var model = repo.Query(7);
   //查询列表
   var list = repo.QueryList("where ParentNamePath like @ParentNamePath", new { ParentNamePath = "%,电气设备,%" });
   //分页查询
   var page = repo.QueryList(1, 10, "where ParentNamePath like @ParentNamePath", new { ParentNamePath = "%,电气设备,%" }, "id", false);

    //删除
    boo b = repo.Delete(model);
    boo b = repo.Delete(new Category(){ Id =7 });

    //更新
    bool model = repo.Update(model);

    //插入
    int id = (int)repo.Insert(new Category() { Name = "EminemJK" });

    //批量插入
    bool b = repo.InsertBatch(sql,List);

    //执行语句
    int res =repo.Execute(sql,param);
```
#### 工作单元
``` csharp
using (UnitOfWork uow = new UnitOfWork())
           {
               var studentRepo = uow.Repository<Student>();
               var model = new Student("啊啊", 1, 1);
               var sid = studentRepo.Insert(model);

               var classRepo = uow.Repository<MClass>();
               var cid = classRepo.Insert(new MClass("五年级"));
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
# Banana.Utility
## 公用库
#### RedisUtils        —— 基于StackExchange.Redis二次封装
#### PinYin            —— 拼音帮助类
#### JavaDate          —— 时间戳
#### ModelConvertUtil  —— 模型拷贝
#### PagingUtil        —— 分页
#### HttpHelper        —— Get & Post
#### EnumDescription   —— 枚举特性说明
👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍👍
