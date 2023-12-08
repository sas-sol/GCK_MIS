using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MeherEstateDevelopers.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class MyUser : IdentityUser<long, MyLogin, MyUserRole, MyClaim>
    {
        public string CNIC { get; set; }

        public string BloodGroup { get; set; }

        public string PermanentAddress { get; set; }

        public string Address { get; set; }

        public string EmergencyContact { get; set; }

        public string RelationShip { get; set; }

        public string Name { get; set; }
        public string Designation { get; set; }
        public string ReportingTo { get; set; }
        public string ProfileImage { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<bool> FirstChangePassword { get; set; }
    }
    public class MyUserRole : IdentityUserRole<long> { }
    public class MyRole : IdentityRole<long, MyUserRole> { }
    public class MyClaim : IdentityUserClaim<long> { }
    public class MyLogin : IdentityUserLogin<long> { }

    public class ApplicationDbContext : IdentityDbContext<MyUser, MyRole, long, MyLogin, MyUserRole, MyClaim>
    {
        public ApplicationDbContext()
            : base("NewIdentity")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Map entities to there table
            modelBuilder.Entity<MyUser>().ToTable("Users");
            modelBuilder.Entity<MyUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<MyRole>().ToTable("Roles");
            modelBuilder.Entity<MyClaim>().ToTable("UserClaims");
            modelBuilder.Entity<MyLogin>().ToTable("UserLogins");
            //Set auto incerament property
            //modelBuilder.Entity<MyUser>.prop
            modelBuilder.Entity<MyUser>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MyRole>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<MyClaim>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<MeherEstateDevelopers.Models.MyUserRole> MyUserRoles { get; set; }



    }
}