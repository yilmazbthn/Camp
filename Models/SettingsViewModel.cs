using System.ComponentModel.DataAnnotations;

namespace KampMVC.Models
{
    // Ayarlar sayfasını navigasyonlu ve sekmeli bir yapı olarak düşünerek 
    // her bir ayar sekmesi için ayrı bir ViewModel oluşturuyoruz.

    public class SettingsIndexViewModel
    {
        // Ana ayarlar sayfasında yan menüde hangi sekmenin aktif olduğunu tutar
        public string ActiveTab { get; set; } = "general";
    }

    // 1. Genel Ayarlar Formu (Email alanı eklendi)
    public class GeneralSettingsViewModel
    {
        [Required(ErrorMessage = "Görünen Ad zorunludur.")]
        [Display(Name = "Görünen Ad")]
        public string DisplayName { get; set; }

        [Display(Name = "Hakkımda (Biyografi)")]
        [DataType(DataType.MultilineText)]
        public string AboutMe { get; set; }
        
        [Display(Name = "Konum")]
        public string Location { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name = "E-posta Adresi")]
        // Bu alanı, kullanıcı bilgisini Controller'da doldurup View'daki 
        // E-posta sekmesinde de göstermek için kullanıyoruz.
        public string Email { get; set; } 
    }

    // 2. Parola Değiştirme Formu
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Parola")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Parola")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Parola Tekrar")]
        [Compare("NewPassword", ErrorMessage = "Parolalar eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
    
    // 3. E-posta ve Bildirim Ayarları Formu
    // (GeneralSettingsViewModel'den farklı bildirim ayarları içerir)
    public class EmailSettingsViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; }

        [Display(Name = "Yeni Aktivite Bildirimleri")]
        public bool ReceiveActivityNotifications { get; set; }

        [Display(Name = "Yeni Mesaj Bildirimleri")]
        public bool ReceiveMessageNotifications { get; set; }
    }
}