#!/usr/bin/python
'''
	Author: Igor Maculan - n3wtron@gmail.com
	A Simple mjpg stream http server
'''
import cv2
from PIL import Image
import threading
from http.server import BaseHTTPRequestHandler,HTTPServer
from socketserver import ThreadingMixIn
from io import BytesIO
import jetson.inference
import jetson.utils
import time
import numpy as np
import ssl
capture=None

class CamHandler(BaseHTTPRequestHandler):
	def do_GET(self):
		if self.path.endswith('.mjpg'):
			self.send_response(200)
			self.send_header('Content-type','multipart/x-mixed-replace; boundary=--jpgboundary')
			self.end_headers()
			while True:
				try:
					#rc,img = capture.read()
					#if not rc:
					#	continue
					#imgRGB=cv2.cvtColor(img,cv2.COLOR_BGR2RGB)
					jpg = Image.fromarray(aimg1, 'RGB')
					tmpFile = BytesIO()
					jpg.save(tmpFile,'JPEG')
					self.wfile.write("--jpgboundary".encode())
					self.send_header('Content-type','image/jpeg')
					self.send_header('Content-length',str(tmpFile.getbuffer().nbytes))
					self.end_headers()
					jpg.save(self.wfile,'JPEG')
					time.sleep(0.05)
				except KeyboardInterrupt:
					break
			return
		if self.path.endswith('.html'):
			self.send_response(200)
			self.send_header('Content-type','text/html')
			self.end_headers()
			self.wfile.write('<html><head></head><body>'.encode())
			self.wfile.write('<img src="https://127.0.0.1:8080/cam.mjpg"/>'.encode())
			self.wfile.write('</body></html>'.encode())
			return


class ThreadedHTTPServer(ThreadingMixIn, HTTPServer):
	"""Handle requests in a separate thread."""

def main():
	global camera
	cam_width = 1280
	cam_height = 960
	net = jetson.inference.detectNet("ssd-mobilenet-v2", threshold=0.5)
	#camera = jetson.utils.gstCamera(640, 480, "/dev/video0")
	camera = jetson.utils.gstCamera(cam_width, cam_height, "/dev/video0")
	#display = jetson.utils.glDisplay()
	counter = 1
	imagecounter = 1
	global img, img1, aimg1
	server = ThreadedHTTPServer(('0.0.0.0', 8080), CamHandler)
	#server.socket = ssl.wrap_socket(server.socket, certfile="./cert.pem", server_side=True)
	t = threading.Thread(target=server.serve_forever)
	t.setDaemon(True) # don't hang on exit
	t.start()
	print ("server started")
		#server.serve_forever()
	while True:
		try:
			img, width, height = camera.CaptureRGBA(zeroCopy=1)
			detections = net.Detect(img, width, height)
			jetson.utils.cudaDeviceSynchronize()
			print("[FRAME]", end = '')
			for d in detections:
				print("|" + net.GetClassDesc(d.ClassID) + ";" + str(int(d.Confidence*100))
				+ ";" + str(int(100 * d.Left / cam_width))
				+ ";" + str(int(100 * d.Top / cam_height))
				+ ";" + str(int(100 * d.Right / cam_width))
				+ ";" + str(int(100 * d.Bottom / cam_height)
				), end = '')
			print( flush=True)
			img1 = jetson.utils.cudaToNumpy(img, width, height,4)
			aimg1 = cv2.cvtColor (img1.astype (np.uint8), cv2.COLOR_RGBA2BGR)
			counter = counter + 1
#			if counter == 20:
#				jetson.utils.saveImageRGBA("/home/jetson/JetsonAPI/images/frame"+str(imagecounter)+".jpg",img,width,height)
#				counter = 0
#				print("[IMAGE]|/home/jetson/JetsonAPI/images/frame"+str(imagecounter)+".jpg|", end='')
#				print( flush=True)
#				if imagecounter == 100:
#					imagecounter = 0
#				imagecounter = imagecounter + 1
		except:
			print("[EXCEPTION]|", end='')
			print( flush=True)

if __name__ == '__main__':
	main()
