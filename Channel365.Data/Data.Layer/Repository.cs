// ==++==
// 
//   Copyright (c) Teo Jörsin.  All rights reserved.
// 
// ==--==
/*============================================================
**
**  Class:  Repository
**
**
**  TODO:   Update it from WindowsAzure to Azure.
**
===========================================================*/

namespace Channel365.Data.Layer {
    using System.Collections.Generic;
    using System.Linq;
    using Channel365.Models.Abstractions;
    using Microsoft.EntityFrameworkCore;
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class {
        protected DbContext db { get; set; }

        public void Add(TEntity model) => db.Set<TEntity>().Add(model);

        public void Delete(TEntity model) => db.Set<TEntity>().Remove(model);

        public void DeleteById(object Id) {
            TEntity entity = db.Set<TEntity>().Find(Id);
            this.Delete(entity);
        }

        public IEnumerable<TEntity> GetAll() => db.Set<TEntity>().ToList();

        public TEntity GetById(object Id) => db.Set<TEntity>().Find(Id);

        public TEntity GetByUrlSlug(string UrlSlug) => throw new System.NotImplementedException();

        public void Modify(TEntity model) => db.Entry<TEntity>(model).State = EntityState.Modified;
    }
}