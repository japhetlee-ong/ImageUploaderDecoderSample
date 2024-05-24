
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using System.Security.Cryptography;

namespace ImageUploaderDecoder.Utils
{
    public static class ImageUtils
    {

        public static bool ProcessImageUpload(IFormFile file, string folderLocation ,string key, string iv, out string imageFilePath)
        {
            try
            {

                var bKey = Convert.FromBase64String(key);
                var ivKey = Convert.FromBase64String(iv);

                var byteArray = ImageToByteArray(file);
                var encryptedBytes = EncryptImageByteArray(byteArray, bKey, ivKey);
                var filePath = Path.Combine(folderLocation, Guid.NewGuid().ToString());
                SaveEncryptedFile(encryptedBytes, filePath);
                imageFilePath = filePath;
                return true;
            }
            catch (Exception e)
            {
                imageFilePath = "";
                return false;
            }

        }

        public static byte[] ProcessDecodeImage(string filePath, string key, string iv)
        {
            var bKey = Convert.FromBase64String(key);
            var ivKey = Convert.FromBase64String(iv);

            var encryptedBytes = ReadEncryptedFile(filePath);
            var decyptBytes = DecryptByteArray(encryptedBytes, bKey, ivKey);

            return decyptBytes;
        }

        private static byte[] ImageToByteArray(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        private static byte[] EncryptImageByteArray(byte[] byteArray, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(byteArray, 0, byteArray.Length);
                        csEncrypt.FlushFinalBlock();
                        return msEncrypt.ToArray();
                    }
                }

            }
        }
        private static void SaveEncryptedFile(byte[] encryptedBytes, string filePath)
        {
            File.WriteAllBytes(filePath, encryptedBytes);
        }

        private static byte[] ReadEncryptedFile(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        private static byte[] DecryptByteArray(byte[] encryptedBytes, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var msDecrypt = new MemoryStream(encryptedBytes))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var msOutput = new MemoryStream())
                {
                    csDecrypt.CopyTo(msOutput);
                    return msOutput.ToArray();
                }
            }
        }
    }
}
