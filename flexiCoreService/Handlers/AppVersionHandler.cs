using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace flexiCoreService.Handlers
{
    public class AppVersionHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var isValidAppVersion = false;

            if (request.Method.Method.ToLowerInvariant() == "post")
            {
                var currentAppVersion = ConfigurationManager.AppSettings.Get("AppVersion");

                IEnumerable<string> appVersions = null;
                request.Headers.TryGetValues("App-Version", out appVersions);
                if (appVersions != null)
                {
                    if (currentAppVersion == appVersions.FirstOrDefault())
                    {
                        isValidAppVersion = true;
                    }
                }

                if (!isValidAppVersion)
                {                    
                    var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("YOUR VERSION OF WISE1NE IS OUT OF DATE!!! \n\nKindly get the latest version of Wise1ne. \n\nFOR WISE1NE WEB: please refresh you browser to get the latest version. \n\nFOR ANDORID PHONES (e.g. Samsung): please go to the PlayStore and search for Wise1ne to download the latest version. \n\nFOR iOS PHONES (e.g. iPhone): launch safari browser on your phone and browse www.wise1ne.com, you will see a link to download the latest Wise1ne app.")
                    };
                    return Task.FromResult(response);
                }

            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}