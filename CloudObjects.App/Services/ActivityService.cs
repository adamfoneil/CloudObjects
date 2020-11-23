using CloudObjects.App.Bases;
using CloudObjects.App.Data;
using CloudObjects.App.Interfaces;
using CloudObjects.Models;

namespace CloudObjects.App.Services
{
    public class ActivityService : RepositoryServiceBase<Activity, long>, IActivityService
    {
        public ActivityService(CloudObjectsDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
