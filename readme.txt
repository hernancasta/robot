Primero instale REDIS en raspberry

https://amalgjose.com/2020/08/11/how-to-install-redis-in-raspberry-pi/

despues configure para que pueda acceder desde afuera

nano redis.conf

comente NETWORKING
#bind 127.0.0.1 -::1

y saque protected mode
protected-mode no

~/redis-62-6/src $ ./redis-server ../redis.conf

instalar dotnet sdk 6.0

https://mycsharpdeveloper.wordpress.com/2021/09/21/installing-net-6-on-raspberry-pi-4-and-get-cpu-temperature-via-c/

RabbitMQ

http://pont.ist/rabbit-mq/

Script autostart en XLDE (Raspberry)
/etc/xdg/lxsession/LXDE-pi/autostart


