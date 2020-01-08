using Microsoft.AspNetCore.Mvc;
using Monitoras.Entity;

namespace Monitoras.Web {
    public class DbController : Controller {
        private MTDContext _db;
        public MTDContext Db => _db ?? (MTDContext) HttpContext?.RequestServices.GetService (typeof (MTDContext));
    }
}