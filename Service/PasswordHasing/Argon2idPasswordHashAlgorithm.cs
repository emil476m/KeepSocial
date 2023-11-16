using System.Text;
using Konscious.Security.Cryptography;

namespace Service;

public class Argon2idPasswordHashAlgorithm : PasswordHashAlgorithm
{
    public const string Name = "argon2id";

    public override string GetName() => Name;

    public override string HashPassword(string password, string salt)
    {
        using var hashAlgo = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = Decode(salt),
            MemorySize = 12288,
            Iterations = 3,
            DegreeOfParallelism = 1,
        };
        return Encode(hashAlgo.GetBytes(256));
    }

    public override bool VerifyHashedPassword(string password, string hash, string salt)
    {
        return HashPassword(password, salt).SequenceEqual(hash);
    }
}