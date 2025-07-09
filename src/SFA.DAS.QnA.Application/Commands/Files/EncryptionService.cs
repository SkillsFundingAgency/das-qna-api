using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SFA.DAS.QnA.Application.Commands.Files
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IKeyProvider _keyProvider;
        private const string VersionMarker = "v2";
        private static readonly byte[] VersionMarkerBytes = Encoding.UTF8.GetBytes(VersionMarker);

        public EncryptionService(IKeyProvider keyProvider)
        {
            _keyProvider = keyProvider;
        }

        public Stream Encrypt(Stream fileStream)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(_keyProvider.GetKey());
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;

            var salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            int keySize = aes.KeySize / 8;
            int ivSize = aes.BlockSize / 8;
            int totalSize = keySize + ivSize;

            var derived = Rfc2898DeriveBytes.Pbkdf2(
                passwordBytes,
                salt,
                100_000,
                HashAlgorithmName.SHA256,
                totalSize
            );

            aes.Key = derived[..keySize];
            aes.IV = derived[keySize..];

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

            resultStream.Write(VersionMarkerBytes, 0, VersionMarkerBytes.Length);
            using (var writer = new BinaryWriter(resultStream, Encoding.UTF8, leaveOpen: true))
            {
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

                if (encryptedStream.Length >= VersionMarkerBytes.Length)
                {
                    var markerBytes = reader.ReadBytes(VersionMarkerBytes.Length);
                    var version = Encoding.UTF8.GetString(markerBytes);

                    if (version == VersionMarker)
                    {
                        var saltLength = reader.ReadInt32();
                        var salt = reader.ReadBytes(saltLength);
                        var encryptedLength = reader.ReadInt32();
                        var encryptedData = reader.ReadBytes(encryptedLength);

                        var passwordBytes = Encoding.UTF8.GetBytes(_keyProvider.GetKey());
                        using var aes = Aes.Create();
                        aes.KeySize = 256;
                        aes.BlockSize = 128;
                        aes.Mode = CipherMode.CBC;

                        int keySize = aes.KeySize / 8;
                        int ivSize = aes.BlockSize / 8;
                        int totalSize = keySize + ivSize;

                        var derived = Rfc2898DeriveBytes.Pbkdf2(
                            passwordBytes,
                            salt,
                            100_000,
                            HashAlgorithmName.SHA256,
                            totalSize
                        );

                        aes.Key = derived[..keySize];
                        aes.IV = derived[keySize..];

                        var decryptedStream = new MemoryStream();
                        using (var cryptoStream = new CryptoStream(decryptedStream, aes.CreateDecryptor(), CryptoStreamMode.Write, leaveOpen: true))
                        {
                            cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                            cryptoStream.FlushFinalBlock();
                        }

                        decryptedStream.Position = 0;
                        return decryptedStream;
                    }
                }

                encryptedStream.Position = 0;
                return LegacyDecrypt(encryptedStream);
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

        private byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;

            var salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            int keySize = aes.KeySize / 8;
            int ivSize = aes.BlockSize / 8;
            int totalSize = keySize + ivSize;

            var derived = Rfc2898DeriveBytes.Pbkdf2(
                passwordBytes,
                salt,
                1000,
                HashAlgorithmName.SHA1,
                totalSize
            );

            aes.Key = derived[..keySize];
            aes.IV = derived[keySize..];

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }
    }
}
