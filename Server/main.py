from server import SocketServer

class BasicChatServer(SocketServer):

    def __init__(self):
        SocketServer.__init__(self)

    def onmessage(self, client, message):
        print ("Client Sent Message",str(message))
        #Sending message to all clients
        self.broadcast(message)

    def onopen(self, client):
        print ("Client Connected")

    def onclose(self, client):
        print ("Client Disconnected")

def main():
    server = BasicChatServer()
    server.run()

if __name__ == "__main__":
    main()