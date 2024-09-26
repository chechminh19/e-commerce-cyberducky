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

       

        public async Task<bool> CheckEmailAddressExisted(string sEmail)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == sEmail);
        }

       

        public async Task<User> GetUserByConfirmationToken(string sToken)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(
               u => u.ConfirmationToken == sToken
           );
        }

        public async Task<User> GetUserByEmailAddressAndPasswordHash(string email, string passwordHash)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(record => record.Email == email && record.Password == passwordHash);
            if (user is null)
            {
                throw new Exception("Email & password is not correct");
            }

            return user;
        }
    }
}
