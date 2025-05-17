using CSharpFunctionalExtensions;

namespace PetFoster.Core.Entities
{
    public abstract class SoftDeletableEntity<TId> : Entity<TId> where TId : IComparable<TId>
    {
        protected SoftDeletableEntity()
        {
        }

        protected SoftDeletableEntity(TId id) : base(id)
        {
        }

        public bool IsDeleted { get; private set; }

        private DateTimeOffset DeletionDate { get; set; }

        public virtual void Delete()
        {
            IsDeleted = true;
        }

        public virtual void Restore()
        {
            IsDeleted = false;
        }
    }
}
