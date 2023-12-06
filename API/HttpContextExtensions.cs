using Service;

namespace API;



public static class HttpContextExtensions
{
    /*
     * sets the sessiondata
     */
    public static void SetSessionData(this HttpContext httpContext, SessionData data)
    {
        httpContext.Items["data"] = data;
    }

    /*
     * gets the sessiondata
     */
    public static SessionData? GetSessionData(this HttpContext httpContext)
    {
        return httpContext.Items["data"] as SessionData;
    }
}