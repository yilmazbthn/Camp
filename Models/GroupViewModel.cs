using System;
using System.Collections.Generic;

namespace KampMVC.Models
{
    public class GroupProfileViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string GroupType { get; set; } // Örn: Public, Private, Hidden
        public string GroupAvatarUrl { get; set; }
        public string CoverPhotoUrl { get; set; }
        
        // İstatistikler
        public int MemberCount { get; set; }
        public int ForumCount { get; set; }
        public bool IsMember { get; set; } // Görüntüleyen üye mi?
        public bool IsAdmin { get; set; } // Görüntüleyen yönetici mi?

        // Sayfa İçerikleri
        public List<ActivityPost> GroupActivityPosts { get; set; } // Akış sekmesi
        public List<SimpleUser> Members { get; set; } // Üyeler sekmesi
        public List<SimpleForum> Forums { get; set; } // Forumlar sekmesi
    }

    // Basitleştirilmiş Forum (Grup içindeki forumlar için)
    
}