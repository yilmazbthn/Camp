using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KampMVC.Models
{
    // *** GENEL KULLANIM: Basit Kullanıcı Bilgisi Modeli (SimpleUser) ***
    // BU TANIMIN BAŞKA HİÇBİR DOSYADA OLMADIĞINDAN EMİN OLUN!
    
    
    // 1. MESAJ KUTUSU LİSTESİ İÇİN MODEL (Index Aksiyonu)
    public class MessageListViewModel
    {
        public List<ConversationSummary> Conversations { get; set; }
    }

    // Her bir Konuşma Kutucuğunu temsil eder
    public class ConversationSummary
    {
        public int ConversationId { get; set; }
        public SimpleUser OtherParticipant { get; set; } 
        public string Subject { get; set; } 
        public string LastMessageContent { get; set; }
        public DateTime LastActivityTime { get; set; }
        public string TimeAgo => $"{(int)(DateTime.Now - LastActivityTime).TotalMinutes} dakika önce";
        public int UnreadCount { get; set; } 
    }

    // 2. YENİ MESAJ OLUŞTURMA VEYA CEVAP İÇİN MODEL (Compose Aksiyonu)
    public class ComposeViewModel 
    {
        // Cevap veriliyorsa bu alan dolu olur. Yeni mesajda 0'dır.
        public int ConversationId { get; set; } 
    
        // YENİ HALİ: [Required(ErrorMessage = "Alıcı alanı boş bırakılamaz.")] SATIRINI SİLİN
        [Display(Name = "Alıcı(lar)")]
        public string Recipients { get; set; } // ARTIK [Required] DEĞİL

        [Display(Name = "Konu")]
        [Required(ErrorMessage = "Konu alanı boş bırakılamaz.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Mesaj içeriği boş bırakılamaz.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mesajınız")]
        public string Content { get; set; }
    }
    
    // 3. TEK BİR KONUŞMANIN DETAYI İÇİN MODEL (Conversation Aksiyonu)
    public class ConversationDetailViewModel
    {
        public int ConversationId { get; set; }
        public string Subject { get; set; }
        public List<Message> Messages { get; set; } 
        public List<SimpleUser> Participants { get; set; } 
    }

    // Gerçek Mesaj Yapısı (ViewModel içinde)
    public class Message
    {
        public int MessageId { get; set; }
        public SimpleUser Sender { get; set; } 
        public string Content { get; set; }
        public DateTime TimeSent { get; set; }
        public string TimeAgo => $"{(int)(DateTime.Now - TimeSent).TotalSeconds} saniye önce";
    }
}