using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Message;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface IMessagesManager
{
    public Task<IEnumerable<ChatDto>> GetUserChats(string userId, int upTo);

    public Task<IEnumerable<MessageDto>> GetChat(string firstUserId, string secondUserId, int upTo, DateTime dateOpened);

    public Task<IEnumerable<MessageNotificationDto>> GetUnseenChats(string userId);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstUserId">The one who opened the chat</param>
    /// <param name="secondUserId">the other user</param>
    /// <returns></returns>
    public Task<bool> UpdateMessageStatusToSeen(string firstUserId, string secondUserId, DateTime dateOpened);
    
    public Task<StatusDto> DeleteMessage(string userId, Guid messageId);

    public Task<Guid> AddMessage(string fromId, string toId, string message, DateTime timeStamp);

    public Task<bool> UpdateMessageStatus(Guid messageId);

    public Task<bool> UpdateMessageStatusToReceived(string userId, DateTime dateOpened);

    public Task<int> GetNotificationCount(string userId);

}
