using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
           : base("DataContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, flexiCoreLibrary.Migrations.Configuration>("DataContext"));
        }

        public DbSet<Function> Functions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleFunction> RoleFunctions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Approval> Approvals { get; set; }
        public DbSet<ApproverEmail> ApproverEmails { get; set; }
        public DbSet<SecondLevelApproverEmail> SecondLevelApproverEmails { get; set; }
        public DbSet<ShiftConfig> ShiftConfigs { get; set; }
        public DbSet<StaffShift> StaffShifts { get; set; }
        public DbSet<ShiftSwap> ShiftSwaps { get; set; }
        public DbSet<ShiftSwapRequest> ShiftSwapRequests { get; set; }
        public DbSet<FinancialYear> FinancialYears { get; set; }
        public DbSet<FinancialYearMonth> FinancialYearMonths { get; set; }
        public DbSet<FinancialYearMonthDay> FinancialYearMonthDays { get; set; }
        public DbSet<StaffLeave> StaffLeaves { get; set; }
        public DbSet<Model.Task> Tasks { get; set; }
        public DbSet<TaskStaff> TaskStaffs { get; set; }
        public DbSet<TaskUpdate> TaskUpdates { get; set; }
        public DbSet<SignInOut> SignInOuts { get; set; }
        public DbSet<StaffShiftFeed> StaffShiftFeeds { get; set; }
        public DbSet<FeedSetting> FeedSettings { get; set; }
        public DbSet<Report> AbsentReports { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure domain classes using Fluent API here
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
