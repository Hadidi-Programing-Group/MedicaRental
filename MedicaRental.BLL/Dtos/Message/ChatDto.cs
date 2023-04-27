using MedicaRental.DAL.Models;

namespace MedicaRental.BLL.Dtos
{
    public record ChatDto
    (
        string UserId,
        string UserName,
        string LastMessage,
        string MessageDate,
        MessageStatus MessageStatus,
        int UnseenMessagesCount,
        string? UserProfileImage
    );
}
