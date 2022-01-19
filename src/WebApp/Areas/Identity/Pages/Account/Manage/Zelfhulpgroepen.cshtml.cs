using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Areas.Identity.Data;

namespace WebApp.Areas.Identity.Pages.Account.Manage
{
    public class ZelfhulpgroepenDataModel : PageModel
    {
        private readonly MyContext _context;
        
        public ZelfhulpgroepenDataModel(MyContext context)
        {
            _context = context;
        }
    }


}