using Infastructure;

namespace Service;

public class AccountService
{
    private readonly AccountRepository _accountRepository;
    private readonly PasswordHashRepository _passwordHashRepository;

    public AccountService(AccountRepository accountRepository, PasswordHashRepository passwordHashRepository)
    {
        _accountRepository = accountRepository;
        _passwordHashRepository = passwordHashRepository;
    }
    
    
   
    
    public User CreateUser(string userDisplayName, string userEmail, string password, DateOnly userBirthday)
    {
        var hashAlgorithm = PasswordHashAlgorithm.Create();
        var salt = hashAlgorithm.GenerateSalt();
        var hash = hashAlgorithm.HashPassword(password, salt);
        var user = _accountRepository.CreateUser(userDisplayName, userEmail, userBirthday);
        _passwordHashRepository.Create(user.userId, hash, salt, hashAlgorithm.GetName());
        return user;
    }
}