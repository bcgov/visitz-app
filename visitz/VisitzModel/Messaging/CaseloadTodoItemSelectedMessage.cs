using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace VisitzModel.Messaging;

public class CaseloadTodoItemSelectedMessage(
    CaseloadItem value,
    EntitySection? section = null,
    PersonVisit visitItem = null)
    : ValueChangedMessage<CaseloadItem>(value)
{
    public EntitySection Section { get; set; } = section ?? EntitySection.Details;

    public PersonVisit VisitItem { get; set; } = visitItem;
}
