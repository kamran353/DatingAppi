using DatingApi.Models;
using DatingApi.ViewModels;
using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using static DatingApi.ViewModels.DatingAppUser;

namespace DatingApi.Controllers
{
    public class UserController : ApiController
    {
        DatingEntities db = new DatingEntities();
        [HttpPost]
        public HttpResponseMessage SignUp()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var keys = httpRequest.Form;
               
                var UserEmail = keys["UserEmail"];
                var UserPhone = keys["UserPhone"];
                var isUserExist = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (isUserExist != null)
                {
                  return  Request.CreateResponse(HttpStatusCode.NotFound,"Email Already Exists");
                }
                var isPhoneExist = db.DatingUsers.FirstOrDefault(x => x.UserPhone == UserPhone);
                if (isPhoneExist != null)
                {
                  return  Request.CreateResponse(HttpStatusCode.NotFound, "Phone number Already Exists");
                }
                DatingUser Datinguser = new DatingUser();
                   
               Datinguser.UserEmail = keys["UserEmail"];
               Datinguser.UserPassword = keys["UserPassword"];
               Datinguser.UserGender = keys["UserGender"];
               Datinguser.UserSex = keys["UserSex"];
               Datinguser.DOB = Convert.ToDateTime(keys["DOB"]);
               Datinguser.UserName = keys["UserName"];
               Datinguser.UserPhone = keys["UserPhone"];
               Datinguser.IsUserActive = false;
               Datinguser.UserInterest = "Cats,Dogs etc";
               Datinguser.LookingFor = "Friendship";
               Datinguser.AboutKarter = "I am a manager";
               Datinguser.Profession = "Art Manager";
               Datinguser.Height = "121cm";
               Datinguser.RecommendSort = true;
               Datinguser.ShareMyFeed = false;
               Datinguser.ShowMeInTopPick = false;
               Datinguser.UserActvieStatus = false;
               Datinguser.IsSuperLikeSubscribed = false;
               Datinguser.UserDistanceUnit = "Km";
               Datinguser.UserImage = "null";
               Datinguser.UserMaxDistance = 100;
               Datinguser.LocationName = "Nigeria";
               Datinguser.UserMartialStatus = "Single";
               Datinguser.LocationLatitude = 9.0820;
               Datinguser.LocationLongititude = 8.6753;
               Datinguser.UserAge = (int)DateTime.Now.Subtract(Datinguser.DOB.Value).TotalDays / 365;
               db.DatingUsers.Add(Datinguser);
               db.SaveChanges();
               CreateUserFilter(Datinguser);
                SendWelcomeNotification(Datinguser.UserEmail);
               //SendVerificationLinkEmail(UserEmail);
               return Request.CreateResponse(HttpStatusCode.OK, Datinguser);
              
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        private void SendWelcomeNotification(string UserEmail)
        {
            UserNotification notification = new UserNotification();
            notification.NotificationDateTime = DateTime.Now;
            notification.UserEmail = UserEmail;
            notification.IsSeen = false;
            notification.NotificationDescription = "Welcome";
            notification.NotificationByEmail = "IMateApp@gmail.com";

            db.UserNotifications.Add(notification);
            db.SaveChanges();

            
        }

        private void CreateUserFilter(DatingUser user)
        {
            UserFilter newFilter = new UserFilter();
            newFilter.LocationName = user.LocationName;
            newFilter.LocationLatitude = user.LocationLatitude;
            newFilter.LocationLongititude = user.LocationLongititude;
            newFilter.Gender = user.UserGender;
            newFilter.Sex = user.UserSex;
            newFilter.IsViewStyleClassic = true;
            newFilter.UserInterest = user.UserInterest;
            newFilter.AgeFrom = user.UserAge - 5;
            newFilter.AgeTo = user.UserAge + 5;
            newFilter.Distance = user.UserMaxDistance;
            newFilter.UserEmail = user.UserEmail;
            db.UserFilters.Add(newFilter);
            db.SaveChanges();
        }

        [HttpGet]
        public HttpResponseMessage GetUserFilter(string UserEmail)
        {
            try
            {
                var UserFilter = db.UserFilters.FirstOrDefault(x => x.UserEmail == UserEmail);
                
                return Request.CreateResponse(HttpStatusCode.OK, UserFilter);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage LoginUser(string UserPhone,string UserPassword)
        {
            try
            {
                var isEmailExist = db.DatingUsers.FirstOrDefault(x => x.UserPhone == UserPhone && x.UserPassword==UserPassword);
                if (isEmailExist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, isEmailExist);
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, "No User");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage IsEmailExist(string UserEmail)
        {
            try
            {
                var isEmailExist = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (isEmailExist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Email Already Exists");
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, "No User");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage IsPhoneExist(string UserPhone)
        {
            try
            {
                var isPhoneExist = db.DatingUsers.FirstOrDefault(x => x.UserPhone == UserPhone);
                if (isPhoneExist != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, isPhoneExist);
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, "No User");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage AddUserPhotos()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var keys = httpRequest.Form;
                var UserEmail = keys["UserEmail"];
                string path = HttpContext.Current.Server.MapPath("~/UserImages/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                for(int i=0;i< httpRequest.Files.Count; i++)
                {
                    UserPhoto photo = new UserPhoto();
                    var postedFile = httpRequest.Files[i];
                    var namefile = System.Guid.NewGuid() + "_" + DateTime.Now.ToString("mmss") + System.IO.Path.GetExtension(postedFile.FileName);
                    var filePath = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/UserImages/"), namefile);
                    photo.UserEmail = UserEmail;
                    photo.Photo="UserImages/" + namefile;
                    photo.PhotoLikes = 0;
                    photo.CreatedDate = DateTime.Now;
                    postedFile.SaveAs(filePath);
                    db.UserPhotos.Add(photo);
                    db.SaveChanges();

                }

              /*  var userPhoto = db.UserPhotos.FirstOrDefault(x=>x.UserEmail==UserEmail);
                if (userPhoto != null)
                {
                    var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                    if (user != null)
                    {
                        user.UserImage = userPhoto.Photo;
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                   
                }*/
                var photos = db.UserPhotos.Where(x => x.UserEmail == UserEmail).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, photos);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage DeleteUserPhoto(string UserEmail,int PhotoId)
        {
            try
            {
                var photo = db.UserPhotos.FirstOrDefault(x => x.PhotoId == PhotoId);
                if (photo != null)
                {
                    db.Entry(photo).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                var userPhoto = db.UserPhotos.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (userPhoto != null)
                {
                    var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                    if (user != null)
                    {
                        user.UserImage = userPhoto.Photo;
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                var photos = db.UserPhotos.Where(x => x.UserEmail == UserEmail).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, photos);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpGet]
        public HttpResponseMessage LikeUser(string UserEmail,string LikeByUserEmail)
        {
            try
            {
                var likes = db.UserLikes.FirstOrDefault(x => x.UserEmail == UserEmail && x.LikeByUserEmail == LikeByUserEmail && x.IsSuperLike==false) ;
                if (likes == null)
                {
                    UserLike like = new UserLike();
                    like.UserEmail = UserEmail;
                    like.LikeByUserEmail = LikeByUserEmail;
                    like.LikeDate = DateTime.Now;
                    like.IsDeleted = false;
                    like.IsSuperLike = false;
                    like.IsSeen = false;
                    db.UserLikes.Add(like);
                    db.SaveChanges();
                }
                
                   UserNotification notification = new UserNotification();
                    var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == LikeByUserEmail);
                    notification.NotificationDateTime = DateTime.Now;
                    notification.NotificationDescription = "Like";
                    notification.UserEmail = UserEmail;
                    notification.NotificationByEmail = LikeByUserEmail;
                    notification.IsSeen = false;
                    db.UserNotifications.Add(notification);
                    db.SaveChanges();

                    var isLikeByHim = db.UserLikes.FirstOrDefault(x=>x.IsSuperLike==false && x.LikeByUserEmail==UserEmail && x.UserEmail==LikeByUserEmail);
                    if (isLikeByHim != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Match");
                    }

                
               
                return Request.CreateResponse(HttpStatusCode.OK, "Liked");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage SuperLikeUser(string UserEmail, string LikeByUserEmail)
        {
            try
            {
                var likes = db.UserLikes.FirstOrDefault(x => x.UserEmail == UserEmail && x.LikeByUserEmail == LikeByUserEmail && x.IsSuperLike==true);
                if (likes == null)
                {
                    UserLike like = new UserLike();
                    like.UserEmail = UserEmail;
                    like.LikeByUserEmail = LikeByUserEmail;
                    like.LikeDate = DateTime.Now;
                    like.IsDeleted = false;
                    like.IsSuperLike = true;
                    like.IsSeen = false;
                    db.UserLikes.Add(like);
                    db.SaveChanges();
                }
               
                UserNotification notification = new UserNotification();
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == LikeByUserEmail);
                notification.NotificationDateTime = DateTime.Now;
                notification.NotificationDescription = "Super Like";
                notification.UserEmail = UserEmail;
                notification.NotificationByEmail = LikeByUserEmail;
                notification.IsSeen = false;
                db.UserNotifications.Add(notification);
                db.SaveChanges();


                

                return Request.CreateResponse(HttpStatusCode.OK, "Liked");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage SuperDisLikeUser(string UserEmail, string LikeByUserEmail)
        {
            try
            {
                var like = db.UserLikes.FirstOrDefault(x => x.UserEmail == UserEmail && x.LikeByUserEmail == LikeByUserEmail && x.IsSuperLike == true);
                if (like != null)
                {
                    db.Entry(like).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();

                    UserNotification notification = new UserNotification();
                    var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == LikeByUserEmail);
                    notification.NotificationDateTime = DateTime.Now;
                    notification.NotificationDescription = "Super DisLike";
                    notification.UserEmail = UserEmail;
                    notification.NotificationByEmail = LikeByUserEmail;
                    notification.IsSeen = false;
                    db.UserNotifications.Add(notification);
                    db.SaveChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK, "UnLiked");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage DisLikeUser(string UserEmail, string LikeByUserEmail)
        {
            try
            {
                var like = db.UserLikes.FirstOrDefault(x => x.UserEmail == UserEmail && x.LikeByUserEmail == LikeByUserEmail && x.IsSuperLike==false);
                if (like != null)
                {
                    db.Entry(like).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();

                    UserNotification notification = new UserNotification();
                    var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == LikeByUserEmail);
                    notification.NotificationDateTime = DateTime.Now;
                    notification.NotificationDescription = "DisLike";
                    notification.UserEmail = UserEmail;
                    notification.NotificationByEmail = LikeByUserEmail;
                    notification.IsSeen = false;
                    db.UserNotifications.Add(notification);
                    db.SaveChanges();
                }
                
                return Request.CreateResponse(HttpStatusCode.OK, "UnLiked");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage GetUserNotifications(string UserEmail)
        {
            try
            {
                //var users = db.DatingUsers.ToList();

                List<UserNotification> list = new List<UserNotification>();
                var Welcomenotification = db.UserNotifications.FirstOrDefault(x=>x.UserEmail==UserEmail && x.NotificationDescription=="Welcome");
                if (Welcomenotification != null)
                {
                    Welcomenotification.NotificationDescription = "Welcome to iMate! Ready to find your match? ";

                    list.Add(Welcomenotification);
                }

                var userslikes = db.UserNotifications.Where(x => x.UserEmail == UserEmail && x.NotificationDescription == "Like").Count();
                var isLikeSeen = db.UserNotifications.Where(x => x.UserEmail == UserEmail && x.NotificationDescription == "Like" && x.IsSeen == false).Count();
                if (userslikes > 0)
                {
                    UserNotification notification = new UserNotification();
                    notification.NotificationDateTime = DateTime.Now;
                    notification.UserEmail = UserEmail;
                    notification.IsSeen = true;
                    if (isLikeSeen > 0)
                    {
                        notification.IsSeen = false;
                    }

                    notification.NotificationDescription = userslikes + " People Like you See Who they are iMate Plus";
                    notification.NotificationByEmail = "IMateApp@gmail.com";
                    list.Add(notification);
                }
                

                


                var query = "Update UserNotification set IsSeen=1 where UserEmail='" + UserEmail + "'";
                db.Database.ExecuteSqlCommand(query);
                //db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, list.OrderByDescending(x=>x.NotificationDateTime));

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage FilterUsers(string UserEmail)
        {
            try
            {
                var blockedUsers = db.BlockUsers.Where(x => x.BlockByEmail == UserEmail).Select(x => x.UserEmail).ToList();
                Users appusers = new Users();
                appusers.filterUsers= db.Database.SqlQuery<FilterUser>("EXEC GetUsers @UserEmail='" + UserEmail+"'").Where(x=>!blockedUsers.Contains(x.UserEmail)).ToList<FilterUser>();
                var uniqueUsers = appusers.filterUsers.GroupBy(x => x.UserEmail).ToList();
                List<FilterUser> fusers = new List<FilterUser>();
                foreach (var item in uniqueUsers)
                {
                    fusers.Add(item.FirstOrDefault());
                }
                appusers.filterUsers = fusers;
                var UserMails = appusers.filterUsers.GroupBy(x => x.UserEmail).Select(x => x.Key).ToList();
                appusers.userPhots = db.UserPhotos.Where(x => UserMails.Contains(x.UserEmail)).ToList();
                appusers.NewNotifications = 0;
                appusers.NewNotifications = db.UserNotifications.Where(x => x.UserEmail == UserEmail && x.IsSeen==false).Count();
                return Request.CreateResponse(HttpStatusCode.OK, appusers);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage ActiveStatus(string UserEmail)
        {
            try
            {
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail==UserEmail);
                user.IsUserActive = true;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
               return Request.CreateResponse(HttpStatusCode.OK, user);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage DeActiveStatus(string UserEmail)
        {
            try
            {
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                user.IsUserActive = false;
                user.UserLastSeen = DateTime.Now;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, user);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpPost]
        public HttpResponseMessage EditProfile()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var keys = httpRequest.Form;
                var UserEmail = keys["UserEmail"];
                var Datinguser = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                Datinguser.UserGender = keys["UserGender"];
                Datinguser.UserAge = int.Parse(keys["UserAge"]);
                Datinguser.UserInterest = keys["UserInterest"];
                Datinguser.UserMartialStatus = keys["UserMartialStatus"];
                Datinguser.Height = keys["Height"];
                Datinguser.LookingFor = keys["LookingFor"];
                Datinguser.AboutKarter = keys["AboutKarter"];
                Datinguser.Profession = keys["Profession"];
                Datinguser.LocationName = keys["LocationName"]; 
                Datinguser.LocationLatitude = double.Parse(keys["LocationLatitude"]);
                Datinguser.LocationLongititude = double.Parse(keys["LocationLongititude"]);
                db.Entry(Datinguser).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
              
                return Request.CreateResponse(HttpStatusCode.OK, Datinguser);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpPost]
        public HttpResponseMessage UpdateUserFilter()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var keys = httpRequest.Form;
                var UserEmail = keys["UserEmail"];
                var newFilter = db.UserFilters.FirstOrDefault(x => x.UserEmail == UserEmail);
                newFilter.LocationName = keys["LocationName"];
                newFilter.LocationLatitude = double.Parse(keys["LocationLatitude"]);
                newFilter.LocationLongititude = double.Parse(keys["LocationLongititude"]);
                newFilter.Gender = keys["Gender"];
                newFilter.Sex = keys["Sex"];
                newFilter.IsViewStyleClassic = bool.Parse(keys["IsViewStyleClassic"]);
                newFilter.UserInterest = keys["UserInterest"];
                newFilter.AgeFrom = int.Parse(keys["AgeFrom"]);
                newFilter.AgeTo = int.Parse(keys["AgeTo"]);
                newFilter.Distance = double.Parse(keys["Distance"]);
                db.Entry(newFilter).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, newFilter);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage UpdateUserSetting()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var keys = httpRequest.Form;
                var UserEmail = keys["UserEmail"];
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                user.ShareMyFeed = bool.Parse(keys["ShareMyFeed"]);
                user.RecommendSort = bool.Parse(keys["RecommendSort"]);
                user.ShowMeInTopPick = bool.Parse(keys["ShowMeInTopPick"]);
                user.UserActvieStatus = bool.Parse(keys["UserActvieStatus"]);
                user.UserDistanceUnit = keys["UserDistanceUnit"];

                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, user);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage UpdateUserProfile()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var keys = httpRequest.Form;
                var UserEmail = keys["UserEmail"];
                var UserImage = keys["UserImage"];
                var isUserExist = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (isUserExist != null)
                {
                    isUserExist.UserImage = UserImage;
                    db.Entry(isUserExist).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
               
               

                return Request.CreateResponse(HttpStatusCode.OK, isUserExist);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage UpdateUserShowMe(string UserEmail, string Gender,string Sex)
        {
            try
            {
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (user != null)
                {
                    user.UserGender = Gender;
                    user.UserSex = Sex;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage UpdateUserEmail(string UserEmail,string NewEmail)
        {
            try
            {
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (user != null)
                {
                    user.UserEmail = NewEmail;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage UpdateUserName(string UserEmail, string UserName)
        {
            try
            {
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (user != null)
                {
                    user.UserName = UserName;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage UpdateUserPhone(string UserEmail, string UserPhone)
        {
            try
            {
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (user != null)
                {
                    user.UserPhone = UserPhone;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage UpdateUserActiveStatus(string UserEmail, bool ActieStatus)
        {
            try
            {
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (user != null)
                {
                    user.UserActvieStatus = ActieStatus;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage UpdateUserRecommendSort(string UserEmail, bool RecomendSort)
        {
            try
            {
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (user != null)
                {
                    user.RecommendSort = RecomendSort;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage UpdateUserShareFeed(string UserEmail, bool ShareFeed)
        {
            try
            {
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (user != null)
                {
                    user.ShareMyFeed = ShareFeed;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage UpdateUserTopPick(string UserEmail, bool TopPick)
        {
            try
            {
                var user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                if (user != null)
                {
                    user.ShowMeInTopPick = TopPick;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage GetUser(string UserEmail)
        {
            try
            {
                UserDetail details = new UserDetail();

                details.user = db.DatingUsers.FirstOrDefault(x => x.UserEmail == UserEmail);
                details.userPhots = db.UserPhotos.Where(x => x.UserEmail == UserEmail).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, details);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage GetUserPhotos(string UserEmail)
        {
            try
            {
                var photos = db.UserPhotos.Where(x => x.UserEmail == UserEmail);

                return Request.CreateResponse(HttpStatusCode.OK, photos);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage UserLikeMeAndTopPicks(string UserEmail)
        {
            try
            {
                var blockedUsers = db.BlockUsers.Where(x=>x.BlockByEmail==UserEmail).Select(x=>x.UserEmail).ToList();
                UsersLikeMeAndTopPick usersLikeMeAndTopPick = new UsersLikeMeAndTopPick();
                var UserLikeMeEmails = db.UserLikes.Where(x => x.UserEmail == UserEmail && x.IsSuperLike==false).Select(x => x.LikeByUserEmail);
                usersLikeMeAndTopPick.UsersLikeMe = db.DatingUsers.Where(x => UserLikeMeEmails.Contains(x.UserEmail) && !blockedUsers.Contains(x.UserEmail)).Select(
                    x=> new UserLikeMe { UserEmail=x.UserEmail,UserPhone=x.UserPhone,IsActive=(bool)x.IsUserActive,UserImage=x.UserImage,UserLastSeen=(DateTime)x.UserLastSeen}).ToList();

                var UserTopickMeEmails = db.UserLikes.Where(x => x.UserEmail == UserEmail && x.IsSuperLike == true).Select(x => x.UserEmail);
                usersLikeMeAndTopPick.TopPicks = db.DatingUsers.Where(x => UserLikeMeEmails.Contains(x.UserEmail) && !blockedUsers.Contains(x.UserEmail) && x.ShowMeInTopPick==true).Select(
                    x => new TopPick { UserEmail = x.UserEmail, UserPhone = x.UserPhone, IsActive = (bool)x.IsUserActive, UserImage = x.UserImage, UserLastSeen = (DateTime)x.UserLastSeen }).ToList();

                
                usersLikeMeAndTopPick.TotalLikes = usersLikeMeAndTopPick.UsersLikeMe.Count;
                usersLikeMeAndTopPick.TotalTopPicks = usersLikeMeAndTopPick.TopPicks.Count;

                string query = "Update UserLikes set IsSeen=1 where UserEmail='" + UserEmail + "'";
                db.Database.ExecuteSqlCommand(query);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, usersLikeMeAndTopPick);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage SendOTP(string UserPhone)
        {
            try
            {
                UserPhone = "+" + UserPhone;
                Random r = new Random();
                int num = r.Next(1001, 9999);
                var results = SMS.Send(new SMS.SMSRequest
                {
                    from = "+447312263051",
                    to = UserPhone,
                    text = "Your iMate confirmation code is "+num+" Don’t share"
                });
               
                return Request.CreateResponse(HttpStatusCode.OK, num);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpGet]
        public HttpResponseMessage Rewind(string UserEmail)
        {
            try
            {
                var Today = DateTime.Now;
                var todayLikes = db.UserLikes.Where(x => x.LikeByUserEmail == UserEmail && x.LikeDate.Value.Year == Today.Year && x.LikeDate.Value.Month == Today.Month && x.LikeDate.Value.Day == Today.Day).ToList();
                foreach(var item in todayLikes)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                Users appusers = new Users();
                appusers.filterUsers = db.Database.SqlQuery<FilterUser>("EXEC GetUsers @UserEmail='" + UserEmail + "'").ToList<FilterUser>();
                var UserMails = appusers.filterUsers.GroupBy(x => x.UserEmail).Select(x => x.Key).ToList();
                appusers.userPhots = db.UserPhotos.Where(x => UserMails.Contains(x.UserEmail)).ToList();
                appusers.NewNotifications = 0;
                appusers.NewNotifications = db.UserNotifications.Where(x => x.UserEmail == UserEmail && x.IsSeen == false).Count();
                return Request.CreateResponse(HttpStatusCode.OK, appusers);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpGet]
        public HttpResponseMessage BlockUser(string UserEmail,string BlockByEmail)
        {
            try
            {
                BlockUser blockUser = new BlockUser();
                blockUser.UserEmail = UserEmail;
                blockUser.BlockByEmail = BlockByEmail;
                db.BlockUsers.Add(blockUser);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, blockUser);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
