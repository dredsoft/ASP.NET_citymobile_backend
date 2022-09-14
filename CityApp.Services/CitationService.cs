using CityApp.Common.Caching;
using CityApp.Data;
using System;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityApp.Data.Models;
using CityApp.Services.Models;
using Microsoft.EntityFrameworkCore;
using CityApp.Data.Enums;
using CityApp.Common.Utilities;

namespace CityApp.Services
{
    public class CitationService : ICustomService
    {
        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        private static readonly object _randomLock = new object();
        private readonly CommonContext _commonCtx;
        private readonly RedisCache _cache;
        private readonly GeoCodeService _geoCodeService;
        private readonly AccountContext _accountCtx;


        private static readonly ILogger _logger = Log.Logger.ForContext<CitationService>();

        public CitationService(CommonContext commonCtx, RedisCache cache, AccountContext accountContext, GeoCodeService geoCodeSvc)
        {
            _commonCtx = commonCtx;
            _cache = cache;
            _geoCodeService = geoCodeSvc;
            _accountCtx = accountContext;

        }

        public async Task<ServiceResponse<Citation>> SaveComment(Guid accountId, Guid citationId, Guid commentId, string comment, bool IsPublic, Guid createdById)
        {
            var result = new ServiceResponse<Citation>();

            try
            {
                using (var scope = _accountCtx.Database.BeginTransaction())
                {
                    //update CommonViolation table
                    if (commentId != Guid.Empty)
                    {

                        var CommentDetails = await _accountCtx.CitationComment.SingleOrDefaultAsync(a => a.Id == commentId);
                        if (CommentDetails != null)
                        {

                            //  CitationDetails.UpdateUserId = createdById;
                            CommentDetails.UpdateUtc = DateTime.Now;
                            CommentDetails.Comment = comment;
                            CommentDetails.IsPublic = IsPublic;

                            _accountCtx.CitationComment.Update(CommentDetails);
                        }
                    }
                    else
                    {
                        CitationComment CommentDetails = new CitationComment
                        {
                            CitationId = citationId,
                            AccountId = accountId,
                            Comment = comment,
                            CreateUserId = createdById,
                            UpdateUserId = createdById,
                            IsPublic = IsPublic
                        };
                        _accountCtx.CitationComment.Add(CommentDetails);

                    }

                    await _accountCtx.SaveChangesAsync();

                    scope.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating a citation");
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }

        public async Task<ServiceResponse<GeoLocation>> ReverseGeoCodeCitation(Guid accountId, Guid citationId)
        {
            ServiceResponse<GeoLocation> result = null;

            //_logger.Error("Get Account");
            var commonAccount = _commonCtx.CommonAccounts.Include(m => m.Partition).AsNoTracking().Where(m => m.Id == accountId).SingleOrDefault();

            if (commonAccount != null)
            {
                var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString));

                var citation = accountContext.Citations.Where(m => m.Id == citationId).SingleOrDefault();
                if (citation != null)
                {
                    // _logger.Error("Call Service");

                    var geoCodeResults = _geoCodeService.ReverseCode(citation.Latitude, citation.Longitude).Result;

                    if (geoCodeResults != null && geoCodeResults.address != null)
                    {
                        citation.State = geoCodeResults.address.state;
                        citation.Street = geoCodeResults.address.road;
                        citation.City = geoCodeResults.address.city;
                        citation.PostalCode = geoCodeResults.address.postcode;
                    }
                }

                accountContext.SaveChanges();
            }

            return result;
        }

        /// <summary>
        /// A violation can have a reminder attached to it.  That can be used when a violation requires a following piece of evidence.
        /// For example.  If you're reporting an abnandoned vehicle, you may be reminded after 24 hours to submit another video. 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="citationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> CreateCitationReminder(Guid accountId, Guid citationId, Guid? userId)
        {
            ServiceResponse<bool> result = null;

            //_logger.Error("Get Account");
            var commonAccount = _commonCtx.CommonAccounts.Include(m => m.Partition).AsNoTracking().Where(m => m.Id == accountId).SingleOrDefault();

            if (commonAccount != null)
            {
                var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString));

                var citation = accountContext.Citations.Include(m => m.Violation).Where(m => m.Id == citationId).SingleOrDefault();
                if (citation != null && citation.Violation.ReminderMinutes.HasValue && citation.Violation.ReminderMinutes.Value > 0)
                {
                    var reminderDelivery = DateTime.UtcNow.AddMinutes(citation.Violation.ReminderMinutes.Value);


                    var newCitationReminder = new CitationReminder { AccountId = accountId, CitationId = citationId, DeliveryDateUTC = reminderDelivery, Message = citation.Violation.ReminderMessage };

                    if (userId.HasValue)
                    {
                        newCitationReminder.CreateUserId = userId.Value;
                        newCitationReminder.UpdateUserId = userId.Value;
                    }

                    accountContext.CitationReminders.Add(newCitationReminder);
                }

                try
                {
                    accountContext.SaveChanges();
                }
                catch (Exception ex)

                {
                    _logger.Error(ex, $"Error trying to create CitationReminder for Account:{accountId} Citation:{citation} for user:{userId}");
                }
            }

            return result;
        }

        /// <summary>
        /// If an event is created during the time of this citation
        /// AND this citation falls within the boundary of this event
        /// AND there is an ovveride price for this particluar violation.
        /// we ovveride the citation fee.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="citationId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> OverrideCitationFee(Guid accountId, Guid citationId)
        {
            ServiceResponse<bool> result = null;

            //_logger.Error("Get Account");
            var commonAccount = _commonCtx.CommonAccounts.Include(m => m.Partition).AsNoTracking().Where(m => m.Id == accountId).SingleOrDefault();

            if (commonAccount != null)
            {
                var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString));

                var citation = accountContext.Citations.Include(m => m.Violation).Where(m => m.Id == citationId).SingleOrDefault();

                //Step 1) Check if there is an eventPrice for this violation
                var eventPrice = accountContext.EventViolationPrices.ForAccount(commonAccount.Id).Include(m => m.Event)
                    .Where(m => m.ViolationId == citation.ViolationId).SingleOrDefault();


                if(eventPrice != null)
                {
                    //Step 2. Check if this citation falls within the dates of this citation
                    var specialEvents = accountContext.Events.Include(m => m.EventBoundaryCoordinates).ForAccount(commonAccount.Id)
                        .Where(m => m.Start.HasValue && citation.CreateUtc >= m.Start)
                        .Where(m => m.End.HasValue && citation.CreateUtc <= m.End).ToList();

                    foreach (var specialEvent in specialEvents)
                    {
                        var eventCoords = accountContext.EventBoundaryCoordinates.ForAccount(commonAccount.Id).Where(m => m.EventId == specialEvent.Id).OrderBy(m => m.Order).ToList();
                        //Step 2) See if this citation falls inside the coordinates of the event
                        Dictionary<decimal, decimal> coords = eventCoords.ToDictionary(m => m.Latitude, m => m.Longitude);
                        if (IsLatLongIsnPolygon(citation.Latitude, citation.Longitude, coords))
                        {

                            citation.FineAmount = eventPrice.Fee;
                            try
                            {
                                accountContext.SaveChanges();
                            }
                            catch (Exception ex)

                            {
                                _logger.Error(ex, $"Error trying to create Override Citation Fee for Account:{accountId} Citation:{citation}");
                            }

                        }
                    }
                    
                }
            }

            return result;
        }


        public async Task<ServiceResponse<CitationAuditLog>> CreateAuditEvent(Guid accountId, Guid citationId, string comment, Guid createdById, CitationAuditEvent auditEvent = CitationAuditEvent.Other)
        {
            ServiceResponse<CitationAuditLog> result = null;

            var auditLog = new CitationAuditLog
            {
                AccountId = accountId,
                CitationId = citationId,
                Comment = comment,
                Event = auditEvent,
                CreateUserId = createdById,
                UpdateUserId = createdById
            };

            _accountCtx.CitationAuditLogs.Add(auditLog);

            try
            {
                await _accountCtx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error creating audit. AccountId:{accountId}, CitationId:{citationId}, event:{auditEvent.ToString()}, comment: {comment}");
            }
            return result;
        }

        public async Task<ServiceResponse<CitationReceipt>> CreateCitationReceipt(Guid accountId, Guid citationId, string DeviceReceipt, string DevicePublicKey, Guid createdById)
        {
            ServiceResponse<CitationReceipt> result = null;

            var receipt = new CitationReceipt
            {
                AccountId = accountId,
                CitationId = citationId,
                ReceiptPayload = DeviceReceipt,
                DevicePublicKey = DevicePublicKey,
                CreateUserId = createdById,
                UpdateUserId = createdById
            };

            _accountCtx.CitationReceipts.Add(receipt);

            try
            {
                await _accountCtx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error creating receipt. AccountId:{accountId}, CitationId:{citationId}");
            }
            return result;
        }

        public bool IsLatLongIsnPolygon(decimal latitude, decimal longitude, Dictionary<decimal, decimal> polygon )
        {
            var point = new Loc(Convert.ToDouble(latitude), Convert.ToDouble(longitude));

            var locCoords = new List<Loc>();
            foreach (var loc in polygon)
            {
                locCoords.Add(new Loc(Convert.ToDouble(loc.Key), Convert.ToDouble(loc.Value)));
            }

            return IsPointInPolygon(locCoords, point);
        }

        public async Task<ServiceResponse<Citation>> DeleteCitation(Citation citation, Guid citationId)
        {
            var result = new ServiceResponse<Citation>() { Success = true };
            try
            {

                if (citation != null)
                {

                    if (citation.Comments.Count > 0)
                    {
                        _accountCtx.CitationComment.RemoveRange(citation.Comments);
                        await _accountCtx.SaveChangesAsync();
                    }

                    if (citation.AuditLogs.Count > 0)
                    {
                        _accountCtx.CitationAuditLogs.RemoveRange(citation.AuditLogs);
                        await _accountCtx.SaveChangesAsync();
                    }

                    if (citation.Attachments.Count > 0)
                    {
                        foreach (var attach in citation.Attachments)
                        {
                            _accountCtx.Attachments.Remove(attach.Attachment);
                        }
                        _accountCtx.CitationAttachments.RemoveRange(citation.Attachments);
                    }


                    var citationReceipt = await _accountCtx.CitationReceipts.Where(x => x.CitationId == citationId).ToListAsync();
                    if (citationReceipt.Count > 0)
                    {
                        _accountCtx.CitationReceipts.RemoveRange(citationReceipt);
                        await _accountCtx.SaveChangesAsync();
                    }

                    _accountCtx.Citations.Remove(citation);
                    await _accountCtx.SaveChangesAsync();

                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;

        }

        public bool IsPointInPolygon(List<Loc> poly, Loc point)
        {

            int i, j;
            bool c = false;
            if (poly.Any())
            {
                for (i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
                {
                    if ((((poly[i].Lt <= point.Lt) && (point.Lt < poly[j].Lt))
                            || ((poly[j].Lt <= point.Lt) && (point.Lt < poly[i].Lt)))
                            && (point.Lg < (poly[j].Lg - poly[i].Lg) * (point.Lt - poly[i].Lt)
                                / (poly[j].Lt - poly[i].Lt) + poly[i].Lg))

                        c = !c;
                }
            }

            return c;
        }


        public async Task<Citation> GetCitation(long accountNumber, int citationNumber)
        {
            var accountDetail = await _commonCtx.CommonAccounts
                .Include(m => m.Partition)
                .Where(account => account.Number == accountNumber)
                .SingleOrDefaultAsync();

            var _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));


            var citation = await _accountCtx.Citations.ForAccount(accountDetail.Id)
                .Where(m => m.CitationNumber == citationNumber)
                .OrderByDescending(m => m.CreateUtc)
                .FirstOrDefaultAsync();

            return citation;

        }
    }
}