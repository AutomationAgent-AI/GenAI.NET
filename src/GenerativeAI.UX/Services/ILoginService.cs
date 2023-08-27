using System.Threading.Tasks;

namespace Automation.GenerativeAI.UX.Services
{
    public class User
    {
        public string DisplayName { get; set; }
        public string ID { get; set; }
        public string Profile { get; set; }
    }

    /// <summary>
    /// Implements login service
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Logs in asynchronously
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> LoginAsync(User user, string password);

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task RegisterUserAsync(User user, string password);

        /// <summary>
        /// Update user's profile pic
        /// </summary>
        /// <param name="user"></param>
        Task UpdateUserProfileAsync(User user);

        /// <summary>
        /// Provides info about current user
        /// </summary>
        /// <returns>User</returns>
        Task<User> GetCurrentUserAsync();
    }
}
