using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KampMVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq; 
using KampMVC.Services; 
using System;

namespace KampMVC.Controllers;

[Authorize]
[Route("messages")]
public class MessageController : Controller
{
    private readonly IMessageService _messageService; 
    private readonly UserManager<IdentityUser> _userManager;

    public MessageController(IMessageService messageService, UserManager<IdentityUser> userManager)
    {
        _messageService = messageService;
        _userManager = userManager;
    }
    
    // 1. MESAJ KUTUSU LİSTELEME (Index)
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var model = await _messageService.GetInboxAsync(currentUserId);
        
        ViewData["Title"] = "Mesaj Kutusu";
        return View(model);
    }

    // 2. YENİ MESAJ OLUŞTURMA SAYFASI (GET)
    [HttpGet("compose")] 
    public IActionResult Compose(string recipientUsername)
    {
        ViewData["Title"] = "Yeni Mesaj Oluştur";
        return View(new ComposeViewModel { Recipients = recipientUsername });
    }
    
    // 3. YENİ MESAJI VEYA YANITI GÖNDERME İŞLEMİ (POST)
    // Controllers/MessageController.cs içinde Compose POST metodu

[HttpPost("compose")] 
[ValidateAntiForgeryToken]
public async Task<IActionResult> Compose(ComposeViewModel model)
{
    var sender = await _userManager.GetUserAsync(User);
    if (sender == null) return Unauthorized();
    
    int resultConversationId = 0;

    if (model.ConversationId > 0)
    {
        // SENARYO 1: MEVCUT KONUŞMAYA CEVAP VERME
        // (Burada Recipient kontrolü YAPILMAZ)
        
        // Sadece Content kontrolü yapılır.
        if (string.IsNullOrWhiteSpace(model.Content))
        {
            ModelState.AddModelError("Content", "Yanıt içeriği boş olamaz.");
        }
        
        // Eğer Content hatası varsa Conversation sayfasına geri yönlendir.
        if (!ModelState.IsValid)
        {
             // DİKKAT: ModelState.IsValid kontrolünü yapıp Conversation'a yönlendiriyoruz.
            return RedirectToAction(nameof(Conversation), new { conversationId = model.ConversationId }); 
        }

        resultConversationId = await _messageService.ReplyToConversationAsync(
            sender.Id, 
            model.ConversationId, 
            model.Content
        );
    }
    else
    {
        // SENARYO 2: YENİ KONUŞMA BAŞLATMA
        // (Burada Hem Content hem Recipient zorunluluğu var)

        // HATA OLUŞURSA View'a geri dön
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Yeni Mesaj Oluştur";
            return View(model); 
        }
        
        // **KRİTİK NOKTA:** Service'i çağır
        resultConversationId = await _messageService.StartNewConversationAsync(sender.Id, model);

        // Service alıcı bulamazsa
        if (resultConversationId == 0)
        {
            ModelState.AddModelError("Recipients", "Belirtilen alıcılardan hiçbiri bulunamadı veya alıcı gereklidir.");
            ViewData["Title"] = "Yeni Mesaj Oluştur";
            return View(model);
        }
    }

    // ... (Başarı ile yönlendirme)
    TempData["SuccessMessage"] = "Mesajınız başarıyla gönderildi!";
    return RedirectToAction(nameof(Conversation), new { conversationId = resultConversationId });
}
    // 4. KONUŞMA DETAYI (Conversation)
    // Düzeltildi: Yanlış URL hatasına karşı daha belirgin bir Route kullanıldı.
    [HttpGet("c/{conversationId:int}")] 
    public async Task<IActionResult> Conversation(int conversationId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var model = await _messageService.GetConversationDetailAsync(conversationId, currentUserId);

        if (model == null)
        {
            return Forbid(); 
        }
        
        ViewData["Title"] = model.Subject;
        return View(model); 
    }
    
    // 5. KULLANICI ARAMA API'SI
    [HttpGet("search-users")]
    public async Task<IActionResult> SearchUsers(string term)
    {
        var simpleUsers = await _messageService.SearchUsersAsync(term);
        
        var results = simpleUsers.Select(u => new 
        {
            id = u.Username, 
            text = $"{u.DisplayName} ({u.Username})", 
            avatar = u.AvatarUrl 
        });
        
        return Json(new { results = results, pagination = new { more = false } });
    }
}