// ==++==
// 
//   Copyright (c) Teo Jörsin.  All rights reserved.
// 
// ==--==
namespace Channel365.Data {

    public interface IUnitOfWork {
        IVideoRepository VideoRepo { get; }
        int SaveChanges();
    }
}