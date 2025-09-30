using KampMVC.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KampMVC.Models;
using Message = KampMVC.Data.Entities.Message;


namespace KampMVC.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Konuşma ve Katılımcı ara tablosu için Primary Key'i tanımlama
        builder.Entity<ConversationParticipant>()
            .HasKey(cp => new { cp.ConversationId, cp.UserId });

        // Conversation <-> Participant İlişkisi
        builder.Entity<ConversationParticipant>()
            .HasOne(cp => cp.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ConversationId);
            
        // User <-> Participant İlişkisi (IdentityUser ile ilişki kurulacak)
        // Eğer IdentityUser kullanıyorsanız, buraya IdentityUser ile ilişki kuran kodu eklemelisiniz.
        /*
        builder.Entity<ConversationParticipant>()
            .HasOne<IdentityUser>() // IdentityUser sınıfınızın adını kullanın
            .WithMany()
            .HasForeignKey(cp => cp.UserId)
            .IsRequired();
        */
        
        // Message <-> Conversation İlişkisi
        builder.Entity<Message>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId);
    }
}
