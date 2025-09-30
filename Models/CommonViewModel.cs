using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace KampMVC.Models
{
    // *** TEK VE RESMİ SimpleUser Tanımı ***
    public class SimpleUser
    {
        // Mesajlaşma ve diğer özellikler için gerekli ID
        public string UserId { get; set; } 

        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        
        // Gerekirse diğer ortak özellikler buraya eklenebilir.
    }
    
    // Basitleştirilmiş Grup (Profil ve Mesajlaşma dışındaki yerler için)
    public class SimpleGroup
    {
        public string GroupName { get; set; }
        public string Slug { get; set; }
        public string GroupAvatarUrl { get; set; }
    }

    // Basitleştirilmiş Forum (Grup içindeki forumlar için)
    public class SimpleForum
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int TopicCount { get; set; }
    }
    
    // ... (Gerekirse ActivityPost Entity'sini temsil eden bir ActivityPostViewModel de buraya taşınabilir.)
}