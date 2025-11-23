using FirebaseAdmin.Auth;

namespace CommonLib.Interfaces
{
    public interface IFirebaseOtpService
    {
        Task<bool> IsSessionValid(string uid);
        Task<bool> SaveLoginSessionToRedis(string uid);
        Task<FirebaseToken?> VerifyFirebaseTokenAsync(string firebaseIdToken);
    }
}