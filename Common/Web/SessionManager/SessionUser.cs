namespace Common.Web.SessionManager
{
    public abstract class SessionUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
