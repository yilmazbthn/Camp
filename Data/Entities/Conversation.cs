using System;
using System.Collections.Generic;

namespace KampMVC.Data.Entities
{
    // Tek bir konuşma odasını temsil eder.
    public class Conversation
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime DateCreated { get; set; }

        // Navigation Properties:
        // Konuşmadaki tüm mesajlar
        public ICollection<Message> Messages { get; set; } 

        // Konuşmadaki tüm katılımcılar (Many-to-Many ilişkisi)
        public ICollection<ConversationParticipant> Participants { get; set; }
    }

    // Konuşma ve Kullanıcı arasındaki ilişkiyi temsil eden ara tablo (Join Entity)
    public class ConversationParticipant
    {
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        // IdentityUser'dan kalıtım aldığımızı varsayarsak, UserID string'dir
        public string UserId { get; set; } 
        // Burada gerçek IdentityUser nesnesi yerine, sadece Id'yi tutuyoruz. 
        // Gerçek entegrasyon için IdentityUser'ı referans almalıyız.
        
        public bool HasRead { get; set; } // Bu katılımcı son mesajı okudu mu?
    }
}