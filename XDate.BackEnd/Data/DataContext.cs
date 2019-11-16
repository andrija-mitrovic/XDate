using Microsoft.EntityFrameworkCore;
using XDate.BackEnd.Models;

namespace XDate.BackEnd.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) {}

        public DbSet<User> Users {get;set;}
    }
}