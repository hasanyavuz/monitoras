using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Monitoras.Web {
    [Authorize]
    public class SecureController : Controller {

    }
}