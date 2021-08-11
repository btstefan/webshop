using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ApiValidateAntiForgeryToken : AuthorizeAttribute
{
    public const string HeaderName = "X-RequestVerificationToken";

    private static string CookieName => AntiForgeryConfig.CookieName;

    public static string GenerateAntiForgeryTokenForHeader(HttpContext httpContext)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        // check that if the cookie is set to require ssl then we must be using it
        if (AntiForgeryConfig.RequireSsl && !httpContext.Request.IsSecureConnection)
        {
            throw new InvalidOperationException("Cannot generate an Anti Forgery Token for a non secure context");
        }

        // try to find the old cookie token
        string oldCookieToken = null;
        try
        {
            var token = httpContext.Request.Cookies[CookieName];
            if (!string.IsNullOrEmpty(token?.Value))
            {
                oldCookieToken = token.Value;
            }
        }
        catch
        {
            // do nothing
        }

        string cookieToken, formToken;
        AntiForgery.GetTokens(oldCookieToken, out cookieToken, out formToken);

        // set the cookie on the response if we got a new one
        if (cookieToken != null)
        {
            var cookie = new HttpCookie(CookieName, cookieToken)
            {
                HttpOnly = true,
            };
            // note: don't set it directly since the default value is automatically populated from the <httpCookies> config element
            if (AntiForgeryConfig.RequireSsl)
            {
                cookie.Secure = AntiForgeryConfig.RequireSsl;
            }
            httpContext.Response.Cookies.Set(cookie);
        }

        return formToken;
    }


    protected override bool IsAuthorized(HttpActionContext actionContext)
    {
        if (HttpContext.Current == null)
        {
            // we need a context to be able to use AntiForgery
            return false;
        }

        var headers = actionContext.Request.Headers;
        var cookies = headers.GetCookies();

        // check that if the cookie is set to require ssl then we must honor it
        if (AntiForgeryConfig.RequireSsl && !HttpContext.Current.Request.IsSecureConnection)
        {
            return false;
        }

        try
        {
            string cookieToken = cookies.Select(c => c[CookieName]).FirstOrDefault()?.Value?.Trim(); // this throws if the cookie does not exist
            string formToken = headers.GetValues(HeaderName).FirstOrDefault()?.Trim();

            if (string.IsNullOrEmpty(cookieToken) || string.IsNullOrEmpty(formToken))
            {
                return false;
            }

            AntiForgery.Validate(cookieToken, formToken);
            return base.IsAuthorized(actionContext);
        }
        catch
        {
            return false;
        }
    }
}