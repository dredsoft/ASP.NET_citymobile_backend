using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PushSharp.Core;
using System.Threading.Tasks;
using System.IO;
using PushSharp.Apple;
using Newtonsoft.Json.Linq;
using CityApp.Data.Models;
using Serilog;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using CityApp.Common.Models;
using Microsoft.Extensions.Options;
using CityApp.Services.Models;

namespace CityApp.Services
{

    public class PushNotification : ICustomService
    {
        private IHostingEnvironment _hostingEnvironment;
        private readonly AppSettings _appSettings;
        private static readonly Serilog.ILogger _logger = Serilog.Log.Logger.ForContext<PushNotification>();

        public PushNotification(IHostingEnvironment environment, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _hostingEnvironment = environment;
        }

        public async Task<bool> APNS_Send(List<UserDeviceInfo> splitList, string Message)
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                Message = "Hello this is broadcast";
            }

            //Find  Certificate Path
            var certificatePath = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.CertificateName);

            //configure certificate with Password
            var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production, certificatePath, _appSettings.CertificatePwd);

            // Create a new broker
            var apnsBroker = new ApnsServiceBroker(config);

            apnsBroker.OnNotificationSucceeded += (notification) =>
            {
                _logger.Information("***Info***: notification success " + notification.DeviceToken);
            };


            apnsBroker.OnNotificationFailed += (notification, aggregateEx) =>
            {

                aggregateEx.Handle(ex =>
                {

                    // See what kind of exception it was to further diagnose
                    if (ex is ApnsNotificationException)
                    {
                        var notificationException = ex as ApnsNotificationException;

                        // Deal with the failed notification
                        var apnsNotification = notificationException.Notification;
                        var statusCode = notificationException.ErrorStatusCode;

                        _logger.Error($"Notification Failed: ID={apnsNotification.DeviceToken}, Code={statusCode}");

                    }
                    else
                    {
                        // Inner exception might hold more useful information like an ApnsConnectionException           
                        _logger.Error($"Notification Failed for some (Unknown Reason) : {ex.Message},token= {notification.DeviceToken}");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            // Start the broker
            apnsBroker.Start();

            var tasks = new Task[splitList.Count];
            //splitList contains multiple list of tokens waiting to queue, a list that holds list of token

            if (splitList.Count > 0)
            {
                int count = 0;
                //start a new task for each list in splitList
                foreach (var lst in splitList)
                {
                    tasks[count] = Task.Factory.StartNew(() =>
                    {
                        _logger.Information("***INFO*** calling task :" + count);
                        QueueAppleNotifications(lst, Message, apnsBroker);
                    },
                        TaskCreationOptions.None);

                    count++;
                }

            }
            return WaitForTasksToFinish(tasks, apnsBroker);
        }
        private bool WaitForTasksToFinish(Task[] tasks, ApnsServiceBroker push)
        {
            try
            {

                bool taskStatus = true;
                Task.WaitAll(tasks);
                foreach (Task t in tasks)
                {
                    _logger.Information("***INFO*** Inside WaitForTasksToFinish :" + t.Status);
                    if (t.Status != TaskStatus.RanToCompletion)
                    {

                        taskStatus = false;
                        break;
                    }
                }


                if (taskStatus)
                    push.Stop(true);

                return taskStatus;
            }
            catch (Exception ex)
            {
                _logger.Error("***Error***: " + this.GetType().Name + ", Method " + MethodBase.GetCurrentMethod().Name + "****** Error details :" + ex.Message);
                return true;
            }
        }

        private void QueueAppleNotifications(UserDeviceInfo lstSplit, string pMessage, ApnsServiceBroker push)
        {
            try
            {
                String jsonPayload = "{ \"aps\" : { \"alert\" : \"" + pMessage + "\",\"badge\" : 0 }}";

                _logger.Information("Inside QueueNotifications with message: " + pMessage);

                if (lstSplit.DeviceToken != null)
                {
                    if (lstSplit.DeviceToken.Length == 64)
                    {
                        _logger.Information("***INFO*** Queing notification for token :" + lstSplit.DeviceToken.ToString());
                        push.QueueNotification(new ApnsNotification()
                        {
                            DeviceToken = (lstSplit.DeviceToken),
                            Payload = JObject.Parse(jsonPayload)
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("***Error***: " + this.GetType().Name + ", Method " + MethodBase.GetCurrentMethod().Name + "****** Error details :" + ex.Message);
                throw ex;

            }

        }
    }
}
