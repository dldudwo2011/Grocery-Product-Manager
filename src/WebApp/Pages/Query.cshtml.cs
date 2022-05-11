using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Helper;
using Backend.BLL;
using Backend.Models.Queries;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages
{
    public class QueryModel : PageModel
    {
        #region Constructor and Dependencies
        private readonly GroceryListService _service;
        public QueryModel(GroceryListService service)
        {
            _service = service;
        }
        #endregion

        private const int PAGE_SIZE = 10;
        public Paginator Paging { get; set; }
        public int TotalCount { get; set; }

        public List<SelectListItem> categories { get; set; }

        public int selectedCategory { get; set; }

        public IEnumerable<Product> Products { get; set; }

        public void OnGet(int? currentPage, int selectedCategory)
        {
            PopulateDropDown();

            int pageNumber = currentPage.HasValue ? currentPage.Value : 1;
            PageState current = new(pageNumber, PAGE_SIZE);
            int total;
            

            if (selectedCategory != 0)
            {
                this.selectedCategory = selectedCategory;
                Products = _service.ListProducts(selectedCategory, pageNumber, PAGE_SIZE, out total);
                TotalCount = total;
                Paging = new(TotalCount, current);
            }

            else
            {
                Products = _service.ListProducts(pageNumber, PAGE_SIZE, out total);
                TotalCount = total;
                Paging = new(TotalCount, current);
            }
        }

        public void PopulateDropDown()
        {
            categories = _service.ListCategories().Select(x => new SelectListItem(x.Description, x.CategoryID.ToString())).ToList();
        }
    }
}
