using OMG.Core.Base.Contract;

namespace OMG.Core.Base;

public abstract class Entity : ISoftDeletable
{
    public virtual int Id { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

}
