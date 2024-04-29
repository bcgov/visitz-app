using Realms;
using VisitzApi.Models;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;

namespace VisitzModel.Models
{
    public partial class CaseloadItem : IRealmObject
    {
        [PrimaryKey]
        public string CaseIncidentNumber { get; set; }

		// TODO: Convert EntityType from string to VisitzModel.Extensions.EntityTypes.EntityType.
		// Will need to workaround RLM024 (Realm does not support enums).
		public string EntityType { get; set; }
        public string CaseIncidentType { get; set; }
        public string WorkerId { get; set; }
        public string WorkerFullName { get; set; }
        public string ServiceOffice { get; set; }
        public string OfficeCode { get; set; }

        /// <summary>
        /// A value of "Y" means that two safety assessments exist and are both "Open" in ICM.
        /// </summary>
        public string SafetyAssessmentExist { get; set; }
        public string UnitNo { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string ProvinceState { get; set; }
        public string Country { get; set; }
        public IList<FamilyMember> FamilyMembers { get; }
        public string KeyPlayerCellPhone { get; set; }
        public string KeyPlayerEmail { get; set; }
        public string KeyPlayerHomePhone { get; set; }
        public string CreatedDate { get; set; }
        public string DateReported { get; set; }
        public string MemoUrgent { get; set; }
        public string MemoCallDate { get; set; }
        public string MemoCallTime { get; set; }
        public string MemoRecordedBy { get; set; }

#pragma warning disable RLM025 // RealmObject/EmbeddedObject properties usually indicate a relationship
        public FamilyMember KeyPlayer => FamilyMembers?
            .Where(mem => mem.IsKeyPlayer)
            .FirstOrDefault();
#pragma warning restore RLM025 // RealmObject/EmbeddedObject properties usually indicate a relationship

		public string DisplayDate => EntityType.ParseEntityType() switch
		{
			EntityTypes.EntityType.Incident => DateReported,
			EntityTypes.EntityType.Memo => MemoCallDate,
			_ => CreatedDate, // Case, etc...
		};

        public string DisplayName
        {
            get
            {
                if (EntityType.ParseEntityType() == EntityTypes.EntityType.Memo)
                    return WorkerFullName;
                else if (TryGetKeyPlayer(out FamilyMember keyPlayer))
                    return $"{keyPlayer.LastName}, {keyPlayer.FirstName}";
                else
                    return ServiceOffice;
            }
        }

        public string KeyPlayerLastName => KeyPlayer?.LastName ?? string.Empty;

        public string FullType => CaseIncidentType + " " + EntityType;

        public string TypeInitials => (EntityType.ParseEntityType() == EntityTypes.EntityType.Incident
            ? EntityType[..2]
            : CaseIncidentType.GetInitials()).ToUpper();

        public bool TryGetKeyPlayer(out FamilyMember keyPlayer)
        {
            keyPlayer = KeyPlayer;
            return keyPlayer != null;
        }

        public static DateTime DisplayDateTransform(CaseloadItem caseloadItem)
        {
            return caseloadItem.DisplayDate?.Length > 0
                ? DateTime.Parse(caseloadItem.DisplayDate)
                : DateTime.MinValue;
        }

        public string Address =>
            (UnitNo.FormatAddressPart("-")
            + AddressLine1.FormatAddressPart(" ")
            + AddressLine2.FormatAddressPart(" ")
            + City.FormatAddressPart(", ")
            + ProvinceState.FormatAddressPart(", ")
            + Country.FormatAddressPart(", ")
            + PostalCode.FormatAddressPart(""))
            .TrimEnd([',', ' ', '-'])
            .TrimEnd([',', ' ', '-']);

        public static CaseloadItem FromApiEntity(CaseloadEntity caseloadEntity)
        {
            var caseloadItem = new CaseloadItem()
            {
                EntityType = caseloadEntity.EntityType,
                CaseIncidentNumber = caseloadEntity.CaseIncidentNumber,
                CaseIncidentType = caseloadEntity.CaseIncidentType,
                WorkerId = caseloadEntity.WorkerId,
                WorkerFullName = caseloadEntity.WorkerFullName,
                ServiceOffice = caseloadEntity.ServiceOffice,
                OfficeCode = caseloadEntity.OfficeCode,
                SafetyAssessmentExist = caseloadEntity.SafetyAssessmentExist,
                UnitNo = caseloadEntity.UnitNo,
                AddressLine1 = caseloadEntity.AddressLine1,
                AddressLine2 = caseloadEntity.AddressLine2,
                City = caseloadEntity.City,
                PostalCode = caseloadEntity.PostalCode,
                ProvinceState = caseloadEntity.ProvinceState,
                Country = caseloadEntity.Country,
                KeyPlayerCellPhone = caseloadEntity.KeyPlayerCellPhone,
                KeyPlayerEmail = caseloadEntity.KeyPlayerEmail,
                KeyPlayerHomePhone = caseloadEntity.KeyPlayerHomePhone,
                CreatedDate = caseloadEntity.CreatedDate,
                DateReported = caseloadEntity.DateReported,
                MemoUrgent = caseloadEntity.MemoUrgent,
                MemoCallDate = caseloadEntity.MemoCallDate,
                MemoCallTime = caseloadEntity.MemoCallTime,
                MemoRecordedBy = caseloadEntity.MemoRecordedBy,
            };

            if (caseloadEntity.FamilyMembers != null)
            {
                var family = FamilyMember.FromApiEntities(caseloadEntity.FamilyMembers);

                foreach (var familyMember in family)
                    caseloadItem.FamilyMembers.Add(familyMember);
            }

            return caseloadItem;
        }

        public static IEnumerable<CaseloadItem> FromApiEntities(IEnumerable<CaseloadEntity> caseloadEntities)
        {
            return caseloadEntities.Select(FromApiEntity);
        }

        public static IQueryable<CaseloadItem> GetAllByDistinctSubtypes(Realm realm, bool sortAsc)
        {
            string subtype = nameof(CaseIncidentType);
            string sortDirection = sortAsc ? "ASC" : "DESC";

            return realm
                .All<CaseloadItem>()
                .Filter($"TRUEPREDICATE DISTINCT({subtype}) SORT({subtype} {sortDirection})");
        }

		/// <summary>
		/// Adds new CaseloadItems to the Realm and cascade-deletes any pre-existing CaseloadItems not found in the incoming list.
		/// The cascade deletion only extends to objects within this Realm—it does not affect any drafts the user may have.
		/// </summary>
		/// <param name="realm"></param>
		/// <param name="newCaseloadItems"></param>
		/// <returns></returns>
		public static async Task ReplaceCaseloadWithAsync(Realm realm, IEnumerable<CaseloadItem> newCaseloadItems)
		{
			var currentCaseload = realm.All<CaseloadItem>();
			var itemsToDelete = currentCaseload.ExceptBy(newCaseloadItems.Select(CaseloadSelector), CaseloadSelector);

			await realm.WriteAsync(() =>
			{
				foreach (var itemToDelete in itemsToDelete)
					CascadeDelete(realm, itemToDelete);

				realm.Add(newCaseloadItems, update: true);
			});
		}

		static void CascadeDelete(Realm realm, CaseloadItem itemToDelete)
		{
			foreach (var note in NoteItem.GetNotesByEntityId(realm, itemToDelete.CaseIncidentNumber))
				realm.Remove(note);

			realm.Remove(itemToDelete);
		}

		static string CaseloadSelector(CaseloadItem caseloadItem) => caseloadItem.CaseIncidentNumber;
	}
}
