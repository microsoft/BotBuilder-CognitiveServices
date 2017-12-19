namespace LuisActions.Samples.Web
{
    using System;
    using System.Text;
    using System.Web.Security;

    public class StringCrypto
    {
        public static string Encrypt(string text)
        {
            return Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(text)));
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                return Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(encryptedText)));
            }
            catch
            {
                return null;
            }
        }
    }
}