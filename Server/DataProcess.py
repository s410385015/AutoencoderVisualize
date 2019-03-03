import numpy as np

def Loader(path):
    data=np.genfromtxt(path,delimiter=',')
    #delete the first column of the data
    #first column is date info
    data=np.delete(data,0,1)
    #get rid of the tags
    data=data[1:]
    return data

def Normalize(_data):
    dataMin=_data.min(axis=0)
    dataMax=_data.max(axis=0)
    data=(_data-dataMin)/(dataMax-dataMin)
    return data,dataMin,dataMax

def Denormalize(data,dataMin,dataMax):
    return data*(dataMax-dataMin)+dataMin