using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var aa = context.Destinations.Where(a => a.DestinationId == 1).ToList();
                context.SaveChanges();
                //context.Destinations.AddOrUpdate
            }
            Console.WriteLine("OK");

        }
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
    }
}
