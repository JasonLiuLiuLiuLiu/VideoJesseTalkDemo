using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using mvcCookieAuthSample.Models;
using mvcCookieAuthSample.Service;
using Microsoft.AspNetCore.Mvc;

namespace mvcCookieAuthSample.Controllers
{
    public class ConsentController : Controller
    {
        private readonly ConsentService _consentService;
        public ConsentController(ConsentService consentService)
        {
            _consentService = consentService;
        }

      

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {

            var model = await _consentService.BuildConsentViewModel(returnUrl);

            if (model == null)
            {

            }


            return View(model);
        }

        public async Task<IActionResult> Index(InputConsentViewModel viewModel)
        {
            var result =await _consentService.ProcessConsent(viewModel);

            if (result.IsCallBack)
                return Redirect(result.ReditectUrl);

            return View(viewModel);
        }
    }
}