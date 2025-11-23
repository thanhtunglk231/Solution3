using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonLib.Handles;
using CommonLib.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CommonLib.Implementations
{
    public class FirebaseOtpService : IFirebaseOtpService
    {
        private readonly IRedisService _redis;
        private readonly IErrorHandler _log;

        private const int SessionExpireMinutes = 30;
        private const string RedisKeyPrefix = "firebase:session:";

        public FirebaseOtpService(
            IRedisService redis,
            IErrorHandler log,
            IConfiguration config)
        {
            _redis = redis;
            _log = log;

            InitializeFirebase(config);
        }

        #region Firebase Initialization

        private void InitializeFirebase(IConfiguration config)
        {
            if (FirebaseApp.DefaultInstance != null)
                return;

            try
            {
                var section = config.GetSection("Firebase:ServiceAccountJson");
                if (!section.Exists())
                {
                    _log.WriteStringToFuncion(nameof(FirebaseOtpService),
                        "Không tìm thấy cấu hình Firebase:ServiceAccountJson.");
                    throw new InvalidOperationException(
                        "Thiếu cấu hình Firebase:ServiceAccountJson trong appsettings.json.");
                }

                var dict = new Dictionary<string, object>();
                section.Bind(dict);

                var json = JsonConvert.SerializeObject(dict);
                var credential = GoogleCredential.FromJson(json);

                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });

                _log.WriteStringToFuncion(nameof(FirebaseOtpService),
                    "FirebaseApp khởi tạo thành công.");
            }
            catch (Exception ex)
            {
                _log.WriteToFile(ex);
                throw; // fail fast
            }
        }

        #endregion

        #region Verify Firebase Token

        /// <summary>
        /// Xác thực Firebase ID Token do client gửi lên.
        /// </summary>
        public async Task<FirebaseToken?> VerifyFirebaseTokenAsync(string firebaseIdToken)
        {
            _log.WriteStringToFuncion(nameof(FirebaseOtpService), "VerifyFirebaseTokenAsync");

            if (string.IsNullOrWhiteSpace(firebaseIdToken))
            {
                _log.WriteStringToFuncion(nameof(FirebaseOtpService),
                    "ID Token rỗng hoặc null.");
                return null;
            }

            try
            {
                return await FirebaseAuth
                    .DefaultInstance
                    .VerifyIdTokenAsync(firebaseIdToken);
            }
            catch (Exception ex)
            {
                _log.WriteToFile(ex);
                return null;
            }
        }

        #endregion

        #region Redis Session

        /// <summary>
        /// Lưu session đăng nhập sau khi Firebase xác thực thành công.
        /// </summary>
        public async Task<bool> SaveLoginSessionToRedis(string uid)
        {
            _log.WriteStringToFuncion(nameof(FirebaseOtpService), "SaveLoginSessionToRedis");

            if (string.IsNullOrWhiteSpace(uid))
                return false;

            try
            {
                var key = $"{RedisKeyPrefix}{uid}";
                await _redis.SetAsync(key, "1", TimeSpan.FromMinutes(SessionExpireMinutes));
                return true;
            }
            catch (Exception ex)
            {
                _log.WriteToFile(ex);
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái session của user.
        /// </summary>
        public async Task<bool> IsSessionValid(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                return false;

            try
            {
                var key = $"{RedisKeyPrefix}{uid}";
                return await _redis.ExistsAsync(key);
            }
            catch (Exception ex)
            {
                _log.WriteToFile(ex);
                return false;
            }
        }

        #endregion
    }
}
