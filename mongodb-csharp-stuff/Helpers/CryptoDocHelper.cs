namespace Mcs.Helpers
{
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;

    using Mcs.Models;

    using MongoDB.Bson;

    public static class CryptoDocHelper
    {
        public static BsonDocument Decrypt(this CryptoDoc cryptoDoc, string rsaKey)
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                rsa.FromXmlString(rsaKey);

                if (rsa.PublicOnly)
                {
                    throw new SecurityException("The key supplied for decryption was not the public key");
                }

                var cipherBytes = rsa.Decrypt(cryptoDoc.Data, false);

                var json = Encoding.UTF8.GetString(cipherBytes, 0, cipherBytes.Length);

                return BsonDocument.Parse(json);
            }
        }
    }
}