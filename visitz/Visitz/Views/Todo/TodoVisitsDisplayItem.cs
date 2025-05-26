using VisitzModel.Models;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Todo;

public class TodoVisitsDisplayItem(PersonVisit visit, CaseloadItem caseloadItem)
{
    public PersonVisit TodoItem { get; set; } = visit;
    public CaseloadItem CaseloadItem { get; set; } = caseloadItem;
    public bool IsOverdue { get; set; } = FindIfOverdue(visit);

    public EntitySection SectionToOpen { get; set; } = EntitySection.ChildYouthVisits;

    public static bool FindIfOverdue(PersonVisit visit)
    {
        return DateTimeOffset.Now.Date > visit.DueDate;
    }
}
