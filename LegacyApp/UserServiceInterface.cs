using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp
{
    public interface UserServiceInterface
    {
        bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId);
    }
}
