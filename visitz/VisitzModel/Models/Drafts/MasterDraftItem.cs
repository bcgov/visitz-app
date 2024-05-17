using CommunityToolkit.Mvvm.ComponentModel;

namespace VisitzModel.Models.Drafts;

public partial class MasterDraftItem : ObservableObject, IComparable<MasterDraftItem>
{
	[ObservableProperty]
	public string name;

	[ObservableProperty]
	public int count;

	public Type ItemType { get; set; }

	public int CompareTo(MasterDraftItem other)
	{
		int nameComparison = Name.CompareTo(other.Name);

		if (nameComparison == 0)
			return Count.CompareTo(other.Count);
		else
			return nameComparison;
	}
}
