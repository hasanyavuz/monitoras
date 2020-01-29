using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Monitoras.Entity;

namespace Monitoras.Web {
    [Route ("api/[controller]")]
    public class ApiController : SecureDbController {
        private UserManager<MTDUser> _userManager;
        public UserManager<MTDUser> UserManager => _userManager ?? (UserManager<MTDUser>) HttpContext?.RequestServices.GetService (typeof (UserManager<MTDUser>));

        public Guid UserId {
            get {
                var userId = UserManager.GetUserId (User);
                return Guid.Parse (userId);
            }
        }

        [NonAction]
        public IActionResult Success (string message = default (string), object data = default (object), int code = 200) {
            return Ok (
                new MTReturn () {
                    Success = true,
                        Message = message,
                        Data = data,
                        Code = code
                }
            );
        }

        [NonAction]
        public IActionResult Error (string message = default (string), string internalMessage = default (string), object data = default (object), int code = 400, List<MTReturnError> errorMessages = null) {
            var rv = new MTReturn () {
            Success = false,
            Message = message,
            InternalMessage = internalMessage,
            Data = data,
            Code = code
            };

            if (rv.Code == 500)
                return StatusCode (500, rv);
            if (rv.Code == 401)
                return Unauthorized ();
            if (rv.Code == 403)
                return Forbid ();
            if (rv.Code == 404)
                return NotFound (rv);

            return BadRequest (rv);
        }
    }
}