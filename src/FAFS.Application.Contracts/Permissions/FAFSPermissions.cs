namespace FAFS.Permissions;

public static class FAFSPermissions
{
    public const string GroupName = "FAFS";

    public static class Destinations
    {
        public const string Default = GroupName + ".Destinations";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Experiences
    {
        public const string Default = GroupName + ".Experiences";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Ratings
    {
        public const string Default = GroupName + ".Ratings";
        public const string Create = Default + ".Create";
    }

    public static class Favorites
    {
        public const string Default = GroupName + ".Favorites";
        public const string Manage = Default + ".Manage";
    }
}
