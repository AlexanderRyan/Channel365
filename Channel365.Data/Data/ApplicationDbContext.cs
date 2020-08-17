using Channel365.Data.Entities;
using Channel365.Data.Models.Video;
using Channel365.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Channel365.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public ApplicationDbContext() { }

        //  this has to be here due to old ver -> do not touch
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured)
                //  this string is wrong because we're using an older api; but it works for somehow so no worries
                optionsBuilder.UseSqlServer(@"Server=tcp:sql01.win19.se,1433;Initial Catalog=Channel365;Persist Security Info=False;User Id=ecuser;Password=EcUtb2019#!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VideosInPlaylist>()
                .HasKey(vip => new { vip.PlaylistId, vip.VideoId });
            modelBuilder.Entity<VideosInPlaylist>()
                .HasOne(vp => vp.Playlist)
                .WithMany(p => p.VideosInPlaylists)
                .HasForeignKey(vp => vp.PlaylistId);
            modelBuilder.Entity<VideosInPlaylist>()
                .HasOne(vp => vp.Video)
                .WithMany(v => v.VideosInPlaylists)
                .HasForeignKey(lp => lp.VideoId);

            modelBuilder.Entity<LikeDislikeModel>()
                .HasKey(x => new { x.UserId, x.VideoId });

            modelBuilder.Entity<UserFollow>(x =>
            {
                x.HasKey(k => new { k.ObserverId, k.TargetId });

                x.HasOne(f => f.Observer)
                .WithMany(f => f.Followings)
                .HasForeignKey(f => f.ObserverId)
                .OnDelete(DeleteBehavior.Restrict);

                x.HasOne(f => f.Target)
                .WithMany(f => f.Followers)
                .HasForeignKey(f => f.TargetId)
                .OnDelete(DeleteBehavior.Restrict);

            });

        }
        public DbSet<UserFollow> UserFollowings { get; set; }
        public DbSet<CommentVideoModel> CommentVideos { get; set; }
        public DbSet<PlaylistModel> Playlists { get; set; }
        public DbSet<VideoModel> Videos { get; set; }
        public DbSet<VideosInPlaylist> VideosInPlaylist { get; set; }
        public DbSet<LikeDislikeModel> LikesAndDislikes { get; set; }
    }
}