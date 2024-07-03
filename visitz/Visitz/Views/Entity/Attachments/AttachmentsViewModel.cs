using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Attachments;

internal partial class AttachmentsViewModel : VisitzViewModel, ICaseloadItemHolder
{
	[ObservableProperty]
	public CaseloadItem caseloadItem;
}
