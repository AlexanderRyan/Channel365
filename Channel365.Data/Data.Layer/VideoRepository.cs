// ==++==
// 
//   Copyright (c) Teo Jörsin.  All rights reserved.
// 
// ==--==

namespace Channel365.Data.Layer {
    using Channel365.Models;
    using Channel365.Web.Data;
    using Microsoft.EntityFrameworkCore;
    public class VideoRepository : Repository<VideoModel>, IVideoRepository {
        private ApplicationDbContext context {
            get { return db as ApplicationDbContext; }
        }

        public VideoRepository(DbContext db) { this.db = db; }
    }
}