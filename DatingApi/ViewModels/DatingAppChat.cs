using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingApi.ViewModels
{
    public class DatingAppChat
    {
        public class UserChatViewModel
        {
            public List<UserChatList> userChatList { get; set; }
            public List<UserLikeMe> userLikeMe { get; set; }
            public int NewLikes { get; set; }
        }
        public class UserChatList
        {
            public string UserEmail { get; set; }
            public string UserName { get; set; }
            public string UserPhone { get; set; }
            public string UserImage { get; set; }
            public DateTime? MessageDateTime { get; set; }
            public string MessageType { get; set; }
            public string MessageContent { get; set; }
            public int TotalUnseenMessages { get; set; }
            public bool IsUserActive { get; set; }
        }
        public class UserLikeMe
        {
            public string UserEmail { get; set; }
            public string UserName { get; set; }
            public string UserPhone { get; set; }
            public string UserImage { get; set; }
            public bool? IsSeen { get; set; }
        }
    }
}