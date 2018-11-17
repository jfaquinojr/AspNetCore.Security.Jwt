﻿using AspNetCore.Security.Jwt.AzureAD;
using AspNetCore.Security.Jwt.Facebook;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.Security.Jwt
{
    /// <summary>
    /// AddSecurityBuilder class
    /// </summary>
    internal class AddSecurityBuilder : IAddSecurityBuilder
    {
        private static SecuritySettings SecuritySettings;
        private static bool IsJwtSchemeAdded = false;
        private static IServiceCollection Services;

        public AddSecurityBuilder(SecuritySettings securitySettings, bool isJwtSchemeAdded, IServiceCollection services)
        {
            SecuritySettings = securitySettings;
            IsJwtSchemeAdded = isJwtSchemeAdded;
            Services = services;
        }

        public IAddSecurityBuilder AddAzureADSecurity(bool addSwaggerSecurity = false)
        {
            IdTypeHelpers.LoadClaimTypes();

            Services.AddSingleton(SecuritySettings);
            Services.AddSingleton<AzureADSecuritySettings>(SecuritySettings.AzureADSecuritySettings);
            Services.AddScoped<IAuthentication<AzureADAuthModel, AzureADResponseModel>, AzureAuthenticator>();

            if (addSwaggerSecurity)
            {
                Services.AddSecureSwaggerDocumentation();
            }

            return this;
        }

        public IAddSecurityBuilder AddFacebookSecurity(Action<IIdTypeBuilder<FacebookAuthModel>> addClaims = null, bool addSwaggerSecurity = false)
        {
            IdTypeHelpers.LoadClaimTypes();

            Services.AddSingleton<BaseSecuritySettings>(SecuritySettings);
            if (addClaims != null)
            {
                Services.AddSingleton<Action<IIdTypeBuilder<FacebookAuthModel>>>(x => addClaims);
            }
            Services.AddScoped<ISecurityService<FacebookAuthModel>, SecurityService<FacebookAuthModel>>();
            Services.AddScoped<IAuthentication<FacebookAuthModel>, FacebookAuthenticator>();

            if (addSwaggerSecurity)
            {
                Services.AddSecureSwaggerDocumentation();
            }

            if (!IsJwtSchemeAdded)
            {
                Services.AddJwtBearerScheme(SecuritySettings);
                IsJwtSchemeAdded = true;
            }

            return this;
        }

        IAddSecurityBuilder IAddSecurityBuilder.AddSecurity<TAuthenticator>(bool addSwaggerSecurity)
        {
            IdTypeHelpers.LoadClaimTypes();

            Services.AddScoped<ISecurityService, SecurityService>();
            Services.AddScoped<IAuthentication, TAuthenticator>();

            if (addSwaggerSecurity)
            {
                Services.AddSecureSwaggerDocumentation();
            }

            if (!IsJwtSchemeAdded)
            {
                Services.AddJwtBearerScheme(SecuritySettings);
                IsJwtSchemeAdded = true;
            }

            return this;
        }

        IAddSecurityBuilder IAddSecurityBuilder.AddSecurity<TAuthenticator, TUserModel>(Action<IIdTypeBuilder<TUserModel>> addClaims, bool addSwaggerSecurity)
        {
            IdTypeHelpers.LoadClaimTypes();

            Services.AddSingleton(SecuritySettings);
            Services.AddSingleton<BaseSecuritySettings>(SecuritySettings);
            if (addClaims != null)
            {
                Services.AddSingleton<Action<IIdTypeBuilder<TUserModel>>>(x => addClaims);
            }
            Services.AddScoped<ISecurityService<TUserModel>, SecurityService<TUserModel>>();
            Services.AddScoped<IAuthentication<TUserModel>, TAuthenticator>();

            if (addSwaggerSecurity)
            {
                Services.AddSecureSwaggerDocumentation();
            }

            if (!IsJwtSchemeAdded)
            {
                Services.AddJwtBearerScheme(SecuritySettings);
                IsJwtSchemeAdded = true;
            }

            return this;
        }
    }
}
