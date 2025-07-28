using Microsoft.Extensions.Logging;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadItemView : BaseContentView
{
    public CaseloadItemView() : base()
    {
        InitializeComponent();    
    }

    protected override ILogger<BaseContentView> MakeLogger()
    {
        return ServiceProvider.GetService<ILogger<CaseloadItemView>>();
    }
}
