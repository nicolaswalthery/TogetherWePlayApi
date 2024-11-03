namespace Common.Web.SessionManager
{
    public interface ISessionManager
    {
        void SetSession();
        SessionUser GetSession();
        SessionUser SessionUser { get; set; }
        void ClearSession();
    }
}
