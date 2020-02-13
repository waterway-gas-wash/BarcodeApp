using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BarcodeApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public string First
        {
            get; set;
        }

        public string Last
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public string expirationDate
        {
            get; set;
        }

        public string passedURL
        {
            get; set;
        }

        public void OnGet()
        {
            First = "";
            Last = "";
            Email = "";
            passedURL = "";
        }

        public Microsoft.AspNetCore.Mvc.RedirectResult OnPost()
        {
            First = Request.Form[nameof(First)];
            Last = Request.Form[nameof(Last)];
            Email = Request.Form[nameof(Email)];
            (string result, string passedurl) = BarcodeApp.EmailBarcode.BarcodeEmail(First, Last, Email);

            passedURL = passedurl;
            expirationDate = DateTime.Now.AddDays(14).ToString("MM/dd/yyyy");

            string url = result + "?url=" + passedURL + "&expDate=" + expirationDate;
            return Redirect(url);
        }

    }
}
