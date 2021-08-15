namespace Reviews
{
    public class Settings
    {
        public static Settings ResolveFromEnvironment()
        {
            var settings = new Settings
            {
                StarColor = System.Environment.GetEnvironmentVariable("STAR_COLOR"),
                RatingsEnabled = !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("ENABLE_RATINGS")),
                ServiceDomain = System.Environment.GetEnvironmentVariable("SERVICES_DOMAIN"),
                RatingsHostName = System.Environment.GetEnvironmentVariable("RATINGS_HOSTNAME"),
                ExtraHeadersToForward = System.Environment.GetEnvironmentVariable("EXTRA_HEADERS_TO_FORWARD"),
            };

            if (string.IsNullOrEmpty(settings.StarColor))
            {
                settings.StarColor = "black";
            }
            if (string.IsNullOrEmpty(settings.RatingsHostName))
            {
                settings.RatingsHostName = "ratings";
            }
            if (string.IsNullOrEmpty(settings.ExtraHeadersToForward))
            {
                settings.ExtraHeadersToForward = "";
            }

            return settings;
        }
        
        
        public bool RatingsEnabled { get; set; }
        public string StarColor { get; set; }
        public string RatingsHostName { get; set; }
        public string ServiceDomain { get; set; }
        public string ExtraHeadersToForward { get; set; }
    }
}