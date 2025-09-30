using System;
using System.Collections.Generic;

namespace KampMVC.Models
{
    public class UserProfileViewModel
    {
        // Kullanıcı Kimlik Bilgileri
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string ActivityStatus { get; set; } // Örn: Çevrimiçi, En son 5dk önce
        public bool IsCurrentUser { get; set; } // Görüntüleyen, kendi profili mi?
        public bool IsFollowing { get; set; } // Görüntüleyen, bu profili takip ediyor mu?

        // Görsel Bilgiler
        public string AvatarUrl { get; set; }
        public string CoverPhotoUrl { get; set; }

        // Profil İstatistikleri
        public int FriendCount { get; set; }
        public int GroupCount { get; set; }
        public int PostCount { get; set; }

        // Tab İçerikleri
        public List<ActivityPost> ActivityPosts { get; set; } // Aktivite sekmesi için
        public List<SimpleUser> Friends { get; set; } // Arkadaşlar sekmesi için (Basit kullanıcı listesi)
        public List<SimpleGroup> Groups { get; set; } // Gruplar sekmesi için
    }

    // Basitleştirilmiş Kullanıcı (Arkadaşlar listesi için)
}