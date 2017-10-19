using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    public interface ISaveEntityEvent
    {
        void Before();
    }
    public interface IEntityBase<TKey> : ISaveEntityEvent
    {
        string StringId { get; }
        TKey Id { get; }
        DateTime CreatedDate { get; }
        DateTime LastModfiyDate { get; }
        int RowVersion { get; }
    }

    public interface IEntityBase : IEntityBase<string>
    {
    }



    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        public EntityBase() { }
        public EntityBase(TKey id)
        {
            this.Id = id;
        }
        public EntityBase(TKey id, DateTime createdDate, DateTime lastModfiyDate)
        {
            this.Id = id;
            this.CreatedDate = createdDate;
            this.LastModfiyDate = lastModfiyDate;
        }
        public EntityBase(DateTime createdDate, DateTime lastModfiyDate)
        {
            this.CreatedDate = createdDate;
            this.LastModfiyDate = lastModfiyDate;
        }

        /// <summary>
        /// 表id
        /// </summary>
        public virtual TKey Id { get; protected internal set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDate { get; protected internal set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModfiyDate { get; protected internal set; }

        public virtual string StringId => this.Id.ToString();

        public int RowVersion { get; protected internal set; } = 0;

        protected virtual void Before()
        {
            if (this.CreatedDate == DateTime.MinValue) this.CreatedDate = DateTime.Now;
            this.LastModfiyDate = DateTime.Now;
            this.RowVersion++;
        }

        void ISaveEntityEvent.Before() => this.Before();
    }
    public class EntityBase : EntityBase<string>, IEntityBase
    {
        string _id;
        public EntityBase() : base(DateTime.Now, DateTime.Now) { }
        public override string Id { get { if (_id == null) _id = IdBuilder.NextStringId(this.GetType()); return this._id; } protected internal set => _id = value; }
    }
}
