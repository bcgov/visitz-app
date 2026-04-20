using CommunityToolkit.Mvvm.ComponentModel;
using Syncfusion.Maui.Toolkit.TabView;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity.Attachments;
using Visitz.Views.Entity.ChildYouthVisits;
using Visitz.Views.Entity.Details;
using Visitz.Views.Entity.FamilyMembers;
using Visitz.Views.Entity.Notes;
using Visitz.Views.Entity.SafetyAssess;
using Visitz.Views.Entity.SupportNetwork;
using Visitz.VisitzConfig;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityViewModel
{
    [ObservableProperty]
    public TabItemCollection tabItems = [];

    readonly List<BaseContentView> _viewsToDispose = [];

    SfTabItem MakeTab<V>()
        where V : BaseContentView
    {
        V view = ServiceProvider.GetService<V>();

        if (view is IIcmRecordInfo info)
        {
            info.RowId = RowId;
            info.EntityType = EntityType;
        }

        _viewsToDispose.Add(view);

        return new()
        {
            Header = view.Title,
            Content = view,
            FontFamily = VisitzFonts.BcSansRegularAlias,
        };
    }

    void BuildNavList()
    {
        if (BusinessObject == null)
            return;

        TabItems.Add(MakeTab<EntityDetailsView>());
        TabItems.Add(MakeTab<EntityContactsView>());
        TabItems.Add(MakeTab<EntityNotesView>());
        TabItems.Add(MakeTab<AttachmentsView>());

        if (ShouldShowSafetyAssessment())
            TabItems.Add(MakeTab<SafetyAssessmentListView>());

        if (ShouldShowChildYouthVisits())
            TabItems.Add(MakeTab<ChildYouthVisitListView>());

        if (ShouldShowSupportNetwork())
            TabItems.Add(MakeTab<SupportNetworkListView>());
    }

    void DisposeTabViews()
    {
        foreach (var item in _viewsToDispose)
            item.Dispose();

        TabItems.Clear();
    }

    bool ShouldShowSafetyAssessment()
    {
        return BusinessObject?.EntityType == EntityType.Incident;
    }

    bool ShouldShowChildYouthVisits()
    {
        return BusinessObject?.EntityType == EntityType.Case
            && BusinessObject?.EntitySubtype == EntitySubtype.ChildServices;
    }

    bool ShouldShowSupportNetwork()
    {
        return BusinessObject?.EntityType == EntityType.Case
            || BusinessObject?.EntityType == EntityType.Incident
            || BusinessObject?.EntityType == EntityType.ServiceRequest;
    }
}
