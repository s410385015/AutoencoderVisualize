using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoencoderVisualize
{
    public partial class PCP : UserControl
    {
        public int w;
        public int h;


        public int max_left;
        public int max_right;
        public int min_left;
        public int min_right;
        public int max_top;
        public int min_top;
        public int max_bottom;
        public int min_bottom;

        public int margin_top;
        public int margin_bottom;
        public int margin_left;
        public int margin_right;



        int startX, startY;
        public Color axis_color;
        public float axis_size;

        //axis length between each others
        public int axis_length;
        public int axis_height;





        public int label_axis_num;
        public int label_axis_size;
        public int label_axis_width;
        public Font label_axis_font;
        public SolidBrush label_axis_brush;
        public StringFormat label_axis_format;
        public int label_axis_sizeOflabel;
        public int label_axis_heightOflabel;
        public int label_axis_widthOflabel;


        public int class_num;
        public int label_size;
        public int label_height;
        public int label_width;
        public Font label_font;
        public Color label_color_org;
        public Color label_color_select;
        public List<string> label_name;
        public int max_label_per_row;

        public int tmp_select;
        public List<DataLabel> data;

        public Graphics g;
        public Graphics tmp_g;

        public Color data_old;
        public Color data_new;
        public int data_line_size;
        public int data_num;

        public bool isExist = false;
        public bool reLabel = true;
        public int Alpha = 255;
        public float alpha_range = 150;
        public List<Color> ColorMap;
        public int mode = 0;

        public Point cur_Pos;
        public Point start_Pos;
        public bool isMouse = false;
        public Color filter_color_out;
        public Color filter_color_in;
        public Color drag_color_selected;
        public int filter_bound_size;
        public int filter_size;
        public int filter_select;
        public Rectangle filter;
        public List<bool> isVisible;

        public string date_label_new;
        public string date_label_old;
        public bool useDate;

        public delegate void rightClickEvent(object sender, MouseEventArgs e);
        public rightClickEvent rce;

        public bool useRay;
        public Color ray_color;
        public int ray_size;
        public int curY;
        public int curX;

        // true  == horizontal
        // false == vertical
        public bool direction = true;
        public bool mouseEnter;
        public float alpha_f;
        public float fade_alpha;
        public float select_error;
        public int select_DataLine;
        public List<DragBoxAttr> dragbox;
        public int DragSelect;
        public delegate void CallBack();
        public CallBack cb;
        
        public PCP()
        {
            InitializeComponent();
            Init();
            pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.Graph_Paint);
            this.pictureBox1.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);
            ColorMap = new List<Color>();
            ColorMap.Add(Color.FromArgb(255, 0, 255));
            ColorMap.Add(Color.FromArgb(0, 0, 255));
            ColorMap.Add(Color.FromArgb(255, 180, 0));
            //ColorMap.Add(Color.FromArgb(255, 128, 0));
            ColorMap.Add(Color.FromArgb(255, 0, 0));
            this.DoubleBuffered = true;
        }

        private void PCP_Load(object sender, EventArgs e)
        {
            Init();
        }
        public void Init()
        {
            tmp_select = -1;

            w = this.Size.Width;
            h = this.Size.Height;

            min_top = margin_top = 50;
            min_bottom = margin_bottom = 50;
            min_left = margin_left = 70;
            min_right = margin_right = 70;




            axis_size = 1;
            axis_color = Color.Black;




            label_axis_size = 1;
            label_axis_width = 3;
            label_axis_num = 10;
            label_axis_sizeOflabel = 8;
            label_axis_heightOflabel = 15;
            label_axis_widthOflabel = 40;
            label_axis_font = new Font("Arial", label_axis_sizeOflabel);
            label_axis_brush = new SolidBrush(Color.Black);
            label_axis_format = new StringFormat();
            label_axis_format.Alignment = StringAlignment.Far;




            class_num = 3;
            label_size = 5;
            label_height = 5;
            label_width = 10;
            label_font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            data = new List<DataLabel>();
            label_name = new List<string>();
            max_label_per_row = 10;

            label_color_org = Color.Black;
            label_color_select = Color.Red;

            data_new = Color.FromArgb(255, 0, 0);
            data_old = Color.FromArgb(0, 0, 255);
            data_line_size = 1;
            data_num = 7;

            axis_length = (w - margin_left - margin_right) / (class_num - 1);
            axis_height = this.Size.Height - margin_top - margin_bottom - label_height;

            pictureBox1.Size = new Size(this.Width, this.Height);

            filter_color_in = Color.White;
            filter_color_out = Color.Black;
           
            filter_bound_size = 2;
            filter_size = 10;
            filter_select = -1;
            isVisible = new List<bool>();

            useRay = false;
            ray_color = Color.FromArgb(46, 139, 87);
            ray_size = 2;
            mouseEnter = false;
            alpha_f = 1;
            fade_alpha = 0.2f;
            select_error = 0.005f;

            dragbox = new List<DragBoxAttr>();
            DragSelect = -1;
            drag_color_selected = Color.Green;
        }

        public void SetMode(int m)
        {
            mode = m;
            NotifyRedraw();
        }


        //lb label axis data
        //d  data
        //ln label name
        public void InsertData(int cn, int dn, List<List<float>> lb, List<List<float>> d, List<string> ln)
        {

            w = this.Size.Width;
            h = pictureBox1.Size.Height;
            class_num = cn;
            data_num = dn;

            axis_length = (w - margin_left - margin_right) / (class_num - 1);
            axis_height = h - margin_top - margin_bottom - label_height;


            label_name = new List<string>();
            data = new List<DataLabel>();


            label_name = ln;

            for (int i = 0; i < cn; i++)
            {
                DragBoxAttr dba = new DragBoxAttr();
                dragbox.Add(dba);

                DataLabel datatmp = new DataLabel();
                for (int j = 0; j < lb[i].Count; j++)
                    datatmp.data_label.Add(lb[i][j]);

                for (int j = 0; j < dn; j++)
                    datatmp._data.Add(d[j][i]);

                data.Add(datatmp);
            }

            for (int i = 0; i < dn; i++)
                isVisible.Add(true);


            max_right = (int)(this.Width - min_left);
            max_left = (int)(this.Width - min_right);
            max_top = (int)(this.Height - min_bottom);
            max_bottom = (int)(this.Height - min_top);
            
            
           
        }

        public void drawGraph()
        {
            reLabel = true;
            pictureBox1.Invalidate();
        }



        public void NotifyRedraw()
        {

            pictureBox1.Invalidate();
        }

        public void ReInit()
        {
            //PrintF();
            pictureBox1.Size = new Size(this.Width, this.Height);

            w = this.Size.Width;
            h = this.Size.Height;


            axis_length = (w - margin_left - margin_right) / (class_num - 1);

            axis_height = this.Size.Height - margin_top - margin_bottom - label_height;

            max_right = (int)(this.Width - min_left);
            max_left = (int)(this.Width - min_right);
            max_top = (int)(this.Height - min_bottom);
            max_bottom = (int)(this.Height - min_top);
            alpha_f = 1f;
            //PrintF();
        }

        public void PrintF()
        {
            Console.WriteLine
                (
                    "w:" + w + " " +
                    "h:" + h + " " +
                    "length:" + axis_length + " " +
                    "height:" + axis_height + " "
                );
        }


        public void Update()
        {
            g.Clear(this.BackColor);
            //GraphInit();
            if (!direction)
            {
                DrawAxis2();
                if (Filter_SearchForAxis2())
                {

                    Rectangle r = getRectangle2();
                    r = RestrictFilter2(r);
                    filter = r;

                    Rectangle r1 = getRectangle2(filter_bound_size);
                    r1 = RestrictFilter2(r1, filter_bound_size);
                    //g.FillRectangle(new SolidBrush(filter_color_in), r.Location.X, r.Location.Y, r.Width, r.Height);


                    for (int i = 0; i < class_num - 1; i++)
                        DrawDataLine2(i);
                    for (int i = 0; i < class_num - 1; i++)
                        DrawSingleDataLine2(i);
                    DrawFilter(r, r1);
                }
                else
                {
                    if (filter_select != -1)
                        FilterDestory();
                    for (int i = 0; i < class_num - 1; i++)
                        DrawDataLine2(i);
                    for (int i = 0; i < class_num - 1; i++)
                        DrawSingleDataLine2(i);

                }
                DrawRay2();
            }
            else
            {

                DrawAxis();
               
                if (Filter_SearchForAxis())
                {

                    Rectangle r = getRectangle();
                    r = RestrictFilter(r);
                    filter = r;

                    Rectangle r1 = getRectangle(filter_bound_size);
                    r1 = RestrictFilter(r1, filter_bound_size);
                    //g.FillRectangle(new SolidBrush(filter_color_in), r.Location.X, r.Location.Y, r.Width, r.Height);


                    for (int i = 0; i < class_num - 1; i++)
                        DrawDataLine(i);
                    for (int i = 0; i < class_num - 1; i++)
                        DrawSingleDataLine(i);
                    DrawFilter(r, r1);
                }
                else
                {
                    if (filter_select != -1)
                        FilterDestory();
                    for (int i = 0; i < class_num - 1; i++)
                        DrawDataLine(i);
                    //for (int i = 0; i < class_num - 1; i++)
                       //DrawSingleDataLine(i);

                }
                DrawDragBox();
                DrawRay();
            }



        }

        public void Reset()
        {
            foreach (DataLabel dl in data)
                pictureBox1.Controls.Remove(dl.label);
            data.Clear();
            label_name.Clear();
            isExist = false;
            select_DataLine = -1;
            NotifyRedraw();
        }




        private void Graph_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (isExist)
                Update();


            //g = CreateGraphics();
            //
            //g.DrawRectangle(new Pen(axis_color, axis_size), new Rectangle(50, 50, 100, 100));
        }




        /*
            Draw axis in horizontal direction  
        */
        public void DrawAxis()
        {

            startX = margin_left;
            startY = margin_top;

            Pen pen = new Pen(axis_color, axis_size);
            for (int i = 0; i < class_num; i++)
            {

                Point p1 = new Point(startX + i * axis_length, startY);
                Point p2 = new Point(startX + i * axis_length, startY + axis_height);

                //axis label length between each others

                if (data.Count == 0)
                    break;
                int axis_label_length = axis_height / (data[i].data_label.Count - 1);





                for (int j = 0; j < data[i].data_label.Count; j++)
                {

                    Point p3 = new Point(p1.X - label_axis_width, p2.Y - j * axis_label_length);
                    Point p4 = new Point(p1.X, p2.Y - j * axis_label_length);
                    g.DrawLine(pen, p3, p4);

                    RectangleF drawRect = new RectangleF(p1.X - label_axis_widthOflabel - label_axis_width, p2.Y - j * axis_label_length - label_axis_sizeOflabel, label_axis_widthOflabel, label_axis_heightOflabel);

                    //Handle the date info
                    if (data[i].data_label.Count == 2)
                    {
                        float _f = 1.5f;
                        drawRect = new RectangleF(p1.X - (int)(label_axis_widthOflabel * _f) - label_axis_width, p2.Y - j * axis_label_length - label_axis_sizeOflabel, (int)(label_axis_widthOflabel * _f), label_axis_heightOflabel);
                        string o = date_label_old;
                        string n = date_label_new;
                        if (data[i].data_label[0] > data[i].data_label[1])
                        {
                            o = date_label_new;
                            n = date_label_old;
                        }
                        if (j == 0)
                            g.DrawString(o, label_axis_font, label_axis_brush, drawRect, label_axis_format);
                        else
                            g.DrawString(n, label_axis_font, label_axis_brush, drawRect, label_axis_format);
                    }
                    else
                        g.DrawString(data[i].data_label[j].ToString(), label_axis_font, label_axis_brush, drawRect, label_axis_format);
                }

                if (reLabel)
                {
                    if (class_num > max_label_per_row)
                    {
                        if (i % 2 == 0)
                            GenerateLabel(new Point(p1.X - label_width * 5, p2.Y + (int)(label_height * 5)), i);
                        else
                            GenerateLabel(new Point(p1.X - label_width * 5, p2.Y + label_height), i);
                    }
                    else
                    {
                        GenerateLabel(new Point(p1.X - label_width * 5, p2.Y + label_height), i);
                    }
                }
                else
                {
                    if (class_num > max_label_per_row)
                    {
                        if (i % 2 == 0)
                            MoveLabel(new Point(p1.X - label_width * 5, p2.Y + (int)(label_height * 5)), i);
                        else
                            MoveLabel(new Point(p1.X - label_width * 5, p2.Y + label_height), i);
                    }
                    else
                    {
                        MoveLabel(new Point(p1.X - label_width * 5, p2.Y + label_height), i);
                    }
                }
                g.DrawLine(pen, p1, p2);
            }

            reLabel = false;

        }

        /*
           Draw axis in vertical direction  
       */
        public void DrawAxis2()
        {
            startX = margin_left;
            startY = margin_top;

            Pen pen = new Pen(axis_color, axis_size);

            int axis_width2 = this.Size.Width - margin_left - margin_right - label_width;
            int axis_height2 = (this.Size.Height - margin_top - margin_bottom) / (class_num - 1);


            for (int i = 0; i < class_num; i++)
            {

                Point p1 = new Point(startX, startY + i * axis_height2);
                Point p2 = new Point(startX + axis_width2, startY + i * axis_height2);

                //axis label length between each others
                int axis_label_length = axis_width2 / (data[i].data_label.Count - 1);





                for (int j = 0; j < data[i].data_label.Count; j++)
                {


                    Point p3 = new Point(p1.X + j * axis_label_length, p1.Y + label_axis_width * 2);
                    Point p4 = new Point(p1.X + j * axis_label_length, p1.Y);


                    g.DrawLine(pen, p3, p4);

                    RectangleF drawRect = new RectangleF(p1.X + j * axis_label_length - (int)(label_axis_widthOflabel / 1.4), p1.Y + (int)(label_axis_width * 2.5), label_axis_widthOflabel, label_axis_heightOflabel);

                    //Handle the date info
                    if (data[i].data_label.Count == 2)
                    {

                        float _f = 1.5f;
                        drawRect = new RectangleF(p1.X + j * axis_label_length - (int)(label_axis_widthOflabel / 1.4), +p1.Y + (int)(label_axis_width * 2.5), (int)(label_axis_widthOflabel * _f), label_axis_heightOflabel);
                        string o = date_label_old;
                        string n = date_label_new;
                        if (data[i].data_label[0] > data[i].data_label[1])
                        {
                            o = date_label_new;
                            n = date_label_old;
                        }
                        if (j == 0)
                            g.DrawString(o, label_axis_font, label_axis_brush, drawRect, label_axis_format);
                        else
                            g.DrawString(n, label_axis_font, label_axis_brush, drawRect, label_axis_format);
                    }
                    else
                        g.DrawString(data[i].data_label[j].ToString(), label_axis_font, label_axis_brush, drawRect, label_axis_format);
                }


                Label l = new Label();


                if (reLabel)
                {
                    GenerateLabel(new Point(p1.X - l.Size.Width, p2.Y - (int)(l.Height / 2)), i, true);
                    /* 
                     if (class_num > max_label_per_row)
                     {
                         if (i % 2 == 0)
                             GenerateLabel(new Point(p1.X - l.Size.Width, p2.Y - (int)(l.Height / 2)), i, offset);
                         else
                             GenerateLabel(new Point(p2.X , p2.Y - (int)(l.Height / 2)), i, offset);
                     }
                     else
                     {
                         GenerateLabel(new Point(p1.X - l.Size.Width , p2.Y-(int)(l.Height/2) ), i,offset);
                     }
                     */
                }
                else
                {
                    MoveLabel(new Point(p1.X - l.Size.Width, p2.Y - (int)(l.Height / 2)), i);
                    /*
                    if (class_num > max_label_per_row)
                    {
                        if (i % 2 == 0)
                            MoveLabel(new Point(p1.X - l.Size.Width + offset, p2.Y - (int)(l.Height / 2)), i);
                        else
                            MoveLabel(new Point(p2.X, p2.Y - (int)(l.Height / 2)), i);
                    }
                    else
                    {
                        MoveLabel(new Point(p1.X - l.Size.Width+offset, p2.Y - (int)(l.Height / 2)), i);
                    }
                     */
                }
                l.Dispose();
                g.DrawLine(pen, p1, p2);
            }

            reLabel = false;
        }




        private void Graph_MouseClick(object sender, MouseEventArgs e)
        {

            //g.DrawLine(new Pen(Color.Black,20),new Point(0,0), new Point(100,100));
        }


        public void GenerateLabel(Point p, int index, bool flag = false)
        {

            data[index].label = new Label();
            data[index].label.TextAlign = ContentAlignment.MiddleCenter;
            data[index].label.Font = label_font;
            data[index].label.Location = new Point(p.X, p.Y);
            data[index].label.Size = new Size(data[index].label.Size.Width, data[index].label.Size.Height);
            data[index].label.BackColor = Color.Transparent;
            if (flag)
                data[index].label.TextAlign = ContentAlignment.MiddleRight;
            if (label_name.Count > 0)
                data[index].label.Text = label_name[index];
            else
                data[index].label.Text = "Label:" + index.ToString();

            data[index].label.Name = index.ToString();
            data[index].label.MouseDown += new MouseEventHandler(label_Click);
            data[index].label.BringToFront();
            pictureBox1.Controls.Add(data[index].label);
        }

        public void MoveLabel(Point p, int index)
        {
            data[index].label.Location = p;
        }

        public void label_Click(object sender, EventArgs e)
        {

            Label l = sender as Label;
            MouseEventArgs me = (MouseEventArgs)e;
            int i = 0;

            for (int j = 0; j < data.Count; j++)
                if (data[j].isObj(l))
                    i = j;
            //Console.WriteLine("pre select " + tmp_select);



            if (me.Button == MouseButtons.Left)
            {
                if (data[i].label.ForeColor == label_color_select)
                {

                    data[i].Reverse();

                    /*
                    DestoryAxis(i);
                    DestroyLineData(i);
                    DestroyLineData(i - 1);

                        
                    RedrawAxis(i);
                    RedrawAxis(i + 1);
                    DrawDataLine(i);
                    DrawDataLine(i - 1);
                    */
                    pictureBox1.Invalidate();

                    data[i].label.ForeColor = label_color_org;
                    tmp_select = -1;
                }
                else
                {


                    if (tmp_select == -1)
                    {
                        data[i].label.ForeColor = label_color_select;
                        tmp_select = i;
                    }
                    else
                    {
                        DisSelectLabel(i, tmp_select);
                        /*
                        DestoryAxis(i);
                        DestoryAxis(tmp_select);
                        DestroyLineData(i);
                        DestroyLineData(i-1);
                        DestroyLineData(tmp_select);
                        DestroyLineData(tmp_select-1);
                        */
                        SwapData(i, tmp_select);





                        /*
                        RedrawAxis(i);
                        RedrawAxis(i+1);
                        RedrawAxis(tmp_select);
                        RedrawAxis(tmp_select+1);



                        DrawDataLine(i);
                        DrawDataLine(i-1);
                        if(i-1!=tmp_select)
                            DrawDataLine(tmp_select);
                        if(tmp_select-1!=i)
                            DrawDataLine(tmp_select - 1);
                        */
                        pictureBox1.Invalidate();




                        //Console.WriteLine("done");

                        tmp_select = -1;
                    }

                }

            }
            else if (me.Button == MouseButtons.Right)
                data[i].label.ForeColor = label_color_org;





            //foreach (DataLabel dl in data)
                //Console.WriteLine(dl.label.ForeColor);
        }

        public void DestoryAxis(int index)
        {
            startX = margin_left + index * axis_length;
            startY = margin_top;
            Brush brush = new SolidBrush(this.BackColor);

            //Console.WriteLine((startX - label_axis_widthOflabel * 2) + " " + (startY - 1) + " " + (label_axis_widthOflabel * 4) + " " + (startY + axis_height + 1));
            g.FillRectangle(brush, startX - label_axis_widthOflabel * 2, startY - label_axis_heightOflabel, label_axis_widthOflabel * 4, startY + axis_height + 1);
            //g.DrawRectangle(pen, startX - label_axis_widthOflabel * 2, startY - 1, label_axis_widthOflabel * 4, startY + axis_height + 1);

        }

        public void DisSelectLabel(int indexA, int indexB)
        {
            //Console.WriteLine(indexA + " " + indexB);
            data[indexA].label.ForeColor = label_color_org;
            data[indexB].label.ForeColor = label_color_org;
            data[indexB].label.ForeColor = label_color_org;

        }




        public void RedrawAxis(int index)
        {

            if (index < class_num && index >= 0)
            {
                //true : bottom to top
                //false: up to bottom
                bool flag = true;

                if (data[index].data_label.Count > 1)
                    flag = (data[index].data_label[1] > data[index].data_label[0]) ? (true) : (false);


                startX = margin_left;
                startY = margin_top;

                Pen pen = new Pen(axis_color, axis_size);

                Point p1 = new Point(startX + index * axis_length, startY);
                Point p2 = new Point(startX + index * axis_length, startY + axis_height);

                //axis label length between each others
                int axis_label_length = axis_height / (data[index].data_label.Count - 1);




                if (flag)
                {
                    for (int j = 0; j < data[index].data_label.Count; j++)
                    {

                        Point p3 = new Point(p1.X - label_axis_width, p2.Y - j * axis_label_length);
                        Point p4 = new Point(p1.X, p2.Y - j * axis_label_length);
                        g.DrawLine(pen, p3, p4);

                        RectangleF drawRect = new RectangleF(p1.X - label_axis_widthOflabel - label_axis_width, p2.Y - j * axis_label_length - label_axis_sizeOflabel, label_axis_widthOflabel, label_axis_heightOflabel);

                        g.DrawString(data[index].data_label[j].ToString(), label_axis_font, label_axis_brush, drawRect, label_axis_format);
                    }
                }
                else
                {
                    for (int j = 0; j < data[index].data_label.Count; j++)
                    {

                        Point p3 = new Point(p1.X - label_axis_width, p1.Y + j * axis_label_length);
                        Point p4 = new Point(p1.X, p1.Y + j * axis_label_length);
                        g.DrawLine(pen, p3, p4);

                        RectangleF drawRect = new RectangleF(p1.X - label_axis_widthOflabel - label_axis_width, p1.Y + j * axis_label_length - label_axis_sizeOflabel, label_axis_widthOflabel, label_axis_heightOflabel);

                        g.DrawString(data[index].data_label[data[index].data_label.Count - j - 1].ToString(), label_axis_font, label_axis_brush, drawRect, label_axis_format);
                    }
                }

                //GenerateLabel(new Point(p1.X - label_width, p2.Y + label_height / 2), index);
                g.DrawLine(pen, p1, p2);
            }
        }


        public void InFilter(int index)
        {
            if (index == -1)
                return;
            startX = margin_left;
            startY = margin_top;
            bool flag = true;
            if (data[index].data_label.Count > 1)
                flag = (data[index].data_label[1] > data[index].data_label[0]) ? (true) : (false);
            int _x = startX + index * axis_length;
            float _range = Math.Abs(data[index].data_label[0] - data[index].data_label[data[index].data_label.Count - 1]);

            for (int i = 0; i < data_num; i++)
            {
                Point p1;
                if (flag)
                {
                    float factor = ((data[index]._data[i] - data[index].data_label[0]) / _range);
                    int offset = (int)(axis_height * factor);
                    p1 = new Point(_x, startY + axis_height - offset);
                }
                else
                {
                    float factor = ((data[index]._data[i] - data[index].data_label[data[index].data_label.Count - 1]) / _range);
                    int offset = (int)(axis_height * factor);
                    p1 = new Point(_x, startY + offset);
                }


                //Console.WriteLine(p1.Y + " " + filter.Top + " " + filter.Bottom);
                if (p1.Y > filter.Bottom || p1.Y < filter.Top)
                    isVisible[i] = false;
                else
                    isVisible[i] = true;
            }
        }

        public void InFilter2(int index)
        {
            if (index == -1)
                return;


            startX = margin_left;
            startY = margin_top;
            bool flag = true;
            if (data[index].data_label.Count > 1)
                flag = (data[index].data_label[1] > data[index].data_label[0]) ? (true) : (false);

            int axis_width2 = this.Size.Width - margin_left - margin_right - label_width;
            int axis_height2 = (this.Size.Height - margin_top - margin_bottom) / (class_num - 1);

            int _y = startY + index * axis_width2;
            float _range = Math.Abs(data[index].data_label[0] - data[index].data_label[data[index].data_label.Count - 1]);

            for (int i = 0; i < data_num; i++)
            {
                Point p1;
                if (flag)
                {
                    float factor = ((data[index]._data[i] - data[index].data_label[0]) / _range);
                    int offset = (int)(axis_width2 * factor);
                    p1 = new Point(startX + offset, _y);
                }
                else
                {
                    float factor = ((data[index]._data[i] - data[index].data_label[data[index].data_label.Count - 1]) / _range);
                    int offset = (int)(axis_width2 * factor);
                    p1 = new Point(startX + axis_width2 - offset, _y);
                }


                Console.WriteLine(p1.X + " " + filter.Right + " " + filter.Left);
                if (p1.X < filter.Left || p1.X > filter.Right)
                    isVisible[i] = false;
                else
                    isVisible[i] = true;
            }
        }

        public void DrawDataLine(int index)
        {

            if (index + 1 < class_num && index >= 0)
            {
                startX = margin_left;
                startY = margin_top;


                //true : bottom to top
                //false: top to bottom
                bool flag_left = true;
                bool flag_right = true;

                if (data[index].data_label.Count > 1)
                    flag_left = (data[index].data_label[1] > data[index].data_label[0]) ? (true) : (false);

                if (data[index + 1].data_label.Count > 1)
                    flag_right = (data[index + 1].data_label[1] > data[index + 1].data_label[0]) ? (true) : (false);



                int left_x = startX + index * axis_length;
                int right_x = startX + (index + 1) * axis_length;


                float range_left = Math.Abs(data[index].data_label[0] - data[index].data_label[data[index].data_label.Count - 1]);
                float range_right = Math.Abs(data[index + 1].data_label[0] - data[index + 1].data_label[data[index + 1].data_label.Count - 1]);


                if (range_left == 0)
                    range_left = 1f;
                if (range_right == 0)
                    range_right = 1f;

                for (int i = 0; i < data_num; i++)
                {
                    if (!isVisible[i])
                        continue;
                    Point p1, p2;



                    if (flag_left)
                    {
                        float factor = ((data[index]._data[i] - data[index].data_label[0]) / range_left);
                        int offset = (int)(axis_height * factor);
                        p1 = new Point(left_x, startY + axis_height - offset);
                    }
                    else
                    {
                        float factor = ((data[index]._data[i] - data[index].data_label[data[index].data_label.Count - 1]) / range_left);
                        int offset = (int)(axis_height * factor);
                        p1 = new Point(left_x, startY + offset);
                    }



                    if (flag_right)
                    {
                        float factor = ((data[index + 1]._data[i] - data[index + 1].data_label[0]) / range_right);
                        int offset = (int)(axis_height * factor);
                        p2 = new Point(right_x, startY + axis_height - offset);
                    }
                    else
                    {
                        float factor = ((data[index + 1]._data[i] - data[index + 1].data_label[data[index + 1].data_label.Count - 1]) / range_right);
                        int offset = (int)(axis_height * factor);
                        p2 = new Point(right_x, startY + offset);
                    }


                    
                    if (isInLine(p1, p2, new Point(curX, curY)))
                    {
                        select_DataLine = i;

                    }

                    alpha_f = fade_alpha;


                    Pen pen = new Pen(GetTimeColor(i), data_line_size);
                    g.DrawLine(pen, p1, p2);
                    //Console.WriteLine(pen.Color.A);



                }
            }

        }




        public void DrawDataLine2(int index)
        {

            if (index + 1 < class_num && index >= 0)
            {
                startX = margin_left;
                startY = margin_top;


                //true : left to right
                //false: right to left
                bool flag_up = true;
                bool flag_down = true;

                if (data[index].data_label.Count > 1)
                    flag_up = (data[index].data_label[1] > data[index].data_label[0]) ? (true) : (false);

                if (data[index + 1].data_label.Count > 1)
                    flag_down = (data[index + 1].data_label[1] > data[index + 1].data_label[0]) ? (true) : (false);




                int axis_width2 = this.Size.Width - margin_left - margin_right - label_width;
                int axis_height2 = (this.Size.Height - margin_top - margin_bottom) / (class_num - 1);

                int up_y = startY + index * axis_height2;
                int down_y = startY + (index + 1) * axis_height2;


                float range_up = Math.Abs(data[index].data_label[0] - data[index].data_label[data[index].data_label.Count - 1]);
                float range_down = Math.Abs(data[index + 1].data_label[0] - data[index + 1].data_label[data[index + 1].data_label.Count - 1]);


                for (int i = 0; i < data_num; i++)
                {
                    if (!isVisible[i])
                        continue;
                    Point p1, p2;



                    if (flag_up)
                    {
                        float factor = ((data[index]._data[i] - data[index].data_label[0]) / range_up);
                        int offset = (int)(axis_width2 * factor);
                        p1 = new Point(startX + offset, up_y);
                    }
                    else
                    {
                        float factor = ((data[index]._data[i] - data[index].data_label[data[index].data_label.Count - 1]) / range_up);
                        int offset = (int)(axis_width2 * factor);
                        p1 = new Point(startX + axis_width2 - offset, up_y);
                    }



                    if (flag_down)
                    {
                        float factor = ((data[index + 1]._data[i] - data[index + 1].data_label[0]) / range_down);
                        int offset = (int)(axis_width2 * factor);
                        p2 = new Point(startX + offset, down_y);
                    }
                    else
                    {
                        float factor = ((data[index + 1]._data[i] - data[index + 1].data_label[data[index + 1].data_label.Count - 1]) / range_down);
                        int offset = (int)(axis_width2 * factor);
                        p2 = new Point(startX + axis_width2 - offset, down_y);
                    }



                    
                    if (isInLine(p1, p2, new Point(curX, curY)))
                    {
                        select_DataLine = i;

                    }

                    alpha_f = fade_alpha;


                    Pen pen = new Pen(GetTimeColor(i), data_line_size);
                    g.DrawLine(pen, p1, p2);
                    //Console.WriteLine(pen.Color.A);



                }
            }

        }








        public void DestroyLineData(int index)
        {
            if (index + 1 < class_num)
            {
                startX = margin_left + index * axis_length + 1;
                startY = margin_top;


                Brush brush = new SolidBrush(this.BackColor);

                g.FillRectangle(brush, startX, startY, axis_length, axis_height);

            }
        }


        public Color GetTimeColor(int cur, bool flag = false)
        {
            if (data_num - 1 == 0)
                return data_new;

            Color newColor = Color.Red;


            switch (mode)
            {
                case 0:

                    float f = 1f;

                    if (mouseEnter)
                        f = alpha_f;
                    float factor = (float)cur / (data_num - 1);
                    int r = Math.Abs(data_old.R - (int)((data_old.R - data_new.R) * factor));
                    int g = Math.Abs(data_old.G - (int)((data_old.G - data_new.G) * factor));
                    int b = Math.Abs(data_old.B - (int)((data_old.B - data_new.B) * factor));
                    if (flag)
                        newColor = Color.FromArgb(255, r, g, b);
                    else
                        newColor = Color.FromArgb((int)(Alpha * f), r, g, b);
                    break;
                case 1:
                    alpha_range = Alpha;
                    if (cur >= data_num / 2)
                    {
                        if (data_num / 2 - 1 == 0)
                            return Color.Red;
                        float a = (alpha_range / (float)Math.Round((float)data_num / 2 - 1)) * (cur - data_num / 2);
                        newColor = Color.FromArgb((int)((255 - alpha_range) + a), Color.Red);
                    }
                    else
                    {
                        if (data_num / 2 - 1 == 0)
                            return Color.Blue;
                        float a = alpha_range - (alpha_range / (data_num / 2 - 1)) * (cur);
                        newColor = Color.FromArgb((int)((255 - alpha_range) + a), Color.Blue);
                    }
                    break;
                case 2:
                    float diff;

                    if (data_num < ColorMap.Count)
                        diff = 1;
                    else
                        diff = (float)(ColorMap.Count) / (float)(data_num);

                    int s = (int)(diff * cur);
                    float _factor = diff * cur - (float)s;

                    Color _old = ColorMap[s];
                    Color _new;
                    if (s + 1 == ColorMap.Count)
                        _new = ColorMap[s];
                    else
                        _new = ColorMap[s + 1];
                    int _r = Math.Abs(_old.R - (int)((_old.R - _new.R) * _factor));
                    int _g = Math.Abs(_old.G - (int)((_old.G - _new.G) * _factor));
                    int _b = Math.Abs(_old.B - (int)((_old.B - _new.B) * _factor));
                    newColor = Color.FromArgb(Alpha, _r, _g, _b);
                    break;
            }

            /*
            float diff;

            if (data_num < ColorMap.Count)
                diff = 1;
            else
                diff=(float)(ColorMap.Count) / (float)(data_num);
            
            int s = (int)(diff * cur);
            float factor = diff*cur - (float)s;

            data_old = ColorMap[s];
            if(s+1==ColorMap.Count)
                data_new = ColorMap[s];
            else
                data_new = ColorMap[s+1];
            */








            return newColor;
        }





        public void SwapData(int indexA, int indexB)
        {

            //Swap(data, indexA, indexB);
            DataLabel l = new DataLabel();
            l.Copy(data[indexA]);
            data[indexA].Copy(data[indexB]);
            data[indexB].Copy(l);


        }

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public void TestFunc()
        {
            Console.WriteLine("Form:" + this.Size.Width);
            Console.WriteLine("Pic:" + pictureBox1.Size.Width);


        }
        public void TestFunc2()
        {
            for (int i = 0; i < class_num - 1; i++)
                DrawDataLine(i);
        }


        public void AlphaChange(int value)
        {
            Alpha = value;
            //Console.WriteLine(Alpha);
            NotifyRedraw();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.pictureBox1.Focus();
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        start_Pos = cur_Pos = e.Location;
                       
                        if( SearchClickOnDragbox())
                        {

                            dragbox[DragSelect].flag = !dragbox[DragSelect].flag;
                            DragSelect = -1;
                        }
                        //rce(sender, e);
                    }
                    break;
                case MouseButtons.Left:
                    {
                        start_Pos = cur_Pos = e.Location;
                        isMouse = true;
                        pictureBox1.Invalidate();
                        SearchClickOnDragbox();
                    }
                    break;
            }

        }

        private void pictureBox1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (direction)
                {
                    margin_left += 10;
                    margin_right += 10;

                    margin_left = Math.Min(max_left, margin_left);
                    margin_right = Math.Min(max_right, margin_right);
                }
                else
                {
                    margin_top += 10;
                    margin_bottom += 10;

                    margin_top = Math.Min(max_top, margin_top);
                    margin_bottom = Math.Min(max_bottom, margin_bottom);
                }
            }
            else if (e.Delta < 0)
            {
                if (direction)
                {
                    margin_left -= 10;
                    margin_right -= 10;

                    margin_left = Math.Max(min_left, margin_left);
                    margin_right = Math.Max(min_right, margin_right);
                }
                else
                {
                    margin_top -= 10;
                    margin_bottom -= 10;

                    margin_top = Math.Max(min_top, margin_top);
                    margin_bottom = Math.Max(min_bottom, margin_bottom);
                }
            }
            ReInit();
            NotifyRedraw();

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouse) 
                cur_Pos = e.Location;

            curY = e.Location.Y;
            curX = e.Location.X;

            if (curX > margin_left
                && curY > margin_top
                && curX < this.Width - margin_right
                && curY < this.Height - margin_bottom)
            {
                //mouseEnter = true;

            }
            else
            {
                alpha_f = 1f;
                select_DataLine = -1;
                mouseEnter = false;
            }

            if(DragSelect!=-1)
            {
                float v = (float)(curY - margin_top) / (float)axis_height;

                //Console.WriteLine((curY - margin_top).ToString()+" "+axis_height.ToString()+" " + curY.ToString() + " " + v.ToString());

                v = Math.Max(0, v);
                v = Math.Min(1, v);
                dragbox[DragSelect].value = v;

               
            }

            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMouse)
            {
                if (direction)
                    InFilter(filter_select);
                else
                    InFilter2(filter_select);
                isMouse = false;
                pictureBox1.Invalidate();
                if (DragSelect != -1)
                {
                    DragSelect = -1;
                    cb();
                }

                
            }
        }

        public void DrawFilter(Rectangle r, Rectangle r1)
        {


            g.FillRectangle(new SolidBrush(filter_color_out), r.Location.X, r.Location.Y, r.Width, r.Height);

            g.FillRectangle(new SolidBrush(filter_color_in), r1.Location.X, r1.Location.Y, r1.Width, r1.Height);



        }

        private void FilterDestory()
        {
            start_Pos = new Point(-1, -1);
            filter_select = -1;
            for (int i = 0; i < isVisible.Count; i++)
                isVisible[i] = true;
        }

        private Rectangle getRectangle(int offset = 0)
        {
            if (cur_Pos.Y - start_Pos.Y > 0)
            {
                return new Rectangle(
                  start_Pos.X + offset, start_Pos.Y + offset, filter_size - 2 * offset, (cur_Pos.Y - start_Pos.Y) - 2 * offset);
            }
            else
            {
                return new Rectangle(
                  start_Pos.X + offset, cur_Pos.Y + offset, filter_size - 2 * offset, (start_Pos.Y - cur_Pos.Y) - 2 * offset);
            }
        }

        private Rectangle getRectangle2(int offset = 0)
        {
            if (cur_Pos.X - start_Pos.X > 0)
            {
                return new Rectangle(
                  start_Pos.X + offset, start_Pos.Y + offset, (cur_Pos.X - start_Pos.X) - 2 * offset, filter_size - 2 * offset);
            }
            else
            {
                return new Rectangle(
                  cur_Pos.X + offset, start_Pos.Y + offset, (start_Pos.X - cur_Pos.X) - 2 * offset, filter_size - 2 * offset);
            }
        }


        private Rectangle RestrictFilter(Rectangle r, int offset = 0)
        {
            startY = margin_top;

            if (r.Top < startY)
                r.Location = new Point(r.Left, startY + offset);

            if (r.Bottom > startY + axis_height)
                r.Height = (startY + axis_height) - r.Top - offset;



            return r;
        }

        private Rectangle RestrictFilter2(Rectangle r, int offset = 0)
        {
            startX = margin_left;
            int axis_width2 = this.Size.Width - margin_left - margin_right - label_width;

            if (r.Left < startX)
                r.Location = new Point(startX + offset, r.Top);

            if (r.Right > startX + axis_width2)
                r.Width = (startX + axis_width2) - r.Left - offset;



            return r;
        }



        public bool Filter_SearchForAxis()
        {
            return false;
            startX = margin_left;

            for (int i = 0; i < class_num; i++)
            {
                int _x = startX + i * axis_length;
                if (Math.Abs(start_Pos.X - _x) <= filter_size * 2)
                {
                    start_Pos.X = _x - filter_size / 2;
                    filter_select = i;
                    return true;
                }
            }
            return false;
        }


        public bool Filter_SearchForAxis2()
        {
            int axis_height2 = (this.Size.Height - margin_top - margin_bottom) / (class_num - 1);
            startY = margin_top;

            for (int i = 0; i < class_num; i++)
            {
                int _y = startY + i * axis_height2;
                if (Math.Abs(start_Pos.Y - _y) <= filter_size * 2)
                {
                    start_Pos.Y = _y - filter_size / 2;
                    filter_select = i;
                    return true;
                }
            }
            return false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == System.Windows.Forms.MouseButtons.Right)
            {
                cur_Pos = start_Pos = new Point(0, 0);
                pictureBox1.Invalidate();
            }

        }

        public void SetDateInfo(string n, string o, bool _flag)
        {
            date_label_new = n;
            date_label_old = o;
            _flag = true;
        }

        public void DrawRay()
        {
            if (!useRay)
                return;


            curY = Math.Max(curY, margin_top);
            curY = Math.Min(curY, this.Height - margin_bottom - 5);
            g.DrawLine(new Pen(ray_color, ray_size), new Point(margin_left, curY), new Point(this.Width - margin_right, curY));
        }

        public void DrawRay2()
        {
            if (!useRay)
                return;


            curX = Math.Max(curX, margin_left);
            curX = Math.Min(curX, this.Width - margin_right - 10);
            g.DrawLine(new Pen(ray_color, ray_size), new Point(curX, margin_top), new Point(curX, this.Height - margin_bottom));
        }

        public void ClearLabel()
        {
            foreach (DataLabel dl in data)
                pictureBox1.Controls.Remove(dl.label);
            reLabel = true;
        }

        public void SetDirection(bool flag)
        {
            direction = flag;




            margin_left = min_left;
            margin_right = min_right;
            margin_top = min_top;
            margin_bottom = min_bottom;

            if (!flag)
            {
                margin_left = (int)(min_left * 1.5);
                margin_right = (int)(min_right * 1.5);
            }
            ReInit();
            ClearLabel();
            FilterDestory();
            NotifyRedraw();

        }

        public bool isInLine(Point p1, Point p2, Point pos)
        {
          

            if (direction)
            {
                if (p1.X > pos.X || p2.X < pos.X)
                    return false;
                if (Math.Abs(((float)(p1.Y - p2.Y) / (p1.X - p2.X)) - ((float)(pos.Y - p2.Y) / (pos.X - p2.X))) < select_error)
                    return true;
            }
            else
            {
                if (p1.Y > pos.Y || p2.Y < pos.Y)
                    return false;
                //Console.WriteLine(((float)(p1.X - p2.X) / (p1.Y - p2.Y)).ToString() + " -- " + ((float)(pos.X - p2.X) / (pos.Y - p2.Y)).ToString());
                if (Math.Abs(((float)(p1.X - p2.X) / (p1.Y - p2.Y)) - ((float)(pos.X - p2.X) / (pos.Y - p2.Y))) < select_error * 5)
                    return true;
            }

            return false;
        }

        public void DrawSingleDataLine(int index)
        {
            if (select_DataLine == -1)
                return;

            if (index + 1 < class_num && index >= 0)
            {
                startX = margin_left;
                startY = margin_top;


                //true : bottom to top
                //false: top to bottom
                bool flag_left = true;
                bool flag_right = true;

                if (data[index].data_label.Count > 1)
                    flag_left = (data[index].data_label[1] > data[index].data_label[0]) ? (true) : (false);

                if (data[index + 1].data_label.Count > 1)
                    flag_right = (data[index + 1].data_label[1] > data[index + 1].data_label[0]) ? (true) : (false);



                int left_x = startX + index * axis_length;
                int right_x = startX + (index + 1) * axis_length;


                float range_left = Math.Abs(data[index].data_label[0] - data[index].data_label[data[index].data_label.Count - 1]);
                float range_right = Math.Abs(data[index + 1].data_label[0] - data[index + 1].data_label[data[index + 1].data_label.Count - 1]);


                if (range_left == 0)
                    range_left = 1f;
                if (range_right == 0)
                    range_right = 1f;

                int i = select_DataLine;
                //if (!isVisible[i])
                //continue;
                Point p1, p2;



                if (flag_left)
                {
                    float factor = ((data[index]._data[i] - data[index].data_label[0]) / range_left);
                    int offset = (int)(axis_height * factor);
                    p1 = new Point(left_x, startY + axis_height - offset);
                }
                else
                {
                    float factor = ((data[index]._data[i] - data[index].data_label[data[index].data_label.Count - 1]) / range_left);
                    int offset = (int)(axis_height * factor);
                    p1 = new Point(left_x, startY + offset);
                }



                if (flag_right)
                {
                    float factor = ((data[index + 1]._data[i] - data[index + 1].data_label[0]) / range_right);
                    int offset = (int)(axis_height * factor);
                    p2 = new Point(right_x, startY + axis_height - offset);
                }
                else
                {
                    float factor = ((data[index + 1]._data[i] - data[index + 1].data_label[data[index + 1].data_label.Count - 1]) / range_right);
                    int offset = (int)(axis_height * factor);
                    p2 = new Point(right_x, startY + offset);
                }




                alpha_f = 1f;



                Pen pen = new Pen(GetTimeColor(i, true), data_line_size);
                g.DrawLine(pen, p1, p2);
                //Console.WriteLine(pen.Color.A);



            }


        }
        public void DrawSingleDataLine2(int index)
        {
            if (select_DataLine == -1)
                return;

            if (index + 1 < class_num && index >= 0)
            {
                startX = margin_left;
                startY = margin_top;


                //true : left to right
                //false: right to left
                bool flag_up = true;
                bool flag_down = true;

                if (data[index].data_label.Count > 1)
                    flag_up = (data[index].data_label[1] > data[index].data_label[0]) ? (true) : (false);

                if (data[index + 1].data_label.Count > 1)
                    flag_down = (data[index + 1].data_label[1] > data[index + 1].data_label[0]) ? (true) : (false);




                int axis_width2 = this.Size.Width - margin_left - margin_right - label_width;
                int axis_height2 = (this.Size.Height - margin_top - margin_bottom) / (class_num - 1);

                int up_y = startY + index * axis_height2;
                int down_y = startY + (index + 1) * axis_height2;


                float range_up = Math.Abs(data[index].data_label[0] - data[index].data_label[data[index].data_label.Count - 1]);
                float range_down = Math.Abs(data[index + 1].data_label[0] - data[index + 1].data_label[data[index + 1].data_label.Count - 1]);



                int i = select_DataLine;
                Point p1, p2;



                if (flag_up)
                {
                    float factor = ((data[index]._data[i] - data[index].data_label[0]) / range_up);
                    int offset = (int)(axis_width2 * factor);
                    p1 = new Point(startX + offset, up_y);
                }
                else
                {
                    float factor = ((data[index]._data[i] - data[index].data_label[data[index].data_label.Count - 1]) / range_up);
                    int offset = (int)(axis_width2 * factor);
                    p1 = new Point(startX + axis_width2 - offset, up_y);
                }



                if (flag_down)
                {
                    float factor = ((data[index + 1]._data[i] - data[index + 1].data_label[0]) / range_down);
                    int offset = (int)(axis_width2 * factor);
                    p2 = new Point(startX + offset, down_y);
                }
                else
                {
                    float factor = ((data[index + 1]._data[i] - data[index + 1].data_label[data[index + 1].data_label.Count - 1]) / range_down);
                    int offset = (int)(axis_width2 * factor);
                    p2 = new Point(startX + axis_width2 - offset, down_y);
                }




                alpha_f = 1f;



                Pen pen = new Pen(GetTimeColor(i, true), data_line_size);
                g.DrawLine(pen, p1, p2);
                //Console.WriteLine(pen.Color.A);




            }
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            alpha_f = 1f;
            mouseEnter = false;
            NotifyRedraw();
        }

        private void DrawDragBox()
        {
            startX = margin_left;
            startY = margin_top;

            for (int i = 0; i < class_num; i++)
            {
                Point pt = new Point(startX + i * axis_length, startY + (int)(axis_height * dragbox[i].value));
                Rectangle r = new Rectangle(pt.X - filter_size, pt.Y - filter_size, filter_size * 2, filter_size * 2);
               
                g.FillRectangle(new SolidBrush(filter_color_out), r.Location.X-filter_bound_size, r.Location.Y-filter_bound_size, r.Width+filter_bound_size*2, r.Height+filter_bound_size*2);
                if(dragbox[i].flag)
                    g.FillRectangle(new SolidBrush(drag_color_selected), r.Location.X, r.Location.Y, r.Width, r.Height);
                else
                    g.FillRectangle(new SolidBrush(filter_color_in), r.Location.X, r.Location.Y, r.Width, r.Height);
            }
        }


        private bool SearchClickOnDragbox()
        {
            startX = margin_left;

            for (int i = 0; i < class_num; i++)
            {
                int _x = startX + i * axis_length;
                int _y = startY + (int)(axis_height * dragbox[i].value);
                if (Math.Abs(start_Pos.X - _x) <= filter_size * 2
                    && Math.Abs(start_Pos.Y - _y) <= filter_size * 2)
                {
                    DragSelect = i;
                
                    return true;
                }
            }
            return false;
        }
        
    }

    public class DragBoxAttr
    {
        public float value;
        public bool flag;
        public DragBoxAttr()
        {
            value = 0.5f;
            flag = false;
                
        }
    }

    public class DataLabel
    {
        public List<float> data_label;
        public Label label;
        public List<float> _data;

        public DataLabel()
        {
            data_label = new List<float>();
            label = new Label();
            _data = new List<float>();
        }

        public void Copy(DataLabel l)
        {
            data_label.Clear();
            foreach (float f in l.data_label)
                data_label.Add(f);
            _data.Clear();
            foreach (float f in l._data)
                _data.Add(f);

            //Console.WriteLine(label.Text+"??  "+ l.label.Text);
            //label.Name = l.label.Name;
            label.Text = l.label.Text;
        }



        public void Reverse()
        {
            data_label.Reverse();
        }

        public bool isObj(Label l)
        {
            return l.Name == label.Name;
        }
    }
}
