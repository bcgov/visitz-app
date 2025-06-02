using VisitzModel.Models.Caseload;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Todo;

public class TodoVisitsDisplayItem(PersonVisit visit, IBusinessObject businessObject)
{
    public PersonVisit TodoItem { get; set; } = visit;
    public IBusinessObject BusinessObject { get; set; } = businessObject;
    public bool IsOverdue { get; set; } = FindIfOverdue(visit);

    public EntitySection SectionToOpen { get; set; } = EntitySection.ChildYouthVisits;

    public static bool FindIfOverdue(PersonVisit visit)
    {
        return DateTimeOffset.Now.Date > visit.DueDate;
    }
}
