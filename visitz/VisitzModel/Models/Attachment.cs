using Realms;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models;

public partial class Attachment : IRealmObject, IRecordInfo
{
	public string RelatedEntityId { get; set; }

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

	public byte[] Thumbnail { get; set; }

	public string Fullpath { get; set; }

	public string Filename
	{
		get => Path.GetFileName(Fullpath);
		set => Fullpath = Path.Join(new FileInfo(Fullpath).DirectoryName, value);
	}
}
