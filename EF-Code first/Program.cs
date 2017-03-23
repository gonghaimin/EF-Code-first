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

namespace EF_Code_first
{
    class Program
    {
        static void Main(string[] args)
        {
            var d = DateTime.Now.Date.ToString("yyyyMM");
            var destination = new Destination
            {
                Country = "Indonesia",
                Description = "EcoTourism at its best in exquisite Bali",
                Name = "Bali"
            };
            using (var context = new Context())
            {
                var aa = context.Destinations.Where(a => a.DestinationId == 1);


                ///使用EntityFramework.Extended插件
                //更新
                context.Destinations.Where(a => a.DestinationId > 1).Update(a => new Destination() { Name = "ghm" });
                //删除
                context.Destinations.Where(a => a.DestinationId == 1).Delete();

                //分页
                var q1 = aa.FutureCount();
                var q2 = aa.Skip(10).Take(10).Future();

                //一次查询
                var data = q2.ToList();
                var count = q1.Value;

                context.SaveChanges();
                //context.Destinations.AddOrUpdate


                //ef删除操作，针对有主键的表
                //自己创建一个对象，然后附加，然后删除
                Destination de = new Destination() { DestinationId = 1 };
                context.Destinations.Attach(de);
                context.Destinations.Remove(de);
                context.SaveChanges();

                //自己创建对象，然后放入EF容器，然后删除。
                Destination del = new Destination() { DestinationId = 1 };
                DbEntityEntry<Destination> dest = context.Entry(del);
                dest.State = EntityState.Deleted;

                context.SaveChanges();


            }
            Console.WriteLine("OK");

        }

        //使用SqlBulkCopy来批量插入数据，这样很大地提高性能
        public static void BulkInsert<T>(SqlConnection conn, string tableName, IList<T> list)
        {
            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();
                var props = TypeDescriptor.GetProperties(typeof(T), new Attribute[] { new DatabaseTableColumnAttribute() })
                    //Dirty hack to make sure we only have system data types 
                    //i.e. filter out the relationships/collections
                    .Cast<PropertyDescriptor>()
                    .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                    .ToArray();

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];
                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }

        //更新实体时操作导航属性
        //用一个例子来说明在更新实体同时如何对导航属性进行操作吧。假设有两个类型
        //那如何在更新Customer的同时Add一个CustomerAddress并且Delete一个CustomerAddress呢?关键一点就是要让EntityFramework的Change Tracker知道有CustomerAddress的存在，只需对Customer增加一个Add操作就行了，代码如下
        public void Modify(Customer entity, CustomerAddress address)
        {
            using (var context = new Context())
            {
                context.Customers.Add(entity);

                //修改Customer
                context.Entry(entity).State = EntityState.Modified;

                //新增CustomerAddress
                entity.CustomerAddresses.Add(address);

                //删除CustomerAddress
                context.Entry(address).State = EntityState.Deleted;
                context.SaveChanges();
            }

        }

        //实现实体的部分更新
        //public void Modify<T>(T t1, T t2)where T: new()
        //{
        //    using (var context = new Context())
        //    {
        //        if (context.Entry(t1).State != EntityState.Unchanged)
        //            context.Entry(t1).State = EntityState.Unchanged;
        //        context.Entry(t1).CurrentValues.SetValues(t2);
        //        context.SaveChanges();
        //    }

        //}
    }


    public class Destination
    {
        public int DestinationId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public byte[] Photo { get; set; }
        public List<Lodging> Lodgings { get; set; }
    }

    public class Lodging
    {
        public int LodgingId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public bool IsResort { get; set; }
        public Destination Destination { get; set; }
    }
    public class Context : DbContext
    {
        public Context() : base("name=test")
        {

        }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Lodging> Lodgings { get; set; }
        public DbSet<CustomerAddress> CustomerAddresss { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }

    public class Customer
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public IList<CustomerAddress> CustomerAddresses { get; set; }
    }

    public class CustomerAddress
    {
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

}
