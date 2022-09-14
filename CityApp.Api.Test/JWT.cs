using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using CityApp.Common.Utilities;

namespace CityApp.Api.Test
{
    [TestClass]
    public class FileHash
    {
        [TestMethod]
        public void HashaFile()
        {
            var fileBytes = GetFileBytes(@"C:\\Temp\36a7ebf0-f53a-4e9e-8613-660807668701.mp4");

            var key = "9cc8653c-dc48-493c-9822-3c6e5d2345c3";

            var fileHash = Cryptography.Hashfile(fileBytes, key);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(fileHash));
        }

        [TestMethod]
        public void GenerateRSAKeyPair()
        {
            var keypair = Cryptography.GenerateRSAKeyPair();

            Assert.IsNotNull(keypair.Key, keypair.Value);

        }

        [TestMethod]
        public void GereateJWTToken()
        {
            var data = "{\"test\":\"hello world\"}";

            //Generate Private and Public Key
            var keypair = Cryptography.GenerateRSAKeyPair();
            var privateKey = keypair.Key;
            var publickKey = keypair.Value;


            //Use the Private Key To create a JWT Token
            var encodedJWT = Cryptography.GenerateJWTToken(privateKey, data);

            //Use the Public Key to Decode the Token and get the json back
            var decodedJWT = Cryptography.DecodeJWTToken(publickKey, encodedJWT);

            //Compare the original Json to the Decoded Json
            Assert.IsTrue(data == decodedJWT);
        }

        private byte[] GetFileBytes(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }


        [TestMethod]
        public void DecryptToken()
        {
            var publicKey = @"-----BEGIN PUBLIC KEY-----
                            MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCpZAs8JKP+N0LAJ+iIfz/WX8AT
                            c0+w3oBlfdlfsxteHgN0AghJNYorz1BIem1Wh6RPw0LYb2FF9YZUYL2S0oSVg4Ke
                            MJVukGltfwjHOn0kUUCDYU2FBCs1qbt80xRLdoZ5zERaQLjRLJBcx9CqlPwrZmUi
                            TgI8OkkFdqqRUjvcewIDAQAB
                            -----END PUBLIC KEY-----";
            var encodedJWT = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.ewogICJpZGVudGlmaWVyIjogIjEwNjY1ZTkzLWZhMjEtNDBkNy05MzI0LWEyYzNhOGI0OTAwNyIsCiAgInN1Ym1pdHRlZFV0YyI6ICIxMDY2NWU5My1mYTIxLTQwZDctOTMyNC1hMmMzYThiNDkwMDciLAogICJkZXZpY2UiOiAiIiwKICAidXNlcmVtYWlsIjogIiIsCiAgInVzZXJJZCI6ICI3MjA1OGUxYi1kOTNjLTQ3N2UtYjMwYi1hN2UxMDE2NzEyNTkiLAogICJjaXRhdGlvbk51bWJlciI6ICIxMDY2NWU5My1mYTIxLTQwZDctOTMyNC1hMmMzYThiNDkwMDciLAogICJhY2NvdW50TnVtYmVyIjogIjEwMDMiLAogICJsYXRpdHVkZSI6ICIzOC44MDc0MTkzOTU2Njc4IiwKICAibG9uZ2l0dWRlIjogIi0xMjEuMjUxOTcyMzgzNDE2IiwKICAicHVibGljS2V5IjogIk1JR2ZNQTBHQ1NxR1NJYjNEUUVCQVFVQUE0R05BRENCaVFLQmdRQ3BaQXM4SktQK04wTEFKK2lJZnovV1g4QVRcbmMwK3czb0JsZmRsZnN4dGVIZ04wQWdoSk5Zb3J6MUJJZW0xV2g2UlB3MExZYjJGRjlZWlVZTDJTMG9TVmc0S2Vcbk1KVnVrR2x0ZndqSE9uMGtVVUNEWVUyRkJDczFxYnQ4MHhSTGRvWjV6RVJhUUxqUkxKQmN4OUNxbFB3clptVWlcblRnSThPa2tGZHFxUlVqdmNld0lEQVFBQlxuXG4iLAogICJmaWxlcyI6IFsKICAgIHsKICAgICAgImNyZWF0ZWRVdGMiOiAiU2VwIDA3LDIwMTcuIDc6NDUgQU0iLAogICAgICAiZmlsZW5hbWUiOiAiMmJjNDkzMDAtYWQ0ZS00YjViLTkzNTItNWE2YjJlMmE0OWJhLm1wNCIsCiAgICAgICJzaGEyNTZoYXNoIjogIjFDNjdGNkI2MTcwRjhGQzExNjkxRkU5QUMwRjcyQzU0NTg5MDY3NjY4OUMwMzc4MzM0NjNCNTRFNkNGOUQ3Q0YiCiAgICB9LAogICAgewogICAgICAiY3JlYXRlZFV0YyI6ICJTZXAgMDcsMjAxNy4gNzo0NSBBTSIsCiAgICAgICJmaWxlbmFtZSI6ICIyYmM0OTMwMC1hZDRlLTRiNWItOTM1Mi01YTZiMmUyYTQ5YmFfdGh1bWIucG5nIiwKICAgICAgInNoYTI1Nmhhc2giOiAiNEE5RTRGMzE4N0Y1RkUzODYxMjk2QzBBRThENzNEQjNBNjU1MDI3MjNEMTVBNjNGMTQ4RkVERTk2Njg0MjM1OSIKICAgIH0KICBdCn0.M3V2y8Wz1j3F-YoFS24aMzE9xVe22kNXI-lT83T_a1Ke3Myrj7xQdEtU0G0MbvOy-QMTt60I5zVNoSjKNEIqFL7XQLZt3efFVBt43BeitvJejWxns_8Xqvnu7KS2lvs9zXua5bTMkP9bhMZWg5EK40O7-u-f37vjzNmPQqSupUA";

            //Use the Public Key to Decode the Token and get the json back
            var decodedJWT = Cryptography.DecodeJWTToken(publicKey, encodedJWT);

        }
    }
    }
