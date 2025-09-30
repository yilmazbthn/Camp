using KampMVC.Data;
using KampMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KampMVC.Services
{
    public class MessageService : IMessageService
    {
        // LÜTFEN 'AppDbContext' yerine projenizdeki gerçek DbContext adını kullanın.
        private readonly AppDbContext _context; 
        private readonly UserManager<IdentityUser> _userManager;

        public MessageService(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // 1. MESAJ KUTUSU LİSTESİNİ ÇEKME (Index aksiyonu için)
        public async Task<MessageListViewModel> GetInboxAsync(string currentUserId)
        {
            var conversationEntities = await _context.ConversationParticipants
                .Where(cp => cp.UserId == currentUserId)
                .Select(cp => cp.Conversation)
                .OrderByDescending(c => c.DateCreated) 
                .ToListAsync();

            var conversations = new List<ConversationSummary>();
            var allParticipantIds = conversationEntities
                .SelectMany(c => _context.ConversationParticipants.Where(cp => cp.ConversationId == c.Id))
                .Select(cp => cp.UserId)
                .Distinct()
                .ToList();

            var usersDictionary = await _userManager.Users
                .Where(u => allParticipantIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            foreach (var entity in conversationEntities)
            {
                var lastMessage = await _context.Messages
                    .Where(m => m.ConversationId == entity.Id)
                    .OrderByDescending(m => m.TimeSent)
                    .FirstOrDefaultAsync();

                var otherParticipantId = await _context.ConversationParticipants
                    .Where(p => p.ConversationId == entity.Id && p.UserId != currentUserId)
                    .Select(p => p.UserId)
                    .FirstOrDefaultAsync();

                SimpleUser otherParticipant = null;
                if (otherParticipantId != null && usersDictionary.ContainsKey(otherParticipantId))
                {
                    var user = usersDictionary[otherParticipantId];
                    otherParticipant = new SimpleUser 
                    { 
                        UserId = user.Id, 
                        DisplayName = user.UserName, 
                        Username = user.UserName, 
                        AvatarUrl = "/img/default-avatar.png" 
                    };
                }
                
                var unreadCount = await _context.Messages
                    .Where(m => m.ConversationId == entity.Id && m.SenderId != currentUserId)
                    .CountAsync(); 

                conversations.Add(new ConversationSummary
                {
                    ConversationId = entity.Id,
                    Subject = entity.Subject,
                    LastMessageContent = lastMessage?.Content ?? "Mesaj yok",
                    LastActivityTime = lastMessage?.TimeSent ?? entity.DateCreated,
                    UnreadCount = unreadCount,
                    OtherParticipant = otherParticipant
                });
            }

            return new MessageListViewModel { Conversations = conversations };
        }

        // 2. YENİ KONUŞMA BAŞLATMA
        public async Task<int> StartNewConversationAsync(string senderId, ComposeViewModel model)
        {
            var recipientUsernames = model.Recipients.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var recipients = await _userManager.Users
                .Where(u => recipientUsernames.Contains(u.UserName))
                .ToListAsync();

            if (!recipients.Any()) return 0;

            var newConversation = new Data.Entities.Conversation
            {
                Subject = model.Subject,
                DateCreated = DateTime.Now,
            };
            _context.Conversations.Add(newConversation);

            var message = new Data.Entities.Message
            {
                SenderId = senderId,
                Content = model.Content,
                TimeSent = DateTime.Now,
                Conversation = newConversation 
            };
            _context.Messages.Add(message);

            var participantIds = recipients.Select(r => r.Id).ToList();
            participantIds.Add(senderId); 
            
            foreach (var userId in participantIds.Distinct())
            {
                _context.ConversationParticipants.Add(new Data.Entities.ConversationParticipant
                {
                    ConversationId = newConversation.Id,
                    UserId = userId,
                    HasRead = (userId == senderId) 
                });
            }
            
            await _context.SaveChangesAsync();
            return newConversation.Id;
        }

        // 3. VAR OLAN KONUŞMAYA CEVAP GÖNDERME
        public async Task<int> ReplyToConversationAsync(string senderId, int conversationId, string content)
        {
            if (string.IsNullOrEmpty(content)) return 0;

            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == senderId);

            if (!isParticipant) return 0;

            var message = new Data.Entities.Message
            {
                SenderId = senderId,
                Content = content,
                TimeSent = DateTime.Now,
                ConversationId = conversationId
            };
            _context.Messages.Add(message);

            var otherParticipants = await _context.ConversationParticipants
                .Where(cp => cp.ConversationId == conversationId && cp.UserId != senderId)
                .ToListAsync();

            foreach (var participant in otherParticipants)
            {
                participant.HasRead = false;
            }

            await _context.SaveChangesAsync();
            return conversationId;
        }
        
        // 4. KONUŞMA DETAYI ÇEKME
        public async Task<ConversationDetailViewModel> GetConversationDetailAsync(int conversationId, string currentUserId)
        {
            var conversationEntity = await _context.Conversations
                .Include(c => c.Participants) 
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversationEntity == null || !conversationEntity.Participants.Any(p => p.UserId == currentUserId))
            {
                return null; 
            }

            var participantIds = conversationEntity.Participants.Select(p => p.UserId).ToList();
            var messageSenderIds = await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .Select(m => m.SenderId)
                .Distinct()
                .ToListAsync();

            var allRequiredUserIds = participantIds.Union(messageSenderIds).Distinct().ToList();
            var usersDictionary = await _userManager.Users
                .Where(u => allRequiredUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.TimeSent)
                .Select(m => new Message
                {
                    MessageId = m.Id,
                    Sender = usersDictionary.ContainsKey(m.SenderId) ? new SimpleUser
                    {
                        UserId = m.SenderId,
                        DisplayName = usersDictionary[m.SenderId].UserName,
                        Username = usersDictionary[m.SenderId].UserName,
                        AvatarUrl = "/img/default-avatar.png" 
                    } : new SimpleUser(),
                    Content = m.Content,
                    TimeSent = m.TimeSent
                })
                .ToListAsync();

            var participantEntry = conversationEntity.Participants.FirstOrDefault(cp => cp.UserId == currentUserId);
            if (participantEntry != null && participantEntry.HasRead == false)
            {
                participantEntry.HasRead = true;
                await _context.SaveChangesAsync();
            }

            return new ConversationDetailViewModel
            {
                ConversationId = conversationEntity.Id,
                Subject = conversationEntity.Subject,
                Messages = messages,
                Participants = usersDictionary.Values.Select(u => new SimpleUser
                {
                    UserId = u.Id,
                    DisplayName = u.UserName,
                    Username = u.UserName,
                    AvatarUrl = "/img/default-avatar.png"
                }).ToList()
            };
        }
        
        // 5. KULLANICI ARAMA (SearchUsers aksiyonu için)
        public async Task<List<SimpleUser>> SearchUsersAsync(string term)
        {
            var users = await _userManager.Users
                .Where(u => u.UserName.Contains(term) || u.Email.Contains(term))
                .Take(10) 
                .Select(u => new SimpleUser 
                {
                    UserId = u.Id,
                    Username = u.UserName,
                    DisplayName = u.UserName, 
                    AvatarUrl = "/img/default-avatar.png" 
                })
                .ToListAsync();
            
            return users;
        }
    }
}