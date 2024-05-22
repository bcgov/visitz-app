using Realms;
using VisitzModel.Extensions;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models
{
    public partial class NoteDraft : IRealmObject, IDraftItem
    {
		// Only allow one note draft per parent entity.
        [PrimaryKey]
		public string ParentEntityId { get; set; }

		private int RelatedEntityTypeInt { get; set; } = (int)EntityType.Unknown;

		public EntityType RelatedEntityType
		{
			get => (EntityType)RelatedEntityTypeInt;
			set => RelatedEntityTypeInt = (int)value;
		}

		private int RelatedEntitySubtypeInt { get; set; } = (int)EntitySubtype.Unknown;

		public EntitySubtype RelatedEntitySubtype
		{
			get => (EntitySubtype)RelatedEntitySubtypeInt;
			set => RelatedEntitySubtypeInt = (int)value;
		}

		public string Draft { get; set; }

        public string DraftBinding
        {
            get => IsValid ? Draft : default;
            set
            {
                bool canSet = !value?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? true;

                if (canSet)
				{
                    this.Commit(() => Draft = value);
					RaisePropertyChanged(nameof(DraftBinding));
					LastUpdatedBinding = DateTimeOffset.Now;
				}
			}
		}

		public DateTimeOffset DraftCreated { get; set; } = DateTimeOffset.Now;

		public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

		public DateTimeOffset LastUpdatedBinding
		{
			get => IsValid ? LastUpdated : default;
			set
			{
				this.Commit(() => LastUpdated = value);
				RaisePropertyChanged(nameof(LastUpdated));
			}
		}

		public string Preview { get => Draft; }

		public string DraftLocation { get; set; }

		public string DraftLocationBinding
		{
			get => IsValid ? DraftLocation : default;
			set
			{
				this.Commit(() => DraftLocation = value);
				RaisePropertyChanged(nameof(DraftLocation));
			}
		}

		public static string MakeId(string parentEntityId)
        {
            return $"{parentEntityId}";
        }

        public static NoteDraft FindByEntityId(Realm realm, string entityId)
        {
            return realm.Find<NoteDraft>(MakeId(entityId));
        }

        public static async Task Delete(Realm realm, string entityNumber)
        {
            var draft = FindByEntityId(realm, entityNumber);

            if (draft != null)
                await realm.WriteAsync(() => realm.Remove(draft));
        }
    }
}
