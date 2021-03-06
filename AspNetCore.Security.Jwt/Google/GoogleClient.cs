﻿using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AspNetCore.Security.Jwt.Google
{
    public class GoogleClient : ISecurityClient<GoogleAuthModel, GoogleResponseModel>
    {
        private readonly SecuritySettings securitySettings;
        private readonly IHttpClient httpClient;

        public GoogleClient(SecuritySettings securitySettings, IHttpClient httpClient)
        {
            this.securitySettings = securitySettings;
            this.httpClient = httpClient;
        }

        public virtual async Task<GoogleResponseModel> PostSecurityRequest(GoogleAuthModel request)
        {
            string codeClient = $"code={HttpUtility.UrlEncode(request.AuthorizationCode.Trim())}&" 
                + $"client_id={HttpUtility.UrlEncode(this.securitySettings.GoogleSecuritySettings.ClientId.Trim())}&";
            string secretUri = $"client_secret={HttpUtility.UrlEncode(this.securitySettings.GoogleSecuritySettings.ClientSecret.Trim())}&" 
                + $"redirect_uri={HttpUtility.UrlEncode(this.securitySettings.GoogleSecuritySettings.RedirectUri.Trim())}&"                
                + "grant_type=authorization_code";

            var postString = codeClient + secretUri;

            byte[] bytes = Encoding.UTF8.GetBytes(postString);

            HttpRequestMessage requestGoogle = new HttpRequestMessage(HttpMethod.Post, this.securitySettings.SystemSettings.GoogleAuthSettings.TokenUrl);
            requestGoogle.Content = new ByteArrayContent(bytes);

            requestGoogle.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            requestGoogle.Content.Headers.Add("Content-Length", $"{bytes.Length}");

            var tokenResponse = await this.httpClient.SendAsync<GoogleResponseModel>(requestGoogle);

            tokenResponse.IsAuthenticated = true;

            return tokenResponse;            
        }
    }
}
