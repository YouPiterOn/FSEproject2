using FSEProject2.Models;

namespace FSEProject2
{
    public static class UserActions
    {
        public static UserId? Forget(string userId)
        {
            var user = Data.Users.Find(u => u.userId == userId);

            if (user == null)
            {
                return null;
            }

            Data.Users.Remove(user);
            Data.ForgottenUsers.Add(userId);
            return new UserId { userId = userId };
        }
    }
}
