using Realms;
using Realms.Schema;
using VisitzModel.Models.InPersonVisits;

namespace VisitzModel.Storage;

public partial class PersonVisitDrafts(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
	public static readonly string Name = "personVisitDraftsRealm.realm";
	public static readonly ulong CurrentVersion = Version2_3_3;

	protected override RealmSchema MakeRealmSchema()
	{
		return new[]
		{
			typeof(PersonVisit),
		};
	}

	protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
	{
		// TODO...
	}
}
