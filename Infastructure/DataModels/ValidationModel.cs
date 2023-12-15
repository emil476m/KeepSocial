namespace Infastructure;

public class ValidationModel
{
    public int userId { get; set; }
    
    public int validationNumber { get; set; }

    public DateTimeOffset created { get; set; }
    
}