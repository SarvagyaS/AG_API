using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AG.Helpers;
using AG.Models;
using Dashboard.Entity.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AG.Controllers
{
    [Route("api/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AGContext _AGContext;
        private readonly AppSettings _appSettings;
        private readonly AvigmaAGContext avigmaAGContext;
        private readonly AvigmaBaseRepo avigmaBaseRepo;
        private readonly IHostingEnvironment _hostingEnvironment;

        public UserController(AGContext aGContext, IOptions<AppSettings> appSettings, AvigmaAGContext _avigmaAGContext, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _AGContext = aGContext;
            _appSettings = appSettings.Value;
            avigmaAGContext = _avigmaAGContext;
            avigmaBaseRepo = new AvigmaBaseRepo(_avigmaAGContext);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDetails>>> Register([FromBody] UserDetails ud)
        {
            try
            {
                ud.registration_date = DateTime.Now;
                ud.user_ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                _AGContext.UserDetails.Add(ud);
                await _AGContext.SaveChangesAsync();

                var postalAdd = ud.UserAddressDetails[0];

                IList<object> lstSqlParam = new List<object>
            {
                new SqlParameter("@UserID", null),
                new SqlParameter("@ClientEmailID", ud.email),
                new SqlParameter("@MaritalStatus", null),
                new SqlParameter("@PanNo", ud.pan_card),
                new SqlParameter("@DOB", ud.dob),
                new SqlParameter("@AnniversaryDate", null),
                new SqlParameter("@ID", ud.Id),
                new SqlParameter("@ClientName", ud.first_name),
                new SqlParameter("@MiddleName", null),
                new SqlParameter("@LastName", ud.last_name),
                new SqlParameter("@CompanyName", null),
                new SqlParameter("@ProfileImage", null),
                new SqlParameter("@TelephoneNumber", null),
                new SqlParameter("@MobileNumber", ud.mobile),
                new SqlParameter("@AddressLine1", postalAdd.address_line_1),
                new SqlParameter("@AddressLine2", postalAdd.address_line_2),
                new SqlParameter("@Landmark", null),
                new SqlParameter("@Pincode", postalAdd.postal_code),
                new SqlParameter("@City", postalAdd.city),
                new SqlParameter("@State", postalAdd.state),
                new SqlParameter("@Country", postalAdd.country),
                new SqlParameter("@ClientPriority", null),
                new SqlParameter("@CurrencyID", Guid.NewGuid()),
                new SqlParameter("@Gender", ud.gender),
                new SqlParameter("@ApproveRejectStatus", null),
                new SqlParameter("@ContactPersonName", null),
                new SqlParameter("@ContactPersonNumber", null),
                new SqlParameter("@ContactPersonEmailID", null),
                new SqlParameter("@Designation", null),
                new SqlParameter("@Total_Spend", Convert.ToDecimal(0)),
                new SqlParameter("@Bid_Limit", Convert.ToDecimal(0)),
                new SqlParameter("@Customer_profession", null),
                new SqlParameter("@Company_profile", null),
                new SqlParameter("@Client_background", false),
                new SqlParameter("@Client_category_master", null),
                new SqlParameter("@WebSiteId", 420),
                new SqlParameter("@Type", 1),
                new SqlParameter("@Id_Out", 0){ Direction = ParameterDirection.Output},
                new SqlParameter("@ReturnValue", 0){ Direction = ParameterDirection.Output},
                new SqlParameter("@ExecutionResult", 0){ Direction = ParameterDirection.Output},
            };
                //TEMP 
                var a = await avigmaBaseRepo.StoreProcedureAsync("InsertUpdateClientManagementData", lstSqlParam.ToArray());
                ud.UserAddressDetails = new List<UserAddressDetails>();
                if (a > 0)
                {
                    return StatusCode(200, new ApiResponse<UserDetails>
                    {
                        IsSuccess = true,
                        Data = ud
                    });
                }
                else
                {
                    return StatusCode(200, new ApiResponse<UserDetails>
                    {
                        IsSuccess = false,
                        Errors = new List<Errors> { new Errors { Key = "", Value = "Not pushed to CRM" } },
                        Data = ud
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserDetails>
                {
                    IsSuccess = false,
                    Data = new UserDetails(),
                });
            }
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDetails>>> SavePersonalDetails([FromBody] UserDetails ud)
        {
            try
            {
                var u = await _AGContext.UserDetails.FirstOrDefaultAsync(a => a.Id == ud.Id);
                if (u != null)
                {
                    u.gender = ud.gender;
                    u.dob = ud.dob;
                    u.nick_name = ud.nick_name;
                    u.country_code = ud.country_code;
                    u.interested_in_bidding = ud.interested_in_bidding;
                    u.hear_aboutus = ud.hear_aboutus;
                    u.interested_in = ud.interested_in;
                    u.birthDay = ud.birthDay;
                    u.birthMonth = ud.birthMonth;
                    u.birthYear = ud.birthYear;
                    await _AGContext.SaveChangesAsync();
                    return StatusCode(200, new ApiResponse<UserDetails>
                    {
                        IsSuccess = true,
                        Data = ud
                    });
                }
                else
                {
                    return StatusCode(200, new ApiResponse<UserDetails>
                    {
                        IsSuccess = false,
                        Data = new UserDetails()
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserDetails>
                {
                    IsSuccess = false,
                    Data = new UserDetails(),
                });
            }
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDetails>>> SaveBankDetails([FromBody] UserDetails ud)
        {
            try
            {
                var u = await _AGContext.UserDetails.FirstOrDefaultAsync(a => a.Id == ud.Id);
                if (u != null)
                {
                    u.bank_name = ud.bank_name;
                    u.account_num = ud.account_num;
                    u.ifsc_code = ud.ifsc_code;
                    u.pan_card = ud.pan_card;
                    u.aadhar_card = ud.aadhar_card;
                    await _AGContext.SaveChangesAsync();
                    return StatusCode(200, new ApiResponse<UserDetails>
                    {
                        IsSuccess = true,
                        Data = ud
                    });
                }
                else
                {
                    return StatusCode(200, new ApiResponse<UserDetails>
                    {
                        IsSuccess = false,
                        Data = new UserDetails()
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserDetails>
                {
                    IsSuccess = false,
                    Data = new UserDetails(),
                });
            }
        }


        [HttpPost]
        public async Task<UserDetails> Authenticate([FromBody] Login u)
        {
            try
            {
                var user = new UserDetails();
                if (u.IsUserNameSelected)
                {
                    user = await _AGContext.UserDetails.Include(a => a.UserAddressDetails).Where(a => a.email == u.Username && a.password == u.Password).FirstOrDefaultAsync();
                }
                else
                {
                    user = await _AGContext.UserDetails.Include(a => a.UserAddressDetails).Where(a => a.mobile == u.PhoneNo).FirstOrDefaultAsync();
                }
                if (user != null)
                {
                    //the following lines of code is written for updating last login
                    //user.last_login_date = DateTime.Now;
                    //var loggedInUser = await _AGContext.UserDetails.AddAsync(user);
                    //await _AGContext.SaveChangesAsync();

                    user.token = GenerateToken(user);
                    var folderName = Path.Combine("wwwroot", "Upload", "ProfilePhoto");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var a = user.profilePicUrl;
                    user.profilePicUrl = pathToSave + "\\" + a;
                    if (user.UserAddressDetails != null)
                    {
                        foreach (var item in user.UserAddressDetails)
                        {
                            item.UserDetails = null;
                        }
                    }
                    return user;
                }
                return new UserDetails();
            }
            catch (Exception ex)
            {
                return new UserDetails { first_name = ex.Message.ToString(), nick_name = ex.StackTrace.ToString() };
            }
        }


        private string GenerateToken(UserDetails userDetails, DateTime? dtExpiry = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                       new Claim(ClaimTypes.Name,userDetails.Id.ToString()),
                }),
                Expires = dtExpiry == null ? DateTime.UtcNow.AddDays(7) : dtExpiry,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var Token = tokenHandler.CreateToken(tokenDescriptor);
            string strToKen = tokenHandler.WriteToken(Token);
            return strToKen;
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserAddressDetails>>>> GetUserAddress(int Id)
        {
            try
            {
                if (Id > 0)
                {
                    var a = await _AGContext.UserAddressDetails.Where(a => a.UserDetailsId == Id).ToListAsync();
                    if (a != null)
                    {
                        return StatusCode(200, new ApiResponse<List<UserAddressDetails>>
                        {
                            IsSuccess = true,
                            Data = a
                        });
                    }
                }
                return StatusCode(200, new ApiResponse<List<UserAddressDetails>>
                {
                    IsSuccess = true,
                    Data = new List<UserAddressDetails>(),
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<UserAddressDetails>>
                {
                    IsSuccess = true,
                    Data = new List<UserAddressDetails>(),
                });
            }
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse<UserDetails>>> GetUserDetails(int Id)
        {
            try
            {
                if (Id > 0)
                {
                    var a = await _AGContext.UserDetails.Where(a => a.Id == Id).FirstOrDefaultAsync();
                    if (a != null)
                    {
                        var folderName = Path.Combine("Upload", "ProfilePhoto");
                        var d = a.profilePicUrl;
                        a.profilePicUrl = folderName + "/" + d;
                        return StatusCode(200, new ApiResponse<UserDetails>
                        {
                            IsSuccess = true,
                            Data = a
                        });
                    }
                }
                return StatusCode(200, new ApiResponse<UserDetails>
                {
                    IsSuccess = true,
                    Data = new UserDetails()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserDetails>
                {
                    IsSuccess = true,
                    Data = new UserDetails(),
                });
            }
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserAddressDetails>>> SaveAddressDetails([FromBody] UserAddressDetails u)
        {
            try
            {
                if (u.Id > 0)
                {
                    var a = await _AGContext.UserAddressDetails.Where(a => a.Id == u.Id).FirstOrDefaultAsync();
                    if (a != null)
                    {
                        a.postal_code = u.postal_code;
                        a.state = u.state;
                        a.address_line_1 = u.address_line_1;
                        a.address_line_2 = u.address_line_2;
                        a.city = u.city;
                        a.country = u.country;
                        a.name = u.name;
                        await _AGContext.SaveChangesAsync();
                        return StatusCode(200, new ApiResponse<UserAddressDetails>
                        {
                            IsSuccess = true,
                            Data = u
                        });
                    }
                }
                else
                {
                    u.created_date = DateTime.Now;
                    _AGContext.Add(u);
                    await _AGContext.SaveChangesAsync();
                    return StatusCode(200, new ApiResponse<UserAddressDetails>
                    {
                        IsSuccess = true,
                        Data = u
                    });
                }
                return StatusCode(200, new ApiResponse<UserAddressDetails>
                {
                    IsSuccess = true,
                    Data = new UserAddressDetails()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserAddressDetails>
                {
                    IsSuccess = true,
                    Data = new UserAddressDetails()
                });
            }
        }

        //added by PK
        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDetails>>> SaveEditUserProfile([FromBody] UserDetails userDetails)
        {
            try
            {
                int Type = 0;
                if (userDetails.Id == 0)
                {
                    //create new user profile
                    Type = 1;
                    if (await _AGContext.UserDetails.FirstOrDefaultAsync(c=>c.email == userDetails.email) != null)
                    {
                        return StatusCode(200, new ApiResponse<UserDetails>
                        {
                            IsSuccess = false,
                            Data = new UserDetails(),
                            Errors = new List<Errors> { new Errors { Key="Email",Value="EMail Already exists." } }
                        }) ;
                    }
                    if (await _AGContext.UserDetails.FirstOrDefaultAsync(c => c.mobile == userDetails.mobile) != null)
                    {
                        return StatusCode(200, new ApiResponse<UserDetails>
                        {
                            IsSuccess = false,
                            Data = new UserDetails(),
                            Errors = new List<Errors> { new Errors { Key = "Mobile", Value = "Mobile Already exists." } }
                        });
                    }
                    userDetails.registration_date = DateTime.Now;
                    userDetails.user_ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    var newUserProfile = await _AGContext.UserDetails.AddAsync(userDetails);
                    await _AGContext.SaveChangesAsync();
                    userDetails.UserAddressDetails = new List<UserAddressDetails>();
                    return StatusCode(200, new ApiResponse<UserDetails>
                    {
                        IsSuccess = true,
                        Data = userDetails
                    });
                }
                else
                {
                    Type = 2;
                    //edit existing user profile
                    var existingUserProfile = _AGContext.UserDetails.Include(a => a.UserAddressDetails).Where(x => x.Id == userDetails.Id).SingleOrDefault();
                    if (existingUserProfile != null)
                    {
                        existingUserProfile.first_name = userDetails.first_name;
                        existingUserProfile.last_name = userDetails.last_name;
                        existingUserProfile.email = userDetails.email;
                        existingUserProfile.password = userDetails.password;
                        existingUserProfile.mobile = userDetails.mobile;
                        existingUserProfile.dob = userDetails.dob;
                        existingUserProfile.gender = userDetails.gender;
                        existingUserProfile.country_code = userDetails.country_code;
                        existingUserProfile.interested_in = userDetails.interested_in;
                        existingUserProfile.interested_in_bidding = userDetails.interested_in_bidding;
                        existingUserProfile.hear_aboutus = userDetails.hear_aboutus;
                        existingUserProfile.pan_card = userDetails.pan_card;
                        existingUserProfile.aadhar_card = userDetails.aadhar_card;
                        existingUserProfile.account_num = userDetails.account_num;
                        existingUserProfile.bank_name = userDetails.bank_name;
                        existingUserProfile.ifsc_code = userDetails.ifsc_code;
                        existingUserProfile.profilePicUrl = userDetails.profilePicUrl;
                        existingUserProfile.profile_update_date = userDetails.profile_update_date;
                        existingUserProfile.birthDay = userDetails.birthDay;
                        existingUserProfile.birthMonth = userDetails.birthMonth;
                        existingUserProfile.birthYear = userDetails.birthYear;
                        existingUserProfile.profile_update_date = DateTime.Now;
                        existingUserProfile.user_ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                        await _AGContext.SaveChangesAsync();

                        //if (userDetails.UserAddressDetails != null)
                        //{
                        //    existingUserProfile.UserAddressDetails = userDetails.UserAddressDetails;
                        //}

                        if (userDetails.UserAddressDetails != null)
                        {
                            var newAdd = userDetails.UserAddressDetails.Where(a => a.Id == 0).ToList();
                            foreach (var item in newAdd)
                            {
                                item.UserDetailsId = userDetails.Id;
                            }
                            if (newAdd != null && newAdd.Any())
                            {
                                await _AGContext.UserAddressDetails.AddRangeAsync(newAdd);
                                await _AGContext.SaveChangesAsync();
                            }

                            var editAdd = userDetails.UserAddressDetails.Where(a => a.Id != 0).ToList();
                            foreach (var item in editAdd)
                            {
                                var obj = await _AGContext.UserAddressDetails.FirstOrDefaultAsync(a => a.Id == item.Id);
                                _AGContext.Entry(obj).CurrentValues.SetValues(item);
                                await _AGContext.SaveChangesAsync();
                            }

                        }
                        //_AGContext.Entry(existingUserProfile).CurrentValues.SetValues(userDetails);
                        //_AGContext.Entry(existingUserProfile).State = EntityState.Modified;
                    }
                    return StatusCode(200, new ApiResponse<UserDetails>
                    {
                        IsSuccess = true,
                        Data = userDetails
                    });
                }
                UserAddressDetails postalAdd = userDetails.UserAddressDetails[0];
                IList<object> lstSqlParam = new List<object>
            {
                new SqlParameter("@UserID", null),
                new SqlParameter("@ClientEmailID", userDetails.email),
                new SqlParameter("@MaritalStatus", null),
                new SqlParameter("@PanNo", userDetails.pan_card),
                new SqlParameter("@DOB", userDetails.dob),
                new SqlParameter("@AnniversaryDate", null),
                new SqlParameter("@ID", userDetails.Id),
                new SqlParameter("@ClientName", userDetails.first_name),
                new SqlParameter("@MiddleName", null),
                new SqlParameter("@LastName", userDetails.last_name),
                new SqlParameter("@CompanyName", null),
                new SqlParameter("@ProfileImage", null),
                new SqlParameter("@TelephoneNumber", null),
                new SqlParameter("@MobileNumber", userDetails.mobile),
                new SqlParameter("@AddressLine1", postalAdd.address_line_1),
                new SqlParameter("@AddressLine2", postalAdd.address_line_2),
                new SqlParameter("@Landmark", null),
                new SqlParameter("@Pincode", postalAdd.postal_code),
                new SqlParameter("@City", postalAdd.city),
                new SqlParameter("@State", postalAdd.state),
                new SqlParameter("@Country", postalAdd.country),
                new SqlParameter("@ClientPriority", null),
                new SqlParameter("@CurrencyID", Guid.NewGuid()),
                new SqlParameter("@Gender", userDetails.gender),
                new SqlParameter("@ApproveRejectStatus", null),
                new SqlParameter("@ContactPersonName", null),
                new SqlParameter("@ContactPersonNumber", null),
                new SqlParameter("@ContactPersonEmailID", null),
                new SqlParameter("@Designation", null),
                new SqlParameter("@Total_Spend", Convert.ToDecimal(0)),
                new SqlParameter("@Bid_Limit", Convert.ToDecimal(0)),
                new SqlParameter("@Customer_profession", null),
                new SqlParameter("@Company_profile", null),
                new SqlParameter("@Client_background", false),
                new SqlParameter("@Client_category_master", null),
                new SqlParameter("@WebSiteId", 420),
                new SqlParameter("@Type", Type),
                new SqlParameter("@Id_Out", 0){ Direction = ParameterDirection.Output},
                new SqlParameter("@ReturnValue", 0){ Direction = ParameterDirection.Output},
                new SqlParameter("@ExecutionResult", 0){ Direction = ParameterDirection.Output},
            };
                //TEMP 
                var a = await avigmaBaseRepo.StoreProcedureAsync("InsertUpdateClientManagementData", lstSqlParam.ToArray());
                userDetails.UserAddressDetails = new List<UserAddressDetails>();
                if (a > 0)
                {
                    return StatusCode(200, new ApiResponse<UserDetails>
                    {
                        IsSuccess = true,
                        Data = userDetails
                    });
                }
                else
                {
                    return StatusCode(200, new ApiResponse<UserDetails>
                    {
                        IsSuccess = false,
                        Errors = new List<Errors> { new Errors { Key = "", Value = "Not pushed to CRM" } },
                        Data = userDetails
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserDetails>
                {
                    IsSuccess = false,
                    Data = new UserDetails()
                });
            }
        }
    }
    }
