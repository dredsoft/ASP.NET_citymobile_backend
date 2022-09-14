using Microsoft.EntityFrameworkCore;
using CityApp.Data;
using CityApp.Data.Models;
using System;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using CityApp.Common.Models;
using Microsoft.Extensions.Options;

namespace CityApp.Services
{
    public class CommonUserService : ICustomService
    {
        public readonly CommonContext _commonCtx;
        private IHostingEnvironment _hostingEnvironment;
        private readonly AppSettings _appSettings;
        public CommonUserService(CommonContext commonCtx, IHostingEnvironment environment, IOptions<AppSettings> appSettings)
        {
            _commonCtx = commonCtx;
            _hostingEnvironment = environment;
            _appSettings = appSettings.Value;
        }

        public async Task<CommonUser> GetUser(string username, string password)
        {
            CommonUser user = null;

            user = await _commonCtx.Users.SingleOrDefaultAsync(m => m.Email.ToLower() == username.ToLower());
            if (user != null)
            {
                if (user.CheckPassword(password))
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }

            return user;
        }

        public async Task<CommonUser> GetUserProfile(Guid Id)
        {
            CommonUser user = null;

            user = await _commonCtx.Users.SingleOrDefaultAsync(m => m.Id == Id);
            if (user != null)
            {
                return user;
            }

            return user;
        }

        public async Task<CommonUser> CheckUser(string username)
        {
            CommonUser user = null;

            user = await _commonCtx.Users.SingleOrDefaultAsync(m => m.Email.ToLower() == username.ToLower());
            if (user != null)
            {

                return user;

            }

            return user;
        }

        public Boolean Mail(string EmailID, string callbackUrl)
        {
            try
            {
                //From Address  
                string FromAddress = _appSettings.FromAddress;
                string FromAdressTitle = "Reset Password";
                //To Address  
                string ToAddress = EmailID;
                string ToAdressTitle = "Reset Password.";
                string Subject = "CityApp: Reset Password.";
                string BodyContent = callbackUrl;
                var builder = new BodyBuilder();

                string physicalWebRootPath = _hostingEnvironment.ContentRootPath;

                using (StreamReader SourceReader = System.IO.File.OpenText(physicalWebRootPath + "/Templates/ResetPassword.html"))
                {
                    builder.HtmlBody = SourceReader.ReadToEnd();
                }

                // BodyContent = builder.HtmlBody.Replace("UserName", EmailID);
                BodyContent = builder.HtmlBody.Replace("LinkUrl", _appSettings.AbsolutePath + callbackUrl).Replace("UserName", EmailID);

                //Smtp Server  
                string SmtpServer = _appSettings.SmtpServer;
                //Smtp Port Number  
                int SmtpPortNumber = _appSettings.SmtpPortNumber;

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(FromAdressTitle, FromAddress));
                mimeMessage.To.Add(new MailboxAddress(ToAdressTitle, ToAddress));
                mimeMessage.Subject = Subject;
                mimeMessage.Body = new TextPart("plain")
                {
                    Text = BodyContent

                };

                using (var client = new SmtpClient())
                {

                    client.Connect(SmtpServer, SmtpPortNumber, false);
                    // Note: only needed if the SMTP server requires authentication  
                    // Error 5.5.1 Authentication   
                    // client.Authenticate("gsingh16@seasiainfotech.com","");
                    client.Send(mimeMessage);
                    //Console.WriteLine("The mail has been sent successfully !!");
                    //Console.ReadLine();
                    client.Disconnect(true);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

    }
}
