using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SFA.DAS.QnA.Application.Commands.Files
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IKeyProvider _keyProvider;

        public EncryptionService(IKeyProvider keyProvider)
        {
            _keyProvider = keyProvider;
        }

        public Stream Encrypt(Stream fileStream)
        {
            var key = Encoding.UTF8.GetBytes(_keyProvider.GetKey());
            using var aes = Aes.Create();
            ConfigureAes(key, aes, out byte[] salt);

            using var inputMemoryStream = new MemoryStream();
            fileStream.CopyTo(inputMemoryStream);
            var originalBytes = inputMemoryStream.ToArray();

            using var encryptedStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(encryptedStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(originalBytes, 0, originalBytes.Length);
                cryptoStream.FlushFinalBlock();
            }

            var encryptedData = encryptedStream.ToArray();

            var resultStream = new MemoryStream();
            using (var writer = new BinaryWriter(resultStream, Encoding.UTF8, leaveOpen: true))
            {
                writer.Write("ENCv2");
                writer.Write(salt.Length);
                writer.Write(salt);
                writer.Write(encryptedData.Length);
                writer.Write(encryptedData);
            }

            resultStream.Position = 0;
            return resultStream;
        }

        public Stream Decrypt(Stream encryptedStream)
        {
            try
            {
                using var reader = new BinaryReader(encryptedStream, Encoding.UTF8, leaveOpen: true);
                var version = reader.ReadString();

                if (version == "ENCv2")
                {
                    var saltLength = reader.ReadInt32();
                    var salt = reader.ReadBytes(saltLength);
                    var encryptedLength = reader.ReadInt32();
                    var encryptedData = reader.ReadBytes(encryptedLength);

                    var key = Encoding.UTF8.GetBytes(_keyProvider.GetKey());
                    using var aes = Aes.Create();
                    using var deriveBytes = new Rfc2898DeriveBytes(key, salt, 100_000, HashAlgorithmName.SHA256);

                    aes.Key = deriveBytes.GetBytes(aes.KeySize / 8);
                    aes.IV = deriveBytes.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;

                    var decryptedStream = new MemoryStream();

                    using (var cryptoStream = new CryptoStream(decryptedStream, aes.CreateDecryptor(), CryptoStreamMode.Write, leaveOpen: true))
                    {
                        cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                        cryptoStream.FlushFinalBlock();
                    }

                    decryptedStream.Position = 0;
                    return decryptedStream;
                }
                else
                {
                    encryptedStream.Position = 0;
                    return LegacyDecrypt(encryptedStream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Stream LegacyDecrypt(Stream encryptedFileStream)
        {
            var key = _keyProvider.GetKey();
            var memoryStream = new MemoryStream();
            encryptedFileStream.CopyTo(memoryStream);
            var encryptedBytes = memoryStream.ToArray();

            var originalBytes = AES_Decrypt(encryptedBytes, Encoding.ASCII.GetBytes(key));
            return new MemoryStream(originalBytes);
        }

        private byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            using var aes = Aes.Create();
            ConfigureAes(passwordBytes, aes, out _);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        private byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            using var aes = Aes.Create();
            ConfigureAes(passwordBytes, aes, out _);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        private static void ConfigureAes(byte[] passwordBytes, Aes aes, out byte[] salt)
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;

            using var rng = RandomNumberGenerator.Create();
            salt = new byte[16];
            rng.GetBytes(salt);

            using var deriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, 100_000, HashAlgorithmName.SHA256);
            aes.Key = deriveBytes.GetBytes(aes.KeySize / 8);
            aes.IV = deriveBytes.GetBytes(aes.BlockSize / 8);
            aes.Mode = CipherMode.CBC;
        }
    }
}