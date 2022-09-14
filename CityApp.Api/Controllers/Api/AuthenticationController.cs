using AutoMapper;
using CityApp.Api.Constants;
using CityApp.Api.Models;
using CityApp.Common.Caching;
using CityApp.Common.Extensions;
using CityApp.Common.Models;
using CityApp.Data;
using CityApp.Data.Enums;
using CityApp.Data.Extensions;
using CityApp.Data.Models;
using CityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using CityApp.Common.Extensions;

namespace CityApp.Api.Controllers.Api
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Authentication")]
    public class AuthenticationController : BaseApiController
    {
        private readonly CommonUserService _commonUserSvc;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly MailService _mailSvc;

        public AuthenticationController(IOptions<JwtIssuerOptions> jwtOptions, CommonUserService commonUserSvc, CommonContext commonContext, RedisCache cache, IOptions<AppSettings> appSettings, MailService mailSvc) :
            base(commonContext, cache, appSettings)
        {
            _commonUserSvc = commonUserSvc;
            _jwtOptions = jwtOptions.Value;
            _mailSvc = mailSvc;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody]UserRegisterModel model)
        {
            var response = new APIResponse<TokenResponseModel>();
            if (ModelState.IsValid)
            {

                var userAlreadyExists = await CommonContext.Users.AnyAsync(q => q.Email.ToLower() == model.Email.ToLower());

                if (userAlreadyExists)
                {
                    // This isn't a security risk because we've verified the email address already                    
                    response.Success = false;
                    response.Errors.Add(new Error { Code = Common.Enums.ErrorCode.None, Message = "A user has already verified that email address." });
                }
                else
                {

                    var user = new CommonUser();
                    user.Email = model.Email;
                    user.SetPassword(model.Password);
                    user.Permission = IsAdmin(model.Email);


                    //put dude in the database
                    using (var tx = CommonContext.Database.BeginTransaction())
                    {
                        CommonContext.Users.Add(user);
                        await CommonContext.SaveChangesAsync();
                        tx.Commit();

                    }

                    //Register user device
                    var device = await RegisterUserDevice(model, user);

                    //Return JWT Token
                    return Ok(await CreateEncodedJwtToken(user));

                }
            }
            else
            {
                response.Success = false;
                response.Errors.AddRange(ModelState.ToErrors());
            }
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            var response = new APIResponse<TokenResponseModel>();
            if (ModelState.IsValid)
            {

                var user = await _commonUserSvc.GetUser(model.Email, model.Password);
                if (user != null)
                {
                    //Register user device
                    var device = await RegisterUserDevice(model, user);


                    return Ok(await CreateEncodedJwtToken(user));
                }
                else
                {
                    response.Success = false;
                    response.Errors.Add(new Error { Code = Common.Enums.ErrorCode.None, Message = "Invalid login" });
                }
            }
            else
            {
                response.Success = false;
                response.Errors.AddRange(ModelState.ToErrors());
            }

            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordModel model)
        {
            var response = new APIResponse<ResetPasswordModel>() { Success = true };
            try
            {
                if (ModelState.IsValid)
                {
                    CommonUser user = await _commonUserSvc.CheckUser(model.Email);
                    if (user != null)
                    {
                        string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        var callbackUrl = Url.Action("ResetPassword", "User", new { code = token });

                        using (var tx = CommonContext.Database.BeginTransaction())
                        {
                            user.Token = token;
                            user.TokenUtc = DateTime.UtcNow;
                            CommonContext.Users.Update(user);

                            await CommonContext.SaveChangesAsync();
                            tx.Commit();

                        }

                        bool result = await _mailSvc.SendPasswordResetEmail(model.Email, callbackUrl);
                        response.Message = "Please check your email for a link to reset your password";

                    }
                    else
                    {
                        response.Message = "Please check your email for a link to reset your password";
                    }

                }
                else
                {
                    response.Success = false;
                    response.Errors.AddRange(ModelState.ToErrors());
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return Ok(response);
            }
        }



        private async Task<Models.APIResponse<TokenResponseModel>> CreateEncodedJwtToken(CommonUser user)
        {
            var claims = new[]
             {
                new Claim(JWTClaim.Sid,user.Id.ToString() ),
                new Claim(JWTClaim.Sub, user.Email),
                new Claim(JWTClaim.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JWTClaim.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                new Claim(JWTClaim.SystemPermission, ((int)user.Permission).ToString())
              };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var now = DateTime.UtcNow;
            var expires = now.AddMonths(6);

            var tokenReponse = new TokenResponseModel { Token = encodedJwt,
                Expires = expires.ToUnixTime(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                Email = user.Email,
                Permission = user.Permission
            };

            if(!string.IsNullOrWhiteSpace(user.ProfileImageKey))
            {
                tokenReponse.ProfileImageUrl = AppSettings.AmazonS3Url + user.ProfileImageKey;
            }

            return new APIResponse<TokenResponseModel> { Data = tokenReponse };
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        /// <summary>
        /// </summary>
        private static Task<ClaimsIdentity> GetClaimsIdentity()
        {

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }


        private SystemPermissions IsAdmin(string email)
        {
            var whiteListEmailDomains = AppSettings.WhiteListAdmin.Split(',').Select(m => m.ToLower()).ToList();

            MailAddress address = new MailAddress(email);
            string host = address.Host; // host contains yahoo.com

            var isInWhiteList = whiteListEmailDomains.Contains(host);

            if (isInWhiteList)
            {
                return SystemPermissions.Administrator;
            }
            else
            {
                return SystemPermissions.None;
            }
        }


        public async Task<bool> RegisterUserDevice(UserDeviceModel model, CommonUser user)
        {
            var deviceAlreadyExists = await CommonContext.CommonUserDevices.SingleOrDefaultAsync(m => m.DeviceToken == model.DeviceToken && m.UserId == user.Id);
            if (deviceAlreadyExists != null)
            {

                deviceAlreadyExists.IsLogin = true;
                deviceAlreadyExists.UpdateUserId = user.Id;
                deviceAlreadyExists.DevicePublicKey = model.DevicePublicKey;

                using (var tx = CommonContext.Database.BeginTransaction())
                {
                    CommonContext.CommonUserDevices.Update(deviceAlreadyExists);
                    await CommonContext.SaveChangesAsync();
                    tx.Commit();

                }
            }
            else
            {
                CommonUserDevice userDevice = new CommonUserDevice()
                {
                    DeviceName = model.DeviceName,
                    DeviceToken = model.DeviceToken,
                    DeviceType = model.DeviceType,
                    DevicePublicKey = model.DevicePublicKey,
                    UserId = user.Id,
                    IsLogin = true,
                    CreateUserId = user.Id
                };

                using (var tx = CommonContext.Database.BeginTransaction())
                {
                    CommonContext.CommonUserDevices.Add(userDevice);
                    await CommonContext.SaveChangesAsync();
                    tx.Commit();

                }

            }

            return true;
        }


    }
}
