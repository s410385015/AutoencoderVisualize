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
        public List<Job> JobQueue;
       

        public Form1()
        {
            InitializeComponent();
            try
            {
                string text = System.IO.File.ReadAllText(@"config.txt");
                databasePath=text+"\\";
            }
            catch(Exception ex)
            {
               
            }
            
            //button1.Visible=false;
            //button2.Visible = false;
            idvData = new List<List<List<double>>>();
            
            this.DoubleBuffered = true;
            JobQueue = new List<Job>();
            RegisterJob();
            UpdateTime.Start();
            InitConnect();
            //latentTimer.Start();
        }

        
        public void RegisterJob()
        {
            Job latentUpdate = new Job("latent");
            latentUpdate.cb = UpdateLatent;
            JobQueue.Add(latentUpdate);

            Job dimgraphUpdate = new Job("dimgraph");
            dimgraphUpdate.cb = UpdateDimGraph;
            JobQueue.Add(dimgraphUpdate);
        }

        public void InitConnect()
        {
            client = new MySocket();
            client.StartClient("127.0.0.1", 7777);
            connector.Start();
            
            string meg = "connect";

            if (client.flag)
            {
               
                client.Send(meg);
                //latentTimer.Start(); 
                client.Recv(GetLatentFig);
            }
        }
        private void metroUserControl1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tags = DataLoader.ReadTags(databasePath+"tags.csv");
            allData = DataLoader.ReadData(databasePath+"effect.csv");

            for (int i = 0; i  < 16;i++ )
            {
                List<List<double>> tmp = DataLoader.ReadData(databasePath+"\\data\\d" + i + ".csv");
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



          
            
            
         
            
            SetPCPGraph();
            LatentFig.Location = new Point((int)(this.Width * 0.07), (int)(this.Height * 0.08));
            LatentFig.Size = new Size((int)(this.Width * 0.28), (int)(this.Width * 0.21));

            dimGraph.Location = new Point(LatentFig.Location.X + LatentFig.Width, (int)(this.Height * 0.05));
            dimGraph.Size = new Size((int)(this.Width * 0.3), (int)(this.Height * 0.5));
            //colorBar.SetSize(20, dimGraph.GetGridHeight(), 20);

            colorBar.Location = new Point(dimGraph.Location.X + colorBar.width + padRight, dimGraph.Location.Y + dimGraph.padTop - colorBar.padTop);
            colorBar.SetSize(20, dimGraph.GetGridHeight(), 20);
            colorBar.SetInfo(-0.3, 0.3, 0, Color.LimeGreen, Color.OrangeRed, Color.White, 4);

            SubDimGraph.Size = new Size((int)(this.Width * 0.3), (int)(this.Height * 0.5));
            SubDimGraph.Location = new Point(dimGraph.Location.X+dimGraph.Width, (int)(this.Height * 0.05));
           

          

            UpdateFrame();
            
        }

        public void SetPCPGraph()
        {
            pcpGraph.Location = new Point((int)(this.Width * 0.05), (int)(this.Height * 0.55) );
            pcpGraph.Size = new Size((int)(this.Width * 0.9), (int)(this.Height * 0.45));
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
            string meg = "dimgraph";
            client.Send(meg);
            //latentTimer.Start(); 
            client.Recv(GetDimFig);
        }

        private void connector_Tick(object sender, EventArgs e)
        {

            if (!client.flag)
            {
                client.StartClient("127.0.0.1", 7777);
                //Console.WriteLine("try connecting...");
            }
        }

        public void ChangeJobState(String n,int s)
        {
            for (int i = 0; i < JobQueue.Count; i++)
                if (JobQueue[i].name == n)
                    JobQueue[i].state = s;
        }

        public void GetDimFig(IAsyncResult result)
        {
            ChangeJobState("dimgraph", 2);
         
        }
        public void GetLatentFig(IAsyncResult result)
        {

            ChangeJobState("latent", 2);
            isHandling = false;
            /*
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
             */ 
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
                //ChangeJobState("latent", 1);
                client.Send(meg);
                //latentTimer.Start(); 
                client.Recv(GetLatentFig);
            }
        }

       

        private void Form1_Shown(object sender, EventArgs e)
        {
        
        }
        
        public void UpdateLatent()
        {

            //bitmap = new Bitmap(databasePath + "latent.png");
            //LatentFig.Image = bitmap;
            using (var fs = new System.IO.FileStream(databasePath + "latent.png", System.IO.FileMode.Open))
            {
                var bmp = new Bitmap(fs);
                LatentFig.Image = (Bitmap)bmp.Clone();
            }
            //bitmap.Dispose();
        }


        public void UpdateDimGraph()
        {
            allData = DataLoader.ReadData(databasePath + "effect.csv");
            idvData.Clear();
            for (int i = 0; i < 16; i++)
            {
                List<List<double>> tmp = DataLoader.ReadData(databasePath + "\\data\\d" + i + ".csv");
                idvData.Add(tmp);
            }
            dimGraph.SetData(allData);
            dimGraph.NotifyRedraw();
        }

        private void UpdateTime_Tick(object sender, EventArgs e)
        {
            for(int i=0;i<JobQueue.Count;i++)
            {
                if (JobQueue[i].state == 1)
                    JobQueue[i].cb();
                else if(JobQueue[i].state == 2)
                {
                    JobQueue[i].cb();
                    JobQueue[i].state = 0;
                }
            }
        }
        
        
    }
}
