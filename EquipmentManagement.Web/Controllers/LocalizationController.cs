using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManagement.Web.Controllers;

public class LocalizationController : Controller
{
    [HttpGet]
    public IActionResult SetLanguage(
        string culture,
        string returnUrl)
    {
        if (culture != "ar" && culture != "en")
        {
            culture = "ar";
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider
                .DefaultCookieName,
            CookieRequestCultureProvider
                .MakeCookieValue(
                    new RequestCulture(culture)
                ),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow
                    .AddYears(1),
                IsEssential = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            }
        );

        if (!Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }

        return LocalRedirect(returnUrl);
    }
}