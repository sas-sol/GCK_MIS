//using System.Collections.Generic;
//using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.Identity;
//namespace RepositoryManagmentSystemDB
//{
//    [Authorize]
//    public class ChatHub : Hub
//    {
//        private static List<string> OnlineUsers = new List<string>();
//        public override System.Threading.Tasks.Task OnConnected()
//        {
//            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
//            OnlineUsers.Add(Context.User.Identity.GetUserId());
//            context.Clients.Users(OnlineUsers).updateOnline();
//            return base.OnConnected();
//        }
//        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
//        {
//            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
//            OnlineUsers.Remove(Context.User.Identity.GetUserId());
//            context.Clients.Users(OnlineUsers).updateOnline();
//            return base.OnDisconnected(stopCalled);
//        }
//        public static void Messenger(string toId, string message, string fromId)
//        {
//            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
//            var data = new
//            {
//                Message = message,
//                From = fromId
//            };
//            context.Clients.User(toId).incomingMessage(data);
//        }
//        public static List<string> GetOnlineUsers()
//        {
//            return OnlineUsers;
//        }
//        public static void UpdateThreadStatus(long id, string msg, string from)
//        {
//            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
//            var data = new
//            {
//                Message = msg,
//                id = from
//            };
//            context.Clients.User(id.ToString()).updateStatus(data);
//        }
//    }
//}