using System.Text;
using Microsoft.Maui.Handlers;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;

namespace Visitz.Views.Entity.Notes;

public partial class EntityNotesView : ViewModelContentView, IBusinessObjectHolder, IRequestedEntitySection
{
    new EntityNotesViewModel ViewModel => base.ViewModel as EntityNotesViewModel;

    public IBusinessObject BusinessObject
    {
        get => ViewModel.BusinessObject;
        set => ViewModel.BusinessObject = value;
    }

    public EntitySection RequestedSection
    {
        get => ViewModel.RequestedSection;
        set => ViewModel.RequestedSection = value;
    }

    public EntityNotesView()
        : base(ServiceProvider.GetService<EntityNotesViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
        EditorHandler.Mapper.AppendToMapping(
            "NoBorder",
            (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
#endif

#if IOS || MACCATALYST
                handler.PlatformView.Layer.BorderWidth = 0;
                handler.PlatformView.Layer.CornerRadius = 0;
#endif

#if WINDOWS
                handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
            }
        );
    }

    private async void NotesCollectionView_Loaded(object sender, EventArgs e)
    {
        await ViewModel.notesLoadedTcs.Task;

        ScrollToItem(ViewModel.LastNoteItem, ViewModel.LastNoteItemGroup);
    }

    private void ScrollToItem(NoteItem item, NoteItemGroup noteItemGroup)
    {
        if (item != null && noteItemGroup != null)
            NotesCollectionView.ScrollTo(item, noteItemGroup, position: ScrollToPosition.End, animate: false);
    }
}
