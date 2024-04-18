namespace API_APBD_06.DTO;


public record GetAllAnimalsResponse(int Id, string Name, string Description, string Category, string Area);
public record CreateAnimalsRequest(string Name, string Descripton, string Category, string Area);