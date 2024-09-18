using Application;
using Application.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserRepo _userRepository;
        public UnitOfWork(AppDbContext dbContext, IUserRepo userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }
        public IUserRepo UserRepository => _userRepository;

        public Task<int> SaveChangeAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}
