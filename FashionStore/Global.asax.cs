using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using FashionStore.Data;

namespace FashionStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(System.Web.Optimization.BundleTable.Bundles);
            // Unity is initialized by UnityMvcActivator

            // Khởi tạo trước DbContext để tránh race-condition khi EF build model lần đầu
            // lỗi "The context cannot be used while the model is being created" trên web nhiều request song song
            using (var db = new ApplicationDbContext())
            {
                db.Database.Initialize(false);
            }
        }
    }
}
