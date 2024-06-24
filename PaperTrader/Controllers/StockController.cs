using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaperTrader.Data;
using PaperTrader.Models;
using Microsoft.AspNetCore.Identity;

namespace PaperTrader.Controllers
{
    public class StockController : Controller
    {
        private readonly PaperTraderContext _context;
        private readonly ILogger<StockController> _logger;

        public StockController(PaperTraderContext context, ILogger<StockController> logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}
