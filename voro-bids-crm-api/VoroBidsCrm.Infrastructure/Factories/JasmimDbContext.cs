using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Entities.Identity;

namespace VoroBidsCrm.Infrastructure.Factories
{
    public class JasmimDbContext : IdentityDbContext<User, Role, Guid,
        IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        private readonly ICurrentUserService _currentUser;

        public JasmimDbContext(
            DbContextOptions<JasmimDbContext> options,
            ICurrentUserService currentUser
        ) : base(options)
        {
            _currentUser = currentUser;
        }

        public Guid? CurrentUserId => _currentUser.UserId;

        // Expor explicitamente a entidade de junção
        //public DbSet<Exemplo> Exemplo { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<UserTenant> UserTenants { get; set; }
        public DbSet<UserExtension> UserExtensions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<AuctionDocument> AuctionDocuments { get; set; }
        public DbSet<DocumentFile> DocumentFiles { get; set; }
        public DbSet<CompanyDocument> CompanyDocuments { get; set; }
        public DbSet<InternalRequest> InternalRequests { get; set; }
        public DbSet<AuctionChecklist> AuctionChecklists { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<TenantModule> TenantModules { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ---------------------------
            // GLOBAL QUERY FILTERS (Multi-Tenant)
            // ---------------------------
            builder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

            builder.Entity<UserExtension>().HasQueryFilter(ue => !ue.User.IsDeleted);

            builder.Entity<Notification>().HasQueryFilter(n =>
                !n.IsDeleted && n.TenantId == _currentUser.TenantId);

            builder.Entity<Auction>().HasQueryFilter(a =>
                !a.IsDeleted && a.TenantId == _currentUser.TenantId);

            builder.Entity<AuctionDocument>().HasQueryFilter(ad =>
                !ad.IsDeleted && ad.TenantId == _currentUser.TenantId);

            builder.Entity<DocumentFile>().HasQueryFilter(df =>
                !df.IsDeleted && df.TenantId == _currentUser.TenantId);

            builder.Entity<CompanyDocument>().HasQueryFilter(cd =>
                !cd.IsDeleted && cd.TenantId == _currentUser.TenantId);

            builder.Entity<InternalRequest>().HasQueryFilter(ir =>
                !ir.IsDeleted && ir.TenantId == _currentUser.TenantId);

            builder.Entity<AuctionChecklist>().HasQueryFilter(ac =>
                !ac.IsDeleted && ac.TenantId == _currentUser.TenantId);

            builder.Entity<ActivityLog>().HasQueryFilter(al =>
                al.TenantId == _currentUser.TenantId);

            builder.Entity<Comment>().HasQueryFilter(c =>
                !c.IsDeleted && c.TenantId == _currentUser.TenantId);

            builder.Entity<TenantModule>().HasQueryFilter(tm =>
                tm.TenantId == _currentUser.TenantId);

            // ---------------------------
            // TENANT
            // ---------------------------
            builder.Entity<Tenant>(b =>
            {
                b.HasKey(t => t.Id);
                b.Property(t => t.Name).HasMaxLength(150).IsRequired();
                b.Property(t => t.Slug).HasMaxLength(100).IsRequired();
                b.HasIndex(t => t.Slug).IsUnique();
                b.Property(t => t.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");
                b.Property(t => t.IsActive).HasDefaultValue(true);
            });

            // ---------------------------
            // TENANT MODULE
            // ---------------------------
            builder.Entity<TenantModule>(b =>
            {
                b.HasKey(tm => tm.Id);
                b.Property(tm => tm.Module).HasConversion<int>().IsRequired();
                b.Property(tm => tm.IsEnabled).HasDefaultValue(true);
                b.Property(tm => tm.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");

                b.HasOne(tm => tm.Tenant)
                 .WithMany()
                 .HasForeignKey(tm => tm.TenantId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(tm => tm.TenantId);
                b.HasIndex(tm => new { tm.TenantId, tm.Module }).IsUnique();
            });

            // ---------------------------
            // USER TENANT (Join Table)
            // ---------------------------
            builder.Entity<UserTenant>(b =>
            {
                b.HasKey(ut => new { ut.UserId, ut.TenantId });

                b.HasOne(ut => ut.User)
                    .WithMany(u => u.UserTenants)
                    .HasForeignKey(ut => ut.UserId);

                b.HasOne(ut => ut.Tenant)
                    .WithMany(t => t.UserTenants)
                    .HasForeignKey(ut => ut.TenantId);

                b.Property(ut => ut.IsDefault).HasDefaultValue(false);
                b.Property(ut => ut.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");
            });

            // ---------------------------
            // AUCTIONS
            // ---------------------------
            builder.Entity<Auction>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.Title).HasMaxLength(255).IsRequired();
                b.Property(a => a.Organization).HasMaxLength(255).IsRequired();
                b.Property(a => a.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");
                b.Property(a => a.Status).HasConversion<int>().IsRequired();

                b.HasIndex(a => a.TenantId);

                b.HasOne<Tenant>()
                 .WithMany()
                 .HasForeignKey(a => a.TenantId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------------------
            // AUCTION DOCUMENTS
            // ---------------------------
            builder.Entity<AuctionDocument>(b =>
            {
                b.HasKey(ad => ad.Id);
                b.Property(ad => ad.Name).HasMaxLength(255).IsRequired();
                b.Property(ad => ad.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");
                b.Property(ad => ad.Status).HasConversion<int>().IsRequired();

                b.HasIndex(ad => ad.TenantId);
                b.HasIndex(ad => ad.AuctionId);

                b.HasOne<Tenant>()
                 .WithMany()
                 .HasForeignKey(ad => ad.TenantId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(ad => ad.Auction)
                 .WithMany(a => a.Documents)
                 .HasForeignKey(ad => ad.AuctionId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------------------
            // DOCUMENT FILES
            // ---------------------------
            builder.Entity<DocumentFile>(b =>
            {
                b.HasKey(df => df.Id);
                b.Property(df => df.FileUrl).HasMaxLength(2048).IsRequired();
                b.Property(df => df.FileName).HasMaxLength(255).IsRequired();
                b.Property(df => df.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");

                b.HasIndex(df => df.TenantId);
                b.HasIndex(df => df.AuctionDocumentId);

                b.HasOne<Tenant>()
                 .WithMany()
                 .HasForeignKey(df => df.TenantId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(df => df.AuctionDocument)
                 .WithMany(ad => ad.Files)
                 .HasForeignKey(df => df.AuctionDocumentId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------------------
            // COMPANY DOCUMENTS
            // ---------------------------
            builder.Entity<CompanyDocument>(b =>
            {
                b.HasKey(cd => cd.Id);
                b.Property(cd => cd.Name).HasMaxLength(255).IsRequired();
                b.Property(cd => cd.FileUrl).HasMaxLength(2048).IsRequired();
                b.Property(cd => cd.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");

                b.HasIndex(cd => cd.TenantId);

                b.HasOne<Tenant>()
                 .WithMany()
                 .HasForeignKey(cd => cd.TenantId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------------------
            // INTERNAL REQUESTS
            // ---------------------------
            builder.Entity<InternalRequest>(b =>
            {
                b.HasKey(ir => ir.Id);
                b.Property(ir => ir.Title).HasMaxLength(255).IsRequired();
                b.Property(ir => ir.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");
                b.Property(ir => ir.Status).HasConversion<int>().IsRequired();

                b.HasIndex(ir => ir.TenantId);

                b.HasOne<Tenant>()
                 .WithMany()
                 .HasForeignKey(ir => ir.TenantId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(ir => ir.Auction)
                 .WithMany(a => a.InternalRequests)
                 .HasForeignKey(ir => ir.AuctionId)
                 .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(ir => ir.Requester)
                 .WithMany()
                 .HasForeignKey(ir => ir.RequesterId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(ir => ir.Assignee)
                 .WithMany()
                 .HasForeignKey(ir => ir.AssigneeId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ---------------------------
            // AUCTION CHECKLIST
            // ---------------------------
            builder.Entity<AuctionChecklist>(b =>
            {
                b.HasKey(ac => ac.Id);
                b.Property(ac => ac.TaskName).HasMaxLength(255).IsRequired();
                b.Property(ac => ac.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");

                b.HasIndex(ac => ac.TenantId);

                b.HasOne<Tenant>()
                 .WithMany()
                 .HasForeignKey(ac => ac.TenantId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(ac => ac.Auction)
                 .WithMany(a => a.Checklists)
                 .HasForeignKey(ac => ac.AuctionId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------------------
            // ACTIVITY LOGS
            // ---------------------------
            builder.Entity<ActivityLog>(b =>
            {
                b.HasKey(al => al.Id);
                b.Property(al => al.Action).HasMaxLength(100).IsRequired();
                b.Property(al => al.EntityType).HasMaxLength(100).IsRequired();
                b.Property(al => al.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");

                b.HasIndex(al => al.TenantId);

                b.HasOne<Tenant>()
                 .WithMany()
                 .HasForeignKey(al => al.TenantId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------------------
            // COMMENTS
            // ---------------------------
            builder.Entity<Comment>(b =>
            {
                b.HasKey(c => c.Id);
                b.Property(c => c.EntityType).HasMaxLength(100).IsRequired();
                b.Property(c => c.Text).HasMaxLength(1024).IsRequired();
                b.Property(c => c.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");

                b.HasIndex(c => c.TenantId);

                b.HasOne<Tenant>()
                 .WithMany()
                 .HasForeignKey(c => c.TenantId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<UserExtension>()
                .HasKey(ue => ue.UserId);

            builder.Entity<UserExtension>()
                .HasOne(ue => ue.User)
                .WithOne(u => u.UserExtension)
                .HasForeignKey<UserExtension>(ue => ue.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------------------
            // IDENTITY CONFIG
            // ---------------------------
            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<UserRole>().ToTable("UserRoles");

            builder.Entity<User>(b =>
            {
                b.Property(u => u.FirstName).HasMaxLength(100);
                b.Property(u => u.LastName).HasMaxLength(100);
                b.Property(u => u.CountryCode).HasMaxLength(3);
                b.Property(u => u.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())");
                b.Property(u => u.IsActive).HasDefaultValue(true);
            });

            builder.Entity<Role>(b =>
            {
                b.Property(r => r.Name).HasMaxLength(256);
            });

            builder.Entity<UserRole>(b =>
            {
                b.HasKey(ur => new { ur.UserId, ur.RoleId });

                b.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                b.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            });

            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        }
    }
}
