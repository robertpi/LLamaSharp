namespace LlamaSharp.WebApp.ViewModels;

public class MessageViewModel
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public string Room { get; set; }
}