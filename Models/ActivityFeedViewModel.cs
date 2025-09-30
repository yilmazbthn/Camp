using System;
using System.Collections.Generic;

namespace KampMVC.Models
{
    // Akış Sayfası Ana Modeli
    public class ActivityFeedViewModel
    {
        public int ProfileCompletionPercentage { get; set; }
        public User CurrentUser { get; set; }
        public List<ActivityPost> ActivityPosts { get; set; }
        public List<ActivityUpdate> LatestUpdates { get; set; }
        public List<BlogPost> BlogPosts { get; set; }
    }

    // Kullanıcı Temel Bilgileri
    public class User
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public int PostCount { get; set; }
        public string Role { get; set; }
    }
    
    // Aktivite Akışı Gönderisi
    public class ActivityPost
    {
        public int Id { get; set; }
        public User Author { get; set; }
        public string Content { get; set; }
        public string MediaUrl { get; set; }
        public DateTime TimePosted { get; set; }
        public string TimeAgo => $"{(int)(DateTime.Now - TimePosted).TotalDays} gün önce";
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public List<Comment> Comments { get; set; }
    }

    // Yorum
    public class Comment
    {
        public User Author { get; set; }
        public string Content { get; set; }
        public string TimePosted { get; set; }
    }

    // Sağ Sütun Güncellemeleri
    public class ActivityUpdate
    {
        public User Author { get; set; }
        public int PostId { get; set; }
        public string TimeAgo { get; set; }
    }

    // Sol Sütun Blog Yazıları
    public class BlogPost
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
    }
}