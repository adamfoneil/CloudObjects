using System;
using System.Threading;
using System.Threading.Tasks;
using CloudObjects.App.Interfaces;
using CloudObjects.App.Models;
using CloudObjects.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudObjects.App.Data
{
    public class CloudObjectsDbContext : IdentityDbContext<AppUser, AppRole, long>
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<StoredObject> StoredObjects { get; set; }

        public CloudObjectsDbContext(DbContextOptions<CloudObjectsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(CloudObjectsDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        OnEntityAdd(entry.Entity);
                        break;

                    case EntityState.Deleted:
                        OnEntityDelete(entry.Entity);
                        break;

                    case EntityState.Detached:
                        OnEntityDetach(entry.Entity);
                        break;

                    case EntityState.Modified:
                        OnEntityUpdate(entry.Entity);
                        break;

                    case EntityState.Unchanged:
                        OnEntityUnchange(entry.Entity);
                        break;
                }   
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        private void OnEntityAdd(object entity)
        {
            if (entity is IAuditedEntity auditedEntity)
            {
                OnAuditedEntityAdd(auditedEntity);
                return;
            }
        }

        private void OnEntityDelete(object entity)
        {

        }

        private void OnEntityDetach(object entity)
        {

        }

        private void OnEntityUpdate(object entity)
        {
            if (entity is IAuditedEntity auditedEntity)
            {
                OnAuditedEntityUpdate(auditedEntity);
                return;
            }
        }

        private void OnEntityUnchange(object entity)
        {

        }

        private void OnAuditedEntityAdd(IAuditedEntity entity)
        {
            entity.DateCreated = DateTime.UtcNow;
        }

        private void OnAuditedEntityUpdate(IAuditedEntity entity)
        {
            entity.DateModified = DateTime.UtcNow;
        }
    }
}
