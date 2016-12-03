namespace UmbracoLatch.Core
{
    public static class LatchConstants
    {
        public const string PluginName = "UmbracoLatch";
        public const string SectionName = "Latch";
        public const string SectionAlias = "latch";
        public const string SectionIcon = "umbracolatch-icon.png";
        public const int SectionOrder = 17;
        public const string SettingsTreeAlias = "latchSettings";
        public const string OperationsTreeAlias = "latchOperations";

        public static class Tables
        {
            public const string LatchApplication = "LatchApplication";
            public const string LatchPairedAccount = "LatchPairedAccount";
            public const string LatchOperation = "LatchOperation";
            public const string LatchOperationUser = "LatchOperationUser";
            public const string LatchOperationNode = "LatchOperationNode";
        }

        public static class OperationTypes
        {
            public const string Login = "login";
            public const string Content = "content";
            public const string Media = "media";
            public const string Dictionary = "dictionary";
        }

        public static class OperationActions
        {
            public const string Create = "create";
            public const string Edit = "edit";
            public const string Delete = "delete";
            public const string Publish = "publish";
            public const string Unpublish = "unpublish";
        }
    }
}