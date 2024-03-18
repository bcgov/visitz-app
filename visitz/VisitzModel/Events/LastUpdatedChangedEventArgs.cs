namespace VisitzModel.Events;

public class LastUpdatedChangedEventArgs(string id, DateTime newLastUpdated) : EventArgs
{
	public string Id { get; set; } = id;

	public DateTime NewLastUpdatedValue { get; set; } = newLastUpdated;
}
