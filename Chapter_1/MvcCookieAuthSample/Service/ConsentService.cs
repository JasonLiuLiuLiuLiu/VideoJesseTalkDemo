using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using mvcCookieAuthSample.Models;

namespace mvcCookieAuthSample.Service
{
    public class ConsentService
    {
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;
        public ConsentService(IClientStore clientStore, IResourceStore resourceStore, IIdentityServerInteractionService identityServerInteractionService)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }

        public async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl)
        {
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
                return null;
            var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);

            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);

            var vm = CreateConsentViewModel(request, client, resources);
            vm.ReturnUrl = returnUrl;

            return vm;
        }

        public async Task<ProcessConsentResult> ProcessConsent(InputConsentViewModel viewModel)
        {
            ConsentResponse consentResponse = null;
            if (viewModel.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;
            }
            else if (viewModel.Button == "yes")
            {
                if (viewModel.ScopesConsented != null && viewModel.ScopesConsented.Any())
                {
                    consentResponse = new ConsentResponse
                    {
                        RememberConsent = viewModel.RemeberConsent,
                        ScopesConsented = viewModel.ScopesConsented,
                    };
                }
            }
            ProcessConsentResult result = new ProcessConsentResult();
            if (consentResponse != null)
            {
                var response = await _identityServerInteractionService.GetAuthorizationContextAsync(viewModel.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(response, consentResponse);

                result.ReditectUrl = viewModel.ReturnUrl;
            }


            var consentViewModel = await BuildConsentViewModel(viewModel.ReturnUrl);
            result.ViewModel = consentViewModel;


            return result;
        }

        #region Private Method
        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources)
        {
            var vm = new ConsentViewModel();
            vm.ClientName = client.ClientName;
            vm.ClientLogoUrl = client.LogoUri;
            vm.ClientUrl = client.ClientUri;
            vm.RemeberConsent = client.AllowRememberConsent;

            vm.IdentityScopes = resources.IdentityResources.Select(CreateScopeViewModel1);
            vm.ResourceScopes = resources.ApiResources.SelectMany(i => i.Scopes).Select(CreateScopeViewModel2);
            return vm;
        }

        private ScopeViewModel CreateScopeViewModel1(IdentityResource identityResource)
        {
            return new ScopeViewModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Emphasize = identityResource.Emphasize,
                Checked = identityResource.Required,
                Required = identityResource.Required,
            };
        }
        private ScopeViewModel CreateScopeViewModel2(Scope scope)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Emphasize = scope.Emphasize,
                Checked = scope.Required,
                Required = scope.Required,
            };
        }
        #endregion


    }
}
