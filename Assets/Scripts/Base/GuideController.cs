using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideController : MonoBehaviour
{
    public GameObject guideFeild;
    public Text hintText;
    public Button backBtn;
    public Button nextBtn;
    List<string> hints = new List<string>();

    int currentHint = 0;
    int hintCount = 0;
    
    public void onHelpClick()
    {
        guideFeild.SetActive(true);
    }

    public void onCloseClick()
    {
        guideFeild.SetActive(false);
    }

    public void onNextClick()
    {
        // first hint 
        if (currentHint == 0)
        {
            backBtn.interactable = true;
        }

        currentHint++;
        if (currentHint < hintCount-1)
        {
            hintText.text = hints[currentHint];
        }
        else if (currentHint == hintCount-1) // show the last hint
        {
            hintText.text = hints[currentHint];
            nextBtn.GetComponentInChildren<Text>().text = "完成";
        }
        else // finish all guidances
        {
            currentHint--; // handle overflow
            guideFeild.SetActive(false);
        }
    }

    public void onBackClick()
    {
        if(currentHint == hintCount-1)
        {
            nextBtn.GetComponentInChildren<Text>().text = "下一步";
        }
        currentHint--;
        hintText.text = hints[currentHint];
        if (currentHint == 0)
        {
            backBtn.interactable = false; // disable back button
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHint = 0;

        addHint("1. 截图\n" + "\n" +
            "点击右侧截图按钮可以截取当前内容，\n" +
            "点击查看图片可查看历史截图。");
        addHint("2. 提交报告\n" + "\n" +
            "点击报告按钮，根据提示登录后，\n" +
            "可以填写并提交实验报告。");
        hintCount = hints.Count;
        hintText.text = hints[0];
        backBtn.interactable = false;
    }

    private void addHint(string hint)
    {
        hints.Add(hint);
    }
}
