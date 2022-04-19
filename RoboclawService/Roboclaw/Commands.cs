using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboclawService.Roboclaw
{
    internal class Commands
    {
        public const int M1FORWARD = 0;
        public const int M1BACKWARD = 1;
        public const int SETMINMB = 2;
        public const int SETMAXMB = 3;
        public const int M2FORWARD = 4;
        public const int M2BACKWARD = 5;
        public const int M17BIT = 6;
        public const int M27BIT = 7;
        public const int MIXEDFORWARD = 8;
        public const int MIXEDBACKWARD = 9;
        public const int MIXEDRIGHT = 10;
        public const int MIXEDLEFT = 11;
        public const int MIXEDFB = 12;
        public const int MIXEDLR = 13;

        public const int GETM1ENC = 16;
        public const int GETM2ENC = 17;
        public const int GETM1SPEED = 18;
        public const int GETM2SPEED = 19;
        public const int RESETENC = 20;
        public const int GETVERSION = 21;
        public const int SETM1ENCCOUNT = 22;
        public const int SETM2ENCCOUNT = 23;
        public const int GETMBATT = 24;
        public const int GETLBATT = 25;
        public const int SETMINLB = 26;
        public const int SETMAXLB = 27;
        public const int SETM1PID = 28;
        public const int SETM2PID = 29;
        public const int GETM1ISPEED = 30;
        public const int GETM2ISPEED = 31;
        public const int M1DUTY = 32;
        public const int M2DUTY = 33;
        public const int MIXEDDUTY = 34;
        public const int M1SPEED = 35;
        public const int M2SPEED = 36;
        public const int MIXEDSPEED = 37;
        public const int M1SPEEDACCEL = 38;
        public const int M2SPEEDACCEL = 39;
        public const int MIXEDSPEEDACCEL = 40;
        public const int M1SPEEDDIST = 41;
        public const int M2SPEEDDIST = 42;
        public const int MIXEDSPEEDDIST = 43;
        public const int M1SPEEDACCELDIST = 44;
        public const int M2SPEEDACCELDIST = 45;
        public const int MIXEDSPEEDACCELDIST = 46;
        public const int GETBUFFERS = 47;
        public const int GETPWMS = 48;
        public const int GETCURRENTS = 49;
        public const int MIXEDSPEED2ACCEL = 50;
        public const int MIXEDSPEED2ACCELDIST = 51;
        public const int M1DUTYACCEL = 52;
        public const int M2DUTYACCEL = 53;
        public const int MIXEDDUTYACCEL = 54;
        public const int READM1PID = 55;
        public const int READM2PID = 56;
        public const int SETMAINVOLTAGES = 57;
        public const int SETLOGICVOLTAGES = 58;
        public const int GETMINMAXMAINVOLTAGES = 59;
        public const int GETMINMAXLOGICVOLTAGES = 60;
        public const int SETM1POSPID = 61;
        public const int SETM2POSPID = 62;
        public const int READM1POSPID = 63;
        public const int READM2POSPID = 64;
        public const int M1SPEEDACCELDECCELPOS = 65;
        public const int M2SPEEDACCELDECCELPOS = 66;
        public const int MIXEDSPEEDACCELDECCELPOS = 67;
        public const int SETM1DEFAULTACCEL = 68;
        public const int SETM2DEFAULTACCEL = 69;
        public const int SETM1DITHER = 70;	//deprecated
        public const int SETM2DITHER = 71;	//deprecated
        public const int GETM1DITHER = 72;	//deprecated
        public const int GETM2DITHER = 73;	//deprecated
        public const int SETPINFUNCTIONS = 74;	//roboclaw only
        public const int GETPINFUNCTIONS = 75;	//roboclaw only
        public const int SETDEADBAND = 76;
        public const int GETDEADBAND = 77;
        public const int GETENCODERS = 78;
        public const int GETISPEEDS = 79;
        public const int RESTOREDEFAULTS = 80;
        public const int GETDEFAULTACCEL = 81;
        public const int GETTEMP = 82;
        public const int GETTEMP2 = 83;

        public const int GETERROR = 90;
        public const int GETENCODERMODE = 91;
        public const int SETM1ENCODERMODE = 92;
        public const int SETM2ENCODERMODE = 93;
        public const int WRITENVM = 94;
        public const int READNVM = 95;

        public const int SETCONFIG = 98;
        public const int GETCONFIG = 99;
        public const int SETCTRLSMODE = 100;
        public const int GETCTRLSMODE = 101;
        public const int SETCTRL1 = 102;
        public const int SETCTRL2 = 103;
        public const int GETCTRLS = 104;
        public const int SETAUTO1 = 105;
        public const int SETAUTO2 = 106;
        public const int GETAUTOS = 107;

        public const int SETM1LR = 128;
        public const int SETM2LR = 129;
        public const int GETM1LR = 130;
        public const int GETM2LR = 131;
        public const int CALIBRATELR = 132;
        public const int SETM1MAXCURRENT = 133;
        public const int SETM2MAXCURRENT = 134;
        public const int GETM1MAXCURRENT = 135;
        public const int GETM2MAXCURRENT = 136;
        public const int SETDOUT = 137;
        public const int GETDOUTS = 138;
        public const int SETPRIORITY = 139;
        public const int GETPRIORITY = 140;
        public const int SETADDRESSMIXED = 141;
        public const int GETADDRESSMIXED = 142;
        public const int SETSIGNAL = 143;
        public const int GETSIGNALS = 144;
        public const int SETSTREAM = 145;
        public const int GETSTREAMS = 146;
        public const int GETSIGNALSDATA = 147;
        public const int SETPWMMODE = 148;
        public const int GETPWMMODE = 149;
        public const int SETNODEID = 150;
        public const int GETNODEID = 151;

        public const int RESETSTOP = 200;
        public const int SETESTOPLOCK = 201;
        public const int GETESTOPLOCK = 202;

        public const int SETSCRIPTAUTORUN = 246;
        public const int GETSCRIPTAUTORUN = 247;
        public const int STARTSCRIPT = 248;
        public const int STOPSCRIPT = 249;

        public const int READEEPROM = 252;
        public const int WRITEEEPROM = 253;
        public const int READSCRIPT = 254;
    }

}
