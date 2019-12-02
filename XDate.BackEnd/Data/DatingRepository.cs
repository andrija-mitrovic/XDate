using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XDate.BackEnd.Helpers;
using XDate.BackEnd.Models;

namespace XDate.BackEnd.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T Entity) where T : class
        {
            _context.Add(Entity);
        }

        public async Task<Photo> GetCurrentPhoto(int userId)
        {
            return await _context.Photos.Where(x=>x.UserId==userId).FirstOrDefaultAsync(x =>x.IsMain);
        }

        public void Delete<T>(T Entity) where T : class
        {
            _context.Remove(Entity);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await _context.Photos.FirstOrDefaultAsync(x => x.Id==id);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos)
                .OrderByDescending(u=>u.LastActive).AsQueryable();

            users=users.Where(u => u.Id != userParams.UserId);
            users=users.Where(u => u.Gender == userParams.Gender);
            
            if(userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId,userParams.Likers);
                users = users.Where(x=>userLikers.Contains(x.Id));
            }

            if(userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId,userParams.Likers);
                users = users.Where(x=>userLikees.Contains(x.Id));
            }

            if(userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge-1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users=users.Where(u=>u.DateOfBirth>=minDob && u.DateOfBirth<=maxDob);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "created":
                        users=users.OrderByDescending(u=>u.Created);
                        break;
                    default:
                        users=users.OrderByDescending(u=>u.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users,userParams.PageNumber,userParams.PageSize);
        }

        public async Task<IEnumerable<int>> GetUserLikes (int id,bool likers)
        {
            var user = await _context.Users
                        .Include(x=>x.Likers)
                        .Include(x=>x.Likees)
                        .FirstOrDefaultAsync(u=>u.Id==id);

            if(likers)
            {
                return user.Likers.Where(x=>x.LikeeId==id).Select(x=>x.LikerId);
            }
            else 
            {
                return user.Likees.Where(x=>x.LikerId==id).Select(x=>x.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync()>0;
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(x=>x.LikerId==userId && x.LikeeId==recipientId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(x => x.Id==id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(x => x.Sender)
                                .ThenInclude(p => p.Photos)
                                .Include(x => x.Recipient)
                                .ThenInclude(p => p.Photos)
                                .AsQueryable();

            switch(messageParams.MessageContainer)
            {
                case "Inbox":
                    messages=messages.Where(u=>u.RecipientId==messageParams.UserId && u.RecipientDeleted==false);
                    break;
                case "Outbox":
                    messages=messages.Where(u=>u.SenderId==messageParams.UserId && u.SenderDeleted==false);
                    break;
                default:
                    messages=messages.Where(u=>u.RecipientId==messageParams.UserId && u.RecipientDeleted==false && u.IsRead==false);
                    break;
            }

            messages = messages.OrderByDescending(u=>u.MessageSent);
            return await PagedList<Message>.CreateAsync(messages,messageParams.PageNumber,messageParams.pageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages.Include(x=>x.Sender)
                                    .ThenInclude(x=>x.Photos)
                                    .Include(x=>x.Recipient)
                                    .ThenInclude(x=>x.Photos)
                                    .Where(x=>x.RecipientId==userId && x.RecipientDeleted==false && x.SenderId==recipientId ||
                                        x.SenderId==userId && x.SenderDeleted==false && x.RecipientId==recipientId)
                                    .OrderByDescending(x=>x.MessageSent)
                                    .ToListAsync();

            return messages;
        }
    }
}