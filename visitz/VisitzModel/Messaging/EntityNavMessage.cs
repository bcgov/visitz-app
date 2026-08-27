using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace VisitzModel.Messaging;

public class EntityNavMessage(
    EntityNavItem navItem,
    IBusinessObject businessObject,
    EntitySection? subsection = null,
    IDraftItem? selectedDraftItem = null
)
    : ValueChangedMessage<(EntityNavItem, IBusinessObject, EntitySection?, IDraftItem?)>(
        (navItem, businessObject, subsection, selectedDraftItem)
    ) { }
