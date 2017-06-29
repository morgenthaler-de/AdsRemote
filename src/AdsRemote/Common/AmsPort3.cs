namespace AdsRemote.Common
{
    /// <summary>
    /// TwinCAT 3 ADS Ports
    /// </summary>
    public enum AmsPort3 : int
    {
        Router = 1,
        Debugger = 2,
        Logger = 100,
        EventLog = 110,
        R0_Realtime = 200,
        R0_Trace = 290,
        R0_IO = 350,
        R0_AdditionalTask1 = 350,
        R0_AdditionalTask2 = 351,
        R0_NC = 500,
        R0_NCSAF = 501,
        R0_NCSVB = 511,
        R0_ISG = 550,
        R0_CNC = 600,
        R0_LINE = 700,
        R0_PLC = 800,
        PlcRuntime1 = 851,
        PlcRuntime2 = 852,
        PlcRuntime3 = 853,
        PlcRuntime4 = 854,
        CamshaftController = 900,
        R0_CAMTOOL = 950,
        R0_USER = 2000,
        R3_CTRLPROG = 10000,
        SystemService = 10000,
        R3_SYSCTRL = 10001,
        R3_SYSSAMPLER = 10100,
        R3_TCPRAWCONN = 10200,
        R3_TCPIPSERVER = 10201,
        R3_SYSMANAGER = 10300,
        R3_SMSSERVER = 10400,
        R3_MODBUSSERVER = 10500,
        R3_S7SERVER = 10600,
        R3_PLCCONTROL = 10800,
        R3_NCCTRL = 11000,
        R3_NCINTERPRETER = 11500,
        R3_STRECKECTRL = 12000,
        R3_CAMCTRL = 13000,
        R3_SCOPE = 14000,
        R3_SINECH1 = 15000,
        R3_CONTROLNET = 16000,
        R3_OPCSERVER = 17000,
        R3_OPCCLIENT = 17500,
        USEDEFAULT = 65535
    }
}
