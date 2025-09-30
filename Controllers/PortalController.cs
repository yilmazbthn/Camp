using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KampMVC.Models;
using KampMVC.Data;
using System.Security.Claims;

namespace KampMVC.Controllers;

[Authorize] 
public class PortalController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public PortalController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    
    public async Task<IActionResult> Portal()
    {
        
        var identityUser = await _userManager.GetUserAsync(User);
        if (identityUser == null)
        {
            
            return RedirectToAction("Logout", "Account");
        }

        
        var model = new ActivityFeedViewModel
        {
            CurrentUser = new KampMVC.Models.User
            {
                DisplayName = "John Doe",
                Username = identityUser.UserName,
                AvatarUrl = "/img/john-avatar.jpg",
            },
            ProfileCompletionPercentage = 73,
            
            
            ActivityPosts = GetSamplePosts(),
            
            LatestUpdates = GetSampleUpdates(),
            BlogPosts = GetSampleBlogPosts()
        };

        return View(model);
    }
    
    

    private List<ActivityPost> GetSamplePosts()
    {
        var sampleAuthor = new KampMVC.Models.User { DisplayName = "Jane Smith", Username = "janesmith", AvatarUrl = "/img/jane.jpg" };
        var currentUser = new KampMVC.Models.User { DisplayName = "John Doe", Username = "johndoe", AvatarUrl = "/img/john-avatar.jpg" };
        
        return new List<ActivityPost>
        {
            new ActivityPost 
            {
                Id = 101,
                Author = sampleAuthor,
                Content = "Yeni Backend mimarimizi tamamladık! Performansta %40 artış sağladık. 🎉",
                TimePosted = DateTime.Now.AddHours(-3),
                LikeCount = 45,
                CommentCount = 3,
                Comments = new List<Comment>
                {
                    new Comment { Author = currentUser, Content = "Tebrikler ekip!", TimePosted = "1 saat önce" }
                }
            },
            new ActivityPost 
            {
                Id = 102,
                Author = currentUser,
                Content = "Yazılım geliştirme sürecindeki en büyük zorluğunuz nedir?",
                TimePosted = DateTime.Now.AddDays(-1),
                LikeCount = 12,
                CommentCount = 8,
                Comments = new List<Comment>()
            }
        };
    }

    private List<ActivityUpdate> GetSampleUpdates()
    {
        var user1 = new KampMVC.Models.User { DisplayName = "Adele", AvatarUrl = "/img/adele.jpg" };
        var user2 = new KampMVC.Models.User { DisplayName = "Mark", AvatarUrl = "/img/mark.jpg" };

        return new List<ActivityUpdate>
        {
            new ActivityUpdate { Author = user1, PostId = 201, TimeAgo = "10 dk önce" },
            new ActivityUpdate { Author = user2, PostId = 202, TimeAgo = "2 saat önce" }
        };
    }
    
    private List<KampMVC.Models.BlogPost> GetSampleBlogPosts()
    {
        return new List<KampMVC.Models.BlogPost>
        {
            new KampMVC.Models.BlogPost { Title = "Yapay Zeka ve Gelecek", Date = DateTime.Now.AddDays(-5) },
            new KampMVC.Models.BlogPost { Title = "Temiz Kod Yazmanın 5 Yolu", Date = DateTime.Now.AddDays(-10) }
        };
    }
}