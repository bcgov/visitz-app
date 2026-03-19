using Visitz.Views.BaseClasses;

namespace Visitz.Views.Todo;

public partial class TodoMasterList : ViewModelContentView
{
    new TodoMasterListViewModel ViewModel => base.ViewModel as TodoMasterListViewModel;

    public TodoMasterList()
        : base(ServiceProvider.GetService<TodoMasterListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
