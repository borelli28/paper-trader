using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace PaperTrader.Controllers;

public class PaperTraderController : Controller
{
    // Each public controller is an endpoint
    // GET: /PaperTrader/
    public string Index()
    {
        return "Hello";
    }
}