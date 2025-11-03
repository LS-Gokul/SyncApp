using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LSSyncApp.Objects
{
    class CircularProgressBar : Control
    {
        #region Enums
        public enum _ProgressShape
        {
            Round,
            Flat
        }

        public enum _TextMode
        {
            None,
            Value,
            Percentage,
            Custom
        }

        public enum _ShadowMode
        {
            Yes,
            No
        }

        public enum _ToolTipRemoveNeLine
        {
            Yes,
            No
        }

        public enum _SecondBar
        {
            Yes,
            No
        }

        #endregion

        #region Private Variables

        private long _Value;
        private long _Maximum = 100;
        private int _LineWitdh = 1;
        private float _BarWidth = 14f;

        private Color _FirstBarColor1 = Color.Orange, _FirstBarColor2 = Color.Orange;
        private Color _SecondBarColor1 = Color.Orange, _SecondBarColor2 = Color.Orange;
        private Color _TextColor = Color.White;
        private Color _ShadowTextColor = Color.Yellow;
        private Color _LineColor = Color.Silver;
        private LinearGradientMode _GradientMode = LinearGradientMode.ForwardDiagonal;
        private _ProgressShape ProgressShapeVal;
        private _TextMode ProgressTextMode;
        private _ShadowMode TextShadow;
        private _SecondBar SecBar;
        private _ToolTipRemoveNeLine RmTTNL;
        private string _ToolTipText;

        #endregion

        #region Contructor

        public CircularProgressBar()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            this.BackColor = SystemColors.Control;
            this.ForeColor = Color.DimGray;

            this.Size = new Size(130, 130);

            this.Font = new Font("Segoe UI", 15);
            this.MinimumSize = new Size(100, 100);
            this.DoubleBuffered = true;

            this.LineWidth = 1;
            this.LineColor = Color.DimGray;

            Value = 57;
            ProgressShape = _ProgressShape.Flat;
            TextMode = _TextMode.Percentage;
            ShadowMode = _ShadowMode.No;
            SecondBar = _SecondBar.No;
            ToolTipRemoveNeLine = _ToolTipRemoveNeLine.No;
        }

        #endregion

        #region Public Custom Properties

        [Description("Integer value that determines the position of the Progress Bar."), Category("Behavior")]
        public long Value
        {
            get { return _Value; }
            set
            {
                if (value > _Maximum)
                    value = _Maximum;
                _Value = value;
                Invalidate();
            }
        }

        [Description("Get or Set the Maximum Value of the Progress bar."), Category("Behavior")]
        public long Maximum
        {
            get { return _Maximum; }
            set
            {
                if (value < 1)
                    value = 1;
                _Maximum = value;
                Invalidate();
            }
        }

        [Description("Initial Color of the Progress Bar 1"), Category("Appearance")]
        public Color FirstBarColor1
        {
            get { return _FirstBarColor1; }
            set
            {
                _FirstBarColor1 = value;
                Invalidate();
            }
        }

        [Description("Initial Color of the Progress Bar 2"), Category("Appearance")]
        public Color SecondBarColor1
        {
            get { return _SecondBarColor1; }
            set
            {
                _SecondBarColor1 = value;
                Invalidate();
            }
        }

        [Description("Text Color"), Category("Appearance")]
        public Color TextColor
        {
            get { return _TextColor; }
            set
            {
                _TextColor = value;
                Invalidate();
            }
        }

        [Description("Text Shadow Color"), Category("Appearance")]
        public Color TextShadowColor
        {
            get { return _ShadowTextColor; }
            set
            {
                _ShadowTextColor = value;
                Invalidate();
            }
        }

        [Description("Progress Bar 1 End Color"), Category("Appearance")]
        public Color FirstBarColor2
        {
            get { return _FirstBarColor2; }
            set
            {
                _FirstBarColor2 = value;
                Invalidate();
            }
        }

        [Description("Progress Bar 2 End Color"), Category("Appearance")]
        public Color SecondBarColor2
        {
            get { return _SecondBarColor2; }
            set
            {
                _SecondBarColor2 = value;
                Invalidate();
            }
        }

        [Description("Progress Bar Width"), Category("Appearance")]
        public float BarWidth
        {
            get { return _BarWidth; }
            set
            {
                _BarWidth = value;
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

        [Description("Intermediate Line Width"), Category("Appearance")]
        public int LineWidth
        {
            get { return _LineWitdh; }
            set
            {
                _LineWitdh = value;
                Invalidate();
            }
        }

        [Description("Gets or Sets the Shape of the progress bar terminals."), Category("Appearance")]
        public _ProgressShape ProgressShape
        {
            get { return ProgressShapeVal; }
            set
            {
                ProgressShapeVal = value;
                Invalidate();
            }
        }


        [Description("Gets or Sets the Tool Tip."), Category("Behavior")]
        public string ToolTipText
        {
            get { return _ToolTipText; }
            set
            {
                _ToolTipText = value;
                Invalidate();
            }
        }

        [Description("Gets or Sets the Tool Tip New Line has to remove or not."), Category("Behavior")]
        public _ToolTipRemoveNeLine ToolTipRemoveNeLine
        {
            get { return RmTTNL; }
            set
            {
                RmTTNL = value;
                Invalidate();
            }
        }

        [Description("Gets or Sets the Mode in which the Text is displayed inside the Progress bar."), Category("Behavior")]
        public _TextMode TextMode
        {
            get { return ProgressTextMode; }
            set
            {
                ProgressTextMode = value;
                Invalidate();
            }
        }

        [Description("Gets or Sets the Shadow Required."), Category("Behavior")]
        public _ShadowMode ShadowMode
        {
            get { return TextShadow; }
            set
            {
                TextShadow = value;
                Invalidate();
            }
        }

        [Description("Gets or Sets the Second Bar Required."), Category("Behavior")]
        public _SecondBar SecondBar
        {
            get { return SecBar; }
            set
            {
                SecBar = value;
                Invalidate();
            }
        }

        [Description("Gets the Text that is displayed inside the Control"), Category("Behavior")]
        public override string Text { get; set; }

        #endregion

        #region EventArgs

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetStandardSize();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetStandardSize();
        }

        protected override void OnPaintBackground(PaintEventArgs p)
        {
            base.OnPaintBackground(p);
        }

        #endregion

        #region Methods

        private void SetStandardSize()
        {
            int _Size = Math.Max(Width, Height);
            Size = new Size(_Size, _Size);
        }

        public void Increment(int Val)
        {
            this._Value += Val;
            Invalidate();
        }

        public void Decrement(int Val)
        {
            this._Value -= Val;
            Invalidate();
        }
        #endregion

        #region Events

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Bitmap bitmap = new Bitmap(this.Width, this.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    //graphics.Clear(Color.Transparent); //<-- this.BackColor, SystemColors.Control, Color.Transparent

                    PaintTransparentBackground(this, e);

                    //Draw the inner white circle:
                    using (Brush mBackColor = new SolidBrush(this.BackColor))
                    {
                        graphics.FillEllipse(mBackColor,
                                18, 18,
                                (this.Width - 0x30) + 12,
                                (this.Height - 0x30) + 12);
                    }
                    // Draw the thin gray line in the middle:
                    using (Pen pen2 = new Pen(LineColor, this.LineWidth))
                    {
                        graphics.DrawEllipse(pen2,
                            18, 18,
                          (this.Width - 0x30) + 12,
                          (this.Height - 0x30) + 12);
                    }

                    //Draw the Progress Bar
                    using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                        this._FirstBarColor1, this._FirstBarColor2, this.GradientMode))
                    {
                        using (Pen pen = new Pen(brush, this.BarWidth))
                        {
                            switch (this.ProgressShapeVal)
                            {
                                case _ProgressShape.Round:
                                    pen.StartCap = LineCap.Round;
                                    pen.EndCap = LineCap.Round;
                                    break;

                                case _ProgressShape.Flat:
                                    pen.StartCap = LineCap.Flat;
                                    pen.EndCap = LineCap.Flat;
                                    break;
                            }

                            //Here the Progress Bar is actually drawn
                            graphics.DrawArc(pen,
                                0x12, 0x12,
                                (this.Width - 0x23) - 2,
                                (this.Height - 0x23) - 2,
                                -90,
                                (int)Math.Round((double)((360.0 / ((double)this._Maximum)) * this._Value)));
                        }
                    }

                    #region Draw Progress Text

                    switch (this.TextMode)
                    {
                        case _TextMode.None:
                            this.Text = string.Empty;
                            break;

                        case _TextMode.Value:
                            this.Text = _Value.ToString();
                            break;

                        case _TextMode.Percentage:
                            this.Text = Convert.ToString(Convert.ToInt32((100 / _Maximum) * _Value));
                            break;

                        default:
                            break;
                    }

                    if (this.Text != string.Empty)
                    {
                        using (Brush FontColor = new SolidBrush(this._TextColor))
                        {
                            int ShadowOffset = 2;
                            //using (Font = new Font("Ebrima", (Width / 10) >= 15 ? 15 : (Width / 100) * 8))
                            //{
                            // Create a StringFormat object with the each line of text, and the block
                            // of text centered on the page.
                            Font = new Font("Ebrima", (Width / 10) >= 15 ? 15 : (Width / 100) * 8);
                            Rectangle rect1 = new Rectangle(0, 0, this.Width, this.Height);

                            StringFormat stringFormat = new StringFormat();
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;

                            SizeF MS1 = graphics.MeasureString(Text, Font);
                            if (TextShadow == _ShadowMode.Yes)
                            {
                                SolidBrush shadowBrush = new SolidBrush(_ShadowTextColor);
                                //Text Shadow:
                                graphics.DrawString(Text, Font, shadowBrush,
                                    Convert.ToInt32(Width / 2 - MS1.Width / 2) + ShadowOffset,
                                    Convert.ToInt32(Height / 2 - MS1.Height / 2) + ShadowOffset
                                );
                            }
                            graphics.DrawString(Text, Font, FontColor, rect1, stringFormat);
                            //}

                            /*
                            this.Font = new Font("Ebrima", (this.Width / 10) >= 15 ? 15 : (this.Width / 100) * 8);
                            SizeF MS = graphics.MeasureString(this.Text, this.Font);
                            if (this.TextShadow == _ShadowMode.Yes)
                            {
                                SolidBrush shadowBrush = new SolidBrush(this._ShadowTextColor);
                                //Text Shadow:
                                graphics.DrawString(this.Text, this.Font, shadowBrush,
                                    Convert.ToInt32(Width / 2 - MS.Width / 2) + ShadowOffset,
                                    Convert.ToInt32(Height / 2 - MS.Height / 2) + ShadowOffset
                                );
                            }
                            //Check Text:
                            MessageBox.Show((Width / 2).ToString() + "-" + (MS.Width / 2).ToString());
                            graphics.DrawString(this.Text, this.Font, FontColor,
                                Convert.ToInt32(Width / 2 - MS.Width / 2),
                                Convert.ToInt32(Height / 2 - MS.Height / 2));
                            */
                        }

                        ToolTip toolTip = new ToolTip();
                        string lsText = ToolTipText == null || ToolTipText == "" ? this.Text : ToolTipText;
                        toolTip.SetToolTip(this, this.RmTTNL == _ToolTipRemoveNeLine.Yes ? lsText.Replace(Environment.NewLine, " ") : lsText);
                    }


                    #endregion

                    /////////////////////////////////2nd Image/////////////////////////////////
                    if (this.SecondBar == _SecondBar.Yes)
                    {
                        using (Bitmap bitmap1 = new Bitmap((this.Width / 10) * 8, (this.Height / 10) * 8))
                        {
                            using (Graphics graphics1 = Graphics.FromImage(bitmap1))
                            {
                                graphics1.InterpolationMode = InterpolationMode.HighQualityBilinear;
                                graphics1.CompositingQuality = CompositingQuality.HighQuality;
                                graphics1.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                graphics1.SmoothingMode = SmoothingMode.AntiAlias;

                                //graphics.Clear(Color.Transparent); //<-- this.BackColor, SystemColors.Control, Color.Transparent

                                PaintTransparentBackground(this, e);

                                //Draw the inner white circle:
                                using (Brush mBackColor = new SolidBrush(this.BackColor))
                                {
                                    graphics1.FillEllipse(mBackColor,
                                            18, 18,
                                            (((this.Width / 10) * 8) - 0x30) + 12,
                                            (((this.Height / 10) * 8) - 0x30) + 12);
                                }
                                // Draw the thin gray line in the middle:
                                using (Pen pen2 = new Pen(LineColor, this.LineWidth))
                                {
                                    graphics1.DrawEllipse(pen2,
                                        18, 18,
                                      (((this.Width / 10) * 8) - 0x30) + 12,
                                      (((this.Height / 10) * 8) - 0x30) + 12);
                                }

                                //Draw the Progress Bar
                                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                                    this._SecondBarColor1, this._SecondBarColor2, this.GradientMode))
                                {
                                    using (Pen pen = new Pen(brush, this.BarWidth))
                                    {
                                        switch (this.ProgressShapeVal)
                                        {
                                            case _ProgressShape.Round:
                                                pen.StartCap = LineCap.Round;
                                                pen.EndCap = LineCap.Round;
                                                break;

                                            case _ProgressShape.Flat:
                                                pen.StartCap = LineCap.Flat;
                                                pen.EndCap = LineCap.Flat;
                                                break;
                                        }

                                        //Here the Progress Bar is actually drawn
                                        graphics1.DrawArc(pen,
                                            0x12, 0x12,
                                            (((this.Width / 10) * 8) - 0x23) - 2,
                                            (((this.Height / 10) * 8) - 0x23) - 2,
                                            -90,
                                            (int)Math.Round((double)((360.0 / ((double)this._Maximum)) * this._Value)));
                                    }
                                }

                                using (Brush FontColor = new SolidBrush(this._TextColor))
                                {
                                    //MessageBox.Show(this.Width.ToString());
                                    this.Font = new Font("Ebrima", (this.Width / 10) >= 15 ? 15 : (this.Width / 100) * 8);

                                    //Check Text:
                                    string lsText = "ABCDEFGHT";
                                    for (int i = 0; i < lsText.Length; i++)
                                    {
                                        //graphics1.RotateTransform(0 - (i * 5));
                                        graphics1.RotateTransform(-1 * ((i > 0 ? i : 0) + 5));
                                        graphics1.DrawString(lsText.Substring(i, 1), this.Font, FontColor,
                                            (this.Width / 2) + (i > 0 ? 1 : 0),
                                            ((this.Height / 100) * 65) - (i > 0 ? 1 : 0)
                                            );
                                    }
                                }

                                //Here the whole Control is Drawn:
                                e.Graphics.DrawImage(bitmap1, this.Width / 10, this.Height / 10);
                                graphics1.Dispose();
                                bitmap1.Dispose();
                            }
                        }
                    }
                    /////////////////////////////////2nd Image////////////////////////////////
                    //Here the whole Control is Drawn:
                    e.Graphics.DrawImage(bitmap, 0, 0);
                    graphics.Dispose();
                    bitmap.Dispose();
                }
            }
            //MessageBox.Show(this.Height.ToString() + " - " + this.Width.ToString());
        }

        private static void PaintTransparentBackground(Control c, PaintEventArgs e)
        {
            if (c.Parent == null || !Application.RenderWithVisualStyles)
                return;

            ButtonRenderer.DrawParentBackground(e.Graphics, c.ClientRectangle, c);
        }
        /*
        /// <summary>Draw a Circle Filled with Color with Perfect Edges.</summary>
        /// <param name="g">'Canvas' of the Object where it will be drawn</param>
        /// <param name="brush">Fill color and style</param>
        /// <param name="centerX">Center of the Circle, on the X axis</param>
        /// <param name="centerY">Center of the Circle, on the Y axis</param>
        /// <param name="radius">Circle Radius</param>
        private void FillCircle(Graphics g, Brush brush, float centerX, float centerY, float radius)
        {
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath gp = new GraphicsPath())
            {
                g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
            }
        }*/
        #endregion
    }
}
