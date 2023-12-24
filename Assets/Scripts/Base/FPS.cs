using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    public Text t;
    public static float f_Fps;
    public float f_UpdateInterval = 0.5f; //每个0.5秒刷新一次  
    private float f_LastInterval; //游戏时间  
    private int i_Frames = 0;//帧数  
    void Awake()
    {
        Application.targetFrameRate = -1;
    }
    void Update()
    {
        ++i_Frames;

        if (Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval)
        {
            f_Fps = i_Frames / (Time.realtimeSinceStartup - f_LastInterval);

            i_Frames = 0;

            f_LastInterval = Time.realtimeSinceStartup;
        }
        t.text = "FPS:" + f_Fps.ToString("f2");
    } 
}
