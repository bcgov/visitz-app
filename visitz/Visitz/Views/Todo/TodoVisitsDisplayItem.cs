using VisitzModel.Models.Caseload;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Todo;

public class TodoVisitsDisplayItem(PersonVisit visit, IBusinessObject businessObject)
{
    public PersonVisit TodoItem { get; set; } = visit;
    public IBusinessObject BusinessObject { get; set; } = businessObject;
    public bool IsOverdue => DateTimeOffset.Now.Date > TodoItem.DueDate;

    public EntitySection SectionToOpen { get; set; } = EntitySection.ChildYouthVisits;
}
