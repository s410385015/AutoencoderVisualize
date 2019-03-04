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
        public MySocket client;
        public string databasePath = @"C:\\Users\\ec131b\\Desktop\\Yu\\Proj\\Database\\";
        public bool isHandling = false;
        public Bitmap bitmap;
        public Form1()
        {
            InitializeComponent();
            idvData = new List<List<List<double>>>();
            client = new MySocket();
            client.StartClient("127.0.0.1", 7777);
            connector.Start();
            this.DoubleBuffered = true;
            //latentTimer.Start();
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
            LatentFig.Location = new Point(200, dimGraph.Location.Y +SubDimGraph.Height+100);
            
            
            SetPCPGraph();
            
            UpdateFrame();
        }

        public void SetPCPGraph()
        {
            pcpGraph.Location = new Point(dimGraph.Location.X, dimGraph.Location.Y + SubDimGraph.Height );
            pcpGraph.Size = new Size(1000, 500);
            pcpGraph.Init();
            pcpGraph.cb = HandlePCPDrag;
            pcpGraph.AlphaChange((int)(10 * 2.55));
            List<List<float>> data = DataLoader.ReadDataFloat(databasePath+"data.csv");
            List<List<float>> range = new List<List<float>>();
            List<float> tmp=new List<float>();
            List<String> tags = DataLoader.ReadTags(databasePath + "tags.csv"); 


            for(int i=0;i<=5;i++)
                tmp.Add(i*0.2f);

            for (int i = 0; i < data[0].Count; i++)
                range.Add(tmp);

            pcpGraph.InsertData(data[0].Count, data.Count, range, data, tags);
            pcpGraph.isExist = true;
            pcpGraph.drawGraph();
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
            pcpGraph.NotifyRedraw();
        }

        public void SetSubData(int index)
        {
            SubDimGraph.SetData(idvData[index]);
            SubDimGraph.TriggerLabel(index);
            SubDimGraph.NotifyRedraw();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.Send("latent");
            client.Recv(GetLatentFig);
        }

        private void connector_Tick(object sender, EventArgs e)
        {

            if (!client.flag)
            {
                client.StartClient("127.0.0.1", 7777);
                //Console.WriteLine("try connecting...");
            }
        }

        public void GetLatentFig(IAsyncResult result)
        {
            
            try
            {
                int len = client.client.EndReceive(result);
                
                if(len==2)
                {
                    bitmap = new Bitmap(databasePath+"latent.png");
                    LatentFig.Image = bitmap;
                  
                    //bitmap.Dispose();
                   
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            isHandling = false;
        }

        public void HandlePCPDrag()
        {
            if (!isHandling)
            {
                isHandling = true;
                string meg = "latent";

                for (int i = 0; i < pcpGraph.dragbox.Count; i++)
                {
                    if (pcpGraph.dragbox[i].flag)
                        meg += "," + (1 - pcpGraph.dragbox[i].value).ToString();
                    else
                        meg += ",-1";
                }
                if(bitmap!=null)
                    bitmap.Dispose();
                client.Send(meg);
                //latentTimer.Start(); 
                client.Recv(GetLatentFig);
            }
        }

        private void latentTimer_Tick(object sender, EventArgs e)
        {
            try
            {

                bitmap = new Bitmap(databasePath + "latent.png");
                LatentFig.Image = bitmap;
                bitmap.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


            isHandling = false;
            
        }

        
     
    }
}
