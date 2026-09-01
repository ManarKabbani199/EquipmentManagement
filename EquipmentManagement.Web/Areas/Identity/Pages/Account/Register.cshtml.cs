using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EquipmentManagement.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        public void OnGet()
        {
            // عرض صفحة توضح أن التسجيل العام متوقف.
        }

        public IActionResult OnPost()
        {
            // منع أي محاولة تسجيل عبر طلب POST.
            return NotFound();
        }
    }
}