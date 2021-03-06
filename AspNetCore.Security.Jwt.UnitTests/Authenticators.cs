﻿using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class DefaultAuthenticator : IAuthentication
    {
        bool returnValue;

        public DefaultAuthenticator()
        {

        }

        public DefaultAuthenticator(bool returnValue)
        {
            this.returnValue = returnValue;
        }

        public virtual async Task<bool> IsValidUser(string id, string password)
        {
            return await Task.Run(() => this.returnValue);
        }
    }

    public class CustomAuthenticator : IAuthentication<UserModel>
    {
        bool returnValue;

        public CustomAuthenticator()
        {

        }

        public CustomAuthenticator(bool returnValue)
        {
            this.returnValue = returnValue;
        }

        public virtual async Task<bool> IsValidUser(UserModel user)
        {
            return await Task.Run(() => this.returnValue);
        }
    }
}
