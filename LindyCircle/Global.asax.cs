﻿using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace LindyCircle
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Add Routes
            RegisterCustomRoutes(RouteTable.Routes);
        }

        void RegisterCustomRoutes(RouteCollection routes) {
            routes.MapPageRoute("HomeRoute", "", "~/Default.aspx");
            routes.MapPageRoute("MembersRoute", "members", "~/Pages/members.aspx");
            routes.MapPageRoute("PracticesRoute", "practices", "~/Pages/practices.aspx");
            routes.MapPageRoute("PunchCardsRoute", "punchcards", "~/Pages/punchcards.aspx");
            routes.MapPageRoute("AttendancesRoute", "attendance", "~/Pages/attendance.aspx");
            routes.MapPageRoute("HistoryRoute", "history", "~/Pages/history.aspx");
            routes.MapPageRoute("FinancesRoute", "finances", "~/Pages/finances.aspx");
            routes.MapPageRoute("MemberRoute", "member/{memberID}", "~/Pages/member.aspx");
            routes.MapPageRoute("PracticeRoute", "practice/{practiceID}", "~/Pages/practice.aspx");
        }
    }
}