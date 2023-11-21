using Infastructure;
using Microsoft.Extensions.Logging;

namespace Service;

public class AccountService
{
    private readonly AccountRepository _accountRepository;
    private readonly PasswordHashRepository _passwordHashRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(AccountRepository accountRepository, PasswordHashRepository passwordHashRepository, ILogger<AccountService> logger)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _passwordHashRepository = passwordHashRepository;
    }
    
    
   
    
    public User CreateUser(string userDisplayName, string userEmail, string password, DateTime userBirthday)
    {
        var hashAlgorithm = PasswordHashAlgorithm.Create();
        var salt = hashAlgorithm.GenerateSalt();
        var hash = hashAlgorithm.HashPassword(password, salt);
        var user = _accountRepository.CreateUser(userDisplayName, userEmail, userBirthday);
        _passwordHashRepository.Create(user.userId, hash, salt, hashAlgorithm.GetName());
        return user;
    }


    public IEnumerable<User> getUserName()
    {
        return _accountRepository.getUserName();
    }

    public User? Authenticate(string email, string password)
    {
        try
        {
            var passwordHash = _passwordHashRepository.GetByEmail(email);
            var hashAlgorithm = PasswordHashAlgorithm.Create(passwordHash.Algorithm);
            var isValid = hashAlgorithm.VerifyHashedPassword(password, passwordHash.Hash, passwordHash.Salt);
            if (isValid) return _accountRepository.GetById(passwordHash.UserId);
        }
        catch (Exception e)
        {
            _logger.LogError("Authenticate error: {Mseeage}", e);
        }

        return null;
    }
}