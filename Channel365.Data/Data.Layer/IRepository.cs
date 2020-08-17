// ==++==
// 
//   Copyright (c) Teo Jörsin.  All rights reserved.
// 
// ==--==

namespace Channel365.Models.Abstractions {
    using System;
    using System.Collections.Generic;
    
    public interface IRepository<TEntity> where TEntity : class {
        void Add(TEntity model);
        IEnumerable<TEntity> GetAll();
        TEntity GetById(object Id);
        TEntity GetByUrlSlug(String UrlSlug);
        void Modify(TEntity model);
        void Delete(TEntity model);
        void DeleteById(object Id);
    }
}