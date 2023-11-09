using FSEProject2.Models;

namespace FSEProject2
{
    public static class UsersActions
    {
        public static List<FirstSeen> GetUsersList()
        {
            var users = new List<FirstSeen>();
            foreach (var user in Data.Users)
            {
                var userData = new FirstSeen();
                if (user.periodsOnline == null)
                {
                    userData.firstSeen = null;
                }
                else
                {
                    var data = user.periodsOnline.Select(x => x.start).ToList();
                    userData.firstSeen = data.Min();
                }
                userData.username = user.nickname;
                userData.userId = user.userId;
                users.Add(userData);
            }
            return users;
        }
    }
}
