import torch
import torch.optim as optim
import torch.nn as nn
from torch.autograd import Variable
import numpy as np

class AE(nn.Module):
    def __init__(self):
        super(AE,self).__init__()
        self.encoder=nn.Sequential(
            nn.Linear(16,8),
            nn.ReLU(),
            nn.Linear(8,2),
        )
        self.decoder=nn.Sequential(
            nn.Linear(2,8),
            nn.ReLU(),
            nn.Linear(8,16),
            nn.Sigmoid())
    def forward(self,x):
        encoded=self.encoder(x)
        decoded=self.decoder(encoded)
        return encoded,decoded


class VAE(nn.Module):
    def __init__(self):
        super(VAE,self).__init__()
        
        self.encoder=nn.Sequential(
            nn.Linear(16,8),
            nn.ReLU(),
            nn.Linear(8,4),
        )
        self.decoder=nn.Sequential(
            nn.Linear(2,8),
            nn.ReLU(),
            nn.Linear(8,16),
            nn.Sigmoid())
        
        self.mu=torch.nn.Linear(4,2)
        self.logvar=torch.nn.Linear(4,2)
    
    def sample_latent(self,en_value):
        mu=self.mu(en_value)
        logvar=self.logvar(en_value)
        var=torch.exp(logvar)
        std_z= torch.from_numpy(np.random.normal(0, 1, size=var.size())).float()
        
        self.z_mean=mu
        self.z_var=var
        return mu + var * Variable(std_z, requires_grad=False)

    def forward(self,x):
        en_value=self.encoder(x)
        z=self.sample_latent(en_value)
        return z,self.decoder(z)