using Application.IRepository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repo
{
    public class UserRepo : GenericRepo<User>,IUserRepo
    {
        private readonly AppDbContext _dbContext;
        public UserRepo(AppDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public Task AddAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckEmailAddressExisted(string sEmail)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == sEmail);
        }

        public Task<List<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByConfirmationToken(string sToken)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(
               u => u.ConfirmationToken == sToken
           );
        }

        public Task Remove(User entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(User entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateE(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
