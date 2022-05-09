
This system is a prove of concept for an autonomous robot composed by a set of microservices over a dotnet core framwework.

The communication between different microservices relies on Redis (http://redis.io). Which is installed on a respberry pi board. 
The installation is very straightforward (https://amalgjose.com/2020/08/11/how-to-install-redis-in-raspberry-pi/), I just change the version for the last available in Redis.

Regarding the hardware:
- RoboClaw ST2x45A https://www.basicmicro.com/RoboClaw-ST-2x45A-Motor-Controller_p_27.html
- Encoders https://www.amazon.com/dp/B07MX5DWF3?ref_=cm_sw_r_cp_ud_dp_KMRBPC4NE24PTZ410QZ7
- Jetson Nano 
- RPLidar https://www.amazon.com/dp/B082FD34R5?ref_=cm_sw_r_cp_ud_dp_1RJMH4ZFKQ35K4NV25PB
- RaspberryPI (x3) + TouchScreen Display
- Switch
- 2 AGM Batteries 12V 35A
- XBox Controller

Redis Configuration:

nano redis.conf

#comment NETWORKING part to allow connect from outside
#bind 127.0.0.1 -::1

#set protected mode to disabled
protected-mode no

~/redis-62-6/src $ ./redis-server ../redis.conf


DOTNET: Used dotnet sdk 6.0

https://mycsharpdeveloper.wordpress.com/2021/09/21/installing-net-6-on-raspberry-pi-4-and-get-cpu-temperature-via-c/


How change autostart in LXDE:

/etc/xdg/lxsession/LXDE-pi/autostart


Microservices:

CartographerService: 
-------------------
Used to MAP. Generates a LOG file with the encoders and Lidar capture. 
The idea is to generate and test a mapping from this log in offline mode.

EncoderESP32Service:
-------------------
DEPRECATED. Alternative to read the encoders from an ESP32 connected directly to the encoders. 
Now I am using BASICMICRO drive board since manage the sinchronization between the drives and the encoders directly.

GamePadService:
--------------
Exposes the XBox controller movements.

JetsonDetectionService:
----------------------
Exposes the Jetson detection library in a microservices environment (run a dotnet microservice on top of a python image detection library)

LidarService:
------------
Service which exposes the Lidar scans

ManualControlService:
--------------------
Service to control manually (using the XBox controller) the movement of the robot interfacing the XBox services with the RoboclawService.

MappingService:
--------------
SLAM implementation. [working progress]

RoboclawService:
---------------
Service to control the drive and expose the board signals (Current, Alarms, Encoders, Voltage, Speed)

SabertoothService:
-----------------
Deprecated. User before switch to Roboclaw board.

RobotHMI:
--------
Blazor UI HMI. Connects to microservices to show live status.

