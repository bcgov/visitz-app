using CommunityToolkit.Mvvm.Messaging;
using Visitz.Messaging;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Todo;

public partial class TodoVisitsView : ViewModelContentView
{
    new TodoVisitsViewModel ViewModel => base.ViewModel as TodoVisitsViewModel;

    public TodoVisitsView() : base(ServiceProvider.GetService<TodoVisitsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();
        await SendTodoMasterSelectedMessageWhenReady();
    }

    private async Task SendTodoMasterSelectedMessageWhenReady()
    {
        while (!ViewModel.IsInitialized)
        {
            await Task.Delay(100);
        }

        StrongReferenceMessenger.Default.Register<TodoMasterSelectedMessage>(this, (recipient, message) =>
        {
            var navItem = message.Value;

            if (navItem != null)
                (recipient as TodoVisitsView).OpenTodoSection(navItem);
        });
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void OpenTodoSection(NavItem navItem)
    {
        ViewModel.LoadTodoItemsForNavItem(navItem);
    }
}
