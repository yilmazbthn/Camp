using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KampMVC.Models;
using System.Threading.Tasks;

namespace KampMVC.Controllers;

[Authorize]
[Route("settings")]
public class SettingsController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public SettingsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // Ana Ayarlar Sayfası (GET)
    [HttpGet]
    public async Task<IActionResult> Index(string tab = "general")
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        // Sadece GeneralSettingsViewModel'i alıp View'a gönderiyoruz.
        // Diğer sekmelerdeki formlar boş olarak yüklenecek.
        var generalModel = new GeneralSettingsViewModel
        {
            DisplayName = user.UserName, 
            AboutMe = "Merhaba, ben bir yazılımcıyım!",
            Location = "Türkiye",
            Email = user.Email // Email bilgisini de general modelde tutalım
        };
        
        ViewData["ActiveTab"] = tab;
        return View(generalModel); 
    }

    // 1. Genel Ayarları Güncelleme POST aksiyonu
    [HttpPost("general")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateGeneral(GeneralSettingsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Hata varsa, View'a dönerken ActiveTab'ı tekrar set et
            ViewData["ActiveTab"] = "general";
            // View, gelen model'i GeneralSettingsViewModel olarak işleyecek
            return View("Index", model); 
        }

        var user = await _userManager.GetUserAsync(User);
        // ... Veritabanı güncelleme işlemleri (örneğin DisplayName, Biyografi)
        
        TempData["SuccessMessage"] = "Genel ayarlarınız başarıyla güncellendi.";
        return RedirectToAction(nameof(Index), new { tab = "general" });
    }

    // 2. Parola Değiştirme POST aksiyonu
    [HttpPost("password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        // ÖNEMLİ: Hata durumunda model geri gönderilmelidir.
        if (!ModelState.IsValid)
        {
            ViewData["ActiveTab"] = "password";
            // Hatalı modeli View'a geri gönder
            return View("Index", model); 
        }

        var user = await _userManager.GetUserAsync(User);
        
        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Parolanız başarıyla değiştirildi.";
            return RedirectToAction(nameof(Index), new { tab = "password" });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        
        ViewData["ActiveTab"] = "password";
        return View("Index", model); 
    }
    
    // 3. E-posta ve Bildirimleri Güncelleme POST aksiyonu
    // (EmailSettingsViewModel yerine bu örneği basitleştirmek için GeneralSettingsViewModel kullanıldı)
    [HttpPost("email")]
    [ValidateAntiForgeryToken]
    // Bu aksiyonun sadece E-posta ve bildirim alanlarını içermesi gerekir.
    // Şimdilik sadece e-posta güncelleme simülasyonunu yapalım:
    public async Task<IActionResult> UpdateEmail(GeneralSettingsViewModel model) 
    {
        ViewData["ActiveTab"] = "email";
        
        if (!ModelState.IsValid)
        {
            return View("Index", model); 
        }
        
        // ... E-posta ve Bildirim ayarları güncelleme mantığı buraya gelir.
        
        TempData["SuccessMessage"] = "E-posta ve bildirim ayarlarınız güncellendi.";
        return RedirectToAction(nameof(Index), new { tab = "email" });
    }
}