using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EscolaInfoSys.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<FormGroup> FormGroups { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Absence> Absences { get; set; }
        public DbSet<StaffMember> StaffMembers { get; set; }
    }
}
