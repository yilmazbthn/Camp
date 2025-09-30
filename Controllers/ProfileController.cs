using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KampMVC.Models; // Modellerinizin bulunduğu namespace
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace KampMVC.Controllers;

[Authorize] // Controller seviyesinde yetkilendirme kontrolü
public class ProfileController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public ProfileController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    // URL: /profile (Kendi Profiline Yönlendirme)
    [HttpGet]
    [Route("profile")] 
    public async Task<IActionResult> MyProfile()
    {
        var identityUser = await _userManager.GetUserAsync(User);
        if (identityUser == null)
        {
            // Kullanıcı oturumu açık değilse Identity/Account'a yönlendir
            return RedirectToAction("Login", "Account");
        }
        
        // Kullanıcı adını alarak Index aksiyonuna yönlendir
        return RedirectToAction(nameof(Index), new { username = identityUser.UserName });
    }

    // URL: /profile/{username} (Belirli Kullanıcının Profilini Gösterme)
    [HttpGet]
    [Route("profile/{username}")]
    public async Task<IActionResult> Index(string username)
    {
        // 1. Görüntülenen kullanıcıyı Identity sisteminde ara
        var viewedUser = await _userManager.FindByNameAsync(username);

        if (viewedUser == null)
        {
            return NotFound(); // Kullanıcı bulunamazsa 404
        }
        
        // 2. Oturum Açmış Kullanıcıyı Al (IsCurrentUser kontrolü için)
        var currentUser = await _userManager.GetUserAsync(User);
        bool isCurrentUser = currentUser != null && currentUser.UserName.Equals(viewedUser.UserName, StringComparison.OrdinalIgnoreCase);

        
        // 3. ViewModel'i doldur (Veritabanı/Servis çağrıları simülasyonu)
        // Burada gerçekte 'viewedUser.Id' kullanarak veritabanından profil bilgileri çekilir.
        var model = new UserProfileViewModel
        {
            UserId = 1, // Gerçek ID
            DisplayName = viewedUser.UserName, // Varsayılan olarak UserName
            Username = viewedUser.UserName,
            ActivityStatus = "Çevrimiçi",
            AvatarUrl = isCurrentUser ? "/img/john-avatar.jpg" : "/img/jane.jpg", // Örnek ayrım
            CoverPhotoUrl = "/img/profile-cover.jpg",
            FriendCount = 42,
            GroupCount = 5,
            PostCount = 120,
            
            IsCurrentUser = isCurrentUser, // Dinamik kontrol
            IsFollowing = !isCurrentUser, // Kendini takip edemezsin
            
            ActivityPosts = GetSamplePostsForProfile(viewedUser.UserName), 
            Friends = GetSampleFriends(),
            Groups = GetSampleGroups()
        };

        ViewData["Title"] = $"{model.DisplayName} Profil";
        return View(model);
    }
    
    // *** VERİ SİMÜLASYON METOTLARI ***
    // (Simülasyon metodlarında kullanılan model namespace'ini Kendi projenizdeki ile değiştirin: KampMVC.Models)
    private List<ActivityPost> GetSamplePostsForProfile(string username)
    {
         var author = new KampMVC.Models.User { DisplayName = username, Username = username, AvatarUrl = "/img/john-avatar.jpg" };
         // Sadece o username'e ait postları simüle eder
         return new List<ActivityPost>
         {
             new ActivityPost { Id = 301, Author = author, Content = $"{username} için ilk post içeriği.", TimePosted = DateTime.Now.AddHours(-1) },
         };
    }
    
    private List<SimpleUser> GetSampleFriends()
    {
        return new List<SimpleUser>
        {
            new SimpleUser { DisplayName = "Jane Smith", Username = "jane", AvatarUrl = "/img/jane.jpg" },
        };
    }
    
    private List<SimpleGroup> GetSampleGroups()
    {
        return new List<SimpleGroup>
        {
            new SimpleGroup { GroupName = "Yazılım Geliştirme", Slug = "yazilim", GroupAvatarUrl = "/img/group-1.jpg" },
        };
    }
}