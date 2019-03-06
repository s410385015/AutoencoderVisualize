from Server import SocketServer
from Model import AE
from Model import VAE
import torch
from DataProcess import * 
import torch.utils.data as Data
import torch.optim as optim
import torch.nn as nn
from torch.autograd import Variable
from ColorProcess import *
import pylab
import matplotlib.pyplot as plt
import matplotlib as mpl
import random
import os
import time
import glob
import pandas as pd

class MyServer(SocketServer):

    def __init__(self):
        SocketServer.__init__(self)
        self.BATCH_SIZE=16
        self.raw_data=Loader('..//Database//Plastics_and_Chemicals_Macro.csv')
        self.data,self._min,self._max=Normalize(self.raw_data)
        self._data=self.data[:,14:30]
        self.data=self.data[:,14:30]
        self.data=torch.Tensor(self.data)
        self.dataSize=self.data.shape[0]
        self._dataset=Data.TensorDataset(self.data,self.data)
        self.test_loader=Data.DataLoader(dataset=self._dataset,batch_size=1,shuffle=False)
        self.train_loader=Data.DataLoader(dataset=self._dataset,batch_size=self.BATCH_SIZE,shuffle=True)   
        
        self.AE=self.LoadAE()
        self.la,self.re=self.ModelEval(self.AE)
         
    
    def onmessage(self, client, message):
        message=np.array(message.decode("utf-8").split(','))
        
        print ("Client Sent Message")
        print(message)

        if message[0]=='connect':
            self.DrawLatent(self.la)
            self.WaitForFile('..\\Database\\latent.png')
            plt.savefig('..\\Database\\latent.png',d=600)
            plt.clf()
            self.broadcast(str.encode('ok'))
        elif message[0]=='latent':

            message=(message[1:]).astype(np.float)
            
            print(message)

            self.DrawLatent(self.la)
            if self.MatchLatent(self.AE,message):
                self.WaitForFile('..\\Database\\latent.png')
                plt.savefig('..\\Database\\latent.png',d=600)
            plt.clf()
            self.broadcast(str.encode('ok'))

        elif message[0]=='dimgraph':
            self.DimMaipulate(self.AE,10)   
            self.broadcast(str.encode('ok'))
        #Sending message to all clients
        #self.broadcast(message)

    def onopen(self, client):
        print ("Client Connected")

    def onclose(self, client):
        print ("Client Disconnected")

    def latent_loss(z_mean, z_stddev):
        mean_sq = z_mean * z_mean
        stddev_sq = z_stddev * z_stddev
        return 0.5 * torch.mean(mean_sq + stddev_sq - torch.log(stddev_sq) - 1)

    def LoadAE(self):

        model=AE()
       
        try:
            model.load_state_dict(torch.load('..//Database//AE_model.pkl'))
            return model
        except:
            model=self.TrainAE()
            return model

    def TrainAE(self):
        LR=0.0005
        EPOCHS=50
        model=AE()
        optimizer=optim.Adam(model.parameters(),lr=LR)
        loss_func=nn.MSELoss()

        model.train()
        for epoch in range(EPOCHS):
            for index,(x,_) in enumerate(self.train_loader):
                x=x.view(x.size(0),-1)
                x=Variable(x)
                encoded,decoded=model(x)
                loss=loss_func(decoded,x)
                optimizer.zero_grad()
                loss.backward()
                optimizer.step()
            print("Epoch: [%3d], Loss: %.5f" %(epoch + 1, loss.data))
        return model

    def LoadVAE(self):

        model=AE()
        try:
            model.load_state_dict(torch.load('..//Database//VAE_model1.pkl'))
            return model
        except:
            model=self.TrainVAE()
            return model

    def TrainVAE(self):
        LR=0.0005
        EPOCHS=50
        model=VAE()
        optimizer=optim.Adam(model.parameters(),lr=LR)
        model.train()
        loss_func=nn.MSELoss()
        for epoch in range(EPOCHS):
            for index,(x,_) in enumerate(self.train_loader):
                x=x.view(-1,DIM_SIZE)
                x=Variable(x)  
                z,decoded=model(x)
                loss=loss_func(x,decoded)+latent_loss(model.z_mean,model.z_var)
                optimizer.zero_grad()
                loss.backward()
                optimizer.step()
            print("Epoch: [%3d], Loss: %.5f" %(epoch + 1, loss.data))
        return model

    def ModelEval(self,model):
        model.eval()
        reconstruct=[]
        latent=[]
        for x,_ in self.test_loader:
            x=x.view(x.size(0),-1)
            x=Variable(x)
            encoded,decoded=model(x)
            tmp=[]
            tmp_l=[]
            for k in decoded[0]:
                tmp.append(k.data)
            for l in encoded[0]:
                tmp_l.append(l.data)
            latent.append(tmp_l)
            reconstruct.append(tmp)
        latent=np.array(latent)
        reconstruct=np.array(reconstruct)
        return latent,reconstruct

    def DrawLatent(self,latent,startColor="#FF0000",endColor="#0000FF",a=.2,d=600):
        colors=linear_gradient(startColor,endColor,self.dataSize)
        plt.scatter(latent[:,0],latent[:,1],alpha=a,c=colors['hex'])
        
    
    def FindClosePoint(self,expect):
        tmp=np.array(self._data)
        print(tmp.shape)
        print(expect.shape)
        for i in range(expect.shape[0]):
            if expect[i]>=0:
                tmp[:,i]=tmp[:,i]-expect[i]
                tmp[:,i]=tmp[:,i]**2
            else:
                tmp[:,i]=tmp[:,i]*0
        tmp=np.sum(tmp,axis=1)
       
        idx=tmp.argmin()
        print(idx)
        print(self._data[idx])
        return idx

    def MatchLatent(self,model,expect):
        #test_x=Variable(torch.randn(1,2),requires_grad=True)
        
        idx=self.FindClosePoint(expect)

        test_in=Variable(torch.FloatTensor([self._data[idx]]),requires_grad=False)
        
        print("Database:")
        print(self._data[idx])

        _x,_=model(test_in)
        test_x=Variable(torch.FloatTensor(_x),requires_grad=True)
        
        print(_x)
        
        loss_func=nn.MSELoss()
        test_opt=optim.Adam([test_x],lr=0.1)

        for epoch in range(100):
            test_output=model.decoder(test_x[0])  
            cost=torch.FloatTensor([[0]])
            
            flag=True
            for i in range(expect.shape[0]):
                if expect[i]>=0:
                    cost+=loss_func(test_output[i],torch.tensor([expect[i]]))
                    flag=False
            if flag:
                return False
            
            #cost=loss_func(test_output[0],torch.tensor(expect[0]))
            test_opt.zero_grad()
            cost.backward()
            test_opt.step()
            if cost<1e-9:
                break
        print(cost)
        pp=test_x.data.numpy()
        print(pp)
        plt.scatter(pp[:,0],pp[:,1],color='green')
        print(model.decoder(test_x[0]))
            #print(test_x)
            #print("Epoch: [%3d], Loss: %.5f" %(epoch + 1, cost.data))
        return True
    
    def WaitForFile(self,filename):
        while True:
            try:
                with open(filename, 'rb') as _:
                    break
            except IOError:
                time.sleep(3)
    def WriteTageCSV(self):
        tags=np.genfromtxt('..//Database//Plastics_and_Chemicals_Macro.csv',delimiter=',',dtype=None)
        tags=np.delete(tags,0,1)
        labels_tags=[tags[0,i] for i in range(tags.shape[1])]
        labels_tags=np.array(labels_tags)
        labels_tags=labels_tags[14:30]
        df=pd.DataFrame(labels_tags)
        df.to_csv("..//Database//tags.csv",header=None,index=None)
    
    def DimMaipulate(self,model,factor):
        Container=[]
        #noise=np.random.uniform(1,2,data.shape[0])

        for i in range(self._data.shape[1]):
            noise=self._data[:,i]*factor
            re=[]
            
            noise_data=np.array(self._data)
            noise_data[:,i]=noise
            _n=torch.Tensor(noise_data)
            n_dataset=Data.TensorDataset(_n,_n)
            noise_loader=Data.DataLoader(dataset=n_dataset,batch_size=self._data.shape[0],shuffle=False)
            for x,_ in noise_loader:
                x=x.view(x.size(0),-1)
                x=Variable(x)
                encoded,decoded=model(x)
                
            
            re=np.array(decoded.data)
            reconstruct=np.array(self.re) 
            d=(re/reconstruct)-1

            d[:,i]=np.ones(self._data.shape[0])
            d=np.array(d,dtype=float)

            Container.append(d)
            print("dim"+str(i))
            df=pd.DataFrame(Container[i])
            self.WaitForFile("..//Database//data//d"+str(i)+".csv")
            df.to_csv("..//Database//data//d"+str(i)+".csv",header=None,index=None)
        b=np.array(Container)
        b=b.sum(axis=1)
        b/=self._data.shape[0]
        df=pd.DataFrame(b)
        df.to_csv("..//Database//effect.csv",header=None,index=None)

def main():
    SaveDirectory = os.getcwd() #印出目前工作目錄
    SaveDirectory=SaveDirectory.replace('\Server','')
    os.chdir("..")
    with open('config.txt', 'w') as outfile:
        outfile.write(SaveDirectory+'\\Database\\')
    os.chdir("Server")
    server = MyServer()
    server.run()
    
if __name__ == "__main__":
    main()