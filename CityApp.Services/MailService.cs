using Microsoft.EntityFrameworkCore;
using CityApp.Data;
using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using CityApp.Common.Models;
using Microsoft.Extensions.Options;
using Serilog;
using CityApp.Data.Enums;

namespace CityApp.Services
{
    public class MailService : ICustomService
    {
        private IHostingEnvironment _hostingEnvironment;
        private readonly AppSettings _appSettings;
        private static readonly ILogger _logger = Log.Logger.ForContext<MailService>();

        public MailService(IHostingEnvironment environment, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _hostingEnvironment = environment;
        }

        /// <summary>
        /// Send email for password reset.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        public async Task<bool> SendPasswordResetEmail(string to, string callbackUrl)
        {

            var builder = new BodyBuilder();

            var resetPasswordTemplate = Path.Combine(_hostingEnvironment.WebRootPath, @"emailTemplates\ResetPassword.html");

            _logger.Error($"Read template from:{resetPasswordTemplate}");

            using (StreamReader SourceReader = System.IO.File.OpenText(resetPasswordTemplate))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }
            _logger.Error($"template loaded.  Yes!!:{resetPasswordTemplate}");


            var body = builder.HtmlBody.Replace("LinkUrl", _appSettings.AbsolutePath + callbackUrl).Replace("UserName", to);

            return await SendEmailAsync(to, "Password Reset", body, true);
        }




        /// <summary>
        /// Send Invitation email .
        /// </summary>
        /// <param name="to"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        public async Task<bool> SendInvitationEmail(string Name, string to, string callbackUrl)
        {

            var builder = new BodyBuilder();

            var resetPasswordTemplate = Path.Combine(_hostingEnvironment.WebRootPath, @"emailTemplates\UserInvitation.html");

            using (StreamReader SourceReader = System.IO.File.OpenText(resetPasswordTemplate))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }

            _logger.Error($"Building Body");

            var body = builder.HtmlBody.Replace("LinkUrl", _appSettings.AbsolutePath + callbackUrl).Replace("UserName", to);
            _logger.Error($"Body Built");

            return await SendEmailAsync(to, "Invitation", body, true);
        }


        /// <summary>
        /// Sends Email
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool html = true)
        {
            try
            {
                //From Address  
                string FromAddress = _appSettings.FromAddress;
                string FromAdressTitle = _appSettings.FromAddressTitle;

                //To Address  
                string ToAddress = to;
                string Subject = subject;
                string BodyContent = body;
                var builder = new BodyBuilder();
                if (html)
                {
                    builder.HtmlBody = body;
                }
                else
                {
                    builder.TextBody = body;
                }

                //Smtp Server  
                string SmtpServer = _appSettings.SmtpServer;
                //Smtp Port Number  
                int SmtpPortNumber = _appSettings.SmtpPortNumber;

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(FromAdressTitle, FromAddress));
                mimeMessage.To.Add(new MailboxAddress(ToAddress));
                mimeMessage.Subject = Subject;
                mimeMessage.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {

                    client.Connect(SmtpServer, SmtpPortNumber, false);
                    client.Authenticate(_appSettings.SmtpUserName, _appSettings.SmtpPassword);

                    _logger.Error($"Send Email");

                    await client.SendAsync(mimeMessage);
                    _logger.Error($"Email Sent to: {ToAddress}");

                    client.Disconnect(true);

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error sending mail to:{to}, subject:{subject}, body:{body}");

                throw ex;
            }


            return true;
        }

        public async Task<bool> SendAssignUserInvitationEmail(string Email, string to, string callbackUrl, long CitationNumber, string ViolationName, DateTime CreateUtc)
        {

            var builder = new BodyBuilder();

            var resetPasswordTemplate = Path.Combine(_hostingEnvironment.WebRootPath, @"emailTemplates\AssignUserToCitation.html");

            _logger.Error($"Read template from:{resetPasswordTemplate}");

            using (StreamReader SourceReader = System.IO.File.OpenText(resetPasswordTemplate))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }
            _logger.Error($"template loaded.  Yes!!:{resetPasswordTemplate}");


            var body = builder.HtmlBody.Replace("LinkUrl", _appSettings.AbsolutePath + callbackUrl).Replace("Email", Email)
                .Replace("CitationNumber", Convert.ToString(CitationNumber)).Replace("ViolationName", ViolationName).Replace("CitationCreatedDate", Convert.ToString(CreateUtc));

            return await SendEmailAsync(to, "Ticket Assignment", body, true);
        }


        public async Task<bool> SendStatusChangedEmail(string Email, string to, string callbackUrl, string Reason, string Status, long CitationNumber, string ViolationName, DateTime CreateUtc)
        {

            var builder = new BodyBuilder();
            string _reason = string.Empty;

            var resetPasswordTemplate = Path.Combine(_hostingEnvironment.WebRootPath, @"emailTemplates\StatusChanged.html");

            _logger.Error($"Read template from:{resetPasswordTemplate}");

            using (StreamReader SourceReader = System.IO.File.OpenText(resetPasswordTemplate))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }
            _logger.Error($"template loaded.  Yes!!:{resetPasswordTemplate}");

            if (!string.IsNullOrWhiteSpace(Reason))
            {
                _reason = $"({Reason})";
            }


            var body = builder.HtmlBody.Replace("LinkUrl", _appSettings.AbsolutePath + callbackUrl).Replace("Email", Email).Replace("(Reason.)", _reason).Replace("Status", Status)
                .Replace("CitationNumber", Convert.ToString(CitationNumber)).Replace("ViolationName", ViolationName).Replace("CitationCreatedDate", Convert.ToString(CreateUtc));

            return await SendEmailAsync(to, "Ticket", body, true);
        }

        public async Task<bool> SendMessageToReceipt(string Message, string Receipt, string displayName, string Sender)
        {
            var builder = new BodyBuilder();
            string _reason = string.Empty;

            var resetPasswordTemplate = Path.Combine(_hostingEnvironment.WebRootPath, @"emailTemplates\SendMessage.html");

            _logger.Error($"Read template from:{resetPasswordTemplate}");

            using (StreamReader SourceReader = System.IO.File.OpenText(resetPasswordTemplate))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }
            _logger.Error($"template loaded.  Yes!!:{resetPasswordTemplate}");


            var body = builder.HtmlBody.Replace("UserName", displayName).Replace("Messages", Message).Replace("sender", Sender);

            return await SendEmailAsync(Receipt, "Message", body, true);
        }

    }
}
