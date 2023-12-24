using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using WEB;

public class PictureInfo
{
    public string Name
    {
        get;
        set;
    }

    public string Des
    {
        get;
        set;
    }

    public byte[] Byte
    {
        get;
        set;
    }
}
public class Report
{
    public string Title
    {
        get;
        set;
    }

    public string Purpose
    {
        get;
        set;
    }

    public int Num
    {
        get;
        set;
    }

    public string listJson
    {
        get;
        set;
    }


}

[Serializable]
public class ReportPic
{
    public string base64;
    public string des;
}



public class ReportController : MonoBehaviour
{
    public string Account = "guest";
    public string CourseWebsiteHostURL = "localhost:8080";
    public string ReportSubURL = "report";

    public Text PictureText;
    public Text ScoreText;
    public GameObject reportPrefab;
    public GameObject scrollContent;
    public GameObject picScrollContent;
    public GameObject picturePanel;
    public GameObject reportWaitPanel;
    public GameObject reportSuccessPanel;
    public GameObject reportFailPanel;
    public GameObject picPrefab;
    public GameObject loginPrefab;
    public GameObject loginError;
    public GameObject newReport;
    public GameObject loginWait;


    public List<GM.PictureInfo> selectedPics;
    public Dictionary<int, PictureInfo> pictures;
    private int selectNum;
    private string title;
    private string purpose;
    private string picfold = "screens";
    private int picID = 1;
    private string _username;
    private string _password;
    public WebRequest webRequest;

    // Start is called before the first frame update
    void Start()
    {

        pictures = new Dictionary<int, PictureInfo>();
        selectNum = 0;

        title = "物理仿真算法编程实验";
        purpose = "本实验项目支持物理仿真算法编程这一数字媒体实践教学的难点内容。学生利用平台提供的框架，直观可视地编程实现交互媒体应用中的物理仿真技术，掌握常用的物理仿真数值求解算法及其特点。";

    }

    public void SetTitle(string titleinput)
    {
        title = titleinput;
    }

    public void SetPurpose(string purposeinput)
    {
        purpose = purposeinput;
    }

    public void SelectPicture()
    {
        picturePanel.GetComponent<ObjectPartController>().pick();
        foreach (Transform t in picScrollContent.transform)
        {
            t.Find("Toggle").gameObject.SetActive(true);
        }
    }

    public void SelectComplete()
    {
        selectedPics = new List<GM.PictureInfo>(GM.GM_Core.instance.selectPics);
        selectNum = selectedPics.Count;
        if (selectNum == 0) PictureText.text = "<color=white>请上传可展示实验结果的截图</color>";
        else PictureText.text = "<color=#ffff00>获取到<color=#00ff00> " + selectNum + " </color>张图片数据</color>";

        for (int i = scrollContent.transform.childCount - 1; i >= 0; i--) Destroy(scrollContent.transform.GetChild(i).gameObject);
        for (int i = 1; i <= selectNum; ++i)
        {
            GameObject g = Instantiate(reportPrefab, scrollContent.transform);
            g.transform.Find("Number").GetComponent<Text>().text = i.ToString();
            g.GetComponentInChildren<RawImage>().texture = LoadImage.GetTextureByByte(selectedPics[i - 1].Byte);
            g.transform.Find("description").GetComponent<Text>().text = selectedPics[i - 1].Des;
        }
        //Debug.Log(selectNum);
    }

    public void Login()
    {
        this.gameObject.SetActive(true);
        webRequest.Login(_username, _password);
        loginPrefab.SetActive(false);
        loginWait.SetActive(true);

        Invoke("DelayOpen", 3);


    }
    public void DelayOpen()
    {
        loginWait.SetActive(false);
        if (!webRequest.IsLogin())
        {
            this.gameObject.SetActive(false);
            newReport.SetActive(false);
            loginError.SetActive(true);
        }
        else
        {
            newReport.SetActive(true);
        }
    }

    public void FinishLogin()
    {
        loginPrefab.SetActive(false);
    }
    public void FinishErrorLogin()
    {
        loginError.SetActive(false);
    }
    public void InputUsername(string username)
    {
        _username = username;
    }
    public void InputPassword(string password)
    {
        _password = password;
    }
    public void Confirm()
    {
        reportWaitPanel.SetActive(true);

        Report report = new Report();

        report.Title = title;
        report.Purpose = purpose;
        report.Num = selectNum;

        List<ReportPic> reportlist = new List<ReportPic>();
        for (int i = 0; i < selectNum; ++i)
        {
            ReportPic reportPic = new ReportPic();
            reportPic.base64 = Convert.ToBase64String(selectedPics[i].Byte);
            reportPic.des = selectedPics[i].Des;
            reportlist.Add(reportPic);
        }
        report.listJson = SerializeList.ListToJson<ReportPic>(reportlist);

        if(webRequest.Report(report.Title, report.Purpose, report.Num, report.listJson, 2))
        {
            //Debug.Log("login!");
        }
        else
        {
            //Debug.Log("not login!");
            loginPrefab.SetActive(true);
        }

        OnReportComplete(true);
        Close();
    }


    public void OnReportComplete(bool success)
    {
        reportWaitPanel.SetActive(false);
        if (success)
        {
            reportSuccessPanel.SetActive(true);
            Invoke("DelayDestroySuccess", 2);
        }
        else
        {
            reportFailPanel.SetActive(true);
            Invoke("DelayDestroyFail", 2);
        }
    }

    public void DelayDestroySuccess()
    {
        reportSuccessPanel.SetActive(false);
    }

    public void DelayDestroyFail()
    {
        reportFailPanel.SetActive(false);
    }

    public void CaptureScreenByUnity()
    {
        string pictureName = "screen" + (picID) + ".jpg";
        string pictureDes = "未添加描述";
        PictureInfo newPic = new PictureInfo();
        newPic.Name = pictureName;
        newPic.Des = pictureDes;
        pictures.Add(picID, newPic);
        ScreenCapture.CaptureScreenshot(picfold + "/" + pictureName);
        StartCoroutine(Addpic(picfold + "/" + pictureName, pictureDes, picID));
        picID++;
    }
    IEnumerator Addpic(string imgPath, string description, int picID)
    {
        while (!File.Exists(imgPath)) { yield return new WaitForSeconds(0.1f); }
        GameObject p = Instantiate(picPrefab, scrollContent.transform);
        byte[] imgByte = LoadImage.SetImageToByte(imgPath);
        Texture2D tex = LoadImage.GetTextureByByte(imgByte);
        p.GetComponentInChildren<RawImage>().texture = tex;
        pictures[picID].Byte = imgByte;
        p.transform.Find("ID").GetComponent<Text>().text = picID.ToString();
        //Debug.Log("capture successfully");
    }
    public void showMySelf()
    {
        if (!webRequest.IsLogin())
        {
            loginPrefab.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(true);
            newReport.SetActive(true);
        }
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        newReport.SetActive(false);
    }
}
