using API;
using Infastructure;
using Microsoft.Extensions.Logging;

namespace Service;

public class AccountService
{
    private readonly AccountRepository _accountRepository;
    private readonly PasswordHashRepository _passwordHashRepository;
    private readonly ILogger<AccountService> _logger;
    private readonly MailService _mailService;

    public AccountService(AccountRepository accountRepository, PasswordHashRepository passwordHashRepository, ILogger<AccountService> logger, MailService mailService)
    {
        _mailService = mailService;
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

    public User? whoAmI(int id)
    {
        return _accountRepository.GetById(id);
    }

    public bool UpdateUser(int id, string updatedValue, string updatedValueName)
    {
        if (updatedValueName == "Account Name")
        {
            return _accountRepository.UpdateUserName(id, updatedValue);
        }
        else if (updatedValueName == "Account Email")
        {
            string message = "Your email has been linked to an Account at KeepSocial " + "\n"+ "" +
                             "if you did not link it, then please contact costumer support at www.KeepSocial/notimplemented.com";
            bool succes = _accountRepository.UpdateUserEmail(id, updatedValue);
            if (succes)
            {
                SendEmailValidation(id, message);
                return true;
            }
        }
        else if (updatedValueName == "Account Password")
        {
            var hashAlgorithm = PasswordHashAlgorithm.Create();
            var salt = hashAlgorithm.GenerateSalt();
            var hash = hashAlgorithm.HashPassword(updatedValue, salt);
            bool succes =_passwordHashRepository.Update(id, hash, salt, hashAlgorithm.GetName());
            
            string message = "Your password on KeepSocial has been changed " + "\n"+ "" +
                             "if you did not change it, then please contact costumer support at www.KeepSocial/notimplemented.com";
            
            if (succes)
            {
                SendEmailValidation(id, message);
                return true;
            }
            
        }
        return false;
    }

    public bool SendEmailValidation(int userId, string mes)
    {
        string email = whoAmI(userId).userEmail;
        Random rnd = new Random();
        int validationNumber= rnd.Next(10000000, 99999999);
        string message = "This is your validation code for KeepSocial please enter it to continue to make changes to your account: " + validationNumber;

        if (mes != "" && mes != null)
        {
            message = mes;
        }
        _mailService.SendEmail(message, email);
        return _accountRepository.StoreValidation(userId, validationNumber);
    }

    public IEnumerable<User> getFriends(int userId, int pageNumber)
    {
        int offset = (10 * pageNumber)-10;
        try
        {
            return _accountRepository.getFriends(userId, offset);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("could not fetch friend data");
        }
    }

    public void UpdateAvatar(SessionData session, string? avatarUrl)
    {
            _accountRepository.updateAvatar(avatarUrl, session.UserId);
        /*try
        {
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("there where an encounter with an error while saving the Image, please try again later");
        }*/
    }
}