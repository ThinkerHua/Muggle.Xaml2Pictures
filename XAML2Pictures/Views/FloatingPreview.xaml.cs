using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Muggle.Xaml2Pictures.ViewModels;

namespace Muggle.Xaml2Pictures.Views;

public partial class FloatingPreview : Window
{
    private Point _prePosition;
    private bool _isPanning;
    private double _factor;
    private double Ratio => Container.ActualWidth / Container.ActualHeight;

    private static readonly DispatcherTimer TipTimer = new() { Interval = TimeSpan.FromSeconds(1) };
    private static readonly Duration FadeDuration = new(TimeSpan.FromMilliseconds(200));

    private FloatingPreviewViewModel ViewModel => (FloatingPreviewViewModel)DataContext;

    public FloatingPreview(FloatingPreviewViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        Loaded += (sender, args) => ViewModel.SetRatio(Ratio);

        Container.SizeChanged += (sender, args) => ViewModel.SetRatio(Ratio);

        ViewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != nameof(ViewModel.DrawingGroup)) return;
            _factor = Math.Max(ViewModel.DrawingGroup.Bounds.Width, ViewModel.DrawingGroup.Bounds.Height) /
                      Math.Max(Container.ActualWidth, Container.ActualHeight);
        };

        TipTimer.Tick += (sender, args) =>
        {
            HideTip();
            TipTimer.Stop();
        };
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _prePosition = e.GetPosition(this);
            _isPanning = true;
        }
        else if (e.RightButton == MouseButtonState.Pressed)
        {
            ViewModel.ScaleMax();
        }
    }

    private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            _isPanning = false;
        }
    }

    private void OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        ShowTip();
        TipTimer.Stop();
        TipTimer.Start();

        if (_isPanning)
        {
            var position = e.GetPosition(this);
            ViewModel.Pan(
                new Vector((_prePosition.X - position.X) * _factor, (_prePosition.Y - position.Y) * _factor));
            _prePosition = position;
        }
    }

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        const double delta = 1.1;

        var position = e.GetPosition(this);
        var percentageX = position.X * 2 / Container.ActualWidth - 1;
        var percentageY = position.Y * 2 / Container.ActualHeight - 1;
        ViewModel.ScaleAt(e.Delta > 0 ? 1 / delta : delta, percentageX, percentageY);
    }

    private void HideTip()
    {
        AnimateOpacity(0);
    }

    private void ShowTip()
    {
        AnimateOpacity(1);
    }

    private void AnimateOpacity(double toOpacity)
    {
        var anim = new DoubleAnimation
        {
            To = toOpacity,
            Duration = FadeDuration,
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };

        if (toOpacity != 0)
        {
            Tip.Visibility = Visibility.Visible;
        }
        else
        {
            anim.Completed += (sender, args) => Tip.Visibility = Visibility.Collapsed;
        }

        Tip.BeginAnimation(OpacityProperty, anim);
    }
}