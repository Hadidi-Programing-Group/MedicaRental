using MedicaRental.DAL.Models;

namespace MedicaRental.BLL.Dtos
{
    public record ChatDto
    (
        string UserName,
        string LastMessage,
        DateTime MessageDate,
        MessageStatus MessageStatus,
        int UnseenMessagesCount,
        byte[]? UserProfileImage
    );
}
