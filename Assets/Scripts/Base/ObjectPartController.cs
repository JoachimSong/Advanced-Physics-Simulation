using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPartController : MonoBehaviour
{
    public GameObject retButton;
    public GameObject comeButton;
    public GameObject pickButton;
    private int RetOrCome = 0;
    private Vector3 position1;
    private Vector3 position2;
    private Vector3 currentV;
    // Start is called before the first frame update
    void Start()
    {
        position1 = transform.position - new Vector3(400, 0, 0);
        position2 = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (RetOrCome == -1)
        {
            // 根据当前屏幕宽度进行更新
            position2.x = Screen.width;
            transform.position = Vector3.SmoothDamp(transform.position, position2, ref currentV, 0.5f);
            if ((position2 - transform.position).x < 0.01)
            {
                RetOrCome = 0;
            }
        }
        else if (RetOrCome == 1)
        {
            // 根据当前屏幕宽度进行更新
            position1.x = Screen.width - 400;
            transform.position = Vector3.SmoothDamp(transform.position, position1, ref currentV, 0.5f);
            if ((transform.position - position1).x < 0.01)
            {
                RetOrCome = 0;
            }
        }
    }

    public void come()
    {
        RetOrCome = 1;
        retButton.SetActive(true);
        comeButton.SetActive(false);
        // 隐藏返回主菜单的按键
        //GM.GM_Core.instance.setReturnButton(false);
        if (this.transform.name != "picturePanel") GM.GM_Core.instance.setPicturePanel(false);
    }

    public void ret()
    {
        RetOrCome = -1;
        retButton.SetActive(false);
        comeButton.SetActive(true);
        // 恢复返回主菜单的按键
        //GM.GM_Core.instance.setReturnButton(true);
        if (this.transform.name != "picturePanel") GM.GM_Core.instance.setPicturePanel(true);
    }

    public void pick()
    {
        RetOrCome = 1;
        pickButton.SetActive(true);
        comeButton.SetActive(false);
        //GM.GM_Core.instance.setReturnButton(false);
    }

    public void select()
    {
        RetOrCome = -1;
        pickButton.SetActive(false);
        comeButton.SetActive(true);
        //GM.GM_Core.instance.setReturnButton(true);
        GM.GM_Core.instance.UploadPic();
    }
}
