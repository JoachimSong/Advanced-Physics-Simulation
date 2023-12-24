using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour {
    public GameObject stepPanel;
    public GameObject objectPanel;
    public GameObject settingsPanel;
    public GameObject completeButton;
    public GameObject systemUI;
    public GameObject coverUI;
    public GameObject UIcontroltext;
    public GameObject nopermissionPanel;
    public GameObject reportwaitPanel;
    public GameObject reportsuccessPanel;
    public GameObject reportfailPanel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CloseSuccess()
    {
        reportsuccessPanel.SetActive(false);
    }

    public void CloseFail()
    {
        reportfailPanel.SetActive(false);
    }



    public void CoverUI()
    {
        if (systemUI.activeSelf)
        {
            systemUI.SetActive(false);
            GM.GM_Core.instance.setPicturePanel(false);
            coverUI.SetActive(true);
        }
        else
        {
            systemUI.SetActive(true);
            GM.GM_Core.instance.setPicturePanel(true);
            coverUI.SetActive(false);
        }
    }

    public void ReportComplete(bool value)
    {
        reportwaitPanel.SetActive(false);
        if (value)
        {
            reportsuccessPanel.SetActive(true);
        }
        else
        {
            reportfailPanel.SetActive(true);
        }
    }
}
