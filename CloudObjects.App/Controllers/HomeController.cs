using CloudObjects.App.Extensions;
using CloudObjects.App.ViewModels;
using CloudObjects.Models;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ModelSync.Models;
using SqlServer.LocalDb;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudObjects.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly DapperCX<long> _data;
        private readonly IConfiguration _config;

        public HomeController(
            IConfiguration config,
            DapperCX<long> data)
        {
            _config = config;
            _data = data;
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
                string connectionString = _config.TryConnections("LiveConnection", "Default");
                return Parse.Token(connectionString, new string[] { "Server", "Data Source" });
            }
        }

        public async Task<IActionResult> CreateLocalDb()
        {
            var result = new CreateLocalDbView();

            try
            {
                using (var cn = LocalDb.GetConnection("CloudObjects"))
                {
                    await DataModel.CreateTablesAsync(new Type[]
                    {
                        typeof(Account),
                        typeof(StoredObject),
                        typeof(Activity)
                    }, cn);

                    result.Success = true;
                    result.Message = "Database created successfully.";
                }                
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
