using FSEProject2.Models;

namespace FSEProject2
{
    public class UserActions
    {
        public static UserId Forget(string userId) 
        {
            var user = Data.Users.FirstOrDefault(u => u.userId == userId);

            if (user == null)
            {
                return null;
            }

            Data.Users.Remove(user);
            Data.ForgottenUsers.Add(userId);
            return new UserId{ userId = userId };
        }
    }
}
