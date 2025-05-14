using Realms;
using VisitzModel.Formats;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.Caseload;

public interface IBusinessObject : IRealmObject
{
    public static readonly string DisplayDateFormat = IcmDateFormats.BasicTimestampShort;

    public string Id { get; set; }

    public string FileNumber { get; set; }

    public string GivenNames { get; set; }

    public string LastName { get; set; }

    public EntityType EntityType { get; }

    public EntitySubtype EntitySubtype { get; set; }

    public string DisplayDate { get; }

    public string DisplayName { get; }

    public string FullType { get; }
}

public static class IBusinessObjectExtensions
{
    public static DateTime DisplayDateTransform(this IBusinessObject businessObject)
    {
        return businessObject.DisplayDate?.Length > 0
            ? DateTime.Parse(businessObject.DisplayDate)
            : DateTime.MinValue;
    }

    public static string GetDisplayName(this IBusinessObject businessObject)
    {
        return $"{businessObject.LastName}, {businessObject.GivenNames}";
    }

    public static string GetFullType(this IBusinessObject businessObject)
    {
        return $"{businessObject.EntitySubtype} {businessObject.EntityType}";
    }
}
