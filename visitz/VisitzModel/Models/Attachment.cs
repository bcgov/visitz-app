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

	/// <summary>
	/// Real path to file on file system. File name will be different than <see cref="Filename"/>.
	/// </summary>
	public string Fullpath { get; set; }

	/// <summary>
	/// Virtual name of the attachment as stored in ICM.
	/// </summary>
	public string Filename { get; set; }
}
