using KampMVC.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace KampMVC.Services
{
    public interface IMessageService
    {
        Task<MessageListViewModel> GetInboxAsync(string currentUserId);
        
        Task<int> StartNewConversationAsync(string senderId, ComposeViewModel model); 

        Task<int> ReplyToConversationAsync(string senderId, int conversationId, string content); 
        
        Task<ConversationDetailViewModel> GetConversationDetailAsync(int conversationId, string currentUserId);

        Task<List<SimpleUser>> SearchUsersAsync(string term);
    }
}