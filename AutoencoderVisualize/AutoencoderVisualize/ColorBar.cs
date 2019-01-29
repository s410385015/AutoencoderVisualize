using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
namespace AutoencoderVisualize
{
    public partial class ColorBar : UserControl
    {
        public double max;
        public double min;
        public double center;
        public Color c_max;
        public Color c_min;
        public Color c_center;
        public int range;
        public int labeloffset;
        public int padTop;
        private Graphics g;
        private bool flag = false;
        public int width;
        public int height;
        public int pinSize = 4;
        public delegate void CallBack();
        public CallBack cbFunc;
        public ColorBar()
        {
            InitializeComponent();
            
            labeloffset=60;
            padTop=10;
        }

        private void ColorBar_Load(object sender, EventArgs e)
        {
            
        }

        public void SetSize(int w,int h,int gs)
        {
            this.Font = new System.Drawing.Font("Arial Unicode MS", gs / 2, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Size = new Size(w + labeloffset, h + padTop * 2);
            bar.Location = new Point(0, padTop);
            bar.Size = new Size(w+pinSize, h);
            width = w;
            height = h;
            
        }

        public void SetInfo(double _min,double _max,double _center,Color cmin,Color cmax,Color ccenter,int r)
        {
            max = _max;
            min = _min;
            center = _center;
            c_max = cmax;
            c_min = cmin;
            c_center = ccenter;
            range = r;
            flag = true;
            
            for (int i = 0; i < range;i++ )
            {
                int y = i * height / (2 * range)+padTop;
                double value=max-i*((max-center)/range);
                GenerateLabel(new Point(width + pinSize, y), value.ToString("0.00"));
            }
            GenerateLabel(new Point(width + pinSize, height/2+padTop), center.ToString("0.00"));

            for (int i = 1; i < range; i++)
            {
                int y = i * height / (2 * range) + padTop+height/2;
                double value =  i * ((min-center) / range);
                GenerateLabel(new Point(width + pinSize, y), value.ToString("0.00"));
            }

            int leftover = 2;
            GenerateLabel(new Point(width + pinSize, height+padTop-leftover), min.ToString("0.00"));
            NotifyRedraw();
        }

        private void bar_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (flag)
                Drawbar();
        }

        public void NotifyRedraw()
        {
            bar.Invalidate();
        }

        public void Drawbar()
        {
            int penSize = 1;
            Pen boxPen = new Pen(Color.Black, penSize);

            LinearGradientBrush ToplinGrBrush = new LinearGradientBrush(
               new Point(0, 0),
               new Point(0, height/2),
               c_max,   
               c_center);  
           
            g.FillRectangle(ToplinGrBrush, 0, 0, width-1, height/2);

            LinearGradientBrush BottomlinGrBrush = new LinearGradientBrush(
               new Point(0, height/2-1),
               new Point(0, height),
               c_center,  
               c_min);  

            g.FillRectangle(BottomlinGrBrush, 0, height / 2-1, width-1, height / 2);

            g.DrawRectangle(boxPen, new Rectangle(0, 0, width - penSize, height - penSize ));


            for(int i=0;i<range*2;i++)
            {
                int y = i * height / (2 * range);
                g.DrawLine(boxPen, new Point(width, y), new Point(width + pinSize, y));
            }
            g.DrawLine(boxPen, new Point(width, height-1), new Point(width + pinSize, height-1));
        }

        public void GenerateLabel(Point p,string str)
        {
            Label label = new Label();

            
            label.Location = new Point(p.X,p.Y-label.Height/2);
            label.BackColor = Color.Transparent;
            label.Text = str;
            label.BringToFront();
            this.Controls.Add(label);
          
        }

        private void bar_Click(object sender, EventArgs e)
        {
            PictureBox l = sender as PictureBox;
            MouseEventArgs me = (MouseEventArgs)e;

            if(me.Button==MouseButtons.Right)
            {
                if (colorPicker.ShowDialog() == DialogResult.OK)
                {
                    if (me.Y < height / 3)
                    {
                        c_max = colorPicker.Color;
                    }
                    else if (me.Y > height / 3 * 2)
                    {
                        c_min = colorPicker.Color;
                    }
                    else
                    {
                        c_center = colorPicker.Color;
                    }
                    cbFunc();
                }
            }
        }

        public Color GetGradientColor(double value)
        {
            value = Math.Max(value, -0.3f);
            value = Math.Min(value, 0.3f);
            if(value>=center)
            {
                double numberOfIntervals = 100; 
                var interval_R = (c_max.R - c_center.R) / numberOfIntervals;
                var interval_G = (c_max.G - c_center.G) / numberOfIntervals;
                var interval_B = (c_max.B - c_center.B) / numberOfIntervals;

                var mul=value/((max-center)/numberOfIntervals);
                var color = Color.FromArgb((int)(interval_R * mul + c_center.R), (int)(interval_G * mul + c_center.G), (int)(interval_B * mul + c_center.B));
                return color;
            }else
            {
                double numberOfIntervals = 100;
                var interval_R = (c_min.R - c_center.R) / numberOfIntervals;
                var interval_G = (c_min.G - c_center.G) / numberOfIntervals;
                var interval_B = (c_min.B - c_center.B) / numberOfIntervals;

                var mul = value / ((min - center) / numberOfIntervals);
                var color = Color.FromArgb((int)(interval_R * mul + c_center.R), (int)(interval_G * mul + c_center.G), (int)(interval_B * mul + c_center.B));
                
                return color;
            }
        }
    }
}
