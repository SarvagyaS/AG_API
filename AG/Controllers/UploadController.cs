using AG.Helpers;
using AG.Models;
using Dashboard.Entity.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AG.Controllers
{
    [Route("api/upload/[action]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly AGContext _AGContext;
        private readonly AppSettings _appSettings;
        private readonly JWTHelpers _jwtHelpers;

        public UploadController(IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor, AGContext aGContext)
        {
            _AGContext = aGContext;
            _appSettings = appSettings.Value;
            _jwtHelpers = new JWTHelpers(httpContextAccessor, appSettings);
        }

        [HttpPost]
        public ActionResult<ApiResponse<string>> UploadImage()
        {
            try
            {

                IFormFile file = Request.Form.Files[0];

                string[] FileType = { "image/jpeg", "image/png", "image/jpg" };
                var folderName = Path.Combine("wwwroot", "Upload", "ProfilePhoto");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                var id = _jwtHelpers.GetUserSessionByToken();
                var u = _AGContext.UserDetails.FirstOrDefault(a => a.Id == id);

                String fileName = id + "_" + u.first_name + "." + file.FileName.Split('.')[1];

                //#region Directory Creation
                //if (!Directory.Exists(TempPATH + FolderName))
                //{
                //    DirectoryInfo dInfo = Directory.CreateDirectory(TempPATH + FolderName);
                //    DirectorySecurity dSecurity = dInfo.GetAccessControl();
                //    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                //    dInfo.SetAccessControl(dSecurity);
                //}
                //#endregion

                u.profilePicUrl = fileName;
                _AGContext.SaveChanges();
                #region Temp File Creation
                using (var stream = new FileStream(pathToSave + "\\" +fileName, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                #endregion


                return StatusCode(200, new ApiResponse<string>
                {
                    IsSuccess = true,
                    Data = fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    IsSuccess = true,
                    Data = "",
                });
            }

        }
    }
}
