using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using mvcCookieAuthSample.Models;
using Microsoft.AspNetCore.Identity;

namespace mvcCookieAuthSample.Service
{
    public class ProfileService : IProfileService
    {
        private UserManager<ApplicationUser> _userManager;
        // private UserManager<ApplicationUserRole> _userRoleManager;



        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        private async Task<List<Claim>> GetClaimsFromUserAsync(ApplicationUser user)
        {
            var claims=new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject,user.Id.ToString()),
                new Claim(JwtClaimTypes.Name,user.UserName),

            };
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role,role));
            }

            if (!string.IsNullOrEmpty(user.Avatar))
            {
                claims.Add(new Claim("Avatar",user.Avatar));
            }

            return claims;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
           
            var subjectId = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            var user = await _userManager.FindByIdAsync(subjectId);
            
            var claims = await GetClaimsFromUserAsync(user);
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = false;
            var subjectId = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = user != null;
        }
    }
}
