
using Jose;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CityApp.Common.Utilities
{
    public class Cryptography
    {
        public static readonly string Key = "MAKV2SPBNI99212";

        #region EncryptAndDecrypt

        /// <summary>
        /// encrypt the text
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = Key;
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        /// <summary>
        /// decrypt text
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = Key;
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        #endregion

        public static string Hashfile(byte[] bytes, string key)
        {
            var hmac = new HMac(new Sha256Digest());
            hmac.Init(new KeyParameter(Encoding.UTF8.GetBytes(key)));
            byte[] result = new byte[hmac.GetMacSize()];

            hmac.BlockUpdate(bytes, 0, bytes.Length);
            hmac.DoFinal(result, 0);


            return GetStringFromHash(result);
        }

        public static string Encode(byte[] data)
        {
            SHA256 sha256 = SHA256Managed.Create();

            byte[] hash = sha256.ComputeHash(data);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        /// <summary>
        /// Generate Private and Public RSA 1024 bit key.
        /// The keys are converted into a string format.  Commonly called PEM format.
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> GenerateRSAKeyPair()
        {
            //Initialize
            RSACryptoServiceProvider rsa = null;
            rsa = new RSACryptoServiceProvider(1024);

            //Convert the private key to a PEM string representation 
            TextWriter privateTextWriter = new StringWriter();
            RSACryptoHelper.ExportPrivateKey(rsa, privateTextWriter);
            privateTextWriter.Close();

            //Convert the public key to a PEM string representation 
            TextWriter publicTextWriter = new StringWriter();
            RSACryptoHelper.ExportPublicKey(rsa, publicTextWriter);
            publicTextWriter.Close();

            var privateKey = privateTextWriter.ToString();
            var publicKey = publicTextWriter.ToString();

            return new KeyValuePair<string, string>(privateKey, publicKey);
        }

        /// <summary>
        /// Generates a JWT Token using a private key in PEM format for the json payload
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string GenerateJWTToken(string privateKey, string json)
        {
            var result = string.Empty;

            string pemString = privateKey;
            string jwt = string.Empty;
            AsymmetricCipherKeyPair keyPair;

            //Convert the PEM string format to an object
            using (StringReader sr = new StringReader(privateKey))
            {
                PemReader pr = new PemReader(sr);
                keyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
            }

            //Create an rsaParamter object from the PemReader object
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024))
            {
                //Generate a RSA Key from the RSAParameters
                rsa.ImportParameters(rsaParams);

                //Generate a JWT Token
                jwt = Jose.JWT.Encode(json, rsa, Jose.JwsAlgorithm.RS256);
            }

            return jwt;

        }


        public static string DecodeJWTToken(string publicKey, string token)
        {
            RSAParameters rsaParams;

            using (var tr = new StringReader(publicKey))
            {
                var pemReader = new PemReader(tr);
                var publicKeyParams = pemReader.ReadObject() as RsaKeyParameters;
                if (publicKeyParams == null)
                {
                    throw new Exception("Could not read RSA public key");
                }
                rsaParams = DotNetUtilities.ToRSAParameters(publicKeyParams);
            }
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                // This will throw if the signature is invalid
                var result = Jose.JWT.Decode(token, rsa, Jose.JwsAlgorithm.RS256);

                return result;
            }
        }

    }
}
