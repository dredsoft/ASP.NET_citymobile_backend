using CityApp.Common.Caching;
using CityApp.Common.Models;
using CityApp.Data;
using CityApp.Data.Models;
using CityApp.Services.Models;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CityApp.Services
{
    /// <summary>
    /// Dealing with evidence package work flow
    /// </summary>
    public class TicketWorkFlowService : ICustomService
    {
        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        private static readonly object _randomLock = new object();
        private readonly FileService _fileService;
        private static readonly ILogger _logger = Log.Logger.ForContext<TicketWorkFlowService>();
        private IHostingEnvironment _env;
        private AppSettings _appSettings;
        private readonly CommonContext _commonCtx;
        private CachedAccount _cachedAccount;


        public TicketWorkFlowService(RedisCache cache, FileService _fileService, IHostingEnvironment environment, CommonContext commonCtx, IOptions<AppSettings> appSettings)
        {
            this._fileService = _fileService;
            _env = environment;
            _commonCtx = commonCtx;
            _appSettings = appSettings.Value;

        }

        public async Task<string> SendEvidencePackage(CachedAccount cachedAccount, long citationNumber, string FTPURl, string FTPFolder, string FTPUsername, string FTPPassword, Citation citationAttachment, string awsAccessKey, string awsSecretKey, string bucketName)
        {
            var result = string.Empty;

            try
            {
                _cachedAccount = cachedAccount;
                // var FilePath = $"c:\\temp";
                //var archivePath = $"c:\\temp\\{AccountNumber}-{citationNumber}.zip";
                //Create folder if not Exists
                var FilePath = Path.Combine(_env.WebRootPath, @"upload");

                var FileLocation = FilePath + @"\CitationReceipt.txt";

                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                else
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(FilePath);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }



                var archivePath = $"{FilePath}\\{cachedAccount.Number}-{citationNumber}.zip";

                //Read the file and copy it to the temp directory
                if (citationAttachment.Attachments != null)
                {
                    foreach (var attach in citationAttachment.Attachments)
                    {
                        var cityAppFile = await _fileService.ReadFile(attach.Attachment.Key, awsAccessKey, awsSecretKey, bucketName);

                        if (cityAppFile.FileName != null)
                        {
                            //Download the file to temp path
                            var newFile = $"{FilePath}\\{attach.Attachment.FileName}";
                            using (var fileStream = File.Create(newFile))
                            {
                                cityAppFile.FileStream.Position = 0;
                                cityAppFile.FileStream.CopyTo(fileStream);
                            }
                        }
                    }
                }


                //Create Text file
                await CitationReceipt(citationAttachment, FileLocation, cachedAccount);

                //Zip all attachment
                result = await CreateArchive(FilePath, archivePath, FTPURl, FTPFolder, FTPUsername, FTPPassword);


            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error SendEvidencePackage file To FTP: Exception: {ex}, InnerException: {ex.InnerException} ");

                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Zip all Downloaded attachment
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="archiveName"></param>
        /// <param name="FTPURl"></param>
        /// <param name="FTPFolder"></param>
        /// <param name="FTPUsername"></param>
        /// <param name="FTPPassword"></param>
        /// <returns></returns>
        public async Task<string> CreateArchive(string folder, string archiveName, string FTPURl, string FTPFolder, string FTPUsername, string FTPPassword)
        {
            var result = string.Empty;
            var filesCount = 0;
            try
            {
                string folderFullPath = Path.GetFullPath(folder);
                string archivePath = Path.Combine(folderFullPath, archiveName);
                if (File.Exists(archivePath))
                {
                    File.Delete(archivePath);

                }
                IEnumerable<string> files = Directory.EnumerateFiles(folder,
                        "*.*", SearchOption.AllDirectories);
                using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
                {
                    foreach (string file in files)
                    {

                        try
                        {
                            var addFile = Path.GetFullPath(file);
                            if (addFile != archivePath)
                            {
                                addFile = addFile.Substring(folderFullPath.Length);
                                archive.CreateEntryFromFile(file, addFile);
                                filesCount++;
                            }
                        }
                        catch (IOException ex)
                        {
                            throw ex;
                        }

                    }

                }

                //Upload Zip File to FTP
                if (filesCount > 0)
                {
                    result = await UploadToS3(folder, archiveName, FTPURl, FTPFolder, FTPUsername, FTPPassword);
                }


            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error CreateArchive file To FTP: Exception: {ex}, InnerException: {ex.InnerException} ");
                throw ex;
            }

            return result;
        }



        /// <summary>
        /// Upload File to FTP
        /// </summary>
        /// <param name="strfilename"></param>
        /// <param name="FTPURl"></param>
        /// <param name="FTPFolder"></param>
        /// <param name="FTPUsername"></param>
        /// <param name="FTPPassword"></param>
        public async Task<int> UploadFtpFile(string FilePath, string strfilename, string FTPURl, string FTPFolder, string FTPUsername, string FTPPassword)
        {
            //FTP Server URL.
            int result = 0;
            string ftp = FTPURl;
            //FTP Folder name. Leave blank if you want to upload to root folder.
            string ftpFolder = FTPFolder;
            string fileName = Path.GetFileName(strfilename);
            try
            {

                //Create FTP Request.
                FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder + fileName);
                ftpReq.UsePassive = true;
                ftpReq.UseBinary = true;
                ftpReq.Proxy = new WebProxy();
                ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                ftpReq.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                byte[] b = File.ReadAllBytes(strfilename);

                ftpReq.ContentLength = b.Length;
                using (Stream s = ftpReq.GetRequestStream())
                {
                    s.Write(b, 0, b.Length);
                }

                FtpWebResponse ftpResp = (FtpWebResponse)ftpReq.GetResponse();
                ftpResp.Close();

                //Delete downloaded file from local temp folder after uploded to FTP
                System.IO.DirectoryInfo di = new DirectoryInfo(FilePath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                result = 1;

            }

            catch (Exception ex)
            {
                //throw ex.InnerException;
                _logger.Error(ex, $"Error uploading file To FTP: Exception: {ex}, InnerException: {ex.InnerException} ");

            }

            return result;

        }


        /// <summary>
        /// Upload File to FTP
        /// </summary>
        /// <param name="strfilename"></param>
        /// <param name="FTPURl"></param>
        /// <param name="FTPFolder"></param>
        /// <param name="FTPUsername"></param>
        /// <param name="FTPPassword"></param>
        public async Task<string> UploadToS3(string FilePath, string strfilename, string FTPURl, string FTPFolder, string FTPUsername, string FTPPassword)
        {
            //FTP Server URL.
            var result = string.Empty;
            string ftp = FTPURl;
            //FTP Folder name. Leave blank if you want to upload to root folder.
            string ftpFolder = FTPFolder;
            string fileName = Path.GetFileName(strfilename);
            try
            {

                //Create FTP Request.
                FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder + fileName);
                ftpReq.UsePassive = true;
                ftpReq.UseBinary = true;
                ftpReq.Proxy = new WebProxy();
                ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                ftpReq.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                byte[] b = File.ReadAllBytes(strfilename);

                var uploadS3Key = $"evidencepackage/{_cachedAccount.Number}/{fileName}";

                await _fileService.UploadFile(b, uploadS3Key, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, false);


                //Delete downloaded file from local temp folder after uploded to FTP
                System.IO.DirectoryInfo di = new DirectoryInfo(FilePath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                result = uploadS3Key;

            }

            catch (Exception ex)
            {
                //throw ex.InnerException;
                _logger.Error(ex, $"Error uploading file To FTP: Exception: {ex}, InnerException: {ex.InnerException} ");

            }

            return result;

        }

        public async Task<bool> CitationReceipt(Citation citation, string FileLocation, CachedAccount cachedAccount)
        {


            var commonAccount = await _commonCtx.CommonAccounts
                .Include(m => m.City)
                .Where(x => x.Id == citation.AccountId).SingleOrDefaultAsync();

            if (commonAccount == null)
            {
                return false;
            }
            var citationReceipt = new CitationReceiptModel()
            {
                CitationNumber = citation.CitationNumber,
                AccountNumber = cachedAccount.Number,
                AccountName = cachedAccount.Name,
                AccountCity = commonAccount.City.Name,
                AccountState = commonAccount.City.StateCode,
                ViolationCode = citation.Violation != null ? citation.Violation.Code : "",
                ViolationTitle = citation.Violation != null ? citation.Violation.Name : "",
                ViolationState = citation.State,
                ViolationCity = citation.City,
                LicensePlate = citation.LicensePlate,
                LicenseState = citation.LicenseState,
                ViolationLatitude = citation.Latitude,
                ViolationLongitude = citation.Longitude,
                ViolationCreatedUTC = citation.Violation != null ? citation.Violation.CreateUtc : DateTime.UtcNow,
            };

            var data = JsonConvert.SerializeObject(citationReceipt);

            TextWriter tw = File.CreateText(FileLocation);
            tw.WriteLine(data);
            tw.Close();


            return true;
        }


        public async Task<bool> SaveVideoAttachment(float Time, string Src, string fileName, string folderPath, string AWSAccessKeyID, string AWSSecretKey)
        {
            var FilePath = Path.Combine(_env.WebRootPath, @"thumbnail");
            Attachment model = new Attachment();
            bool result = false;
            try
            {
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                var thumbnailPath = FilePath + @"\" + fileName;


                //Create thumbnail from video
                Src = Regex.Replace(Src, @"\bhttps\b", "http");
                var inputFile = new MediaFile { Filename = Src };
                var outputFile = new MediaFile { Filename = thumbnailPath };

                var webRoot = _env.WebRootPath;
                var file = System.IO.Path.Combine(webRoot, "fmpeg.exe");

                using (var engine = new Engine(file))
                {
                    engine.GetMetadata(inputFile);
                    var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(Time) };
                    engine.GetThumbnail(inputFile, outputFile, options);
                }

                byte[] imagebyte = File.ReadAllBytes(thumbnailPath);

                //upload to AWS bucket
                result = await _fileService.UploadFile(imagebyte, folderPath, AWSAccessKeyID, AWSSecretKey);

                //delete folder files after upload
                CleanFolder(FilePath);


            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred with the message '{0}' when writing an object", ex.Message);
                result = false;
            }

            return result;
        }


        public void CleanFolder(string FilePath)
        {
            System.IO.DirectoryInfo dinfo = new DirectoryInfo(FilePath);

            foreach (FileInfo file in dinfo.GetFiles())
            {
                file.Delete();
            }
        }


    }
}
