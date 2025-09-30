using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KampMVC.Data.Entities
{
    // Konuşma içindeki tek bir mesajı temsil eder.
    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string Content { get; set; }
        public DateTime TimeSent { get; set; }

        // Mesajı gönderen kullanıcı (IdentityUser ID'sini tutar)
        public string SenderId { get; set; } 
        
        // Navigation Property:
        public Conversation Conversation { get; set; }
        
        // Bu noktada, daha önce yarattığımız SimpleUser/IdentityUser
        // modeline EF Core üzerinden ilişki kurmanız gerekecek.
        // Şimdilik sadece SenderId'yi bırakıyoruz.
    }
}