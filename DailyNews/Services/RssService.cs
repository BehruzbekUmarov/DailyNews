using CodeHollow.FeedReader;
using DailyNews.Data;
using DailyNews.Models;
using System;

namespace DailyNews.Services;

public class RssService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _rssUrl = "https://kun.uz/news/rss";

    public RssService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                        HttpResponseMessage response = await client.GetAsync(_rssUrl);
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Failed to fetch RSS feed: {response.StatusCode}");
                            return;
                        }

                        string rssContent = await response.Content.ReadAsStringAsync();
                        if (string.IsNullOrWhiteSpace(rssContent))
                        {
                            Console.WriteLine("RSS feed response is empty!");
                            return;
                        }

                        Console.WriteLine("RSS Content Fetched Successfully:");
                        Console.WriteLine(rssContent.Substring(0, Math.Min(500, rssContent.Length))); 

                        var feed = FeedReader.ReadFromString(rssContent);
                        foreach (var item in feed.Items)
                        {
                            bool exists = dbContext.NewsItems.Any(n => n.Link == item.Link);
                            if (!exists)
                            {
                                dbContext.NewsItems.Add(new News
                                {
                                    Title = item.Title,
                                    Link = item.Link,
                                    PubDate = item.PublishingDate ?? DateTime.Now,
                                    Summary = item.Description ?? "No summary available"
                                });
                            }
                        }

                        await dbContext.SaveChangesAsync();
                        Console.WriteLine("RSS feed successfully saved to database.");
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(25), stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching RSS feed: {ex.Message}");
            }
        }
    }
}
