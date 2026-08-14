using FAFS.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace FAFS.Permissions;

public class FAFSPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(FAFSPermissions.GroupName, L("Permission:FAFS"));

        var destinationsPermission = myGroup.AddPermission(FAFSPermissions.Destinations.Default, L("Permission:Destinations"));
        destinationsPermission.AddChild(FAFSPermissions.Destinations.Create, L("Permission:Destinations.Create"));
        destinationsPermission.AddChild(FAFSPermissions.Destinations.Edit, L("Permission:Destinations.Edit"));
        destinationsPermission.AddChild(FAFSPermissions.Destinations.Delete, L("Permission:Destinations.Delete"));

        var experiencesPermission = myGroup.AddPermission(FAFSPermissions.Experiences.Default, L("Permission:Experiences"));
        experiencesPermission.AddChild(FAFSPermissions.Experiences.Create, L("Permission:Experiences.Create"));
        experiencesPermission.AddChild(FAFSPermissions.Experiences.Edit, L("Permission:Experiences.Edit"));
        experiencesPermission.AddChild(FAFSPermissions.Experiences.Delete, L("Permission:Experiences.Delete"));

        var ratingsPermission = myGroup.AddPermission(FAFSPermissions.Ratings.Default, L("Permission:Ratings"));
        ratingsPermission.AddChild(FAFSPermissions.Ratings.Create, L("Permission:Ratings.Create"));

        var favoritesPermission = myGroup.AddPermission(FAFSPermissions.Favorites.Default, L("Permission:Favorites"));
        favoritesPermission.AddChild(FAFSPermissions.Favorites.Manage, L("Permission:Favorites.Manage"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FAFSResource>(name);
    }
}
