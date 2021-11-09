using DatingApi.Models;
using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static DatingApi.ViewModels.DatingAppChat;

namespace DatingApi.Controllers
{
    public class ChatController : ApiController
    {
        DatingEntities db = new DatingEntities();
        [HttpGet]
        public HttpResponseMessage GetChatList(string UserEmail)
        {
            try
            {
                UserChatViewModel MyChatList = new UserChatViewModel();
                var blockedUsers = db.BlockUsers.Where(x => x.BlockByEmail == UserEmail).Select(x => x.UserEmail).ToList();
                MyChatList.userChatList = db.Database.SqlQuery<UserChatList>("EXEC UserChatList @UserEmail='" + UserEmail + "'").OrderByDescending(x=>x.MessageDateTime).Where(x=> !blockedUsers.Contains(x.UserEmail)).ToList<UserChatList>();
                var LikeMe = db.UserLikes.Where(x => x.UserEmail == UserEmail).Select(x => new { x.LikeByUserEmail ,x.IsSeen}).ToList();
                var LikeThem= db.UserLikes.Where(x => x.LikeByUserEmail == UserEmail).Select(x => x.UserEmail).ToList();
                var commonEmails = LikeMe.Join(LikeThem,
                    x => x.LikeByUserEmail,
                    y => y,
                    (x, y) => new { x.LikeByUserEmail,x.IsSeen }).ToList();
                var emails = commonEmails.Select(X => X.LikeByUserEmail).ToList();
                var matches = db.DatingUsers.Where(x => emails.Contains(x.UserEmail)).ToList();
                MyChatList.userLikeMe = matches.Join(commonEmails,
                    m=>m.UserEmail,
                    c=>c.LikeByUserEmail,
                    (m,c)=> new UserLikeMe
                   {
                    UserEmail=m.UserEmail,
                    UserImage=m.UserImage,
                    UserName=m.UserName,
                    UserPhone=m.UserPhone,
                    IsSeen=c.IsSeen
                  }).ToList();
              //  MyChatList.userLikeMe = db.Database.SqlQuery<UserLikeMe>("EXEC UserLikeMe @UserEmail='" + UserEmail + "'").Where(x => !blockedUsers.Contains(x.UserEmail)).ToList<UserLikeMe>();
                MyChatList.NewLikes = 0;
                MyChatList.NewLikes = db.UserLikes.Where(x => x.UserEmail == UserEmail && x.IsSeen==false).Count();
                return Request.CreateResponse(HttpStatusCode.OK, MyChatList);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage GetMessageList(string UserEmail,string MyEmail)
        {
            try
            {
               
                var SmsList = db.UserMessages.Where(x=>(x.MessageFrom==MyEmail && x.MessageTo==UserEmail && x.IsDeletedFrom!=true) ||(x.MessageTo == MyEmail && x.MessageFrom == UserEmail && x.IsDeletedTo != true)).OrderBy(x=>x.MessageDateTime).ToList();

                var query = "Update UserMessage set IsSeen=1 where MessageFrom='"+UserEmail+"' and MessageTo='"+MyEmail+"' ";
                db.Database.ExecuteSqlCommand(query);
                db.SaveChanges();
               
                query = "Update UserLikes set IsSeen=1 where LikeByUserEmail='" + UserEmail + "' and UserEmail='" + MyEmail + "'";
                db.Database.ExecuteSqlCommand(query);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, SmsList);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage SendMessage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var keys = httpRequest.Form;
                string path = HttpContext.Current.Server.MapPath("~/ChatFiles/");
                UserMessage message = new UserMessage();
                message.IsDeletedFrom = false;
                message.IsDeletedTo = false;
                message.IsSeen = false;
                message.MessageFrom = keys["MessageFrom"];
                message.MessageTo= keys["MessageTo"];
                message.MessageType= keys["MessageType"];
                message.MessageContent= keys["MessageContent"];
                message.MessageDateTime= Convert.ToDateTime(keys["MessageDateTime"]);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (httpRequest.Files.Count > 0)
                {
                    var postedFile = httpRequest.Files[0];
                    var namefile = System.Guid.NewGuid() + "_" + DateTime.Now.ToString("mmss") + System.IO.Path.GetExtension(postedFile.FileName);
                    var filePath = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ChatFiles/"), namefile);
                    postedFile.SaveAs(filePath);
                    message.MessageContent = "ChatFiles/" + namefile;
                  
                }
                db.UserMessages.Add(message);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Sent");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
       
    }
}
