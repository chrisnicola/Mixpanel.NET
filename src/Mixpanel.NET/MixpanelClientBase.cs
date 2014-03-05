namespace Mixpanel.NET
{
    public abstract class MixpanelClientBase
    {
        protected IMixpanelHttp http;
        protected string token;

        protected MixpanelClientBase(string token, IMixpanelHttp http = null)
        {
            this.http = http ?? new MixpanelHttp();
            this.token = token;
        }
    }
}