namespace Visitz.ViewModels
{
    public interface IVisitzLifecycle
    {
        void Window_Created(object sender, EventArgs e);

        void Window_Activated(object sender, EventArgs e);

        void Window_Deactivated(object sender, EventArgs e);

        void Window_Stopped(object sender, EventArgs e);

        void Window_Resumed(object sender, EventArgs e);
    }
}
