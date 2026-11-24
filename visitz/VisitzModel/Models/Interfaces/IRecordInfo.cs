using Realms;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.Interfaces;

public interface IRecordInfo
{
    string RelatedEntityId { get; set; }

    EntityType RelatedEntityType { get; set; }

    EntitySubtype RelatedEntitySubtype { get; set; }
}

public static class IRecordInfoExtensions
{
    extension(IRecordInfo item)
    {
        public IRecordInfo InitWith(IBusinessObject businessObject)
        {
            item.RelatedEntityId = businessObject.Id;
            item.RelatedEntityType = businessObject.EntityType;
            item.RelatedEntitySubtype = businessObject.EntitySubtype;

            return item;
        }
    }

    extension<TRecordInfo>(TRecordInfo)
        where TRecordInfo : IRealmObject, IRecordInfo
    {
        /// <summary>
        /// Gets all <seealso cref="TRecordInfo"/> objects (the calling type) related by the provided
        /// <b>relatedType</b> and <b>relatedId</b> arguments.
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="relatedType"></param>
        /// <param name="relatedId"></param>
        /// <returns></returns>
        public static IQueryable<TRecordInfo> GetAllByRelated(Realm realm, EntityType relatedType, string relatedId)
        {
            // Running an RQL query using a hardcoded fieldname as a workaround for internal fields once here rather
            // than copy/pasting the same code in every class.

            // Realm does not allow Enums to be used in its queries, so all IRecordInfo implementers also manually
            // implement internal "Related...TypeInt" fields to be used only by the implementing class for EntityType /
            // EntitySubtype lookups.

            // Normally I would use inheritance to solve this problem, but Realm does not allow inheritance in its
            // objects. So we are stuck with these hacky workarounds.

            string entityTypeIntField = "RelatedEntityTypeInt";
            string entityIdField = nameof(IRecordInfo.RelatedEntityId);

            return realm
                .All<TRecordInfo>()
                .Filter($"{entityTypeIntField} == $0 AND {entityIdField} == $1", (int)relatedType, relatedId);
        }
    }
}
