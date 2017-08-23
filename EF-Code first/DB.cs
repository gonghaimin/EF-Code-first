using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace EF_Code_first
{
    public class DB
    {
        static string key = "DbContext-Single";
        public static testContext Instance
        {
            get
            {
                testContext temp = CallContext.GetData(key) as testContext;
                if (temp == null)
                {
                    temp = new testContext();
                    CallContext.SetData(key, temp);
                }
                return temp;
            }
            private set { }
        }
    }
    public class testContext : DbContext
    {
        public testContext() : base("name=testContext")
        {
        }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<@Class> @Classs { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonPhoto> PersonPhotos { get; set; }
    }

    /// <summary>
    /// T为引用类型，且有个无参的构造函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseDAL<T> where T : class, new()
    {
        static testContext context = DB.Instance;

        public int Add(T model)
        {
            context.Set<T>().Add(model);
            return context.SaveChanges();
        }
        public int Del(T model)
        {
            //Attach之后，实体的对象为 EntityState.Unchanged
            context.Set<T>().Attach(model);
            context.Set<T>().Remove(model);
            return context.SaveChanges();
        }
        //为避免先查询数据库，可以直接将 被修改的实体对象 添加到 EF中管理(此时为附加状态Attached)，并手动设置其为未修改状态(Unchanged)，同时设置被修改的实体对象 的 包装类对象 对应属性为修改状态。
        public int Modify(T model, params string[] proName)
        {
            DbEntityEntry<T> entityEntry = context.Entry<T>(model);
            entityEntry.State = EntityState.Unchanged;
            foreach (string s in proName)
            {
                entityEntry.Property(s).IsModified = true;
            }
            return context.SaveChanges();
        }
        public void update()
        {
            //1.先查询要修改的原数据  
            //2.设置修改后的值
            //3.跟新到数据库SaveChanges();
        }
        public List<T> GetPageList<TKey>(Expression<Func<T, TKey>> orderLambda, Expression<Func<T, bool>> whereLambda, int pagesize, int pageIndex)
        {
            return context.Set<T>().Where(whereLambda)
                .OrderBy(orderLambda)
                .Skip((pageIndex - 1) * pagesize)
                .Take(pagesize)
                .Select(u => u).ToList();
        }

    }
}
