// ==++==
// 
//   Copyright (c) Teo Jörsin.  All rights reserved.
// 
// ==--==
/*============================================================
**
**  Class:  UnitOfWork
**
===========================================================*/
namespace Channel365.Data.Layer {
    using Channel365.Web.Data;
    using Microsoft.EntityFrameworkCore;
    public class UnitOfWork : IUnitOfWork {
        private DbContext _db;
        private IVideoRepository _videoRepo;

        public UnitOfWork() => _db = new ApplicationDbContext();

        public IVideoRepository VideoRepo {
            get {
                if (_videoRepo == null)
                    _videoRepo = new VideoRepository(_db);
                return _videoRepo;
            }
        }

        public int SaveChanges() {
            return _db.SaveChanges();
        }
    }
}