using Visitz.Views.BaseClasses;

namespace Visitz.Views.Todo;

#nullable enable

public partial class TodoListView : ViewModelContentView
{
    public TodoListView()
        : base(ServiceProvider.GetService<TodoListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
