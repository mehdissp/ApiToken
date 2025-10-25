using Microsoft.EntityFrameworkCore;
using JWTApi.Domain.Entities;
using System.Reflection.Metadata;

namespace JWTApi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Todo> Todos => Set<Todo>();
        public DbSet<TodoStatus> TodoStatuses => Set<TodoStatus>();

        
        public DbSet<TodoTag> TodoTags => Set<TodoTag>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Reminder> Reminders => Set<Reminder>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<IpLock> IpLocks { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<UserPackage> UserPackages { get; set; }
        public DbSet<ExtraProject> ExtraProjects { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Comment> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Attachment>()
    .HasKey(d => d.Id);

            modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
            modelBuilder.Entity<LoginAttempt>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.IPAddress).HasMaxLength(50).IsRequired();
                b.Property(x => x.Username).HasMaxLength(200);
                b.Property(x => x.Reason).HasMaxLength(200);
                b.HasIndex(x => new { x.UserId, x.AttemptTime });
                b.HasIndex(x => x.AttemptTime);
            });

            modelBuilder.Entity<IpLock>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.IPAddress).HasMaxLength(50).IsRequired();
                b.HasIndex(x => x.IPAddress).IsUnique();
            });
            modelBuilder.Entity<Role>(b =>
            {
            
                b.Property(x => x.Name).HasMaxLength(250).IsRequired();
              
            });
            // ---------------- User ----------------
            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Name).HasMaxLength(100).IsRequired();
                b.Property(u => u.Email).HasMaxLength(200);
                b.Property(u => u.FullName).HasMaxLength(200);
                b.Property(u => u.MobileNumber).HasMaxLength(200).IsRequired();
                b.Property(u => u.PasswordHash).IsRequired();
                b.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.Property(r => r.IsActive).HasDefaultValue(true);
                b.HasMany(u => u.Todos)
                 .WithOne(t => t.User)
                 .HasForeignKey(t => t.UserId);

                b.HasMany(u => u.ExtraProjects)
                 .WithOne(p => p.User)
                 .HasForeignKey(p => p.UserId);
               
                b.HasMany(p => p.UserPackages)
               .WithOne(t => t.User)
               .HasForeignKey(t => t.UserId);

                b.HasMany(u => u.Projects)
                 .WithOne(p => p.User)
                 .HasForeignKey(p => p.UserId);

            });

            // ---------------- Project ----------------
            modelBuilder.Entity<Project>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).HasMaxLength(200).IsRequired();
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

                //b.HasMany(p => p.Todos)
                // .WithOne(t => t.Project)
                // .HasForeignKey(t => t.ProjectId);

                b.HasMany(p => p.TodoStatuses)
              .WithOne(t => t.Project)
              .HasForeignKey(t => t.ProjectId);

            });
            // ---------------- Package ----------------
            modelBuilder.Entity<Package>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).HasMaxLength(200).IsRequired();
                b.Property(s=>s.MaxProjects).IsRequired();
                b.Property(s => s.MaxUsers).IsRequired();
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

                b.HasMany(p => p.UserPackages)
                 .WithOne(t => t.Package)
                 .HasForeignKey(t => t.PackageId);

            });
            // ---------------- UserPackage ----------------
            modelBuilder.Entity<UserPackage>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");


            });

            // ---------------- ExtraProject ----------------
            modelBuilder.Entity<ExtraProject>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");


            });

            // ---------------- Todo ----------------
            modelBuilder.Entity<Todo>(b =>
            {
                b.HasKey(t => t.Id);
                b.Property(t => t.Title).HasMaxLength(200).IsRequired();
                b.Property(t => t.Description).HasMaxLength(1000);


                b.Property(t => t.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.Property(r => r.IsDeleted).HasDefaultValue(false);
            });

            // ---------------- TodoStatus ----------------
            modelBuilder.Entity<TodoStatus>(b =>
            {
                b.HasKey(t => t.Id);
                b.Property(t => t.Name).HasMaxLength(75).IsRequired();
                b.Property(t => t.CreatedAt).HasDefaultValueSql("GETDATE()");

            });

            // ---------------- Comment ----------------
            modelBuilder.Entity<Comment>(b =>
            {
                b.HasKey(t => t.Id);
                b.Property(t => t.Message).HasMaxLength(1000).IsRequired();
                b.Property(t => t.CreatedAt).HasDefaultValueSql("GETDATE()");
        
            });

            // ---------------- Tag ----------------
            modelBuilder.Entity<Tag>(b =>
            {
                b.HasKey(tag => tag.Id);
                b.Property(tag => tag.Name).HasMaxLength(100).IsRequired();
            });

            // ---------------- TodoTag (Many-to-Many) ----------------
            modelBuilder.Entity<TodoTag>(b =>
            {
                b.HasKey(tt => new { tt.TodoId, tt.TagId });

                b.HasOne(tt => tt.Todo)
                 .WithMany(t => t.TodoTags)
                 .HasForeignKey(tt => tt.TodoId);



                b.HasOne(tt => tt.Tag)
                 .WithMany(t => t.TodoTags)
                 .HasForeignKey(tt => tt.TagId);

            });

            // ---------------- Reminder ----------------
            modelBuilder.Entity<Reminder>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.ReminderTime).IsRequired();
                b.Property(r => r.IsSent).HasDefaultValue(false);

                b.HasOne(r => r.Todo)
                 .WithMany(t => t.Reminders)
                 .HasForeignKey(r => r.TodoId);

            });
            modelBuilder.Entity<RoleMenu>(b =>
            {
                b.HasKey(rm => new { rm.RoleId, rm.MenuId });

                b.HasOne(rm => rm.Role)
                 .WithMany(r => r.RoleMenus)
                 .HasForeignKey(rm => rm.RoleId);

                b.HasOne(rm => rm.Menu)
                 .WithMany(m => m.RoleMenus)
                 .HasForeignKey(rm => rm.MenuId);

                b.HasOne(rm => rm.Permission)
            .WithMany(m => m.RoleMenus)
            .HasForeignKey(rm => rm.PermissionId);


            });

            // ---------------- Menu ----------------
            modelBuilder.Entity<Menu>(b =>
            {
                b.HasKey(m => m.Id);
                b.Property(m => m.Name).HasMaxLength(200).IsRequired();
                b.Property(m => m.Url).HasMaxLength(500);

                b.HasOne(m => m.Parent)
                 .WithMany(p => p.Children)
                 .HasForeignKey(m => m.ParentId)
                 .OnDelete(DeleteBehavior.Restrict); // حذف منوی والد، فرزندان حذف نشوند
            });
        }
    }
}
