using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using VesalCommon;

public class DebugLog : MonoBehaviour {

    public static DebugLog instance;
    public bool printLog = false; //是否允许打印信息

    static List<string> mLines = new List<string> ();
    int count;
    // private string outpath;  
    public void Awake () {
        instance = this;
    }

    void Start () {
        count = 0;
        // outpath = Application.streamingAssetsPath + "/outLog.txt";  
        // //每次启动客户端删除之前保存的Log  
        // if (System.IO.File.Exists(outpath))  
        // {  
        //     File.Delete(outpath);  
        // }  
        //在这里做一个Log的监听,转载的原文中是用Application.RegisterLogCallback(HandleLog);但是这个方法在unity5.0版本已经废弃不用了  
        Application.logMessageReceived += HandleLog;

        DontDestroyOnLoad (this.gameObject);
    }

    void HandleLog (string logString, string stackTrace, LogType type) {
        if (type == LogType.Error || type == LogType.Exception) {
            Log (logString);
            Log (stackTrace);
        }
    }

    void Update () {
        ////因为写入文件的操作必须在主线程中完成，所以在Update中哦给你写入文件。  
        //if (mWriteTxt.Count > 0)  
        //{  
        //    string[] temp = mWriteTxt.ToArray();  
        //    foreach (string t in temp)  
        //    {  
        //        using (StreamWriter writer = new StreamWriter(outpath, true, Encoding.UTF8))  
        //        {  
        //            writer.WriteLine(t+"— —"+System.DateTime.Today);  
        //        }  
        //        mWriteTxt.Remove(t);  
        //    }  
        //}  
    }
    void OnGUI () {
// #if UNITY_DEBUG
        if (printLog) {
            if (DateTime.Now < new DateTime(2018, 12, 31, 23, 16, 0, DateTimeKind.Local)) {
                GUI.color = Color.red;
            for (int i = 0, imax = mLines.Count; i < imax; ++i) {
                GUILayout.Label (mLines[i]);
            }

            // GUIStyle bb = new GUIStyle ();
            // bb.normal.background = null; //这是设置背景填充的
            // bb.normal.textColor = new Color (1.0f, 0.5f, 0.0f); //设置字体颜色的
            // bb.fontSize = 40; //当然，这是字体大小

            // ////居中显示FPS
            // //GUI.Label(new Rect((Screen.width / 2) - 40, 0, 200, 200), "FPS: " + m_FPS, bb);

            // GUI.Label (new Rect ((Screen.width / 2) - 100, 0, 200, 200), "reallyTime: " + System.DateTime.Now.Second, bb);
            // GUI.Label (new Rect ((Screen.width / 2) - 100, 50, 200, 200), " data_list_count: " + PublicClass.data_list_count, bb);
            }
        }
// #endif
    }



    //这里我把错误的信息保存起来，用来输出在手机屏幕上  
    static public void Log (params object[] objs) {
        string text = "";
        for (int i = 0; i < objs.Length; ++i) {
            if (i == 0) {
                text += objs[i].ToString ();
            } else {
                text += ", " + objs[i].ToString ();
            }
        }
        if (Application.isPlaying) {
            try {
                if (mLines.Count > 20) {
                    mLines.Clear ();
                }
                mLines.Add (text);
            } catch (System.Exception) { }

        }
    }
    public static void DebugLogInfo(string color, string str)
    {
        vesal_log.vesal_write_log (str);
        Debug.Log("<color=#" + color + ">" + str + "</color>");
    }
    public static void DebugLogInfo (float log) {

    }
    public static void DebugLogInfo (string log) {
// #if UNITY_DEBUG
//         log +="---------"+ System.DateTime.Now;
        Log ("\r\n" + log); //屏幕显示日志文件
// #endif
        Debug.Log (log); //unity  自带日志文件
        vesal_log.vesal_write_log (log); //外部日志文件
    }
    public void DebugLogInfo (int log) {
        Log ("\r\n" + log);
    }

    [ContextMenu ("clear")]
    void Clear () { } //上下文菜单操作函数
    [DllImport ("dllname")]
    private static extern void ExtendC ();
}

public class vesal_log {
    public static String _log_path = PublicClass.tempPath + "unity.log";
    static String _pid = "xxx";
    static public void vesal_write_log (String logcontent) {
        try {
            _log_path = PublicClass.tempPath + "unity.log";
            FileStream fs;
            if (!File.Exists(_log_path))
            {
                Vesal_DirFiles.CreateDir(PublicClass.tempPath);
                fs = new FileStream(_log_path, System.IO.FileMode.CreateNew);
            }
            else {
                fs= new FileStream (_log_path, System.IO.FileMode.Append);
            }
            String time_now = DateTime.Now.ToString ();
            // FileStream fs = new FileStream("D:\\vesal.log", System.IO.FileMode.Append, FileAccess.Read, FileShare.ReadWrite);
            //获得字节数组
            byte[] data = System.Text.Encoding.UTF8.GetBytes (time_now + " : [" + _pid + "] " + logcontent + "\r\n\r\n");
            //开始写入
            fs.Write (data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush ();
            fs.Close ();
        } catch (Exception e) {
            UnityEngine.Debug.Log (e.Message);
        }
    }
}