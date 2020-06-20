from http.server import HTTPServer, BaseHTTPRequestHandler
from io import BytesIO
import json
import re
import os


HOST = '0.0.0.0'
PORT = os.environ.get("PORT", 80)
#PORT = 8000

class SimpleHTTPRequestHandler(BaseHTTPRequestHandler):

    def do_GET(self):

        repnumber = 200

        response = BytesIO() #Create a Byte Buffer that will store the bytes to return

        html = open("Mobile Client/mobile_index.html")
        response.write(html.read().encode())

        self.send_response(repnumber) #Add Response header -->       200:OK
        self.end_headers() #Close Headers --> No more headers to add

        self.wfile.write(response.getvalue()) #Write the buffer bytes to the returned socketWriter

    def do_POST(self):

        response = BytesIO() #Create a mutable Byte Buffer

        repnumber = 200

        content_length = int(self.headers['Content-Length']) #Get ContentLength header (length of json)
        body = self.rfile.read(content_length) #Read bytes from buffer upto "content-length" nb bytes
        parsed_json = json.loads(body) #Transform bytes into a json dictionnary

        self.wfile.write(response.getvalue()) #Write the buffer bytes to the returned socketWriter

httpd = HTTPServer((HOST, int(PORT)), SimpleHTTPRequestHandler)
print("SERVER STARTED at ", HOST, ":", PORT, sep="")
httpd.serve_forever()
