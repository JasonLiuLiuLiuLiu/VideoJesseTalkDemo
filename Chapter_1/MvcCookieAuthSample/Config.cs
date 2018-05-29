﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace mvcCookieAuthSample
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "API  Application"),
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    ClientUri = "http://localhost:5001",
                    LogoUri = "data:image/png;base64,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",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = {"http://localhost:5001/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5001/signout-callback-oidc"},
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1000",
                    Username = "lzy",
                    Password = "123456",
                }
            };
        }
    }
}
