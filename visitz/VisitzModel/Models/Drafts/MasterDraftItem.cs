namespace VisitzModel.Models.Drafts;

public class MasterDraftItem : IComparable<MasterDraftItem>
{
	public string Name { get; set; }
	public int Count { get; set; }

	public int CompareTo(MasterDraftItem other)
	{
		int nameComparison = Name.CompareTo(other.Name);

		if (nameComparison == 0)
			return Count.CompareTo(other.Count);
		else
			return nameComparison;
	}
}
