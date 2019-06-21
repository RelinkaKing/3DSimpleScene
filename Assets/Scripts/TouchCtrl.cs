using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchCtrl : MonoBehaviour
{

    Interaction touchOper;

    Vector2 oposition1;
    Vector2 oposition2;

    Vector3 mousePos;

    bool isTouchFunBan = false;
    public bool isMouseBan = false;
    //光标模式相关
    public GameObject cursor;
    public static bool isCurMode;
    float timeCur;
    //public static bool isOneCilck;
    //public static bool isTwiceClick;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "totalScence")
        {
            isTouchFunBan = true;
        }
        else
        {
            isTouchFunBan = false;
        }
        touchOper = Camera.main.GetComponent<Interaction>();
        //cursor = GameObject.FindGameObjectWithTag("Cursor");
        if (GameObject.Find("Cursor") != null)
            cursor.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            touchOper.isRotate = true;

            touchOper.x += Input.GetAxis("Mouse X") * Interaction.rotateSpeedX;
            touchOper.y -= Input.GetAxis("Mouse Y") * Interaction.rotateSpeedY * Interaction.AnimationRotateY;

            touchOper.RotateOpera(touchOper.x, touchOper.y);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            //DebugLog.DebugLogInfo("----------touchOper.distance -----------" + touchOper.distance);
            //if (PublicClass.currentModle == ModleChoose.AniModel)
            //{
            //    touchOper.distance -= Input.GetAxis("Mouse ScrollWheel") * touchOper.distance;
            //}
            //else
            //{
            touchOper.distance -= Input.GetAxis("Mouse ScrollWheel") * Interaction.distanceSpeed;
            //}
            //touchOper.distance = Mathf.Clamp(touchOper.distance, Interaction.minDistance, Interaction.maxDistance);
            touchOper.ZoomOpera(touchOper.distance);
        }

        if (Input.GetMouseButton(1))
        {
            if (touchOper.lastTransPos.Count == 0)
            {
                touchOper.lastTransPos.Add(touchOper.orthCamera.ScreenToWorldPoint(Input.mousePosition));
            }
            else
            {
                Vector3 tmp = touchOper.orthCamera.ScreenToWorldPoint(Input.mousePosition);
                touchOper.TranslateOpera(touchOper.lastTransPos[0], tmp);
                touchOper.lastTransPos[0] = tmp;
            }
        }
        else
        {
            touchOper.lastTransPos.Clear();
        }
    }


    //点击鼠标
    public void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //判断点击到的物体
            //如果是UI则实施相应的功能
            //如果是空白处则取消选择
            //如果单击模型，则单击部分被选中
            //touchOper.ChooseOpera();
            //isOneCilck = true;
        }
        else
        {
            //isOneCilck = false;
        }
        if (Input.GetMouseButtonDown(1))
        {
            //单机右键暂无功能

        }
    }

    //拖拽鼠标
    public void MouseDrag()
    {
        //判断当前模式，普通模式下实现如下内容
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
        }

        //拖拽鼠标左键，实现旋转
        if (Input.GetMouseButton(0) && ( !Input.GetMouseButton(1)))
        {
#if UNITY_EDITOR
            //鼠标按下开始计时
            timeCur += Time.deltaTime;
            //鼠标光标模式
            if (!isMouseBan && !isTouchFunBan && timeCur >= Interaction.timeCurMode && Vector2.Distance(Input.mousePosition, mousePos) < 1)
            {
                if (cursor != null)
                {
                    isCurMode = true;
                    cursor.transform.position = Input.mousePosition;
                    cursor.SetActive(true);
                }
            }
#elif UNITY_ANDROID

#endif
            if (!isCurMode)
            {
                touchOper.isRotate = true;
                touchOper.x += Input.GetAxis("Mouse X") * Interaction.rotateSpeedX;
                touchOper.y -= Input.GetAxis("Mouse Y") * Interaction.rotateSpeedY * Interaction.AnimationRotateY;
                touchOper.RotateOpera(touchOper.x, touchOper.y);
            }

        }
        else
        {
            timeCur = 0;
            if (isCurMode)
            {
                cursor.SetActive(false);

                isCurMode = false;

            }
            if (touchOper.isRotate)
            {
                touchOper.isRotate = false;
            }
        }

        //拖拽鼠标右键，实现平移
        //if (Input.GetMouseButtonDown(1))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    isModel = Physics.Raycast(ray, out hit);
        //}
        if (Input.GetMouseButton(1))
        {
            if (touchOper.lastTransPos.Count == 0)
            {
                touchOper.lastTransPos.Add(touchOper.orthCamera.ScreenToWorldPoint(Input.mousePosition));
            }
            else
            {
                Vector3 tmp = touchOper.orthCamera.ScreenToWorldPoint(Input.mousePosition);
                touchOper.TranslateOpera(touchOper.lastTransPos[0], tmp);
                touchOper.lastTransPos[0] = tmp;
            }
        }
        else
        {
            touchOper.lastTransPos.Clear();
        }
        if (Input.GetMouseButtonUp(1))
        {
            //isModel = false;
        }

    }

    //滑动滚轮
    public void MouseRollWheel()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            //DebugLog.DebugLogInfo("----------touchOper.distance -----------" + touchOper.distance);
            if (PublicClass.currentModle == ModleChoose.AniModel)
            {
                touchOper.distance -= Input.GetAxis("Mouse ScrollWheel") * touchOper.distance;
            }
            else
            {
                touchOper.distance -= Input.GetAxis("Mouse ScrollWheel") * Interaction.distanceSpeed;
            }
            touchOper.distance = Mathf.Clamp(touchOper.distance, Interaction.instance.minDistance, Interaction.instance.maxDistance);
            touchOper.ZoomOpera(touchOper.distance);
        }
    }

    //单指的情况下所实现的操作
    public void TouchOneFinger()
    {
        if (Input.touchCount == 1)
        {

            // DebugLog.DebugLogInfo("touch_1: " + Input.GetTouch(0) + "  touch_2 : " + pos + "distance" + Vector2.Distance(Input.GetTouch(0).position, pos) + "time" + touchOper.time);
            timeCur += Time.deltaTime;

            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {

                mousePos = Input.GetTouch(0).position;

            }
            if (Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                //长按进入光标模式
                if (!isTouchFunBan && timeCur >= Interaction.timeCurMode && !touchOper.isRotate)
                {
                    if (cursor != null)
                    {
                        isCurMode = true;
                        cursor.transform.position = Input.mousePosition;
                        cursor.SetActive(true);
                    }

                }

            }

            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {

                if (Vector2.Distance(Input.GetTouch(0).position, mousePos) < 5)
                {

                    //触摸手指移动一定范围内也判断为单击选择
                }

                if (!isCurMode)
                {
                    touchOper.isRotate = true;
                    touchOper.x += Input.GetTouch(0).deltaPosition.x;
                    touchOper.y -= Input.GetTouch(0).deltaPosition.y * Interaction.rotateTouchY * Interaction.AnimationRotateY;
                    touchOper.RotateOpera(touchOper.x, touchOper.y);
                }
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                timeCur = 0;
                if (isCurMode)
                {

                    cursor.SetActive(false);
                    isCurMode = false;
                }
                if (touchOper.isRotate)
                {
                    touchOper.isRotate = false;
                }

            }
        }
    }

    //多指的情况下实现的操作
    public void TouchTwoFinger()
    {
        //timer++;
        //if (timer <= 2)
        //    return;

        if (Input.touchCount > 1)
        {
            if (isCurMode) return;

            //if (Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(1).phase == TouchPhase.Began)
            //{
            //    Ray ray1 = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            //    Ray ray2 = Camera.main.ScreenPointToRay(Input.GetTouch(1).position);

            //    RaycastHit hit1, hit2;
            //    if (Physics.Raycast(ray1, out hit1) || Physics.Raycast(ray2, out hit2))
            //    {
            //        isModel = true;
            //    }

            //}
            if (Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(1).phase == TouchPhase.Began)
            {

                XT_MouseFollowRotation.instance.enter_special_two_finger_state();

            }

            //双指位移的情况下
            if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                Vector2 temposition1 = Input.GetTouch(0).position;
                Vector2 temposition2 = Input.GetTouch(1).position;



                //float newDistance = Vector2.Distance(temposition1, temposition1);
                //float offset = newDistance - oldDistance;

                //用于判定触摸手势
                float flag = Input.GetTouch(0).deltaPosition.x * Input.GetTouch(1).deltaPosition.x + Input.GetTouch(0).deltaPosition.y * Input.GetTouch(1).deltaPosition.y;

                //DebugLog.DebugLogInfo("touch_1: " + temposition1 + "  touch_2 : " + temposition2);
                //DebugLog.DebugLogInfo(" flag:  " + flag);

                //DebugLog.DebugLogInfo(" data_list_count: " + touchOper.distance);
                //说明两个触摸点间位移变动，实现缩放功能
                if (flag < 0)
                {
                    if (IsEnlarge(oposition1, oposition2, temposition1, temposition2))
                    {
                        //DebugLog.DebugLogInfo("touch_dis1: " + touchOper.distance);
                        touchOper.distance -= Interaction.distanceSpeedTouch * touchOper.distance;
                    }
                    else
                    {
                        // DebugLog.DebugLogInfo("touch_dis2: " + touchOper.distance);
                        touchOper.distance += Interaction.distanceSpeedTouch * touchOper.distance;
                    }

                    touchOper.distance = Mathf.Clamp(touchOper.distance, Interaction.instance.minDistance, Interaction.instance.maxDistance);

                    touchOper.ZoomOpera(touchOper.distance);

                    //记住最新的触摸点，下次使用  
                    oposition1 = temposition1;
                    oposition2 = temposition2;

                    //touchOper.ZoomOpera(touchOper.distance);
                }

                //实现平移效果
                else
                {
                    //if (isModel)
                    //{

                    if (touchOper.lastTransPos.Count == 0)
                    {
                        touchOper.lastTransPos.Add(touchOper.orthCamera.ScreenToWorldPoint((Input.GetTouch(0).position + Input.GetTouch(1).position) / 2));
                    }
                    else
                    {
                        Vector3 tmp = touchOper.orthCamera.ScreenToWorldPoint((Input.GetTouch(0).position + Input.GetTouch(1).position) / 2);
                        touchOper.TranslateOpera(touchOper.lastTransPos[0], tmp);
                        touchOper.lastTransPos[0] = tmp;
                    }
                    //}
                }


            }
            //结束双指触摸状态
            if ((Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended))
            {
                touchOper.lastTransPos.Clear();
                //isModel = false;
                //退出性能优化模式
                XT_MouseFollowRotation.instance.leave_special_two_finger_state();


            }
        }

        //timer = 0;
    }

    bool IsEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2) //函数返回真为放大，返回假为缩小
    {
        //函数传入上一次触摸两点的位置与本次触摸两点的位置计算出用户的手势
        var leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
        var leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
        if (leng1 < leng2)
        {
            return true; //放大手势
        }
        else
        {
            return false; //缩小手势
        }
    }

}


public class WindowsCursorHandling : MonoBehaviour
{
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ClipCursor(ref RECT rcClip);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetClipCursor(out RECT rcClip);
    [DllImport("user32.dll")]
    static extern int GetForegroundWindow();
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowRect(int hWnd, ref RECT lpRect);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int SetCursorPos(int x, int y);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out POINT point);

    private static RECT currentClippingRect;
    private static RECT originalClippingRect = new RECT();
    private static bool isClipped = false;

    public static int centerX;
    public static int centerY;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
        public POINT(int X, int Y)
        {
            x = X;
            y = Y;
        }
    }

    void Start()
    {
        GetClipCursor(out originalClippingRect);
        centerX = originalClippingRect.Left + (originalClippingRect.Right - originalClippingRect.Left) / 2;
        centerY = originalClippingRect.Top + (originalClippingRect.Bottom - originalClippingRect.Top) / 2;
    }

    public static void StartClipping()
    {
        if (isClipped)
            return;
        var hndl = GetForegroundWindow();
        GetWindowRect(hndl, ref currentClippingRect);
        ClipCursor(ref currentClippingRect);

#if UNITY_EDITOR
        centerX = Screen.width / 2;
        centerY = Screen.height / 2;
#else
        centerX = currentClippingRect.Left + (currentClippingRect.Right - currentClippingRect.Left) / 2;
        centerY = currentClippingRect.Top + (currentClippingRect.Bottom - currentClippingRect.Top) / 2;
#endif
        isClipped = true;
    }

    static public void CenterCursor()
    {
        SetCursorPos(centerX, centerY);
    }

    void OnApplicationQuit()
    {
        StopClipping();
    }
    public static void StopClipping()
    {
        isClipped = false;
        ClipCursor(ref originalClippingRect);
    }
}