namespace Common.ResultPattern
{
    public enum ReasonType
    {
        // ✅ 0 = Aucun problème
        None = 0,

        // 🟡 Erreurs de logique métier ou validation
        Undefine = 1,
        BadParameter = 2,
        NotFound = 3,
        Conflict = 4,
        Unauthorized = 5,
        Forbidden = 6,
        ValidationFailed = 7,
        AlreadyExists = 8,

        // 🔴 Erreurs techniques ou inattendues
        Failure = 20,
        Unexpected = 21,
        NotImplemented = 22,
        Timeout = 23,
        DependencyFailure = 24,
        ExternalServiceError = 25,
        DatabaseError = 26,

        // 🔐 Auth / tokens
        RefreshTokenFailure = 40,
        InvalidCredentials = 41,
        TokenExpired = 42,
        TokenInvalid = 43,
        SessionExpired = 44
    }
}
