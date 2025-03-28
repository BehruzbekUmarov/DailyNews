using DailyNews.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace DailyNews.Controllers;

public class NewsController(AppDbContext context) : Controller
{
    private readonly AppDbContext _context = context;
    private const int PageSize = 5;

    public async Task<IActionResult> Index(int page = 1)
    {
        int totalNews = await _context.NewsItems.CountAsync();
        int totalPages = (int)Math.Ceiling(totalNews / (double)PageSize);

        var news = await _context.NewsItems
            .OrderByDescending(n => n.PubDate)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(news);
    }
}
