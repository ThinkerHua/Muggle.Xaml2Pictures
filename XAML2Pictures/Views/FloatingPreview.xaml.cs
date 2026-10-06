using System.Windows;
using System.Windows.Input;

namespace Muggle.Xaml2Pictures.Views;

public partial class FloatingPreview : Window
{
    private Point _pressedPosition;

    private bool _isDragMoving;

    public FloatingPreview()
    {
        InitializeComponent();
    }

    private void OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (Mouse.LeftButton is MouseButtonState.Pressed && _pressedPosition != e.GetPosition(this))
        {
            DragMove();
        }
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        _pressedPosition = e.GetPosition(this);
    }

    private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragMoving)
        {
            _isDragMoving = false;
            e.Handled = true;
        }
    }

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        const double factor = 0.05;

        var toHeight = this.ActualHeight * (1 + factor * (e.Delta > 0 ? 1 : -1));
        var toWidth = this.ActualWidth * (1 + factor * (e.Delta > 0 ? 1 : -1));
        if (toHeight < this.MinHeight) toHeight = this.MinHeight;
        if (toHeight > this.MaxHeight) toHeight = this.MaxHeight;
        if (toWidth < this.MinWidth) toWidth = this.MinWidth;
        if (toWidth > this.MaxWidth) toWidth = this.MaxWidth;

        this.Height = toHeight;
        this.Width = toWidth;
    }
}