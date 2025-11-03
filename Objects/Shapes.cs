using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LSSyncApp.Objects
{
    class Shapes : Control
    {
        public enum _Shape
        {
            Circle,
            Line,
            Square,
            Triangle,
            Rectangle,
            Prompt
        }

        public enum _PromptTailPosition
        {
            BottomRight,
            BottomLeft,
            BottomCenter,
            TopRight,
            TopLeft,
            TopCenter,
            RightTop,
            RightBottom,
            RightCenter,
            LeftTop,
            LeftBottom,
            LeftCenter
        }

        public enum _PolygonType
        {
            Equilateral,
            RightAngle
        }

        /*public enum _IsRounded
        {
            Yes,
            No
        }
        */

        private Color _Color = Color.DarkCyan;
        private Color _LineColor = Color.DarkCyan;
        private _Shape ShapeType;
        private _PolygonType _PolyType = _PolygonType.Equilateral;
        private _PromptTailPosition _PTP = _PromptTailPosition.BottomRight;
        private int TailSize = 30, TailStartPosition = 25;
        //private _IsRounded IsRounded = _IsRounded.No;
        private LinearGradientMode _GradientMode = LinearGradientMode.ForwardDiagonal;
        private float _BorderWidth = 10f;
        private GlobalVariable _GlobalVariable = new GlobalVariable();

        public Shapes()
        {
            Width = 200;
            Height = 200;
            ShapeType = _Shape.Circle;
            LineColor = _LineColor;
        }

        private void SetStandardSize()
        {
            int _Size = Math.Max(Width, Height);
            Size = new Size(_Size, _Size);
        }

        [Description("Fill Color for the shape"), Category("Appearance")]
        public Color FillColor
        {
            get { return _Color; }
            set
            {
                _Color = value;
                Invalidate();
            }
        }

        [Description("Polygon Types"), Category("Appearance")]
        public _PolygonType PolygonType
        {
            get { return _PolyType; }
            set
            {
                _PolyType = value;
                Invalidate();
            }
        }

        [Description("Prompt Box Tail Position"), Category("Appearance")]
        public _PromptTailPosition PromptTailPosition
        {
            get { return _PTP; }
            set
            {
                _PTP = value;
                Invalidate();
            }
        }

        [Description("Prompt Box Tail Size"), Category("Appearance")]
        public int PromptTailSize
        {
            get { return TailSize; }
            set
            {
                TailSize = value;
                Invalidate();
            }
        }

        /*
        [Description("Rounded Edges"), Category("Appearance")]
        public _IsRounded RoundedEdge
        {
            get { return IsRounded; }
            set
            {
                IsRounded = value;
                Invalidate();
            }
        }
        */

        [Description("Initial Color of the Progress Bar 1"), Category("Appearance")]
        public _Shape Shape
        {
            get { return ShapeType; }
            set
            {
                ShapeType = value;
                Invalidate();
            }
        }


        [Description("Intermediate Line Color"), Category("Appearance")]
        public Color LineColor
        {
            get { return _LineColor; }
            set
            {
                _LineColor = value;
                Invalidate();
            }
        }

        [Description("Color Gradient Mode"), Category("Appearance")]
        public LinearGradientMode GradientMode
        {
            get { return _GradientMode; }
            set
            {
                _GradientMode = value;
                Invalidate();
            }
        }

        [Description("Progress Bar Width"), Category("Appearance")]
        public float BorderWidth
        {
            get { return _BorderWidth; }
            set
            {
                _BorderWidth = value;
                Invalidate();
            }
        }

        private static void PaintTransparentBackground(Control c, PaintEventArgs e)
        {
            if (c.Parent == null || !Application.RenderWithVisualStyles)
                return;

            ButtonRenderer.DrawParentBackground(e.Graphics, c.ClientRectangle, c);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Bitmap bitmap = new Bitmap(Width, Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    PaintTransparentBackground(this, e);

                    switch (Shape)
                    {
                        case _Shape.Circle:
                            //Draw the inner white circle:
                            using (Brush mBackColor = new SolidBrush(FillColor))
                            {
                                graphics.FillEllipse(mBackColor, 18, 18, (Width - 0x30) + 12, (Height - 0x30) + 12);
                            }
                            break;
                        case _Shape.Rectangle:
                        case _Shape.Square:
                            using (Brush mBackColor = new SolidBrush(FillColor))
                            {
                                graphics.FillRectangle(mBackColor, 0, 0, Width, Height);
                            }
                            break;
                        case _Shape.Prompt:
                        case _Shape.Triangle:
                            PointF[] curvePoints = CurvePoints();
                            using (Brush mBackColor = new SolidBrush(FillColor))
                            {
                                graphics.FillPolygon(mBackColor, curvePoints);
                            }
                            break;
                    }

                    //Draw the Progress Bar
                    using (LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle,
                        _LineColor, _LineColor, GradientMode))
                    {
                        using (Pen pen = new Pen(brush, BorderWidth))
                        {
                            pen.StartCap = LineCap.Flat;
                            pen.EndCap = LineCap.Flat;

                            switch (Shape)
                            {
                                case _Shape.Circle:
                                    graphics.DrawArc(pen, 0x12, 0x12, (Width - 0x23) - 2, (Height - 0x23) - 2, -90, 360);
                                    break;
                                case _Shape.Rectangle:
                                case _Shape.Square:
                                    if (Shape == _Shape.Square) SetStandardSize();
                                    Rectangle _NewRectangle = new Rectangle(0, 0, Width, Height);
                                    graphics.DrawRectangle(pen, _NewRectangle);
                                    break;
                                case _Shape.Line:
                                    graphics.DrawLine(pen, 10, 10, Width, 10);
                                    break;
                                case _Shape.Prompt:
                                case _Shape.Triangle:
                                    PointF[] curvePoints = CurvePoints();
                                    // Draw polygon curve to screen.
                                    graphics.DrawPolygon(pen, curvePoints);
                                    break;
                            }
                        }
                    }
                    //if(RoundedEdge == _IsRounded.Yes)
                    //{
                    if (Shape == _Shape.Prompt)
                    {
                        this.Region = Region.FromHrgn(_GlobalVariable.createRoundRect(0, 0, this.Width, this.Height, 50, 50));
                    }
                    //}

                    e.Graphics.DrawImage(bitmap, 0, 0);
                    graphics.Dispose();
                    bitmap.Dispose();
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (Shape == _Shape.Square)
            {
                base.OnResize(e);
                SetStandardSize();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (Shape == _Shape.Square)
            {
                base.OnSizeChanged(e);
                SetStandardSize();
            }
        }

        private PointF[] CurvePoints()
        {
            PointF[] curvePoints = { };
            PointF point1, point2, point3, point4, point5, point6, point7;

            switch (Shape)
            {
                case _Shape.Prompt:
                    switch (PromptTailPosition)
                    {
                        case _PromptTailPosition.BottomCenter:
                            point1 = new PointF(10, 10);
                            point2 = new PointF(Width - 10, 10);
                            point3 = new PointF(Width - 10, Height - TailSize);
                            point4 = new PointF((Width / 2) + (TailSize / 2), Height - TailSize);
                            point5 = new PointF((Width / 2), Height - 10);
                            point6 = new PointF((Width / 2) - (TailSize / 2), Height - TailSize);
                            point7 = new PointF(10, Height - TailSize);
                            break;
                        case _PromptTailPosition.BottomRight:
                            point1 = new PointF(10, 10);
                            point2 = new PointF(Width - 10, 10);
                            point3 = new PointF(Width - 10, Height - TailSize);
                            point4 = new PointF(Width - TailStartPosition, Height - TailSize);
                            point5 = new PointF(Width - (TailStartPosition + (TailSize / 2)), Height - 10);
                            point6 = new PointF(Width - (TailStartPosition + TailSize), Height - TailSize);
                            point7 = new PointF(10, Height - TailSize);
                            break;
                        case _PromptTailPosition.BottomLeft:
                            point1 = new PointF(10, 10);
                            point2 = new PointF(Width - 10, 10);
                            point3 = new PointF(Width - 10, Height - TailSize);
                            point4 = new PointF((TailStartPosition + TailSize), Height - TailSize);
                            point5 = new PointF((TailStartPosition + (TailSize / 2)), Height - 10);
                            point6 = new PointF(TailStartPosition, Height - TailSize);
                            point7 = new PointF(10, Height - TailSize);
                            break;
                        case _PromptTailPosition.TopCenter:
                            point1 = new PointF(10, TailSize);
                            point2 = new PointF((Width / 2) - (TailSize / 2), TailSize);
                            point3 = new PointF((Width / 2), 10);
                            point4 = new PointF((Width / 2) + (TailSize / 2), TailSize);
                            point5 = new PointF(Width - 10, TailSize);
                            point6 = new PointF(Width - 10, Height - 10);
                            point7 = new PointF(10, Height - 10);
                            break;
                        case _PromptTailPosition.TopRight:
                            point1 = new PointF(10, TailSize);
                            point2 = new PointF(Width - (TailStartPosition + TailSize), TailSize);
                            point3 = new PointF(Width - (TailStartPosition + (TailSize / 2)), 10);
                            point4 = new PointF(Width - TailStartPosition, TailSize);
                            point5 = new PointF(Width - 10, TailSize);
                            point6 = new PointF(Width - 10, Height - 10);
                            point7 = new PointF(10, Height - 10);
                            break;
                        case _PromptTailPosition.TopLeft:
                            point1 = new PointF(10, TailSize);
                            point2 = new PointF(TailStartPosition, TailSize);
                            point3 = new PointF((TailStartPosition + (TailSize / 2)), 10);
                            point4 = new PointF((TailStartPosition + TailSize), TailSize);
                            point5 = new PointF(Width - 10, TailSize);
                            point6 = new PointF(Width - 10, Height - 10);
                            point7 = new PointF(10, Height - 10);
                            break;
                        case _PromptTailPosition.LeftBottom:
                            point1 = new PointF(TailSize, 10);
                            point2 = new PointF(Width - 10, 10);
                            point3 = new PointF(Width - 10, Height - 10);
                            point4 = new PointF(TailSize, Height - 10);
                            point5 = new PointF(TailSize, Height - TailStartPosition);
                            point6 = new PointF(10, Height - (TailStartPosition + (TailSize / 2)));
                            point7 = new PointF(TailSize, Height - (TailStartPosition + TailSize));
                            break;
                        case _PromptTailPosition.LeftCenter:
                            point1 = new PointF(TailSize, 10);
                            point2 = new PointF(Width - 10, 10);
                            point3 = new PointF(Width - 10, Height - 10);
                            point4 = new PointF(TailSize, Height - 10);
                            point5 = new PointF(TailSize, (Height / 2) + (TailSize / 2));
                            point6 = new PointF(10, (Height / 2));
                            point7 = new PointF(TailSize, (Height / 2) - (TailSize / 2));
                            break;
                        case _PromptTailPosition.LeftTop:
                            point1 = new PointF(TailSize, 10);
                            point2 = new PointF(Width - 10, 10);
                            point3 = new PointF(Width - 10, Height - 10);
                            point4 = new PointF(TailSize, Height - 10);
                            point5 = new PointF(TailSize, (TailStartPosition + TailSize));
                            point6 = new PointF(10, (TailStartPosition + (TailSize / 2)));
                            point7 = new PointF(TailSize, TailStartPosition);
                            break;
                        case _PromptTailPosition.RightBottom:
                            point1 = new PointF(10, 10);
                            point2 = new PointF(Width - TailSize, 10);
                            point3 = new PointF(Width - TailSize, Height - (TailStartPosition + TailSize));
                            point4 = new PointF(Width - 10, Height - (TailStartPosition + (TailSize / 2)));
                            point5 = new PointF(Width - TailSize, Height - TailStartPosition);
                            point6 = new PointF(Width - TailSize, Height - 10);
                            point7 = new PointF(10, Height - 10);
                            break;
                        case _PromptTailPosition.RightCenter:
                            point1 = new PointF(10, 10);
                            point2 = new PointF(Width - TailSize, 10);
                            point3 = new PointF(Width - TailSize, (Height / 2) - (TailSize / 2));
                            point4 = new PointF(Width - 10, (Height / 2));
                            point5 = new PointF(Width - TailSize, (Height / 2) + (TailSize / 2));
                            point6 = new PointF(Width - TailSize, Height - 10);
                            point7 = new PointF(10, Height - 10);
                            break;
                        case _PromptTailPosition.RightTop:
                            point1 = new PointF(10, 10);
                            point2 = new PointF(Width - TailSize, 10);
                            point3 = new PointF(Width - TailSize, TailStartPosition);
                            point4 = new PointF(Width - 10, (TailStartPosition + (TailSize / 2)));
                            point5 = new PointF(Width - TailSize, (TailStartPosition + TailSize));
                            point6 = new PointF(Width - TailSize, Height - 10);
                            point7 = new PointF(10, Height - 10);
                            break;
                        default:
                            point1 = new PointF(10, 10);
                            point2 = new PointF(Width - 10, 10);
                            point3 = new PointF(Width - 10, Height - TailSize);
                            point4 = new PointF(Width - TailStartPosition, Height - TailSize);
                            point5 = new PointF(Width - (TailStartPosition + (TailSize / 2)), Height - 10);
                            point6 = new PointF(Width - TailSize, Height - TailSize);
                            point7 = new PointF(10, Height - TailSize);
                            break;
                    }

                    curvePoints = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                    break;
                case _Shape.Triangle:
                    switch (PolygonType)
                    {
                        case _PolygonType.Equilateral:
                            point1 = new PointF(Width / 2, 6);
                            point2 = new PointF(Width - 6, Height - 6);
                            point3 = new PointF(6, Height - 6);
                            curvePoints = new PointF[] { point1, point2, point3 };
                            break;
                        case _PolygonType.RightAngle:
                            point1 = new PointF(10, 10);
                            point2 = new PointF(Width - 10, Height - 10);
                            point3 = new PointF(10, Height - 10);
                            curvePoints = new PointF[] { point1, point2, point3 };
                            break;
                    }
                    break;
            }

            return curvePoints;
        }
    }
}
