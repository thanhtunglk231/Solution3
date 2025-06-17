using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using WebBrowser.Models;
using WebBrowser.Services.ApiServices;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Services.Implements
{
    public class LoginService : BaseApiService, ILoginService
    {

        private readonly string _baseUrl;
        private readonly HttpClient _client;
        public LoginService(IConfiguration configuration, HttpClient client)
            : base(configuration, client)
        {
            _baseUrl = configuration["PathStrings:Url"];
            _client = client;
        }
        public async Task<LoginResponse> Register(string username , string password)
        {
            var para = new LoginRequest { password = password, username= username};
            var url = _baseUrl + ApiRouter.register;
            Console.Write(url);
            var result = await PostJsonAsync<LoginResponse>(ApiRouter.register, para);
            return result;

        }
        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            var body = new LoginRequest
            {
                username = username,
                password = password
            };
            var url = _baseUrl + ApiRouter.log_in;

            var result = await PostJsonAsync<LoginResponse>(ApiRouter.log_in, body);

            return result; 
        }

    }

}
