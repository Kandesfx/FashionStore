using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace FashionStore.Utilities.Security
{
    public class CookieHelper
    {
        // Key để mã hóa (nên lưu trong config trong thực tế)
        private static readonly string EncryptionKey = "FashionStore_RememberMe_Key_2024";

        /// <summary>
        /// Mã hóa thông tin đăng nhập để lưu vào cookie
        /// </summary>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                byte[] key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
                byte[] iv = new byte[16];
                Array.Copy(key, iv, 16);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Giải mã thông tin từ cookie
        /// </summary>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            try
            {
                byte[] key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
                byte[] iv = new byte[16];
                Array.Copy(key, iv, 16);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipherText);
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Lưu thông tin đăng nhập vào cookie (lưu nhiều tài khoản dưới dạng JSON)
        /// </summary>
        public static void SaveLoginCookie(HttpResponseBase response, HttpRequestBase request, string username, string password, bool rememberMe)
        {
            // Debug: Log để kiểm tra
            System.Diagnostics.Debug.WriteLine($"SaveLoginCookie called: rememberMe={rememberMe}, username={username}, password={(string.IsNullOrEmpty(password) ? "empty" : "has_value")}");
            
            if (rememberMe && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // Mã hóa password trước khi lưu
                string encryptedPassword = Encrypt(password);

                if (!string.IsNullOrEmpty(encryptedPassword))
                {
                    // Đọc cookie hiện tại (nếu có)
                    Dictionary<string, string> savedAccounts = GetAllSavedAccounts(request);
                    
                    // Thêm hoặc cập nhật tài khoản mới
                    savedAccounts[username.ToLower()] = encryptedPassword;

                    // Chuyển đổi dictionary thành JSON
                    string jsonData = JsonConvert.SerializeObject(savedAccounts);

                    // Mã hóa toàn bộ JSON để bảo mật
                    string encryptedJson = Encrypt(jsonData);

                    // Xóa cookie cũ nếu có (để tương thích ngược)
                    if (response.Cookies["remember_username"] != null)
                    {
                        response.Cookies.Remove("remember_username");
                    }
                    if (response.Cookies["remember_password"] != null)
                    {
                        response.Cookies.Remove("remember_password");
                    }

                    // Tạo cookie mới chứa tất cả tài khoản
                    HttpCookie accountsCookie = new HttpCookie("remember_accounts")
                    {
                        Value = encryptedJson,
                        Expires = DateTime.Now.AddDays(30), // Lưu 30 ngày
                        HttpOnly = false, // Cho phép JavaScript đọc được
                        Secure = false, // Có thể Secure cookie không hoạt động trên localhost
                        Path = "/", // Đảm bảo cookie có thể truy cập từ mọi path
                        SameSite = SameSiteMode.Lax // Đảm bảo cookie hoạt động với redirect
                    };

                    response.Cookies.Add(accountsCookie);
                    System.Diagnostics.Debug.WriteLine($"Cookie remember_accounts added successfully with {savedAccounts.Count} account(s).");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to encrypt password");
                }
            }
            else
            {
                // Nếu không tích remember me, chỉ xóa tài khoản hiện tại (không xóa tất cả)
                if (!string.IsNullOrEmpty(username))
                {
                    Dictionary<string, string> savedAccounts = GetAllSavedAccounts(request);
                    if (savedAccounts.ContainsKey(username.ToLower()))
                    {
                        savedAccounts.Remove(username.ToLower());
                        
                        if (savedAccounts.Count > 0)
                        {
                            // Vẫn còn tài khoản khác, lưu lại
                            string jsonData = JsonConvert.SerializeObject(savedAccounts);
                            string encryptedJson = Encrypt(jsonData);
                            
                            HttpCookie accountsCookie = new HttpCookie("remember_accounts")
                            {
                                Value = encryptedJson,
                                Expires = DateTime.Now.AddDays(30),
                                HttpOnly = false,
                                Secure = false,
                                Path = "/",
                                SameSite = SameSiteMode.Lax
                            };
                            response.Cookies.Add(accountsCookie);
                        }
                        else
                        {
                            // Không còn tài khoản nào, xóa cookie
                            HttpCookie accountsCookie = new HttpCookie("remember_accounts")
                            {
                                Expires = DateTime.Now.AddDays(-1),
                                Path = "/"
                            };
                            response.Cookies.Add(accountsCookie);
                        }
                    }
                }
                
                // Xóa cookie cũ (để tương thích ngược)
                if (response.Cookies["remember_username"] != null)
                {
                    response.Cookies.Remove("remember_username");
                }
                HttpCookie usernameCookie = new HttpCookie("remember_username")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Path = "/"
                };
                response.Cookies.Add(usernameCookie);

                if (response.Cookies["remember_password"] != null)
                {
                    response.Cookies.Remove("remember_password");
                }
                HttpCookie passwordCookie = new HttpCookie("remember_password")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Path = "/"
                };
                response.Cookies.Add(passwordCookie);
            }
        }

        /// <summary>
        /// Đọc tất cả tài khoản đã lưu từ cookie (hỗ trợ cả cookie cũ và mới)
        /// </summary>
        private static Dictionary<string, string> GetAllSavedAccounts(HttpRequestBase request)
        {
            Dictionary<string, string> accounts = new Dictionary<string, string>();

            if (request == null || request.Cookies == null)
                return accounts;

            try
            {
                // Đọc cookie mới (chứa nhiều tài khoản)
                HttpCookie accountsCookie = request.Cookies["remember_accounts"];
                if (accountsCookie != null && !string.IsNullOrEmpty(accountsCookie.Value))
                {
                    string decryptedJson = Decrypt(accountsCookie.Value);
                    if (!string.IsNullOrEmpty(decryptedJson))
                    {
                        accounts = JsonConvert.DeserializeObject<Dictionary<string, string>>(decryptedJson);
                        if (accounts == null)
                        {
                            accounts = new Dictionary<string, string>();
                        }
                    }
                }
                
                // Đọc cookie cũ (để tương thích ngược) - chỉ đọc nếu chưa có cookie mới
                if (accounts.Count == 0)
                {
                    HttpCookie usernameCookie = request.Cookies["remember_username"];
                    HttpCookie passwordCookie = request.Cookies["remember_password"];

                    if (usernameCookie != null && !string.IsNullOrEmpty(usernameCookie.Value) &&
                        passwordCookie != null && !string.IsNullOrEmpty(passwordCookie.Value))
                    {
                        string username = usernameCookie.Value.Trim();
                        string encryptedPassword = passwordCookie.Value;
                        
                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(encryptedPassword))
                        {
                            accounts[username.ToLower()] = encryptedPassword;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading saved accounts: {ex.Message}");
                accounts = new Dictionary<string, string>();
            }

            return accounts;
        }

        /// <summary>
        /// Đọc thông tin đăng nhập từ cookie theo username (hỗ trợ cả cookie cũ và mới)
        /// </summary>
        public static LoginCookieData GetLoginCookie(HttpRequestBase request, string username = null)
        {
            if (request == null || request.Cookies == null)
                return null;

            try
            {
                Dictionary<string, string> savedAccounts = GetAllSavedAccounts(request);
                
                if (savedAccounts.Count == 0)
                    return null;

                // Nếu không có username, lấy tài khoản đầu tiên (tương thích ngược)
                if (string.IsNullOrEmpty(username))
                {
                    foreach (var account in savedAccounts)
                    {
                        string decryptedPassword = Decrypt(account.Value);
                        if (!string.IsNullOrEmpty(decryptedPassword))
                        {
                            return new LoginCookieData
                            {
                                Username = account.Key,
                                Password = decryptedPassword
                            };
                        }
                    }
                }
                else
                {
                    // Tìm tài khoản theo username
                    string usernameKey = username.ToLower();
                    if (savedAccounts.ContainsKey(usernameKey))
                    {
                        string encryptedPassword = savedAccounts[usernameKey];
                        string decryptedPassword = Decrypt(encryptedPassword);
                        
                        if (!string.IsNullOrEmpty(decryptedPassword))
                        {
                            return new LoginCookieData
                            {
                                Username = username,
                                Password = decryptedPassword
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading login cookie: {ex.Message}");
                // Không thể xóa cookie từ HttpRequestBase, chỉ log lỗi
            }

            return null;
        }

        /// <summary>
        /// Xóa cookie ghi nhớ đăng nhập (xóa tất cả tài khoản)
        /// </summary>
        public static void RemoveLoginCookie(HttpResponseBase response)
        {
            // Xóa cookie mới
            HttpCookie accountsCookie = new HttpCookie("remember_accounts")
            {
                Expires = DateTime.Now.AddDays(-1),
                Path = "/"
            };
            response.Cookies.Add(accountsCookie);

            // Xóa cookie cũ (để tương thích ngược)
            HttpCookie usernameCookie = new HttpCookie("remember_username")
            {
                Expires = DateTime.Now.AddDays(-1),
                Path = "/"
            };
            response.Cookies.Add(usernameCookie);

            HttpCookie passwordCookie = new HttpCookie("remember_password")
            {
                Expires = DateTime.Now.AddDays(-1),
                Path = "/"
            };
            response.Cookies.Add(passwordCookie);
        }
    }

    public class LoginCookieData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

