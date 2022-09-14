using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Models
{
    /// <summary>
    /// Statically typed settings that are loaded from appsettings.{environment}.json.
    /// </summary>
    public class AppSettings
    {
        public string WhiteListAdmin { get; set; }
        public string AbsolutePath { get; set; }
        public string EncryptionKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string AWSAccessKeyID { get; set; }
        public string AmazonS3Bucket { get; set; }
        public string AmazonS3Url { get; set; }
        public string FileTypes { get; set; }
        public string ImageTypes { get; set; }
        public long ImageSize { get; set; }
        public double ProcessingFee { get; set; } = 0;
        public string WebhookToken { get; set; }

        public string GoogleMapsAPIKey { get; set; }

        public string GoogleTrackingID { get; set; }

        public double CitationTimeSpan { get; set; }

        public string LocationIQKey { get; set; }

        public long UploadFileSize { get; set; }

        public decimal AccountMaxDistanceMiles { get; set; }

        #region Stripe Settings

        public string StripeSecretKey { get; set; }
        public string StripePublishableKey { get; set; }

        #endregion

        #region SMTP Settings

        public string SmtpServer { get; set; }

        public int SmtpPortNumber { get; set; }

        public string SmtpUserName { get; set; }

        public string SmtpPassword { get; set; }

        public string FromAddress { get; set; }

        public string FromAddressTitle { get; set; }

        #endregion

        #region FTP Settings
        public string FTPUrl { get; set; }
        public string FTPFolderName { get; set; }
        public string FTPUserName { get; set; }
        public string FTPPassWord { get; set; }


        #endregion

        #region PushNotification
        public string CertificateName { get; set; }
        public string CertificatePwd { get; set; }
        #endregion
    }

}