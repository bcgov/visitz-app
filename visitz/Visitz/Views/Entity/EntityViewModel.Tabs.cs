using CommunityToolkit.Mvvm.ComponentModel;
using Syncfusion.Maui.Toolkit.TabView;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity.Attachments;
using Visitz.Views.Entity.ChildYouthVisits;
using Visitz.Views.Entity.Details;
using Visitz.Views.Entity.FamilyMembers;
using Visitz.Views.Entity.Notes;
using Visitz.Views.Entity.SafetyAssess;
using Visitz.Views.Entity.SupportNetwork;
using Visitz.Views.Navigation;
using Visitz.VisitzConfig;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Navigation;
using VisitzModel.Utilities;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityViewModel
{
    [ObservableProperty]
    public TabItemCollection tabItems = [];

    [ObservableProperty]
    public int selectedIndex;

    [ObservableProperty]
    public SfTabItem? selectedTab;

    readonly List<ViewModelContentView> _viewsToDispose = [];

    readonly SemaphoreSlim _lazySemaphore = new(1);

    AsyncLazy<V> BuildTab<V>()
        where V : ViewModelContentView
    {
        return new(async () =>
        {
            V view = ServiceProvider.GetService<V>();

            if (view is IIcmRecordInfo info)
            {
                info.RowId = RowId;
                info.EntityType = EntityType;
            }

            if (view is IRequestedEntitySection sectionView)
                sectionView.RequestedSection = RequestedSection ?? EntitySection.Unknown;

            if (view is IFocusDraftItem focusView)
                focusView.FocusedDraftItem = FocusedDraftItem;

            _viewsToDispose.Add(view);

            return view;
        });
    }

    SfTabItem MakeTab<V>(string title)
        where V : ViewModelContentView
    {
        return new()
        {
            Header = title,
            Content = new AsyncLazyContentView<V>(BuildTab<V>(), semaphore: _lazySemaphore),
            FontFamily = VisitzFonts.BcSansRegularAlias,
        };
    }

    void BuildNavList()
    {
        if (BusinessObject == null)
            return;

        TabItems.Add(MakeTab<EntityDetailsView>(LocalizedStrings.Details));
        TabItems.Add(MakeTab<EntityContactsView>(LocalizedStrings.FamilyMembers));
        TabItems.Add(MakeTab<EntityNotesView>(LocalizedStrings.Notes));
        TabItems.Add(MakeTab<AttachmentsView>(LocalizedStrings.Attachments));

        if (ShouldShowSafetyAssessment())
            TabItems.Add(MakeTab<SafetyAssessmentListView>(LocalizedStrings.SafetyAssessment));

        if (ShouldShowChildYouthVisits())
            TabItems.Add(MakeTab<ChildYouthVisitListView>(LocalizedStrings.ChildYouthVisits));

        if (ShouldShowSupportNetwork())
            TabItems.Add(MakeTab<SupportNetworkListView>(LocalizedStrings.SupportNetwork));
    }

    void DisposeTabViews()
    {
        foreach (var item in _viewsToDispose)
            item.Dispose();

        TabItems.Clear();
    }

    SfTabItem? GetTabByType<T>()
        where T : BaseContentView
    {
        return TabItems.FirstOrDefault(t => t.Content is AsyncLazyContentView<T>);
    }

    SfTabItem? GetMappedNavItem(EntitySection? section)
    {
        return section switch
        {
            EntitySection.Family => GetTabByType<EntityContactsView>(),
            EntitySection.Notes or EntitySection.NoteEntry => GetTabByType<EntityNotesView>(),
            EntitySection.Attachments => GetTabByType<AttachmentsView>(),
            EntitySection.SafetyAssessment or EntitySection.SafetyAssessmentEntry =>
                GetTabByType<SafetyAssessmentListView>(),
            EntitySection.ChildYouthVisits or EntitySection.ChildYouthVisitsEntry =>
                GetTabByType<ChildYouthVisitListView>(),
            EntitySection.SupportNetwork => GetTabByType<SupportNetworkListView>(),
            _ => GetTabByType<EntityDetailsView>(),
        };
    }

    partial void OnSelectedIndexChanged(int value)
    {
        SelectedTab = value != -1 ? TabItems.ElementAt(value) : null;
    }

    async partial void OnSelectedTabChanged(SfTabItem? value)
    {
        SelectedIndex = value != null ? TabItems.IndexOf(value) : -1;
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
