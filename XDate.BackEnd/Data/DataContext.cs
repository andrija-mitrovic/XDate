using Microsoft.EntityFrameworkCore;
using XDate.BackEnd.Models;

namespace XDate.BackEnd.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) {}

        public DbSet<User> Users {get;set;}
        public DbSet<Photo> Photos {get;set;}
        public DbSet<Like> Likes {get;set;}
        public DbSet<Message> Messages{get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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