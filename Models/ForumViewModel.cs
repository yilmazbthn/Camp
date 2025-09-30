using System;
using System.Collections.Generic;

namespace KampMVC.Models
{
    // 1. Forumlar Ana Sayfası için Model (Tüm kategorileri tutar)
    public class ForumListViewModel
    {
        public List<ForumCategory> ForumCategories { get; set; }
    }

    // Forum Kategorisi
    public class ForumCategory
    {
        public string CategoryName { get; set; }
        public List<Forum> Forums { get; set; }
    }
    
    // Bireysel Forum (Konuları tutar)
    public class Forum
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int TopicCount { get; set; }
        public int PostCount { get; set; }
        public Topic LastTopic { get; set; } // Son aktivite için
    }

    // 2. Konu Detay Sayfası için Model
    public class TopicViewModel
    {
        public int TopicId { get; set; }
        public string Title { get; set; }
        public string ForumName { get; set; }
        public string ForumSlug { get; set; }
        public int ReplyCount { get; set; }
        
        public List<Post> Posts { get; set; } // İlk mesaj + tüm cevaplar
    }

    // Forum Post/Cevap Yapısı (Kullanıcı profili içindeki ActivityPost'tan farklı)
    public class Post
    {
        public int Id { get; set; }
        public SimpleUser Author { get; set; } // SimpleUser modelini kullanıyoruz
        public string Content { get; set; }
        public DateTime TimePosted { get; set; }
        public string TimeAgo => $"{(int)(DateTime.Now - TimePosted).TotalHours} saat önce";
        public int LikeCount { get; set; }
        public string AuthorRole { get; set; } // Örn: Yazar, Moderatör
    }
    
    // Forum Konusu
    public class Topic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public SimpleUser StartedBy { get; set; }
        public string TimeAgo { get; set; }
        public int PostCount { get; set; }
    }
}