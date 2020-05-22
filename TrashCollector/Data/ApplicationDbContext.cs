using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrashCollector.Models;

namespace TrashCollector.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<WeekDay> WeekDays { get; set; }
        public DbSet<WeeklyPickup> WeeklyPickups { get; set; }
        public DbSet<DatePickup> DatePickups { get; set; }
        public DbSet<SuspendPickup> SuspendPickups { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>()
                .HasData(
                        new IdentityRole
                        {
                            Name = "Customer",
                            NormalizedName = "CUSTOMER"
                        },
                        new IdentityRole
                        {
                            Name = "Employee",
                            NormalizedName = "EMPLOYEE"
                        }
                );

            builder.Entity<WeekDay>()
                .HasData(
                    new WeekDay { WeekDayId = 1, Day = "Monday" },
                    new WeekDay { WeekDayId = 2, Day = "Tuesday" },
                    new WeekDay { WeekDayId = 3, Day = "Wednesday" },
                    new WeekDay { WeekDayId = 4, Day = "Thursday" },
                    new WeekDay { WeekDayId = 5, Day = "Friday" },
                    new WeekDay { WeekDayId = 6, Day = "Saturday" },
                    new WeekDay { WeekDayId = 7, Day = "Sunday" }
                );
        }
    }
}
