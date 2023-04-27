using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Message;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.Repositories;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public class MessagesManager : IMessagesManager
{
    private readonly IUnitOfWork _unitOfWork;

    public MessagesManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> AddMessage(string fromId, string toId, string message, DateTime timeStamp)
    {
        var msg = new Message
        {
            Content = message,
            MesssageStatus = MessageStatus.Sent,
            SenderId = fromId,
            ReceiverId = toId,
            Timestamp = timeStamp
        };
        await _unitOfWork.Messages.AddAsync(msg);
        
        _unitOfWork.Save();

        return msg.Id;
    }

    public async Task<StatusDto> DeleteMessage(string userId, Guid messageId)
    {
        var succeeded = await _unitOfWork.Messages.DeleteOneById(messageId);

        if (succeeded)
        {
            _unitOfWork.Save();
            return new("Message deleted successfully", HttpStatusCode.NoContent);
        }

        return new("Message couldn't be deleted", HttpStatusCode.BadRequest);
    }

    public async Task<IEnumerable<MessageDto>> GetChat(string firstUserId, string secondUserId, int upTo, DateTime dateOpened)
    {
        var succeeded = await UpdateMessageStatusToSeen(firstUserId, secondUserId, dateOpened);

        if (succeeded) _unitOfWork.Save();
        else throw new Exception("WTF");

        return await ((IMessagesRepo)_unitOfWork.Messages).GetChat<MessageDto>(firstUserId, secondUserId, upTo, m => new(m.Id, m.Content, m.SenderId, m.Timestamp.ToString("o"), m.MesssageStatus));
    }

    public async Task<IEnumerable<ChatDto>> GetUserChats(string userId, int upTo)
    {
        var chats = await ((IMessagesRepo)_unitOfWork.Messages).GetUserChats
            (
                userId,
                upTo,
                g =>
                    new ChatDto
                    (
                        g.First().ReceiverId == userId ? g.First().SenderId : g.First().ReceiverId,
                        g.First().ReceiverId == userId ? g.First().Sender!.User!.FirstName : g.First()!.Receiver!.User!.FirstName,
                        g.OrderByDescending(m => m.Timestamp).FirstOrDefault()!.Content,
                        g.OrderByDescending(m => m.Timestamp).FirstOrDefault()!.Timestamp.ToString("o"),
                        g.OrderByDescending(m => m.Timestamp).FirstOrDefault()!.MesssageStatus,
                        g.Count(m => m.MesssageStatus != MessageStatus.Seen && m.ReceiverId == userId),
                        g.First().ReceiverId == userId ? Convert.ToBase64String(g.First().Sender!.ProfileImage ?? new byte[0]) : Convert.ToBase64String(g.First()!.Receiver!.ProfileImage ?? new byte[0])
                    )
           );

        return chats.OrderByDescending(chat => chat.MessageDate);
    }

    public async Task<bool> UpdateMessageStatusToSeen(string firstUserId, string secondUserId, DateTime dateOpened)
    {
        var messages = await _unitOfWork.Messages.FindAllAsync
                            (
                                predicate: m => m.ReceiverId == firstUserId && m.SenderId == secondUserId,
                                disableTracking: false
                            );

        foreach (var msg in messages)
        {
            msg.MesssageStatus = MessageStatus.Seen;
        }

        var res = _unitOfWork.Messages.UpdateRange(messages);
        
        if(res) _unitOfWork.Save();

        return res;
    }

    public async Task<bool> UpdateMessageStatusToReceived(string userId, DateTime dateOpened)
    {
        var messages = await _unitOfWork.Messages.FindAllAsync
                            (
                                predicate: m => m.ReceiverId == userId,
                                disableTracking: false
                            );

        foreach (var msg in messages)
        {
            msg.MesssageStatus = MessageStatus.Received;
        }

        var res = _unitOfWork.Messages.UpdateRange(messages);
        
        if (res)
            _unitOfWork.Save();

        return res;
    }

    public async Task<int> GetNotificationCount(string userId)
    {
        return await _unitOfWork.Messages.GetCountAsync(
            predicate: m => m.ReceiverId == userId && m.MesssageStatus != MessageStatus.Seen);
    }

    public async Task<IEnumerable<MessageNotificationDto>> GetLastNUnseenChats(string userId, int number)
    {
        return await ((IMessagesRepo)_unitOfWork.Messages).GetLastNUnseenChats<MessageNotificationDto>(userId, number, m => 
        new(m.Sender!.User!.Name, Convert.ToBase64String(m.Sender.ProfileImage ?? new byte[0]), m.Content, m.Timestamp.ToString("o")));
    }

    public async Task<bool> UpdateMessageStatus(Guid messageId)
    {
        var msg = await _unitOfWork.Messages.FindAsync(predicate: m => m.Id == messageId, disableTracking: false);

        if (msg is null)
            return false;
        
        msg.MesssageStatus = MessageStatus.Seen;
        _unitOfWork.Messages.Update(msg);
        _unitOfWork.Save();
        return true;
    }
}
