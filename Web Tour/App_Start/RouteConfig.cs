using System.Web.Mvc;
using System.Web.Routing;

namespace Web_Tour
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Home",
                url: "Home",
                defaults: new { controller = "User", action = "Home" }
            );

            routes.MapRoute(
                name: "About",
                url: "About",
                defaults: new { controller = "User", action = "About" }
            );

            routes.MapRoute(
                name: "Tours",
                url: "Tours",
                defaults: new { controller = "User", action = "Tours" }
            );

            routes.MapRoute(
                name: "News",
                url: "News",
                defaults: new { controller = "User", action = "News" }
            );

            routes.MapRoute(
                name: "Contact",
                url: "Contact",
                defaults: new { controller = "User", action = "Contact" }
            );

            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { controller = "Auth", action = "Login" }
            );

            routes.MapRoute(
                name: "SignUp",
                url: "SignUp",
                defaults: new { controller = "Auth", action = "SignUp" }
            );

            routes.MapRoute(
                name: "LogOut",
                url: "LogOut",
                defaults: new { controller = "Auth", action = "LogOut" }
            );

            routes.MapRoute(
                name: "ProfileInfo",
                url: "ProfileInfo",
                defaults: new { controller = "Auth", action = "ProfileInfo" }
            );

            routes.MapRoute(
                name: "Details",
                url: "Details",
                defaults: new { controller = "Tour", action = "Details" }
            );

            routes.MapRoute(
                name: "Booking",
                url: "Booking",
                defaults: new { controller = "Tour", action = "Booking" }
            );

            routes.MapRoute(
                name: "BookingWaitingPayment",
                url: "BookingWaitingPayment",
                defaults: new { controller = "Tour", action = "BookingWaitingPayment" }
            );

            routes.MapRoute(
                name: "Admin",
                url: "Admin",
                defaults: new { controller = "Admin", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "User", action = "Home", id = UrlParameter.Optional }
            );
        }
    }
}
