using Cotrust.Intefaces;
using System.Security.Cryptography;
using System.Text;

namespace Cotrust.Classes
{
    public class Utilities : IUtilities
    {
        public string Encrypting(string txt)
        {
            string hash = string.Empty;

            SHA256 sha256 = SHA256.Create();

            byte[] value = sha256.ComputeHash(Encoding.UTF8.GetBytes(txt));

            foreach (byte b in value) { hash += $"{b:X2}"; }

            return hash;
        }

        public string GenerateToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
