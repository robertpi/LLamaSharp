namespace LlamaSharp.WebApp.Models;

public class Message
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public string FromUserName { get; set; }
    public string Avatar { get; set; }

    public IList<TokenScore> TokenScores { get; set; }
}