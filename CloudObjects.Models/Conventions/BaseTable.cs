using AO.Models;

namespace CloudObjects.Models.Conventions
{
    [Identity(nameof(Id))]
    public abstract class BaseTable
    {
        public long Id { get; set; }        
    }
}
