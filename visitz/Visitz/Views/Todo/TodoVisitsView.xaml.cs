using Visitz.Views.BaseClasses;

namespace Visitz.Views.Todo;

public partial class TodoVisitsView : ViewModelContentView
{
    new TodoVisitsViewModel ViewModel => base.ViewModel as TodoVisitsViewModel;

    public TodoVisitsView()
        : base(ServiceProvider.GetService<TodoVisitsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
