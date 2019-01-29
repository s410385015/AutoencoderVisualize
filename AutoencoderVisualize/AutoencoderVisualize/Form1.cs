using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoencoderVisualize
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public int padRight = 500;
        public List<String> tags;
        public List<List<double>> allData;
        public List<List<List<double>>> idvData;
        public Form1()
        {
            InitializeComponent();
            idvData = new List<List<List<double>>>();
        }

        private void metroUserControl1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tags = DataLoader.ReadTags(".\\data\\tags1.csv");
            allData = DataLoader.ReadData(".\\data\\all1.csv");

            for (int i = 0; i  < 16;i++ )
            {
                List<List<double>> tmp = DataLoader.ReadData(".\\data\\d" + i + ".csv");
                idvData.Add(tmp);
            }
            SubDimGraph.horizontalLabel = false;
            SubDimGraph.balance = true;
            SubDimGraph.SetVerLabel(tags);
            SubDimGraph.SetInfo(idvData[0].Count, tags.Count, 20, 1, 20, tags.Count, tags.Count * 20, colorBar);
            SubDimGraph.GenerateVerticalLabel(true, null, null);
            List<String> v = new List<String>();
            List<double> r = new List<double>();
            for (int i = 0; i <= 5; i++){
                v.Add((i * 500).ToString());
                r.Add(i * (500.0/(double)idvData[0].Count ));
            }
            SubDimGraph.GenerateHorizontalLabel(false, r, v);
           
            //SubDimGraph.Visible = false;
            //dimGraph.balance = true;
            dimGraph.SetData(allData);
            dimGraph.SetHorLabel(tags);
            dimGraph.SetVerLabel(tags);
            dimGraph.SetInfo(tags.Count, tags.Count, 20, 20, 20, tags.Count, tags.Count * 20, colorBar);
            dimGraph.cbFunc = SetSubData;
            colorBar.cbFunc = UpdateFrame;
            colorBar.Location = new Point(this.Width-colorBar.width-padRight, dimGraph.Location.Y+dimGraph.padTop-colorBar.padTop);
            colorBar.SetSize(20, dimGraph.GetGridHeight(),20);
            colorBar.SetInfo(-0.3, 0.3, 0, Color.LimeGreen, Color.OrangeRed, Color.White, 4);

            dimGraph.Location = new Point(colorBar.Location.X - dimGraph.Width, dimGraph.Location.Y);
            SubDimGraph.Location = new Point(200, dimGraph.Location.Y);
            UpdateFrame();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dimGraph.NotifyRedraw();
            colorBar.NotifyRedraw();
            SubDimGraph.NotifyRedraw();
        }

        public void UpdateFrame()
        {
            colorBar.NotifyRedraw();
            dimGraph.NotifyRedraw();
            SubDimGraph.NotifyRedraw();
        }

        public void SetSubData(int index)
        {
            SubDimGraph.SetData(idvData[index]);
            SubDimGraph.TriggerLabel(index);
            SubDimGraph.NotifyRedraw();
        }
    }
}
