using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    public interface IEntityBase<TKey>
    {
        string StringId { get; }
        TKey Id { get; }
        DateTime CreatedDate { get; }
        DateTime LastModfiyDate { get; }
        byte Version { get; }
    }
    public interface IEntityBase : IEntityBase<string>
    {
    }
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        public EntityBase() { }
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
        public virtual TKey Id { get; protected set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDate { get; protected set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModfiyDate { get; protected set; }
        /// <summary>
        /// 数据版本。修改自增
        /// </summary>
        public byte Version { get; protected set; }

        public string StringId => this.Id.ToString();
    }
    public class EntityBase : EntityBase<string>, IEntityBase
    {
        string _id;

        public EntityBase() : base(DateTime.Now, DateTime.Now) { }

        public override string Id
        {
            get => this._id ?? (this._id = IdBuilder.NextStringId(this.GetType()));
            protected set => this._id = value;
        }
    }
}
