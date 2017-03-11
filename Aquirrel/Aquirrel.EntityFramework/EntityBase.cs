using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    public interface IEntityBase<TKey>
    {
        string StringId { get; }
        TKey Id { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime LastModfiyDate { get; set; }
        byte[] TimeStamp { get; set; }
    }
    public interface IEntityBase : IEntityBase<string>
    {
    }
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        /// <summary>
        /// 表id
        /// </summary>
        public virtual TKey Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModfiyDate { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public virtual byte[] TimeStamp { get; set; }

        public string StringId
        {
            get
            {
                return this.Id.ToString();
            }
        }
    }
    public class EntityBase : EntityBase<string>, IEntityBase
    {
        public EntityBase()
        {
            //this.Id = IdBuilder.NextStringId(this.GetType());
            this.CreatedDate = DateTime.Now;
            this.LastModfiyDate = DateTime.Now;
        }
        string _id;
        public override string Id
        {
            get
            {
                if (_id == null)
                    _id = IdBuilder.NextStringId(this.GetType());
                return _id;
            }

            set
            {
                _id = value;
            }
        }
    }
}
