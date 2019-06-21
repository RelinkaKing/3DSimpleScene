using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VesalCommon;

public class PublicClass
{
    public static List<string> id_list = new List<string> { "RA0801004", "RA0801005", "RA0801006" };//"RA0801001", "RA0801002", "RA0801003",
    public static bool initail_flag = true;
    public static Run_Quality Quality = Run_Quality.GOOD;
    public static int MAX_Ab_num = 10;//加载AB超限的阈值，用于卸载多余的AB
    public static string low_res_ablist = "";
    public static int ThresholdModelNumber = 2000;
    static string the_version = "3.2.0";
    static string assets_version = "3.2.0";
    public static int MaxTryDownloadNums = 3;
    public static bool isShowExpPanel = true;
    public static Vector3 cameraBackAxisPosition; //摄像机旋转中心轴Position
    public static Vector3 cameraBackAxisRotation; //摄像机旋转中心轴Rotation
    public static RunState currentState;
    public static ModleChoose currentModle;
    public static Transform[] modelAndChild;

    public static App_State appstate;

    public static int int_load_AB_nums = 0;//预先加载ab包的数量，用于控制加载时间。

    public static string get_version()
    {
        return the_version;
    }

    public static string get_assets_version()
    {
        return assets_version;
    }

    public static GameObject currentGameobject;
    public static GameObject lastGameobject;
    public static FingerState currentFingerState = FingerState.Null;
    public static FingerState CurrentFingerState
    {
        get { return currentFingerState; }
        set
        {
            currentFingerState = value;
        }
    }
    public static void InitFingerState()
    {
        currentFingerState = FingerState.Null;
    }

    public static int Difficult_Index; //游戏难度系数

    public static string fileLocalStreamingPath;
    public static string vesal_db_path;
    public static string sign_texture_path;
    public static string filePath;
    public static string tempPath;
    public static string UpdatePath;
    public static string SignPath;
    public static string ModelPath;
    public static string MedicalPath;
    public static string WeiKePlayer_path;
    public static string Anim_TimelinePath;
    public static string TimelineFilePath;
    public static string Anim_ABPath;
    public static string PPT_Url = "";
    public static string xluaPath;
    public static string Video_path;
    public static string BookMarkPath;

    public static string server_ip;
    public static string server_test_url;
    public static bool online_mode;
    public static int data_list_count;
    public static int tableCount;
    public static Dictionary<string, string> Model_AB_dic = new Dictionary<string, string>();


    public static string get_server_interface;
    public static string fix_server_interface;
    public static bool is_enter_server_control;
    public static bool isRotateScreen;

    public static void InitStaticData()
    {
        int mem = SystemInfo.systemMemorySize;
#if UNITY_EDITOR
        //QualitySettings.currentLevel = QualityLevel.Fastest;
        //Quality = Run_Quality.POOL;
        QualitySettings.currentLevel = QualityLevel.Good;
        Quality = Run_Quality.GOOD;
#elif UNITY_STANDALONE_WIN
        if (mem <= 4400)
        {
            QualitySettings.currentLevel = QualityLevel.Fastest;
            Quality = Run_Quality.POOL;
        }
        else
        {
            QualitySettings.currentLevel = QualityLevel.Good;
            Quality = Run_Quality.GOOD;
        }
#elif UNITY_IOS
        if (mem <= 1100)
        {
            QualitySettings.currentLevel = QualityLevel.Fastest;
            Quality = Run_Quality.POOL;
        }
        else
        {
            QualitySettings.currentLevel = QualityLevel.Good;
            Quality = Run_Quality.GOOD;
        }
#elif UNITY_ANDROID
        if (mem <= 3300)
        {
            QualitySettings.currentLevel = QualityLevel.Fastest;
            Quality = Run_Quality.POOL;
        }
        else
        {
            QualitySettings.currentLevel = QualityLevel.Good;
            Quality = Run_Quality.GOOD;
        }
#else
#endif
        Debug.Log("Quality :" + Quality);
        cameraBackAxisPosition = Vector3.zero;
        cameraBackAxisRotation = Vector3.zero;
        currentState = RunState.Loading;
        currentModle = ModleChoose.MainModel;
        Difficult_Index = 0;
        data_list_count = 0;
        tableCount = 15;
        filePath = Application.persistentDataPath + "/";
#if UNITY_EDITOR
        fileLocalStreamingPath = "file://" + Application.streamingAssetsPath + "/";
        filePath = Application.streamingAssetsPath + "/";
#elif UNITY_IOS
        fileLocalStreamingPath = "file://" + Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
        fileLocalStreamingPath = "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_STANDALONE_WIN
        fileLocalStreamingPath =  Application.streamingAssetsPath + "/";
        filePath = Application.persistentDataPath.Substring(0, Application.persistentDataPath.LastIndexOf("AppData") + 7) + "/roaming/Vesal_unity_PC/";
        Debug.Log("UNITY_STANDALONE_WIN filePath:"+filePath );
#endif
        tempPath = filePath + "temp/";
        Vesal_DirFiles.DelFile(tempPath + "temp.dat");
        vesal_db_path = filePath + "db/";
        sign_texture_path = filePath + "sign_texture/";
        UpdatePath = filePath + "Update/";
        SignPath = filePath + "Android_sign/";
        ModelPath = filePath + "model/";
        MedicalPath = filePath + "Mediacl/";
        xluaPath = filePath + "Lua-HotFix/";
        TimelineFilePath = filePath + "Anim_Timeline/";
        WeiKePlayer_path = filePath + "microlesson/";
        Video_path = filePath + "acu_vdo/";
        Anim_TimelinePath=filePath+"Anim_Timeline/";
        BookMarkPath = filePath + "BookmarkFile/";
        if (Quality == Run_Quality.GOOD)
            MAX_Ab_num = 15;
        else
            MAX_Ab_num = 10;

        //get_server_interface = "http://api.vesal.cn:8000/vesal-jiepao-prod/server?type=0";//正式服
        get_server_interface = "http://114.115.210.145:8083/vesal-jiepao-test/server?type=0";//测试服
        fix_server_interface = "v1/app/member/getCode";
        online_mode = true;
        is_enter_server_control = false;
        is_has_app_buff = false;
        isRotateScreen = false;
        DebugLog.DebugLogInfo("---------------设备内存-------------------" + mem);
    }

    public static void InitDirFiles()
    {
        Vesal_DirFiles.CreateDir(PublicClass.filePath);
        Vesal_DirFiles.CreateDir(PublicClass.tempPath);
        Vesal_DirFiles.CreateDir(PublicClass.vesal_db_path);
        Vesal_DirFiles.CreateDir(PublicClass.sign_texture_path);
        Vesal_DirFiles.CreateDir(PublicClass.UpdatePath);
        Vesal_DirFiles.CreateDir(PublicClass.SignPath);
        Vesal_DirFiles.CreateDir(PublicClass.ModelPath);
        Vesal_DirFiles.CreateDir(PublicClass.MedicalPath);
        Vesal_DirFiles.CreateDir(PublicClass.xluaPath);
        Vesal_DirFiles.CreateDir(PublicClass.Anim_TimelinePath);
        Vesal_DirFiles.CreateDir(PublicClass.Video_path);
        Vesal_DirFiles.CreateDir(PublicClass.WeiKePlayer_path);
        // DebugLog.DebugLogInfo("---------------设备内存-------------------" + mem);
        Vesal_DirFiles.CreateDir(PublicClass.BookMarkPath);
    }

    public static void SetScenceQuality()
    {
#if UNITY_EDITOR
        if (Quality == Run_Quality.POOL)
        {
            QualitySettings.currentLevel = QualityLevel.Fastest;
            QualitySettings.antiAliasing = 4;
        }
        else
        {
            QualitySettings.currentLevel = QualityLevel.Fantastic;
            QualitySettings.antiAliasing = 8;
        }
#elif UNITY_STANDALONE_WIN
        if (Quality== Run_Quality.POOL)
        {
            QualitySettings.currentLevel = QualityLevel.Fastest;
            QualitySettings.antiAliasing=4;
        }
        else
        {
            QualitySettings.currentLevel = QualityLevel.Fantastic;
            QualitySettings.antiAliasing=8;
        }

#elif UNITY_IOS
        if (Quality == Run_Quality.POOL)
        {
            QualitySettings.currentLevel = QualityLevel.Fastest;
            QualitySettings.antiAliasing = 0;
        }
        else
        {
            QualitySettings.currentLevel = QualityLevel.Fastest;
            QualitySettings.antiAliasing = 2;
        }
#elif UNITY_ANDROID
        if (Quality == Run_Quality.POOL)
        {
            QualitySettings.currentLevel = QualityLevel.Fastest;
            QualitySettings.antiAliasing = 0;
        }
        else
        {
            QualitySettings.currentLevel = QualityLevel.Fastest;
            QualitySettings.antiAliasing = 2;
        }
#else
#endif


    }


    public static void ClearTempAsset()
    {
        try
        {
            //清空所有命令
            DirectoryInfo dir = new DirectoryInfo(PublicClass.filePath);
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].FullName.Contains("Command"))
                {
                    try
                    {
                        File.Delete(files[i].FullName);
                    }
                    catch (System.Exception)
                    {

                    }
                }
            }
        }
        catch (System.Exception)
        {

        }
    }

    // public static GameObject Model_A;
    public static bool is_rotate_screen; //是否旋转屏幕
    public static Transform Transform_parent;
    public static Transform Transform_temp;
    //以下字典提供通过名称找到在Allmodes中的位置
    public static Dictionary<string, int> id_model_dic;
    public static bool is_has_app_buff;
}


public enum App_State
{
    Init = 1,
    LoadAsset = 2,
    Init_2 = 3,
    Init_3 = 4,
    Init_4 = 5,
    Running = 6
}


public enum Run_Quality
{
    POOL = 1,
    GOOD = 2
}

public enum RunState
{
    Loading,
    Horizontal_screen,
    Vertical_screen,
    UI,
    Playing
}
public enum ModleChoose
{
    MainModel,
    SignModel,
    AniModel
}

public enum FingerState
{
    Null,
    SingleFingerDown, //单指按下
    SingleFingerSinglePress, //单击
    SingleFingerDoublePress, //双击
    SingleFingerUp, //单指抬起
    SingleFingerRotate, //单指旋转
    DoubleFingerDown, //双指按下
    DoubleFingerMove, //双指移动
    DoubleFingerZoom //双指缩放
}

public enum OperaType
{
    //模型操作类型
    model,
    rightmenu,
    changeCamera,
}