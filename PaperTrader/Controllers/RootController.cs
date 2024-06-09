using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace PaperTrader.Controllers;

public class RootController : Controller
{
    // Each public controller is an endpoint
    // GET: /
    public string Index()
    {
        return "Hello";
    }
}