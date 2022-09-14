using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using CityApp.Common.Caching;
using CityApp.Common.Models;
using CityApp.Data;
using CityApp.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityApp.Services.Models;
using Microsoft.Extensions.Options;

namespace CityApp.Services
{
    public class FileService : ICustomService
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<FileService>();
        private readonly AppSettings _appSettings;

        public FileService()
        {
        }
        public FileService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

        }

        /// <summary>
        /// Upload File to AWS S3
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <param name="awsAccessKey"></param>
        /// <param name="awsSecretKey"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task<bool> UploadFile(byte[] file, string fileName, string awsAccessKey, string awsSecretKey, bool isPublic = false)
        {
            bool result = true;

            try
            {
                await Task.FromResult(1);
                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.USWest2))
                {
                    try
                    {
                        // simple object put
                        PutObjectRequest request = new PutObjectRequest()
                        {
                            InputStream = new MemoryStream(file),
                            BucketName = _appSettings.AmazonS3Bucket,
                            Key = fileName,
                        };

                        if(isPublic)
                        {
                            request.CannedACL = S3CannedACL.PublicRead;
                        }

                        PutObjectResponse response = await client.PutObjectAsync(request);

                    }
                    catch (AmazonS3Exception amazonS3Exception)
                    {
                        if (amazonS3Exception.ErrorCode != null &&
                            (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                            amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                        {
                            _logger.Error("Please check the provided AWS Credentials.");
                            _logger.Error("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                        }
                        else
                        {
                            _logger.Error("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message);
                        }
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error uploading file with bytes : {file.Count()}");
                result = false;
            }

            return result;
        }


        /// <summary>
        /// Read file from AWS S3
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="awsAccessKey"></param>
        /// <param name="awsSecretKey"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task<CityAppFile> ReadFile(string fileName, string awsAccessKey, string awsSecretKey, string bucketName)
        {

            CityAppFile file = null;
            try
            {
                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.USWest2))
                {
                    file = new CityAppFile();

                    GetObjectRequest request = new GetObjectRequest()
                    {
                        BucketName = bucketName,
                        Key = fileName
                    };

                    try
                    {
                        using (GetObjectResponse response = await client.GetObjectAsync(request))
                        {
                            MemoryStream stream = new MemoryStream();
                            response.ResponseStream.CopyTo(stream);

                            file.FileStream = stream;
                            file.FileName = fileName.Split('/')[2];
                            file.FileBytes = stream.ToArray();
                        }
                    }
                    catch (Amazon.S3.AmazonS3Exception ex)
                    {
                        if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                            return file;
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error reading file from aws: fileName: {fileName}, awsAccessKey: {awsAccessKey}, awsSecretKey: {awsSecretKey}, bucketName: {bucketName} ");
            }
            return file;
        }


        /// <summary>
        /// Read file from AWS S3
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="awsAccessKey"></param>
        /// <param name="awsSecretKey"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFile(string fileName, string awsAccessKey, string awsSecretKey, string bucketName)
        {

            bool result = false;
            try
            {
                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.USWest2))
                {

                    DeleteObjectRequest request = new DeleteObjectRequest()
                    {
                        BucketName = bucketName,
                        Key = fileName
                    };


                    await client.DeleteObjectAsync(request);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error deleting file from aws: fileName: {fileName}, awsAccessKey: {awsAccessKey}, awsSecretKey: {awsSecretKey}, bucketName: {bucketName} ");

                result = false;
            }
            return result;
        }


        public string ReadFileUrl(string fileName, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            string url = string.Empty;

            try
            {
                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.USWest2))
                {
                    GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest()
                    {
                        BucketName = bucketName,
                        Key = fileName,
                        Expires = DateTime.Now.Add(new TimeSpan(0, 0, 15, 0))
                    };

                    url = client.GetPreSignedURL(request1);

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error reading file from aws: fileName: {fileName}, awsAccessKey: {awsAccessKey}, awsSecretKey: {awsSecretKey}, bucketName: {bucketName} ");
            }
            return url;
        }
    }
}
