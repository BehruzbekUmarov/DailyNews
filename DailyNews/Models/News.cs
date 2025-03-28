namespace DailyNews.Models;

public class News
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public DateTime PubDate { get; set; }
    public string Summary { get; set; }
}
