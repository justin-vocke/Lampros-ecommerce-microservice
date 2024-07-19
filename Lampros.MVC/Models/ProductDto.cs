namespace Lampros.MVC.Models
{
    public record ProductDto(int ProductId, 
     string Name, 
     double Price, 
     string Description, 
     string Category, 
     string ImageUrl );
    
}
