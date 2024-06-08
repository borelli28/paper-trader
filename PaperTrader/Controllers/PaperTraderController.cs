using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace MvcMovie.Controllers;

public class PaperTraderController : Controller
{
    // 
    // GET: /HelloWorld/
    public string Index()
    {
        return "Hello";
    }
}