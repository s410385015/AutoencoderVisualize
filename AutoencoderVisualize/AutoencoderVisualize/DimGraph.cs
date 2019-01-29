using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace AutoencoderVisualize
{
    public partial class DimGraph : UserControl
    {
        public int row, col;
        public int gridsize;
        public int gridsize_col;
        public int gridsize_row;
        public int balanceSize;
        public List<double> grids;
        private Graphics g;
        private Random rnd = new Random();
        public int padTop,padLeft,padBottom,padRight;
        public List<Label> labels;
        public int labeloffset;
        public int lalelNum;
        public int selectRow;
        public int selectCol;
        public ColorBar colorbar;
        public Bitmap myBitmap;
        public bool verticalLabel = true;
        public bool horizontalLabel = true;
        public bool balance = false;
        public bool flag = false;
        public bool protectLock = true;
        private List<String> VerLabel;
        private List<String> HorLabel;
        private List<List<double>> data;
        public delegate void CallBack(int index);
        public CallBack cbFunc;
        public Color BoxColor = Color.Blue;

        public DimGraph()
        {
            InitializeComponent();
            padTop = 50;
            padLeft = 150;
            padBottom = 50;
            padRight = 50;
            labeloffset = 5;
            selectCol = -1;
            selectRow = -1;
            grid.Location = new Point(padLeft,padTop);
            labels = new List<Label>();
            VerLabel=new List<String>();
            HorLabel=new List<String>();
            data = new List<List<double>>();
        }

        private void DimGraph_Load(object sender, EventArgs e)
        {
            
        }

        public double GetRandomNumber(double minimum, double maximum)
        {

            return rnd.NextDouble() * (maximum - minimum) + minimum;
        }

        public void ColorDim()
        {
           
            
            for (int r = 0; r < data.Count; r++)
            {
                for (int c = 0; c < data[0].Count; c++)
                {
                    double value = data[r][c];
         
                    Color color = colorbar.GetGradientColor(value);
                    System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(color);
                    g.FillRectangle(myBrush, new Rectangle(c * gridsize+labeloffset, r * gridsize_row, gridsize, gridsize_row));
                }
                
            }

            

           
            
            
           
            //g.DrawLine(blackPen, new Point(labeloffset, penSize/2), new Point(grid.Width, penSize/2));
            //g.DrawLine(blackPen, new Point(labeloffset,0), new Point(labeloffset,grid.Height-1-labeloffset));
            //g.DrawLine(blackPen, new Point(grid.Width - penSize/2, 0), new Point(grid.Width - penSize/2, grid.Height - labeloffset));
            //g.DrawLine(blackPen, new Point(labeloffset-1, grid.Height - penSize/2 - labeloffset), new Point(grid.Width, grid.Height - penSize/2 - labeloffset));
        }

        
       

        private void grid_Paint(object sender, PaintEventArgs e)
        {
            if(!protectLock)
            {
                protectLock = true;
                return;
            }
            //g = e.Graphics;
            if (flag)
            {
                if (balance) { 
                    g = Graphics.FromImage(myBitmap);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    ColorDim();
                    

                    Bitmap resized = new Bitmap(myBitmap, new Size(balanceSize , balanceSize));

                    g = Graphics.FromImage(resized);

                    int penSize = 1;
                    Pen blackPen = new Pen(Color.Black, penSize);
                    g.DrawRectangle(blackPen, new Rectangle(labeloffset, 0, resized.Width-labeloffset-1, resized.Height-1));
                    DrawHighlight();
                    grid.Image = resized;
                    protectLock = false;
                    
                 
                } else
                {
                    g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    ColorDim();
                    DrawBoundary();
                    DrawHighlight();
                }
            }
        }

        public void DrawBoundary()
        {
            int penSize = 1;
            int pinSize = 1;
            Pen blackPen = new Pen(Color.Black, penSize);
            Pen smallPen = new Pen(Color.Black, pinSize);

            if (verticalLabel)
            {
                for (int c = 0; c < col; c++)
                {
                    int x = c * gridsize + (int)(gridsize / 2) + labeloffset;
                    int y = balanceSize + penSize;
                    g.DrawLine(smallPen, new Point(x, y), new Point(x, y + labeloffset));
                }
            }

            if (horizontalLabel)
            {
                for (int r = 0; r < row; r++)
                {
                    int y = r * gridsize + (int)(gridsize / 2);
                    g.DrawLine(smallPen, new Point(0, y), new Point(labeloffset, y));
                }
            }

           
            g.DrawRectangle(blackPen, new Rectangle(labeloffset, penSize / 2, balanceSize, balanceSize));
        }


        public void GenerateVerticalLabel(bool islabel, List<double> range, List<string> d)
        {
            if (balance)
            {
                if (islabel)
                {
                    for (int i = 0; i < VerLabel.Count; i++)
                    {
                        string str = VerLabel[i];
                        GenerateLabel(new Point(padLeft + i * gridsize_col + labeloffset, padTop + balanceSize + labeloffset * 2), true, str, "c" + i.ToString());
                        
                    }
                }
                else
                {
                    //TODO
                }
            }
        }
        public void GenerateHorizontalLabel(bool islabel, List<double> range, List<string> d)
        {
            if (balance)
            {
                if (islabel)
                {
                    for (int i = 0; i < HorLabel.Count; i++)
                    {
                        string str = HorLabel[i];
                        GenerateLabel(new Point(padLeft - gridsize * str.Length, padTop + (i) * gridsize_row), false, str, "r" + i.ToString());
                      
                    }
                }else
                {
                   ;

                    for(int i=0;i<d.Count;i++)
                    {
                        string str = d[i];
                        GenerateLabel(new Point(padLeft - gridsize * str.Length, padTop+(int)(balanceSize*range[i])-gridsize/2), false, str, "t" + i.ToString());
                    }
                }
            }
        }
        public void SetInfo(int r,int c,int gs,int gr,int gc,int ln,int bs,ColorBar cb)
        {
            row = r;
            col = c;
            gridsize = gs;
            gridsize_col = gc;
            gridsize_row = gr;
            lalelNum = ln;
            colorbar = cb;
            flag = true;
            balanceSize = bs;
            
            this.Font = new System.Drawing.Font("Arial Unicode MS", gridsize /2, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));


            grid.Location = new Point(padLeft, padTop);

            if(balance)
            {
                labeloffset = 0;
                myBitmap = new Bitmap(balanceSize, balanceSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                grid.Size = new Size(balanceSize,balanceSize);
                this.Size = new Size((int)(grid.Width * 1.5), (int)(grid.Height * 1.5));
                
            }
            else
            {
                
                grid.Size = new Size(col * gridsize_col + labeloffset * 2, row * gridsize_row + labeloffset * 2);
                //Arial Unicode MS
                this.Size = new Size((int)(grid.Width * 1.5), (int)(grid.Height * 1.5));

                if (verticalLabel)
                {
                    for (int i = 0; i < VerLabel.Count; i++)
                    {
                        string str=VerLabel[i];
                        GenerateLabel(new Point(padLeft + i * gridsize_col + labeloffset, padTop + (row) * gridsize_row + labeloffset * 2), true, str, "c" + i.ToString());

                    }
                }
                if (horizontalLabel)
                {
                    for (int i = 0; i < HorLabel.Count; i++)
                    {
                        string str=HorLabel[i];
                        GenerateLabel(new Point(padLeft - gridsize * str.Length, padTop + (i) * gridsize_row), false, str, "r" + i.ToString());

                    }
                }
            }
            

            
            NotifyRedraw();
        }

        public void NotifyRedraw()
        {
            grid.Invalidate();
        }

        public int GetGridHeight()
        {
            return row*gridsize;
        }
        public void label_Click(object sender, EventArgs e)
        {
            Label l = sender as Label;
            MouseEventArgs me = (MouseEventArgs)e;

            int index = 0;
            string type="a";

            for(int i=0;i<labels.Count;i++)
            {
                if (labels[i].Name == l.Name)
                {
                    index = i;
                    type= labels[i].Name.Substring(0,1);
                }
            }


            if (me.Button == MouseButtons.Left)
            {
                if(type=="c")
                {
                    if(selectCol!=-1)
                        labels[selectCol].ForeColor = Color.Black;
                    if (selectCol == index)
                    {
                        selectCol = -1;
                        labels[index].ForeColor = Color.Black;
                    }
                    else
                    {
                        selectCol = index;
                        labels[index].ForeColor = Color.Red;
                    }
                }
                else if(type=="r")
                {
                    if (selectRow != -1)
                        labels[selectRow].ForeColor = Color.Black;
                    if (selectRow == index)
                    {
                        selectRow = -1;
                        labels[index].ForeColor = Color.Black;
                    }
                    else
                    {
                        selectRow = index;
                        labels[index].ForeColor = Color.Red;
                        cbFunc(selectRow - lalelNum);
                    }
                }
                else if(type=="t")
                {

                }
                NotifyRedraw();
            }
            if (me.Button == MouseButtons.Right)
            {
                if (colorPicker.ShowDialog() == DialogResult.OK)
                {
                    BoxColor = colorPicker.Color;
                }
            }
        }
        //false horizontal ; true vertical
        public void GenerateLabel(Point p, bool type,string str,string tag)
        {

            Label label = new Label();
        
            label.Location = p;
            if (type)
                label.Size = new Size(gridsize, gridsize * str.Length);
            else
            {
                label.Size = new Size(gridsize * str.Length, gridsize);
                label.TextAlign = ContentAlignment.MiddleRight;
            }
            label.BackColor = Color.Transparent;

            label.Name = tag;
            label.MouseDown += new MouseEventHandler(label_Click);
            label.Text = str;
            label.BringToFront();
            this.Controls.Add(label);
            labels.Add(label);
        }

        public void DrawHighlight()
        {
            Pen boxPen = new Pen(BoxColor, 4);
            if(selectCol!=-1)
            {
                g.DrawRectangle(boxPen, new Rectangle(selectCol * gridsize+labeloffset, 0, gridsize, balanceSize));
            }

            if(selectRow!=-1)
            {
                int pos = selectRow - lalelNum;
                g.DrawRectangle(boxPen, new Rectangle(labeloffset, pos*gridsize, gridsize*col, gridsize));
            }
        }

        public void SetVerLabel(List<String> vl)
        {
            VerLabel=vl;
        }
        public void SetHorLabel(List<String> hl)
        {
            HorLabel=hl;
        }
        public void SetData(List<List<double>> d)
        {
            data = d;
        }
        public void TriggerLabel(int index)
        {
            /*
            if (selectCol != -1)
                labels[selectCol].ForeColor = Color.Black;
            selectCol = index;
            labels[selectCol].ForeColor = Color.Red;
        
         */
            for(int i=0;i<labels.Count;i++)
            {
                labels[i].ForeColor = Color.Black;
                if(i==index)
                    labels[i].ForeColor = Color.Red;
            }
        }
    }
}
