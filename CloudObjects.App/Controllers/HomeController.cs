using CloudObjects.App.Extensions;
using CloudObjects.App.ViewModels;
using CloudObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SqlServer.LocalDb;
using System;
using System.Threading.Tasks;
using CloudObjects.App.Data;

namespace CloudObjects.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly CloudObjectsDbContext _dbContext;
        private readonly IConfiguration _config;

        public HomeController(
            IConfiguration config,
            CloudObjectsDbContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var model = new HomeView()
            {
                DbServerName = GetDbServerName(),
                IsLocal = Request.IsLocal(),
                HasValidDb = true
            };

            if (model.IsLocal) model.HasValidDb = TryConnection();

            return View(model);

            bool TryConnection()
            {
                try
                {
                    using (var cn = _data.GetConnection())
                    {
                        cn.Open();
                        return true;
                    }
                }
                catch 
                {
                    return false;
                }
            }

            string GetDbServerName()
            {
                string connectionString = _config.GetConnectionString("Default");
                return ConnectionString.Token(connectionString, new string[] { "Server", "Data Source" });
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> CreateLocalDb()
        {
            var result = new CreateLocalDbView();

            try
            {
                await using var cn = LocalDb.GetConnection("CloudObjects");
                await DataModel.CreateTablesAsync(new Type[]
                {
                        typeof(Account),
                        typeof(StoredObject),
                        typeof(Activity)
                }, cn);

                result.Success = true;
                result.Message = "Database created successfully.";
            }
            catch (Exception exc)
            {
                result.Success = false;
                result.Message = exc.Message;
            }

            return View(result);
        }
    }
}
