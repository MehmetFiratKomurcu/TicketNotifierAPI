namespace TicketNotifier.Models
{
    public class BaseResponse<T>
    {
        public bool HasError => !string.IsNullOrEmpty(Message);
        public string Message { get; set; }
        public T Result { get; set; }
    }
}