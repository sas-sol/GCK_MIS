//using Microsoft.AspNet.SignalR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Collections.Concurrent;
//using System.Threading.Tasks;


//namespace MeherEstateDevelopers
//{
//    [Authorize]
//    public class User
//    {
//        public string Name { get; set; }
//        public HashSet<string> ConnectionIds { get; set; }
//    }
//    public class AuthHub : Hub
//    {
//        private static readonly ConcurrentDictionary<string, User> ActiveUsers =
//          new ConcurrentDictionary<string, User>(StringComparer.InvariantCultureIgnoreCase);
//        public IEnumerable<string> GetConnectedUsers()
//        {
//            return ActiveUsers.Select(x => x.Key);
//        }

//        public override Task OnConnected()
//        {
//            string userName = Context.User.Identity.Name;
//            string connectionId = Context.ConnectionId;

//            var user = ActiveUsers.GetOrAdd(userName, _ => new User
//            {
//                Name = userName,
//                ConnectionIds = new HashSet<string>()
//            });

//            lock (user.ConnectionIds)
//            {

//                user.ConnectionIds.Add(connectionId);

//            }

//            return base.OnConnected();
//        }

//        public override Task OnDisconnected(bool stopCalled)
//        {
//            string userName = Context.User.Identity.Name;
//            string connectionId = Context.ConnectionId;

//            User user;
//            ActiveUsers.TryGetValue(userName, out user);

//            if (user != null)
//            {
//                lock (user.ConnectionIds)
//                {
//                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));

//                    if (!user.ConnectionIds.Any())
//                    {

//                        User removedUser;
//                        ActiveUsers.TryRemove(userName, out removedUser);
//                    }
//                }
//            }

//            return base.OnDisconnected(stopCalled);
//        }

//        private User GetUser(string username)
//        {
//            User user;
//            ActiveUsers.TryGetValue(username, out user);

//            return user;
//        }

//        public void forceLogOut(string to)
//        {
//            User receiver;
//            if (ActiveUsers.TryGetValue(to, out receiver))
//            {
//                IEnumerable<string> allReceivers;
//                lock (receiver.ConnectionIds)
//                {
//                    allReceivers = receiver.ConnectionIds.Concat(receiver.ConnectionIds);
//                }

//                foreach (var cid in allReceivers)
//                {
//                    Clients.Client(cid).Signout();
//                }
//            }
//        }
//    }
//}