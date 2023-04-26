using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

public interface IMessagesRepo
{
    public Task<IEnumerable<TResult>> GetChat<TResult>(string firstUserId, string secondUserId, int upTo, Expression<Func<Message, TResult>> selector);

    public Task<IEnumerable<TResult>> GetUserChats<TResult>(string userId, int upTo, Expression<Func<IGrouping<string, Message>, TResult>> selector);
    
    public Task<IEnumerable<TResult>> GetLastNUnseenChats<TResult>(string userId, int number, Expression<Func<Message, TResult>> selector);
}
