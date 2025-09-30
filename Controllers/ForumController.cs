using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KampMVC.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace KampMVC.Controllers;

[Authorize]
public class ForumController : Controller
{
    // *** 1. TÜM FORUMLARIN LİSTESİ (URL: /forums) ***
    [HttpGet]
    [Route("forums")]
    public IActionResult Index()
    {
        // Simülasyon: Forum Kategorilerini ve İçindeki Forumları Çek
        var model = new ForumListViewModel
        {
            ForumCategories = GetSampleForumCategories()
        };

        ViewData["Title"] = "Topluluk Forumları";
        return View(model);
    }
    
    // *** 2. TEK BİR FORUMUN İÇERİĞİ (URL: /forums/forum/{slug}) ***
    [HttpGet]
    [Route("forums/forum/{slug}")]
    public IActionResult ForumDetail(string slug)
    {
        // Simülasyon: Slug'a göre ilgili forumu bul
        var forumData = GetSampleForumCategories()
                        .SelectMany(c => c.Forums)
                        .FirstOrDefault(f => f.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
        
        if (forumData == null)
        {
            return NotFound(); 
        }

        // Forumun içindeki konuları da (Topics) doldur
        var model = new ForumTopicsViewModel 
        {
            Forum = forumData,
            Topics = GetSampleTopicsForForum(forumData.Id)
        };

        ViewData["Title"] = $"{forumData.Title} Forumu";
        return View(model);
    }
    
    // *** VERİ SİMÜLASYON METOTLARI ***
    
    // Tüm Forumları ve Kategorilerini Doldur
    private List<ForumCategory> GetSampleForumCategories()
    {
        var lastTopic = new Topic 
        { 
            Id = 1, Title = "Beehive Tema Hakkında Ne Düşünüyorsunuz?", 
            Slug = "what-do-you-like-about-beehive-theme",
            TimeAgo = "2 gün önce" 
        };
        
        return new List<ForumCategory>
        {
            new ForumCategory
            {
                CategoryName = "Genel Tartışmalar",
                Forums = new List<Forum>
                {
                    new Forum { Id = 1, Title = "Hoşgeldin Köşesi", Slug = "welcome", Description = "Yeni üyeler için tanışma alanı.", TopicCount = 150, PostCount = 500, LastTopic = lastTopic },
                    new Forum { Id = 2, Title = "Yardım ve Destek", Slug = "support", Description = "Sorularınızı burada sorun.", TopicCount = 80, PostCount = 300, LastTopic = lastTopic }
                }
            },
            new ForumCategory
            {
                CategoryName = "Teknik Konular",
                Forums = new List<Forum>
                {
                    new Forum { Id = 3, Title = "Backend Geliştirme", Slug = "backend", Description = ".NET ve Java tartışmaları.", TopicCount = 60, PostCount = 250, LastTopic = lastTopic }
                }
            }
        };
    }
    
    // Tek bir Forum içindeki Konuları Doldur
    private List<Topic> GetSampleTopicsForForum(int forumId)
    {
        var author = new SimpleUser { DisplayName = "Admin", AvatarUrl = "/img/admin.jpg" };
        
        return new List<Topic>
        {
            new Topic { Id = 1, Title = "Yeni Özellik Talepleri", Slug = "new-features", StartedBy = author, TimeAgo = "1 saat önce", PostCount = 5 },
            new Topic { Id = 2, Title = "Hata Bildirimleri", Slug = "bugs", StartedBy = author, TimeAgo = "1 gün önce", PostCount = 12 }
        };
    }
    // ... diğer metotların sonuna ekleyin

    // *** 3. KONU DETAY SAYFASI (URL: /forums/topic/{slug}) ***
    [HttpGet]
    [Route("forums/topic/{slug}")]
    public IActionResult TopicDetail(string slug)
    {
        // Simülasyon: Slug'a göre konuyu çek
        var topicModel = GetSampleTopicModel(slug); 
        
        if (topicModel == null)
        {
            return NotFound();
        }

        ViewData["Title"] = topicModel.Title;
        return View(topicModel);
    }

    // Konu Detay Simülasyonu
    private TopicViewModel GetSampleTopicModel(string slug)
    {
        var adminUser = new SimpleUser { DisplayName = "Admin", Username = "admin", AvatarUrl = "/img/admin.jpg" };
        var user1 = new SimpleUser { DisplayName = "Jane Doe", Username = "jane", AvatarUrl = "/img/jane.jpg" };

        return new TopicViewModel
        {
            TopicId = 1,
            Title = "Beehive Tema Hakkında Ne Düşünüyorsunuz?",
            ForumName = "Hoşgeldin Köşesi",
            ForumSlug = "welcome",
            ReplyCount = 2,
            Posts = new List<Post>
            {
                // İlk Post
                new Post { Id = 1, Author = adminUser, Content = "<p>Bu temayı kullanmaya başladık ve ilk izlenimlerinizi merak ediyoruz. Tema hakkındaki düşüncelerinizi paylaşın!</p>", TimePosted = DateTime.Now.AddDays(-2), AuthorRole = "Yönetici" },
                // Cevap 1
                new Post { Id = 2, Author = user1, Content = "<p>Kullanıcı arayüzü çok temiz, mobil deneyimi de harika. Tebrikler!</p>", TimePosted = DateTime.Now.AddHours(-12), AuthorRole = "Üye" },
                // Cevap 2
                new Post { Id = 3, Author = adminUser, Content = "<p>Geri bildirim için teşekkürler Jane!</p>", TimePosted = DateTime.Now.AddHours(-1), AuthorRole = "Yönetici" }
            }
        };
    }
}

// ForumDetail için geçici bir ViewModel oluşturuyoruz (ForumViewModel.cs'e eklenecek)
// Bu model, Forum bilgisi ile o forumdaki konuları birleştirir.
public class ForumTopicsViewModel
{
    public Forum Forum { get; set; }
    public List<Topic> Topics { get; set; }
}