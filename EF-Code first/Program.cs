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
using System.Data.Entity.Core.Objects;

namespace EF_Code_first
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var context = DB.Instance)
            {
                
                //禁止自动检测跟踪功能。
                context.Configuration.AutoDetectChangesEnabled = false;
                //context.Database.Delete();
                context.Database.CreateIfNotExists();
                ////如果实体类有变化，那么就重新生成一下数据库(DropCreateDatabaseIfModelChanges)
                Database.SetInitializer(new DropCreateDatabaseIfModelChanges<testContext>());

                //新增实体对象，并加入到context上下文容器中，设置其跟踪状态为Added
                User user = new EF_Code_first.User() { Phone = 123456, UserName = "ghm" };
                //context.Entry<User>(user).State = EntityState.Added;
                //context.SaveChanges();
                //新增实体对象，使用DbSet Add()方法
                //user = new User() { Phone = 123456, UserName = "wu" };
                //context.Users.Add(user);
                //context.SaveChanges();

                user = context.Users.Find(2);
                //会更新实体所有的字段，如果不设置则为null
                user = new User() { UserId = 2 };
                
                user.UserName = "a";
                Exists<User>(user);
                context.Entry<User>(user).State = EntityState.Modified;
                var en = context.Entry<User>(user);
                context.SaveChanges();
                user = context.Users.Find(2);
                DbChangeTracker dt = context.ChangeTracker;
                var users = dt.Entries<User>();

                user=context.Users.FirstOrDefault(o=>o.UserName=="a");
                user.UserName = "ghm";
                context.SaveChanges();
                //更新实体对象，使用DbSet查找指定的对象，如果上下文中存在，则直接返回，否则到数据库中查找并返回后、加入到上下文跟踪对象中
                user.UserName = "gonghaim";
                //context.SaveChanges();
                var user2 = context.Users.Find(2);
                user = new User() { UserId = 1 };
                if (Exists<User>(user))
                {

                }
                context.Users.Attach(user);
                ///指定更新的字段
                user.UserName = "fengpei";
                context.Entry<User>(user).Property("UserName").IsModified = true;
                context.SaveChanges();

                //Exists<User>(user);
                user = new User() { UserId = 1 };
                user.UserName = "fengpei444";
                
                var entry=context.Entry<User>(user);
                entry.State = EntityState.Unchanged;
                entry.Property("UserName").IsModified = true;
                context.SaveChanges();

                user = new User() { UserId = 1 };
                Exists<User>(user);
                context.Users.Attach(user);
                context.Users.Remove(user);
                context.SaveChanges();

                user = context.Users.Find(1);
                context.Users.Remove(user);
                context.SaveChanges();

                user = new User() { UserId = 1 };
                context.Entry(user).State = EntityState.Deleted;
                context.SaveChanges();
                ///使用EntityFramework.Extended插件
                //删除
                context.Students.Where(a => a.Id == 1).Delete();
                context.SaveChanges();

                //分页
                var q1 = context.Users.FutureCount();//sum值
                var q2 = context.Users.Skip(10).Take(10).Future();

                //一次查询
                var data = q2.ToList();
                var count = q1.Value;

                context.SaveChanges();

                //ef删除操作，针对有主键的表
                Student stud = new Student() { Id = 1 };
                context.Students.Attach(stud);
                context.Students.Remove(stud);
                context.SaveChanges();

                user = context.Users.First(s => s.UserId == 1);
                context.Users.Remove(user);
                context.SaveChanges();

                user = new User() { UserId = 1 };
                context.Entry<User>(user).State = EntityState.Deleted;
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
        private static  bool Exists<TEntity>(TEntity entity) where TEntity : class
        {
            ObjectContext oc = ((IObjectContextAdapter)DB.Instance).ObjectContext;
            var name = oc.CreateObjectSet<TEntity>().EntitySet.Name;
            var entityKey = oc.CreateEntityKey(name, entity);
            Object foundEntity;
            var exists = oc.TryGetObjectByKey(entityKey, out foundEntity);
            if (exists)
            {
                oc.Detach(foundEntity);
            }
            return exists;
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
