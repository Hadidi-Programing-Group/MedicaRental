using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public class MessagesRepo : EntityRepo<Message>, IMessagesRepo
{
    private MedicaRentalDbContext _context;

    public MessagesRepo(MedicaRentalDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TResult>> GetChat<TResult>(string firstUserId, string secondUserId, int upTo, Expression<Func<Message, TResult>> selector)
    {
        return await _context.Messages
            .Where(m => ((m.SenderId == firstUserId && m.ReceiverId == secondUserId) || (m.ReceiverId == firstUserId && m.SenderId == secondUserId)) && m.Timestamp > DateTime.UtcNow.AddDays(-upTo)) 
            .OrderBy(m => m.Timestamp)
            .Select(selector).ToListAsync();
    }

    public async Task<IEnumerable<TResult>> GetUnseenChats<TResult>(string userId, Expression<Func<IGrouping<string, Message>, TResult>> selector)
    {
        return await _context.Messages
         .Where(m => m.ReceiverId == userId)
         .Where(m => m.MesssageStatus == MessageStatus.Sent)
         .GroupBy(m => m.SenderId)
         .Select(selector).ToListAsync();
    }

    public async Task<IEnumerable<TResult>> GetUserChats<TResult>(string userId, int upTo, Expression<Func<IGrouping<string, Message>, TResult>> selector)
    {
        return await _context.Messages
        .Where(m => m.SenderId == userId || m.ReceiverId == userId)
        .Where(m => m.Timestamp > DateTime.UtcNow.AddDays(-upTo))
        .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
        .Select(selector).ToListAsync();
    }
}