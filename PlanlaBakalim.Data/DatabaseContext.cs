using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options){}
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
   
      
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<BusinessService> Services { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<BusinessWorkingHour> BusinessWorkingHours { get; set; }
        public DbSet<BusinessAdress> BusinessAdresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentService> AppointmentServices { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<UserFavorites> UserFavorites { get; set; }

        }
}
