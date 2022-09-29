using Microsoft.AspNetCore.Http;

namespace CeskyRozhlasMiner.WebApp.Command.State
{
    /// <summary>
    /// Utility for manipulating with session.
    /// </summary>
    public class SessionManipulator
    {
        private const string _sessionUserId = "_userId";
        private readonly ISession _session;

        /// <summary>
        /// Represents that user was not found.
        /// </summary>
        public const int UserIdNotFound = -1;

        /// <summary>
        /// Initializes class for usage in controller handlers.
        /// </summary>
        /// <param name="session">Request session from controller handler (HttpContext.Session)</param>
        public SessionManipulator(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Sets user id.
        /// </summary>
        /// <param name="userId">User id to set</param>
        public void SetUserId(int userId)
        {
            _session.SetInt32(_sessionUserId, userId);
        }

        /// <summary>
        /// Gets id of the actually signed user.
        /// </summary>
        /// <returns>User id or <value>SessionManipulator.UserIdNotFound</value> 
        /// if not previously set.</returns>
        public int GetUserId()
        {
            return _session.GetInt32(_sessionUserId) ?? UserIdNotFound;
        }

        /// <summary>
        /// Checks whether a user is signed or not.
        /// </summary>
        /// <returns>User is signed</returns>
        public bool IsSignedIn()
        {
            return GetUserId() != UserIdNotFound;
        }
    }
}
