using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XDate.BackEnd.Models;

namespace XDate.BackEnd.Data
{
    public class DataContext:IdentityDbContext<User, Role, int, IdentityUserClaim<int>,
        UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) {}

        public DbSet<Photo> Photos {get;set;}
        public DbSet<Like> Likes {get;set;}
        public DbSet<Message> Messages{get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>(userRole => {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur=>ur.Role)
                    .WithMany(r=>r.UserRoles)
                    .HasForeignKey(ur=>ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur=>ur.User)
                    .WithMany(r=>r.UserRoles)
                    .HasForeignKey(ur=>ur.UserId)
                    .IsRequired();
            });
            

            modelBuilder.Entity<Like>()
                .HasKey(k => new {k.LikerId,k.LikeeId});

            modelBuilder.Entity<Like>()
                .HasOne(k => k.Likee)
                .WithMany(k => k.Likers)
                .HasForeignKey(k => k.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                .HasOne(k=>k.Liker)
                .WithMany(k=>k.Likees)
                .HasForeignKey(k=>k.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(k=>k.Sender)
                .WithMany(k=>k.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(k=>k.Recipient)
                .WithMany(k=>k.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}