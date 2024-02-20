using BethanysPieShopAdmin.Models;
using BethanysPieShopAdmin.Repositories;
using BethanysPieShopAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BethanysPieShopAdmin.Controllers;

public class PieController : Controller
{
    private readonly IPieRepository _pieRepository;
    private readonly ICategoryRepository _categoryRepository;

    public PieController(IPieRepository pieRepository, ICategoryRepository categoryRepository)
    {
        _pieRepository = pieRepository;
        _categoryRepository = categoryRepository;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var pies = await _pieRepository.GetAllPiesAsync();
        return View(pies);
    }

    public async Task<IActionResult> Details(int id)
    {
        var pie = await _pieRepository.GetPieByIdAsync(id);

        return View(pie);
    }

    // ADD
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var allCategories = await _categoryRepository.GetAllCategories();
        IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories, "CategoryId", "Name", null);
        PieAddViewModel pieAddViewModel = new() { Categories = selectListItems };
        return View((pieAddViewModel));
    }
    
    [HttpPost]
    public async Task<IActionResult> Add(PieAddViewModel pieAddViewModel)
    {
        if (ModelState.IsValid)
        {
            Pie pie = new()
            {
                Name = pieAddViewModel?.Pie?.Name ?? "",
                Price = pieAddViewModel?.Pie?.Price ?? 0,
                ShortDescription = pieAddViewModel?.Pie?.ShortDescription,
                LongDescription = pieAddViewModel?.Pie?.LongDescription,
                CategoryId = pieAddViewModel?.Pie?.CategoryId ?? 0,
            };
            
            await _pieRepository.AddPieAsync(pieAddViewModel.Pie);
            return RedirectToAction("Index");
        }
        return View(pieAddViewModel);
    }
}