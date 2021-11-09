using DatingApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DatingApi.Controllers
{
    public class SubscriptionController : ApiController
    {
        DatingEntities db = new DatingEntities();
        [HttpGet]
        public HttpResponseMessage GetUserSubscribe(string UserEmail,int month)
        {
            try
            {
                SubscriptionPlan plan = new SubscriptionPlan();
                plan.UserEmail = UserEmail;
                plan.PlanDescription = "Super Like,Boost Likes and See User Who Likes me is activated... ";
                plan.PlanMonths = month;
                plan.PlanStartDate = DateTime.Now;
                plan.IsPlanActive = true;
                if (month == 1)
                {
                    plan.PlanEndDate = plan.PlanStartDate.Value.AddMonths(1);
                    plan.PlanAmount = 10;
                }
                if (month == 3)
                {
                    plan.PlanAmount = 14;
                    plan.PlanEndDate = plan.PlanStartDate.Value.AddMonths(3);
                }
                if (month == 6)
                {
                    plan.PlanAmount = 20;
                    plan.PlanEndDate = plan.PlanStartDate.Value.AddMonths(6);
                }
                db.SubscriptionPlans.Add(plan);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Subscribed");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage GetUserUnSubscribe(string UserEmail)
        {
            try
            {
                var userSubscriptions = db.SubscriptionPlans.Where(x => x.UserEmail == UserEmail && x.IsPlanActive == true && DateTime.Now > x.PlanEndDate.Value).FirstOrDefault();
                 if (userSubscriptions!=null)
                    {
                        userSubscriptions.IsPlanActive = false;
                        db.Entry(userSubscriptions).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                
                return Request.CreateResponse(HttpStatusCode.OK, "UnSubscribed");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

    }
}
