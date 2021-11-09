using DatingApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingApi.ViewModels
{
    public class DatingAppUser
    {
        public class Users
        {
            public List<FilterUser> filterUsers { get; set; }
            public List<UserPhoto> userPhots { get; set; }
            public int NewNotifications { get; set; }
        }
        public class UserDetail
        {
            public DatingUser user { get; set; }
            public List<UserPhoto> userPhots { get; set; }
           

        }
        public class UsersLikeMeAndTopPick
        {
            public List<TopPick> TopPicks { get; set; }
            public List<UserLikeMe> UsersLikeMe { get; set; }
            public int TotalTopPicks { get; set; }
            public int TotalLikes { get; set; }
        }
        public class UserLikeMe
        {
            public string UserEmail { get; set; }
            public string UserPhone { get; set; }
            public bool? IsActive { get; set; }
            public string UserImage { get; set; }
            public DateTime? UserLastSeen { get; set; }
        }
        public class TopPick
        {
            public string  UserEmail { get; set; }
            public string UserPhone { get; set; }
            public bool? IsActive { get; set; }
            public string UserImage { get; set; }
            public DateTime? UserLastSeen { get; set; }
        }
        public class FilterUser
        {
            public string UserEmail { get; set; }
            public string UserName { get; set; }
            public string UserPhone { get; set; }
            public string UserInterest { get; set; }
            public string AboutKarter { get; set; }
            public string LookingFor { get; set; }
            public string Height { get; set; }
            public int UserAge { get; set; }
            public string Profession { get; set; }
            public string UserGender { get; set; }
            public string UserSex { get; set; }
            public string IsLikeByMe { get; set; }
            public int IsSuperLikeByMe { get; set; }
            public string UserImage { get; set; }
            public string LocationName { get; set; }
            public double Distance { get; set; }
            public string DistanceUnit { get; set; }
            public DateTime? UserLastSeen { get; set; }
            public bool IsUserActive { get; set; }
        }

    }
}