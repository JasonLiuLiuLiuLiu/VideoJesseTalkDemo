using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace mvcCookieAuthSample.Models
{
    public class InputConsentViewModel:ConsentViewModel
    {
        public string Button { get; set; }

        public IEnumerable<string> ScopesConsented { get; set; }

        public bool RemeberConsent { get; set; }

        public string ReturnUrl { get; set; }
    }
}
