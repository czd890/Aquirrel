using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    /// <summary>
    /// 持久化事件通知
    /// </summary>
    public interface ISaveEntityEvent
    {
        /// <summary>
        /// 在持久化前通知
        /// </summary>
        void Before();
    }
    /// <summary>
    /// 基础实体结构
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEntityBase<TKey> : ISaveEntityEvent
    {
        /// <summary>
        /// 主键id的string表现形式
        /// </summary>
        string StringId { get; }
        /// <summary>
        /// 主键id
        /// </summary>
        TKey Id { get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreatedDate { get; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        DateTime LastModfiyDate { get; }
        /// <summary>
        /// 版本号
        /// </summary>
        int RowVersion { get; }
    }
    /// <summary>
    /// 基础实体结构
    /// </summary>
    public interface IEntityBase : IEntityBase<string>
    {
    }


    /// <summary>
    /// 基础实体
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
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
        /// 主键id
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
        /// <summary>
        /// 主键id的string表现形式
        /// </summary>
        public virtual string StringId => this.Id.ToString();
        private int _rowVersion = 0;
        /// <summary>
        /// 版本号
        /// </summary>
        public int RowVersion { get => _rowVersion; protected internal set => _rowVersion = value; }
        /// <summary>
        /// 持久化前的通知，修改创建时间、最后修改时间、累加版本号
        /// </summary>
        protected virtual void Before()
        {
            if (this.CreatedDate == DateTime.MinValue) this.CreatedDate = DateTime.Now;
            this.LastModfiyDate = DateTime.Now;
            System.Threading.Interlocked.Add(ref this._rowVersion, 1);
        }

        void ISaveEntityEvent.Before() => this.Before();
    }
    public class EntityBase : EntityBase<string>, IEntityBase
    {
        string _id;
        public EntityBase() : base(DateTime.Now, DateTime.Now) { }
        /// <summary>
        /// 主键id
        /// </summary>
        public override string Id { get { if (_id == null) _id = IdBuilder.NextStringId(this.GetType()); return this._id; } protected internal set => _id = value; }
    }
}
