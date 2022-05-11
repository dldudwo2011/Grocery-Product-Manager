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
using System.IO;
using Microsoft.AspNetCore.Http;

namespace WebApp.Pages
{
    public class CRUDModel : PageModel
    {
        #region Constructor and Dependencies
        private readonly GroceryListService _service;
        public CRUDModel(GroceryListService service)
        {
            _service = service;
        }
        #endregion

        [BindProperty] // This attribute allows our Razor Page to "assign" the form item to this property
        public IFormFile photo { get; set; }

        [BindProperty]
        public Product product { get; set; }

        public List<SelectListItem> categories { get; set; }

        public bool redirected { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet(int productId)
        {
            if (productId != 0) product = _service.LookupProduct(productId);
     
            PopulateDropDown();
            
        }

        //this is the handler made specially for redirect so the user can continue adding
        public void OnGetRedirected(int productId)
        {
            if (productId != 0)
            {
                product = _service.LookupProduct(productId);

                //"redirected" bool variable is set to true so it could be recognized in the cshtml file
                redirected = true;
            }

            PopulateDropDown();
        }
     

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("Query");
        }

        public IActionResult OnPostAdd()
        {
            if (photo != null && photo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    photo.CopyTo(ms);
                    var fileBytes = ms.ToArray();

                    product.Photo = fileBytes;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _service.AddProduct(product);

                    return RedirectToPage("CRUD", new { productID = product.ProductId });
                }

                catch (Exception ex)
                {
                    Exception rootCause = ex;

                    while (rootCause.InnerException != null)
                        rootCause = rootCause.InnerException;

                    ErrorMessage = rootCause.Message;
                    PopulateDropDown();

                    return Page();
                }
            }
 
            PopulateDropDown();
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (photo != null &&  photo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    photo.CopyTo(ms);
                    var fileBytes = ms.ToArray();

                    product.Photo = fileBytes;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _service.UpdateProduct(product.ProductId, product);

                    return RedirectToPage("CRUD", new { productID = product.ProductId });
                }

                catch (Exception ex)
                {
                    Exception rootCause = ex;

                    while (rootCause.InnerException != null)
                        rootCause = rootCause.InnerException;

                    ErrorMessage = rootCause.Message;
                    PopulateDropDown();

                    return Page();
                }
            }

            PopulateDropDown();
            return Page();
        }


        public IActionResult OnPostDelete()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _service.DeleteProduct(product.ProductId);
                    return RedirectToPage("CRUD", new { productID = product.ProductId });
                }

                catch (Exception ex)
                {
                    Exception rootCause = ex;

                    while (rootCause.InnerException != null)
                        rootCause = rootCause.InnerException;

                    ErrorMessage = rootCause.Message;
                    PopulateDropDown();

                    return Page();
                }
            }

            PopulateDropDown();
            return Page();
        }

        public void PopulateDropDown()
        {
            categories = _service.ListCategories().Select(x => new SelectListItem(x.Description, x.CategoryID.ToString())).ToList();
        }
    }
}
