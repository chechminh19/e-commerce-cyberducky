using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepository
{
    public interface IUserRepo : IGenericRepo<User>
    {
        Task<bool> CheckEmailAddressExisted(string sEmail);
        Task<User> GetUserByConfirmationToken(string sToken);
    }
}
