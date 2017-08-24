using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Extensions;
using EF_Code_first;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Remoting.Messaging;

namespace EF_Code_first
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var context = DB.Instance)
            {
                context.Database.CreateIfNotExists();
                ////如果实体类有变化，那么就重新生成一下数据库(DropCreateDatabaseIfModelChanges)
                Database.SetInitializer(new DropCreateDatabaseIfModelChanges<testContext>());
                Student stu = new Student() { Age=5};
                context.Entry<Student>(stu).State = EntityState.Added;
                context.SaveChanges();

                stu = new Student() { Age = 5 };
                context.Students.Add(stu);
                context.SaveChanges();

                var student = context.Students.First(s=>s.Id==1);
                student.UserName = "ghm";
                context.SaveChanges();

                student = new Student() { Id = 1 };
                context.Students.Attach(student);
                student.UserName = "ghm";
                context.Entry<Student>(student).State = EntityState.Modified;
                context.SaveChanges();

                student = new Student() { Id = 1 };
                student.UserName = "ghm";
                context.Entry<Student>(student).Property("UserName").IsModified = true;
                context.SaveChanges();

                
                ///使用EntityFramework.Extended插件
                //删除
                context.Students.Where(a => a.Id == 1).Delete();
               
                //分页
                var q1 = context.Students.FutureCount();//sum值
                var q2 = context.Students.Skip(10).Take(10).Future();

                //一次查询
                var data = q2.ToList();
                var count = q1.Value;

                context.SaveChanges();

                //ef删除操作，针对有主键的表
                Student stud = new Student() { Id = 1 };
                context.Students.Attach(stud);
                context.Students.Remove(stud);
                context.SaveChanges();

                student = context.Students.First(s => s.Id == 1);
                context.Students.Remove(student);
                context.SaveChanges();

                student = new Student() { Id = 1 };
                context.Entry<Student>(student).State = EntityState.Deleted;
                context.SaveChanges();

            

                ////自己创建对象，然后放入EF容器，然后删除。
                //Destination del = new Destination() { DestinationId = 1 };
                //DbEntityEntry<Destination> dest = context.Entry(del);
                //dest.State = EntityState.Deleted;
                context.Students.AsNoTracking();
                
                context.SaveChanges();

             
            }
            Console.WriteLine("OK");

        }
      

        public void Modify(@Class entity, Student stu)
        {
            using (var context = DB.Instance)
            {
                context.@Classs.Add(entity);

                //修改Customer
                context.Entry(entity).State = EntityState.Modified;

                //新增CustomerAddress
                entity.Stus.Add(stu);

                //删除CustomerAddress
                context.Entry(stu).State = EntityState.Deleted;
                context.SaveChanges();
            }

        }

        //实现实体的部分更新
        public void Modify<T>(T t1, T t2) where T :class, new()
        {
            using (var context = DB.Instance)
            {
                if (context.Entry(t1).State != EntityState.Unchanged)
                    context.Entry(t1).State = EntityState.Unchanged;
                context.Entry(t1).CurrentValues.SetValues(t2);
                context.SaveChanges();
            }

        }

        //添加实体方法Entity Framwork中Dbset<T>.Add()与EntityState.Added区别:
        //使用Dbset<T>.Add(t)时，如果t对象中有一个导航属性，且它不为空，那么EF总会在导航属性表中新建一个对应的数据。而使用EntityState.Added方法时不会出现上述的问题
    }







}
