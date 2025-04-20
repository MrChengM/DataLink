using System;
using System.Security.Cryptography;
using System.Text;

namespace Utillity.Security
{
    public static class SHA256Code
    {
        // 对密码进行哈希
        public static string HashPassword(string password)
        {
            // 使用SHA256算法创建一个SHA256对象
            using (SHA256 sha256 = SHA256.Create())
            {
                // 将输入字符串转换为字节数组
                byte[] bytes = Encoding.UTF8.GetBytes(password);

                // 计算哈希值
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // 将哈希值的字节数组转换为十六进制字符串
                StringBuilder hashBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashBuilder.Append(b.ToString("x2")); // "x2"表示以两位十六进制格式输出
                }

                // 返回哈希值的十六进制字符串
                return hashBuilder.ToString();
            }
        }
        // 生成随机盐
        public static byte[] GenerateRNGSalt()
        {
            const int saltSize = 16; // 盐的长度（16字节）
            using (var rng = new RNGCryptoServiceProvider())
            {
                var salt = new byte[saltSize];
                rng.GetBytes(salt);
                return salt;
            }
        }
        /// <summary>
        /// 将字符串转换为盐值，至少16个字节
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ConvertToSalt(string saltStr="GMGMGMGMGMGMGMGM")
        {
            var bytes = Encoding.UTF8.GetBytes(saltStr);
            if (bytes.Length >= 16)
            {
                return bytes;
            }
            else
            {
                byte[] result = new byte[16];
                Array.Copy(bytes, result, bytes.Length);
                return result;
            }

        }
        // 使用盐对密码进行哈希
        public static string HashPasswordWithSalt(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                // 将密码和盐组合在一起
                var combinedBytes = new byte[salt.Length + Encoding.UTF8.GetByteCount(password)];
                Array.Copy(salt, 0, combinedBytes, 0, salt.Length);
                Array.Copy(Encoding.UTF8.GetBytes(password), 0, combinedBytes, salt.Length, password.Length);

                // 计算哈希值
                byte[] hashBytes= sha256.ComputeHash(combinedBytes);
                // 将哈希值的字节数组转换为十六进制字符串
                StringBuilder hashBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashBuilder.Append(b.ToString("x2")); // "x2"表示以两位十六进制格式输出
                }

                // 返回哈希值的十六进制字符串
                return hashBuilder.ToString();
            }
        }
       
    }
}
