namespace CityApp.Web
{
    public static class EnvironmentNames
    {
        /// <summary>
        /// Environments where scripts and styles should NOT be bundled and minified.
        /// </summary>
        public const string DEVELOPMENT = "Development";

        /// <summary>
        /// Environments where scripts and styles SHOULD be bundled and minified.
        /// </summary>
        public const string NOT_DEVELOPMENT = "Staging,QA,Production,Prod";
    }
}
