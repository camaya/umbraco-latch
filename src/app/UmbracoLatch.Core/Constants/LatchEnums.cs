namespace UmbracoLatch.Core
{
    public enum LatchOperationType
    {
        Login,
        Content,
        Media,
        Dictionary
    }

    public enum LatchOperationAction
    {
        Edit,
        Delete,
        Publish,
        Unpublish,
        None
    }
}