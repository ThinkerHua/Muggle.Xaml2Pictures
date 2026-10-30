using System.Windows;

namespace Muggle.Xaml2Pictures.ViewModels;

/// <summary>
/// 带有限制区域的视口。
/// </summary>
/// <remarks>提供的所有操作 (Scale 和 Pan) 均是对视口本身的操作，观察到的效果与操作是相反的：
/// 往左下角移动视口，观察到的效果是对象往右上角移动；缩小视口，观察到的效果是对象放大。</remarks>
public class ViewPortWithBounds
{
    private const double MinSize = 0.001;

    private Point _viewPoint;
    private double _width;
    private double _height;
    private double _ratio;

    public Point ViewPoint
    {
        get => _viewPoint;
        set
        {
            if (_viewPoint.Equals(value)) return;
            _viewPoint = value;
            FitLimits();
        }
    }

    public double Ratio
    {
        get => _ratio;
        set
        {
            if (_ratio.Equals(value))
            {
                return;
            }

            if (_ratio < value)
            {
                _width = _height * value;
            }
            else
            {
                _height = _width / value;
            }

            _ratio = value;

            FitLimits();
        }
    }

    public double Width
    {
        get => _width;
        set
        {
            if (_width.Equals(value)) return;
            _width = value;
            _height = _width / Ratio;
            FitLimits();
        }
    }

    public double Height
    {
        get => _height;
        set
        {
            if (_height.Equals(value)) return;
            _height = value;
            _width = _height * Ratio;
            FitLimits();
        }
    }

    public Rect Bounds
    {
        get;
        set
        {
            if (field.Equals(value)) return;
            var limitX = value.Width < 0 ? value.X + value.Width : value.X;
            var limitY = value.Height < 0 ? value.Y + value.Height : value.Y;
            var limitWidth = Math.Abs(value.Width) < MinSize ? MinSize : Math.Abs(value.Width);
            var limitHeight = Math.Abs(value.Height) < MinSize ? MinSize : Math.Abs(value.Height);
            field = new Rect(limitX, limitY, limitWidth, limitHeight);
            FitLimits();
        }
    }

    public ViewPortWithBounds(Point viewPoint, double width, double height, Rect bounds)
    {
        _viewPoint = viewPoint;
        _width = width;
        _height = height;
        _ratio = width / height;
        Bounds = bounds;
    }

    public ViewPortWithBounds(Rect viewPort, Rect bounds)
        : this(new Point(viewPort.X + viewPort.Width * 0.5, viewPort.Y + viewPort.Height * 0.5),
            viewPort.Width, viewPort.Height, bounds)
    {
    }

    private void FitLimits()
    {
        if (_width > Bounds.Width && _height > Bounds.Height)
        {
            ScaleMax();
            return;
        }

        if (_width < MinSize || _height < MinSize)
        {
            if (Ratio < 1)
            {
                _width = MinSize;
                _height = _width / Ratio;
            }
            else
            {
                _height = MinSize;
                _width = _height * Ratio;
            }
        }

        if (_viewPoint.X - _width * 0.5 < Bounds.X)
            _viewPoint.X = Bounds.X + Math.Min(_width, Bounds.Width) * 0.5;
        if (_viewPoint.X + _width * 0.5 > Bounds.X + Bounds.Width)
            _viewPoint.X = Bounds.X + Bounds.Width - Math.Min(_width, Bounds.Width) * 0.5;
        if (_viewPoint.Y - _height * 0.5 < Bounds.Y)
            _viewPoint.Y = Bounds.Y + Math.Min(_height, Bounds.Height) * 0.5;
        if (_viewPoint.Y + _height * 0.5 > Bounds.Y + Bounds.Height)
            _viewPoint.Y = Bounds.Y + Bounds.Height - Math.Min(_height, Bounds.Height) * 0.5;
    }

    public Rect ToRect()
    {
        return new Rect(ViewPoint.X - Width * 0.5, ViewPoint.Y - Height * 0.5, Width, Height);
    }

    public Point GetPosition(double percentOfHalfWidth, double percentOfHalfHeight)
    {
        return new Point(_viewPoint.X + percentOfHalfWidth * _width * 0.5,
            _viewPoint.Y + percentOfHalfHeight * _height * 0.5);
    }

    public void Pan(Vector vector)
    {
        ViewPoint += vector;
    }

    public void ScaleX(double scale)
    {
        Width *= scale;
    }

    public void ScaleY(double scale)
    {
        Height *= scale;
    }

    public void Scale(double scale)
    {
        _width *= scale;
        _height *= scale;
        FitLimits();
    }

    public void ScaleMax()
    {
        _viewPoint = new Point(Bounds.X + Bounds.Width * 0.5, Bounds.Y + Bounds.Height * 0.5);
        if (Ratio < Bounds.Width / Bounds.Height)
        {
            _width = Bounds.Width;
            _height = _width / Ratio;
        }
        else
        {
            _height = Bounds.Height;
            _width = _height * Ratio;
        }
    }

    public void ScaleAt(double scale, Point position)
    {
        _viewPoint.X = position.X - scale * (position.X - _viewPoint.X);
        _viewPoint.Y = position.Y - scale * (position.Y - _viewPoint.Y);
        _width *= scale;
        _height *= scale;
        FitLimits();
    }
}