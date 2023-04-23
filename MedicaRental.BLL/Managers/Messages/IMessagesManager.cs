using MedicaRental.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface IMessagesManager
{
    public Task<IEnumerable<ChatDto>> GetUserChats(string userId, int upTo);

    public Task<IEnumerable<MessageDto>> GetChat(string firstUserId, string secondUserId, DateTime dateOpened);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstUserId">The one who opened the chat</param>
    /// <param name="secondUserId">the other user</param>
    /// <returns></returns>
    public Task<bool> UpdateSeenStatus(string firstUserId, string secondUserId, DateTime dateOpened);
    
    public Task<StatusDto> DeleteMessage(string userId, Guid messageId);
}
