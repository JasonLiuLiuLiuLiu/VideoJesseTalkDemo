using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcCookieAuthSample.Models
{
    public class ProcessConsentResult
    {
        public string ReditectUrl { get; set; }

        public bool IsCallBack => ReditectUrl != null;

        public ConsentViewModel ViewModel { get; set; }
    }
}
