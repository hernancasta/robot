using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboclawService.Roboclaw
{
    internal class Roboclaw : SerialPort
    {
        private static object _lock = new object();
        readonly byte m_address;
        public Roboclaw(string port, int baudrate, byte address)
        {
            this.PortName = port;
            this.BaudRate = baudrate;
            m_address = address;
        }

        public bool ST_M1Forward(byte pwr)
        {
            return SendCommand(m_address, Commands.M1FORWARD, pwr);
        }

        public bool ST_M2Forward(byte pwr)
        {
            return SendCommand(m_address, Commands.M2FORWARD, pwr);
        }

        public bool ST_M1Backward(byte pwr)
        {
            return SendCommand(m_address, Commands.M1BACKWARD, pwr);
        }

        public bool ST_M2Backward(byte pwr)
        {
            return SendCommand(m_address, Commands.M2BACKWARD, pwr);
        }

        public bool ST_M1Drive(byte pwr)
        {
            return SendCommand(m_address, Commands.M17BIT, pwr);
        }

        public bool ST_M2Drive(byte pwr)
        {
            return SendCommand(m_address, Commands.M27BIT, pwr);
        }

        public bool ST_MixedForward(byte pwr)
        {
            return SendCommand(m_address, Commands.MIXEDFORWARD, pwr);
        }

        public bool ST_MixedBackward(byte pwr)
        {
            return SendCommand(m_address, Commands.MIXEDBACKWARD, pwr);
        }

        public bool ST_MixedLeft(byte pwr)
        {
            return SendCommand(m_address, Commands.MIXEDLEFT, pwr);
        }

        public bool ST_MixedRight(byte pwr)
        {
            return SendCommand(m_address, Commands.MIXEDRIGHT, pwr);
        }

        public bool ST_MixedDrive(byte pwr)
        {
            return SendCommand(m_address, Commands.MIXEDFB, pwr);
        }

        public bool ST_MixedTurn(byte pwr)
        {
            return SendCommand(m_address, Commands.MIXEDLR, pwr);
        }

        public bool ST_SetMinMainVoltage(byte set)
        {
            return SendCommand(m_address, Commands.SETMINMB, set);
        }

        public bool ST_SetMaxMainVoltage(byte set)
        {
            return SendCommand(m_address, Commands.SETMAXMB, set);
        }

        public bool GetM1Encoder(ref Int32 enc, ref byte status)
        {
            ArrayList args = new ArrayList();
            args.Add(enc);
            args.Add(status);
            if (ReadCommand(m_address, Commands.GETM1ENC, ref args))
            {
                enc = (Int32)args[0];
                status = (byte)args[1];
                return true;
            }
            return false;
        }

        public bool GetM2Encoder(ref Int32 enc, ref byte status)
        {
            ArrayList args = new ArrayList();
            args.Add(enc);
            args.Add(status);
            if (ReadCommand(m_address, Commands.GETM2ENC, ref args))
            {
                enc = (Int32)args[0];
                status = (byte)args[1];
                return true;
            }
            return false;
        }

        public bool GetM1Speed(ref Int32 speed, ref byte status)
        {
            ArrayList args = new ArrayList();
            args.Add(speed);
            args.Add(status);
            if (ReadCommand(m_address, Commands.GETM1SPEED, ref args))
            {
                speed = (Int32)args[0];
                status = (byte)args[1];
                return true;
            }
            return false;
        }

        public bool GetM2Speed(ref Int32 speed, ref byte status)
        {
            ArrayList args = new ArrayList();
            args.Add(speed);
            args.Add(status);
            if (ReadCommand(m_address, Commands.GETM2SPEED, ref args))
            {
                speed = (Int32)args[0];
                status = (byte)args[1];
                return true;
            }
            return false;
        }

        public bool GetEncoders(ref Int32 M1cnt, ref Int32 M2cnt)
        {
            ArrayList args = new ArrayList();
            args.Add(M1cnt);
            args.Add(M2cnt);
            if (ReadCommand(m_address, Commands.GETENCODERS, ref args))
            {
                M1cnt = (Int32)args[0];
                M2cnt = (Int32)args[1];
                return true;
            }
            return false;
        }

        public bool GetISpeeds(ref Int32 M1speed, ref Int32 M2speed)
        {
            ArrayList args = new ArrayList();
            args.Add(M1speed);
            args.Add(M2speed);
            if (ReadCommand(m_address, Commands.GETISPEEDS, ref args))
            {
                M1speed = (Int32)args[0];
                M2speed = (Int32)args[1];
                return true;
            }
            return false;
        }

        public bool ResetEncoders()
        {
            return SendCommand(m_address, Commands.RESETENC);
        }

        public bool SetEncoder1(UInt32 pos)
        {
            return SendCommand(m_address, Commands.SETM1ENCCOUNT, pos);
        }

        public bool SetEncoder2(UInt32 pos)
        {
            return SendCommand(m_address, Commands.SETM2ENCCOUNT, pos);
        }

        public bool GetMainVoltage(ref double voltage)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt16)voltage);
            if (ReadCommand(m_address, Commands.GETMBATT, ref args))
            {
                voltage = Convert.ToDouble(args[0]) / 10.0;
                return true;
            }
            return false;
        }

        public bool GetLogicVoltage(ref double voltage)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt16)voltage);
            if (ReadCommand(m_address, Commands.GETLBATT, ref args))
            {
                voltage = Convert.ToDouble(args[0]) / 10.0;
                return true;
            }
            return false;
        }

        public bool ST_SetMaxLogicVoltage(byte set)
        {
            return SendCommand(m_address, Commands.SETMAXLB, set);
        }

        public bool ST_SetMinLogicVolage(byte set)
        {
            return SendCommand(m_address, Commands.SETMINLB, set);
        }

        public bool SetM1VelocityConstants(double P, double I, double D, UInt32 qpps)
        {
            UInt32 p, i, d;
            p = (UInt32)(P * 65536.0);
            i = (UInt32)(I * 65536.0);
            d = (UInt32)(D * 65536.0);

            return SendCommand(m_address, Commands.SETM1PID, (UInt32)d,
                                          (UInt32)p,
                                          (UInt32)i,
                                          qpps);
        }

        public bool SetM2VelocityConstants(double P, double I, double D, UInt32 qpps)
        {
            UInt32 p, i, d;
            p = (UInt32)(P * 65536.0);
            i = (UInt32)(I * 65536.0);
            d = (UInt32)(D * 65536.0);

            return SendCommand(m_address, Commands.SETM2PID, (UInt32)d,
                                          (UInt32)p,
                                          (UInt32)i,
                                          qpps);
        }

        public bool GetM1ISpeed(ref Int32 speed, ref byte status)
        {
            ArrayList args = new ArrayList();
            args.Add(speed);
            args.Add(status);
            if (ReadCommand(m_address, Commands.GETM1ISPEED, ref args))
            {
                speed = (Int32)args[0];
                status = (byte)args[1];
                return true;
            }
            return false;
        }

        public bool GetM2ISpeed(ref Int32 speed, ref byte status)
        {
            ArrayList args = new ArrayList();
            args.Add(speed);
            args.Add(status);
            if (ReadCommand(m_address, Commands.GETM2ISPEED, ref args))
            {
                speed = (Int32)args[0];
                status = (byte)args[1];
                return true;
            }
            return false;
        }

        public bool M1Duty(Int16 duty)
        {
            return SendCommand(m_address, Commands.M1DUTY, duty);
        }

        public bool M2Duty(Int16 duty)
        {
            return SendCommand(m_address, Commands.M2DUTY, duty);
        }

        public bool MixedDuty(Int16 duty1, Int16 duty2)
        {
            return SendCommand(m_address, Commands.MIXEDDUTY, duty1, duty2);
        }

        public bool M1Speed(Int32 speed)
        {
            return SendCommand(m_address, Commands.M1SPEED, speed);
        }

        public bool M2Speed(Int32 speed)
        {
            return SendCommand(m_address, Commands.M2SPEED, speed);
        }

        public bool MixedSpeed(Int32 speed1, Int32 speed2)
        {
            return SendCommand(m_address, Commands.MIXEDSPEED, speed1, speed2);
        }

        public bool M1SpeedAccel(UInt32 accel, Int32 speed)
        {
            return SendCommand(m_address, Commands.M1SPEEDACCEL, accel, speed);
        }

        public bool M2SpeedAccel(UInt32 accel, Int32 speed)
        {
            return SendCommand(m_address, Commands.M2SPEEDACCEL, accel, speed);
        }

        public bool MixedSpeedAccel(UInt32 accel, Int32 speed1, Int32 speed2)
        {
            return SendCommand(m_address, Commands.MIXEDSPEEDACCEL, accel, speed1, speed2);
        }

        public bool M1SpeedDistance(Int32 speed, UInt32 distance, byte buffer)
        {
            return SendCommand(m_address, Commands.M1SPEEDDIST, speed, distance, buffer);
        }

        public bool M2SpeedDistance(Int32 speed, UInt32 distance, byte buffer)
        {
            return SendCommand(m_address, Commands.M2SPEEDDIST, speed, distance, buffer);
        }

        public bool MixedSpeedDistance(Int32 speed1, UInt32 distance1, Int32 speed2, UInt32 distance2, byte buffer)
        {
            return SendCommand(m_address, Commands.MIXEDSPEEDDIST, speed1, distance1, speed2, distance2, buffer);
        }

        public bool M1SpeedAccelDistance(Int32 accel, UInt32 speed, UInt32 distance, byte buffer)
        {
            return SendCommand(m_address, Commands.M1SPEEDACCELDIST, accel, speed, distance, buffer);
        }

        public bool M2SpeedAccelDistance(Int32 accel, UInt32 speed, UInt32 distance, byte buffer)
        {
            return SendCommand(m_address, Commands.M2SPEEDACCELDIST, accel, speed, distance, buffer);
        }

        public bool MixedSpeedAccelDistance(UInt32 accel, Int32 speed1, UInt32 distance1, Int32 speed2, UInt32 distance2, byte buffer)
        {
            return SendCommand(m_address, Commands.MIXEDSPEEDACCELDIST, accel, speed1, distance1, speed2, distance2, buffer);
        }

        public bool GetBuffers(ref byte buffer1, ref byte buffer2)
        {
            ArrayList args = new ArrayList();
            args.Add(buffer1);
            args.Add(buffer2);
            if (ReadCommand(m_address, Commands.GETBUFFERS, ref args))
            {
                buffer1 = (byte)args[0];
                buffer2 = (byte)args[1];
                return true;
            }
            return false;
        }

        public bool GetPWMs(ref Int16 PWM1, ref Int16 PWM2)
        {
            ArrayList args = new ArrayList();
            args.Add(PWM1);
            args.Add(PWM2);
            if (ReadCommand(m_address, Commands.GETPWMS, ref args))
            {
                PWM1 = (Int16)args[0];
                PWM2 = (Int16)args[1];
                return true;
            }
            return false;
        }

        public bool GetCurrents(ref Int16 current1, ref Int16 current2)
        {
            ArrayList args = new ArrayList();
            args.Add(current1);
            args.Add(current2);
            if (ReadCommand(m_address, Commands.GETCURRENTS, ref args))
            {
                current1 = (Int16)args[0];
                current2 = (Int16)args[1];
                return true;
            }
            return false;
        }

        public bool MixedSpeedAccel2(UInt32 accel1, Int32 speed1, UInt32 accel2, Int32 speed2)
        {
            return SendCommand(m_address, Commands.MIXEDSPEED2ACCEL, accel1, speed1, accel2, speed2);
        }

        public bool MixedSpeedAccelDistance2(UInt32 accel1, Int32 speed1, UInt32 distance1, UInt32 accel2, Int32 speed2, UInt32 distance2, byte buffer)
        {
            return SendCommand(m_address, Commands.MIXEDSPEED2ACCELDIST, accel1, speed1, distance1, accel2, distance2, buffer);
        }

        public bool M1DutyAccel(Int16 duty, UInt32 accel)
        {
            return SendCommand(m_address, Commands.M1DUTYACCEL, duty, accel);
        }

        public bool M2DutyAccel(Int16 duty, UInt32 accel)
        {
            return SendCommand(m_address, Commands.M2DUTYACCEL, duty, accel);
        }

        public bool MixedDutyAccel(Int16 duty1, UInt32 accel1, Int16 duty2, UInt32 accel2)
        {
            return SendCommand(m_address, Commands.MIXEDDUTYACCEL, duty1, accel1, duty2, accel2);
        }

        public bool GetM1VelocityConstants(ref double p, ref double i, ref double d, ref UInt32 qpps)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt32)p);
            args.Add((UInt32)i);
            args.Add((UInt32)d);
            args.Add(qpps);
            if (ReadCommand(m_address, Commands.READM1PID, ref args))
            {
                p = Convert.ToDouble(args[0]) / 65536;
                i = Convert.ToDouble(args[1]) / 65536;
                d = Convert.ToDouble(args[2]) / 65536;
                qpps = (UInt32)args[3];
                return true;
            }
            return false;
        }

        public bool GetM2VelocityConstants(ref double p, ref double i, ref double d, ref UInt32 qpps)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt32)p);
            args.Add((UInt32)i);
            args.Add((UInt32)d);
            args.Add(qpps);
            if (ReadCommand(m_address, Commands.READM2PID, ref args))
            {
                p = Convert.ToDouble(args[0]) / 65536;
                i = Convert.ToDouble(args[1]) / 65536;
                d = Convert.ToDouble(args[2]) / 65536;
                qpps = (UInt32)args[3];
                return true;
            }
            return false;
        }

        public bool SetMainVoltageLimits(double Min, double Max)
        {
            UInt16 min = (UInt16)(Min * 10.0);
            UInt16 max = (UInt16)(Max * 10.0);
            return SendCommand(m_address, Commands.SETMAINVOLTAGES, min, max);
        }

        public bool SetLogicVoltageLimits(double Min, double Max)
        {
            UInt16 min = (UInt16)(Min * 10.0);
            UInt16 max = (UInt16)(Max * 10.0);
            return SendCommand(m_address, Commands.SETLOGICVOLTAGES, min, max);
        }

        public bool GetMainVoltageLimits(ref double min, ref double max)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt16)min);
            args.Add((UInt16)max);
            if (ReadCommand(m_address, Commands.GETMINMAXMAINVOLTAGES, ref args))
            {
                min = Convert.ToDouble(args[0]) / 10.0;
                max = Convert.ToDouble(args[1]) / 10.0;
                return true;
            }
            return false;
        }

        public bool GetLogicVoltageLimits(ref double min, ref double max)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt16)min);
            args.Add((UInt16)max);
            if (ReadCommand(m_address, Commands.GETMINMAXLOGICVOLTAGES, ref args))
            {
                min = Convert.ToDouble(args[0]) / 10.0;
                max = Convert.ToDouble(args[1]) / 10.0;
                return true;
            }
            return false;
        }

        public bool SetM1PositionConstants(double P, double I, double D, UInt32 imax, UInt32 deadzone, Int32 minlimit, Int32 maxlimit)
        {
            UInt32 p, i, d;
            p = (UInt32)(P * 1024.0);
            i = (UInt32)(I * 1024.0);
            d = (UInt32)(D * 1024.0);

            return SendCommand(m_address, Commands.SETM1POSPID, (UInt32)d, (UInt32)p, (UInt32)i, imax, deadzone, minlimit, maxlimit);
        }

        public bool SetM2PositionConstants(double P, double I, double D, UInt32 imax, UInt32 deadzone, Int32 minlimit, Int32 maxlimit)
        {
            UInt32 p, i, d;
            p = (UInt32)(P * 1024.0);
            i = (UInt32)(I * 1024.0);
            d = (UInt32)(D * 1024.0);

            return SendCommand(m_address, Commands.SETM2POSPID, (UInt32)d, (UInt32)p, (UInt32)i, imax, deadzone, minlimit, maxlimit);
        }

        public bool GetM1PositionConstants(ref double p, ref double i, ref double d, ref UInt32 imax, ref UInt32 deadzone, ref Int32 minlimit, ref Int32 maxlimit)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt32)p);
            args.Add((UInt32)i);
            args.Add((UInt32)d);
            args.Add(imax);
            args.Add(deadzone);
            args.Add(minlimit);
            args.Add(maxlimit);
            if (ReadCommand(m_address, Commands.READM1POSPID, ref args))
            {
                p = Convert.ToDouble(args[0]) / 1024;
                i = Convert.ToDouble(args[1]) / 1024;
                d = Convert.ToDouble(args[2]) / 1024;
                imax = (UInt32)args[3];
                deadzone = (UInt32)args[4];
                minlimit = (Int32)args[5];
                maxlimit = (Int32)args[6];
                return true;
            }
            return false;
        }

        public bool GetM2PositionConstants(ref double p, ref double i, ref double d, ref UInt32 imax, ref UInt32 deadzone, ref Int32 minlimit, ref Int32 maxlimit)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt32)p);
            args.Add((UInt32)i);
            args.Add((UInt32)d);
            args.Add(imax);
            args.Add(deadzone);
            args.Add(minlimit);
            args.Add(maxlimit);
            if (ReadCommand(m_address, Commands.READM2POSPID, ref args))
            {
                p = Convert.ToDouble(args[0]) / 1024;
                i = Convert.ToDouble(args[1]) / 1024;
                d = Convert.ToDouble(args[2]) / 1024;
                imax = (UInt32)args[3];
                deadzone = (UInt32)args[4];
                minlimit = (Int32)args[5];
                maxlimit = (Int32)args[6];
                return true;
            }
            return false;
        }

        public bool M1SpeedAccelDeccelPosition(UInt32 accel, UInt32 speed, UInt32 deccel, Int32 position, byte buffer)
        {
            return SendCommand(m_address, Commands.M1SPEEDACCELDECCELPOS, accel, speed, deccel, position, buffer);
        }

        public bool M2SpeedAccelDeccelPosition(UInt32 accel, UInt32 speed, UInt32 deccel, Int32 position, byte buffer)
        {
            return SendCommand(m_address, Commands.M2SPEEDACCELDECCELPOS, accel, speed, deccel, position, buffer);
        }

        public bool MixedSpeedAccelDeccelPosition(UInt32 accel1, UInt32 speed1, UInt32 deccel1, Int32 position1, UInt32 accel2, UInt32 speed2, UInt32 deccel2, Int32 position2, byte buffer)
        {
            return SendCommand(m_address, Commands.MIXEDSPEEDACCELDECCELPOS, accel1, speed1, deccel1, position1, accel2, speed2, deccel2, position2, buffer);
        }

        public bool SetM1DefaultAccel(UInt32 accel, UInt32 deccel)
        {
            return SendCommand(m_address, Commands.SETM1DEFAULTACCEL, accel, deccel);
        }

        public bool SetM2DefaultAccel(UInt32 accel, UInt32 deccel)
        {
            return SendCommand(m_address, Commands.SETM2DEFAULTACCEL, accel, deccel);
        }

        public bool GetDefaultAccel(ref UInt32 accel1, ref UInt32 deccel1, ref UInt32 accel2, ref UInt32 deccel2)
        {
            ArrayList args = new ArrayList();
            args.Add(accel1);
            args.Add(deccel1);
            args.Add(accel2);
            args.Add(deccel2);
            if (ReadCommand(m_address, Commands.GETDEFAULTACCEL, ref args))
            {
                accel1 = (UInt32)args[0];
                deccel1 = (UInt32)args[1];
                accel2 = (UInt32)args[2];
                deccel2 = (UInt32)args[3];
                return true;
            }
            return false;
        }

        public bool SetPinModes(byte s3mode, byte s4mode, byte s5mode)
        {
            return SendCommand(m_address, Commands.SETPINFUNCTIONS, s3mode, s4mode, s5mode);
        }

        public bool GetPinModes(ref byte s3mode, ref byte s4mode, ref byte s5mode)
        {
            ArrayList args = new ArrayList();
            args.Add(s3mode);
            args.Add(s4mode);
            args.Add(s5mode);
            if (ReadCommand(m_address, Commands.GETPINFUNCTIONS, ref args))
            {
                s3mode = (byte)args[0];
                s4mode = (byte)args[1];
                s5mode = (byte)args[2];
                return true;
            }
            return false;
        }

        public bool SetCtrlsMode(byte ctrl1, byte ctrl2)
        {
            return SendCommand(m_address, Commands.SETCTRLSMODE, ctrl1, ctrl2);
        }

        public bool GetCtrlsMode(ref byte ctrl1, ref byte ctrl2)
        {
            ArrayList args = new ArrayList();
            args.Add(ctrl1);
            args.Add(ctrl2);
            if (ReadCommand(m_address, Commands.GETCTRLSMODE, ref args))
            {
                ctrl1 = (byte)args[0];
                ctrl2 = (byte)args[1];
                if (ctrl1 > 3)
                    ctrl1 = 0;
                if (ctrl2 > 3)
                    ctrl2 = 0;
                return true;
            }
            return false;
        }

        public bool SetCtrl1(UInt16 ctrl1)
        {
            return SendCommand(m_address, Commands.SETCTRL1, ctrl1);
        }

        public bool SetCtrl2(UInt16 ctrl2)
        {
            return SendCommand(m_address, Commands.SETCTRL2, ctrl2);
        }

        public bool GetCtrls(ref UInt16 ctrl1, ref UInt16 ctrl2)
        {
            ArrayList args = new ArrayList();
            args.Add(ctrl1);
            args.Add(ctrl2);
            if (ReadCommand(m_address, Commands.GETCTRLS, ref args))
            {
                ctrl1 = (UInt16)args[0];
                ctrl2 = (UInt16)args[1];
                return true;
            }
            return false;
        }

        public bool SetAuto1(UInt16 autoduty, UInt32 autotimeout)
        {
            return SendCommand(m_address, Commands.SETAUTO1, autoduty, autotimeout);
        }

        public bool SetAuto2(UInt16 autoduty, UInt32 autotimeout)
        {
            return SendCommand(m_address, Commands.SETAUTO2, autoduty, autotimeout);
        }

        public bool GetAutos(ref UInt16 autoduty1, ref UInt32 autotimeout1, ref UInt16 autoduty2, ref UInt32 autotimeout2)
        {
            ArrayList args = new ArrayList();
            args.Add(autoduty1);
            args.Add(autotimeout1);
            args.Add(autoduty2);
            args.Add(autotimeout2);
            if (ReadCommand(m_address, Commands.GETAUTOS, ref args))
            {
                autoduty1 = (UInt16)args[0];
                autotimeout1 = (UInt32)args[1];
                autoduty2 = (UInt16)args[2];
                autotimeout2 = (UInt32)args[3];
                return true;
            }
            return false;
        }

        public bool SetDeadBand(byte min, byte max)
        {
            return SendCommand(m_address, Commands.SETDEADBAND, min, max);
        }

        public bool GetDeadBand(ref byte min, ref byte max)
        {
            ArrayList args = new ArrayList();
            args.Add(min);
            args.Add(max);
            if (ReadCommand(m_address, Commands.GETDEADBAND, ref args))
            {
                min = (byte)args[0];
                max = (byte)args[1];
                if (min > 250)
                    min = 250;
                if (max > 250)
                    max = 250;
                return true;
            }
            return false;
        }

        public bool Defaults()
        {
            return SendCommand(m_address, Commands.RESTOREDEFAULTS);
        }

        public bool GetTemperature(ref double temperature)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt16)temperature);
            if (ReadCommand(m_address, Commands.GETTEMP, ref args))
            {
                temperature = Convert.ToDouble(args[0]) / 10.0;
                return true;
            }
            return false;
        }

        public bool GetTemperature2(ref double temperature2)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt16)temperature2);
            if (ReadCommand(m_address, Commands.GETTEMP2, ref args))
            {
                temperature2 = Convert.ToDouble(args[0]) / 10.0;
                return true;
            }
            return false;
        }

        public bool GetStatus(ref UInt32 status)
        {
            ArrayList args = new ArrayList();
            args.Add(status);
            if (ReadCommand(m_address, Commands.GETERROR, ref args))
            {
                status = (UInt32)args[0];
                return true;
            }
            return false;
        }

        public bool GetEncoderModes(ref byte m1mode, ref byte m2mode)
        {
            ArrayList args = new ArrayList();
            args.Add(m1mode);
            args.Add(m2mode);
            if (ReadCommand(m_address, Commands.GETENCODERMODE, ref args))
            {
                m1mode = (byte)args[0];
                m2mode = (byte)args[1];
                return true;
            }
            return false;
        }

        public bool SetEncoder1Mode(byte mode)
        {
            return SendCommand(m_address, Commands.SETM1ENCODERMODE, mode);
        }

        public bool SetEncoder2Mode(byte mode)
        {
            return SendCommand(m_address, Commands.SETM2ENCODERMODE, mode);
        }

        public bool WriteNVM()
        {
            return SendCommand(m_address, Commands.WRITENVM, 0xE22EAB7A);
        }

        public bool ReadNVM()
        {
            return SendCommand(m_address, Commands.READNVM);
        }

        public bool SetConfig(UInt16 config)
        {
            return SendCommand(m_address, Commands.SETCONFIG, config);
        }

        public bool GetConfig(ref UInt16 config)
        {
            ArrayList args = new ArrayList();
            args.Add(config);
            if (ReadCommand(m_address, Commands.GETCONFIG, ref args))
            {
                config = (UInt16)args[0];
                return true;
            }
            return false;
        }

        public bool SetM1LR(double L, double R)
        {
            UInt32 l, r;
            l = (UInt32)(L * 0x1000000);
            r = (UInt32)(R * 0x1000000);

            return SendCommand(m_address, Commands.SETM1LR, l, r);
        }

        public bool SetM2LR(double L, double R)
        {
            UInt32 l, r;
            l = (UInt32)(L * 0x1000000);
            r = (UInt32)(R * 0x1000000);

            return SendCommand(m_address, Commands.SETM2LR, l, r);
        }

        public bool GetM1LR(ref double l, ref double r)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt32)l);
            args.Add((UInt32)r);
            if (ReadCommand(m_address, Commands.GETM1LR, ref args))
            {
                l = Convert.ToDouble(args[0]) / 0x1000000;
                r = Convert.ToDouble(args[1]) / 0x1000000;
                return true;
            }
            return false;
        }

        public bool GetM2LR(ref double l, ref double r)
        {
            ArrayList args = new ArrayList();
            args.Add((UInt32)l);
            args.Add((UInt32)r);
            if (ReadCommand(m_address, Commands.GETM2LR, ref args))
            {
                l = Convert.ToDouble(args[0]) / 0x1000000;
                r = Convert.ToDouble(args[1]) / 0x1000000;
                return true;
            }
            return false;
        }

        public bool CalibrateLR()
        {
            return SendCommand(m_address, Commands.CALIBRATELR);
        }

        public bool SetM1Current(double fmin, double fmax)
        {
            UInt32 min, max;
            min = (UInt32)(fmin * 100);
            max = (UInt32)(fmax * 100);

            return SendCommand(m_address, Commands.SETM1MAXCURRENT, max, min);
        }

        public bool SetM2Current(double fmin, double fmax)
        {
            UInt32 min, max;
            min = (UInt32)(fmin * 100);
            max = (UInt32)(fmax * 100);

            return SendCommand(m_address, Commands.SETM2MAXCURRENT, max, min);
        }

        public bool GetM1Current(ref double min, ref double max)
        {
            ArrayList args = new ArrayList();
            args.Add((Int32)min);
            args.Add((Int32)max);
            if (ReadCommand(m_address, Commands.GETM1MAXCURRENT, ref args))
            {
                max = Convert.ToDouble(args[0]) / 100;
                min = Convert.ToDouble(args[1]) / 100;
                return true;
            }
            return false;
        }

        public bool GetM2Current(ref double min, ref double max)
        {
            ArrayList args = new ArrayList();
            args.Add((Int32)min);
            args.Add((Int32)max);
            if (ReadCommand(m_address, Commands.GETM2MAXCURRENT, ref args))
            {
                max = Convert.ToDouble(args[0]) / 100;
                min = Convert.ToDouble(args[1]) / 100;
                return true;
            }
            return false;
        }

        public bool SetDOUT(byte i, byte action)
        {
            return SendCommand(m_address, Commands.SETDOUT, i, action);
        }

        private bool SendCommand(byte address, int cmd, params object[] args)
        {
            int len = 0;
            foreach (Object obj in args)
            {
                if (obj.GetType() == typeof(UInt32))
                    len += 4;
                else if (obj.GetType() == typeof(Int32))
                    len += 4;
                else if (obj.GetType() == typeof(UInt16))
                    len += 2;
                else if (obj.GetType() == typeof(Int16))
                    len += 2;
                else if (obj.GetType() == typeof(byte))
                    len++;
            }
            byte[] data = new byte[len + 2];
            uint index = 0;
            data[index++] = address;
            data[index++] = (byte)cmd;
            foreach (object obj in args)
            {
                byte[] converted = obj.GetBytes().Reverse().ToArray();
                Array.Copy(converted,0, data, index, converted.Length);
                index += (uint)converted.Length;
            }

            var crc = data.CalculateCRC16();
            var crcBytes = crc.GetBytes().Reverse().ToArray();

            byte[] buffer = new byte[data.Length + 2];

            Array.Copy(data,buffer,data.Length);
            Array.Copy(crcBytes, 0, buffer, data.Length, 2);

            lock (_lock)
            {
                Write(buffer, 0, buffer.Length);
                try
                {
                    ReadByte();
                }
                catch (TimeoutException)
                {
//                    Trace.WriteLine("SendCommand Timeout:" + data[1].ToString());
                    return false;
                }
            }
            return true;

        }

        private bool ReadCommand(byte address, byte cmd, ref ArrayList args)
        {
            uint len = 0;
            foreach (Object obj in args)
            {
                if (obj.GetType() == typeof(UInt32))
                    len += 4;
                else if (obj.GetType() == typeof(Int32))
                    len += 4;
                else if (obj.GetType() == typeof(UInt16))
                    len += 2;
                else if (obj.GetType() == typeof(Int16))
                    len += 2;
                else if (obj.GetType() == typeof(byte))
                    len++;
            }

            byte[] arr = new byte[len+2];

            lock (_lock) {
                DiscardInBuffer();

                Write(new byte[] { address, cmd }, 0, 2);

                arr[0] = address;
                arr[1] = cmd;

                for (Int32 i = 2; i < len+2; i++)
                {
                    try
                    {
                        arr[i] = (byte)ReadByte();
                       // crc_update(arr[i]);
                    }
                    catch (TimeoutException)
                    {
                        DiscardInBuffer();
                        //Trace.WriteLine("ReadCommand Read Data Timeout:" + cmd.ToString() + " " + i.ToString());
                        return false;
                    }
                }

                UInt16 ccrc;
                try {
                    ccrc = (UInt16)(ReadByte() << 8);
                    ccrc |= (UInt16)ReadByte();
                }
                catch (TimeoutException)
                {
                    DiscardInBuffer();
                    return false;
                }

                UInt16 crc = arr.ToArray().CalculateCRC16();

                if (crc != ccrc) { 
                    DiscardInBuffer ();
                    return false;
                }

                int index = 2;
                for (int i = 0; i < args.Count; i++)
                {
                    if (args[i].GetType() == typeof(UInt32))
                        args[i] = (UInt32)(arr[index++] << 24 | arr[index++] << 16 | arr[index++] << 8 | arr[index++]);
                    else if (args[i].GetType() == typeof(Int32))
                        args[i] = (Int32)(arr[index++] << 24 | arr[index++] << 16 | arr[index++] << 8 | arr[index++]);
                    else if (args[i].GetType() == typeof(UInt16))
                        args[i] = (UInt16)(arr[index++] << 8 | arr[index++]);
                    else if (args[i].GetType() == typeof(Int16))
                        args[i] = (Int16)(arr[index++] << 8 | arr[index++]);
                    else if (args[i].GetType() == typeof(byte))
                        args[i] = (byte)(arr[index++]);
                    else
                    {
                        return false;
                    }
                }
                return true;
            }

        }

    }
}
