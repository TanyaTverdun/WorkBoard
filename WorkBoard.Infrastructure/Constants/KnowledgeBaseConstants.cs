namespace WorkBoard.WebAPI.Constants;

public static class KnowledgeBaseConstants
{
    public const string DirectoryName = "Resources";
    public const string PluginName = "KnowledgeBase";

    public static class General
    {
        public const string DocumentId = "workboard-general-kb";
        public const string FileName = "workboard-kb-general.md";
    }

    public static class Admin
    {
        public const string DocumentId = "workboard-admin-kb";
        public const string FileName = "workboard-kb-admin.md";
    }

    public static class Tags
    {
        public const string TypeKey = "type";
        public const string TypeDocumentation = "documentation";

        public const string ProjectKey = "project";
        public const string ProjectWorkboard = "workboard";

        public const string AccessLevelKey = "accessLevel";
        public const string AccessLevelGeneral = "general";
        public const string AccessLevelOwner = "ownerOnly";
    }
}
