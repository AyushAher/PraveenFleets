using Domain.Account;
using Domain.Common;
using Domain.FileShare;
using Domain.Mail;
using Domain.MasterData;
using Domain.Organization;
using Interfaces.Account;
using Microsoft.EntityFrameworkCore;

namespace DB.Contexts;

public class ApplicationDbContext : AppIdentityDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(DbContextOptions options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<MailTemplate> MailTemplates { get; set; }
    public DbSet<ListType> ListType { get; set; }
    public DbSet<ListTypeItem> ListTypeItems { get; set; }
    public DbSet<Vw_ListTypeItems> Vw_ListTypeItems { get; set; }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        // Set the Entity Audit fields based on state
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedOn = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    break;

                case EntityState.Deleted:
                    entry.Entity.DeletedOn = DateTime.UtcNow;
                    entry.Entity.DeletedBy = _currentUserService.UserId;
                    entry.State = EntityState.Modified;
                    break;
            }
        }

        // If User is not set, then cancel the authorization
        if (_currentUserService.UserId == Guid.Empty)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        return await base.SaveChangesAsync(_currentUserService.UserId, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (var property in builder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        base.OnModelCreating(builder);

        #region Common Data

        builder.Entity<MailTemplate>(entity =>
        {
            entity.ToTable(name: "emailtemplates");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });


        builder.Entity<DocumentFilesUpload>(entity =>
        {
            entity.ToTable("Fileshare");
            entity.HasKey(e => e.Id);
        });

        builder.Entity<Address>(entity =>
        {
            entity.ToTable("address");
            entity.HasKey(e => e.Id);
        });


        builder.Entity<DocuFile_View>(entity =>
        {
            entity.ToView("Fileshare_view");
            entity.HasKey(e => e.Id);
        });

        builder.Entity<ListType>(entity =>
        {
            entity.ToTable("listtype");
            entity.HasKey(e => e.Id);
        });

        builder.Entity<ListTypeItem>(entity =>
        {
            entity.ToTable("listtypeitems");
            entity.HasKey(e => e.Id);
        });

        builder.Entity<Vw_ListTypeItems>(entity =>
        {
            entity.ToView("vw_listitems");
            entity.HasKey(e => e.Id);
        });

        #endregion

        #region Organization

        builder.Entity<Organizations>(entity =>
        {
            entity.ToTable("organizations");
            entity.HasKey(e => e.Id);
        });

        builder.Entity<OrganizationRoles>(entity =>
        {
            entity.ToTable("organizationRoles");
            entity.HasKey(e => e.Id);
        });

        #endregion



    }
}
