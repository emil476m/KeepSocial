namespace API.TransferModels;

public class ResponseDTO
{
    public class ResponseDto
    {
        public string MessageToClient { get; set; }
        public object? ResponseData { get; set; }
    }
}