using System;

namespace LegacyApp
{
    public class UserService : UserServiceInterface
    {
        private readonly ClientRepository _clientRepository;
        private readonly UserCreditService _userCreditService;

        public UserService()
        {
            _clientRepository = new ClientRepository();
            _userCreditService = new UserCreditService();
        }

        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!ValidateUserInput(firstName, lastName, email))
                return false;

            int age = CalculateAge(dateOfBirth);
            if (age < 21)
                return false;

            var client = _clientRepository.GetById(clientId);
            if (client == null)
                return false;

            var user = CreateUser(firstName, lastName, email, dateOfBirth, client);
            if (user == null)
                return false;

            SetUserCreditLimit(user, client.Type);

            if (!CheckCreditLimit(user))
                return false;

            UserDataAccess.AddUser(user);
            return true;
        }

        private bool ValidateUserInput(string firstName, string lastName, string email)
        {
            return !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) &&
                   email.Contains("@") && email.Contains(".");
        }

        private User CreateUser(string firstName, string lastName, string email, DateTime dateOfBirth, Client client)
        {
            return new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName
            };
        }

        private void SetUserCreditLimit(User user, string clientType)
        {
            if (clientType == "VeryImportantClient")
            {
                user.HasCreditLimit = false;
            }
            else
            {
                user.HasCreditLimit = true;
                using (var userCreditService = new UserCreditService())
                {
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    if (clientType == "ImportantClient")
                        creditLimit *= 2;

                    user.CreditLimit = creditLimit;
                }
            }
        }

        private bool CheckCreditLimit(User user)
        {
            return !user.HasCreditLimit || user.CreditLimit >= 500;
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
                age--;

            return age;
        }

    }
}
