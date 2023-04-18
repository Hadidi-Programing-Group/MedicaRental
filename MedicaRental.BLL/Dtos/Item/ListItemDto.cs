namespace MedicaRental.BLL.Dtos
{
    //include categ, subcateg, reviews
    public record ListItemDto
    (
        Guid Id,
        string Name,
        decimal Price,
        int Stock,
        string CategoryName,
        string SubCategoryName,
        int Rating
    );
}
