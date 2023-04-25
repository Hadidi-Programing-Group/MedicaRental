using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.Repositories;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<bool> AddMessage(string fromId, string toId, string message, DateTime timeStamp)
    {
        var msg = new Message
        {
            Content = message,
            MesssageStatus = MessageStatus.Sent,
            SenderId = fromId,
            ReceiverId = toId,
            Timestamp = timeStamp
        };

        return await _unitOfWork.Messages.AddAsync(msg);
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

        return await ((IMessagesRepo)_unitOfWork.Messages).GetChat<MessageDto>(firstUserId, secondUserId, upTo, m => new(m.Id, m.Content, m.SenderId, m.Timestamp, m.MesssageStatus));
    }

    public async Task<IEnumerable<ChatDto>> GetUserChats(string userId, int upTo)
    {
        return await ((IMessagesRepo)_unitOfWork.Messages).GetUserChats
            (
                userId,
                upTo,
                g =>
                    new ChatDto
                    (
                        g.First().ReceiverId == userId ? g.First().SenderId : g.First().ReceiverId,
                        g.First().ReceiverId == userId ? g.First().Sender!.User!.FirstName : g.First()!.Receiver!.User!.FirstName,
                        g.OrderByDescending(m => m.Timestamp).FirstOrDefault()!.Content,
                        g.OrderByDescending(m => m.Timestamp).FirstOrDefault()!.Timestamp,
                        g.OrderByDescending(m => m.Timestamp).FirstOrDefault()!.MesssageStatus,
                        g.Count(m => m.MesssageStatus != MessageStatus.Seen && m.ReceiverId == userId),
                        g.First().ReceiverId == userId ? Convert.ToBase64String(g.First().Sender!.ProfileImage?? new byte[0]) : Convert.ToBase64String(g.First()!.Receiver!.ProfileImage?? new byte[0])
                    )
           );

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

        return _unitOfWork.Messages.UpdateRange(messages);
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

        return _unitOfWork.Messages.UpdateRange(messages);
    }

    public Task<bool> GetNotificationCount(string userId, DateTime dateOpened)
    {
        throw new NotImplementedException();
    }
}
