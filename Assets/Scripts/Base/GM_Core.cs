using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

namespace GM
{
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
    };

    public class GM_Core : MonoBehaviour
    {

        public static GM_Core instance = null;

        public GameObject catchscreenButton;
        public GameObject picturePanel;
        public GameObject scrollContent;
        public GameObject picPrefab;

        private string picfold = "screens";
        private int picID = 1;

        public Dictionary<string, bool> eventDic;
        public Dictionary<string, int> eventID;
        public List<string> eventIdList;
        public List<Dropdown.OptionData> options;
        public Dictionary<int, PictureInfo> pictures;
        public List<int> selectIDs;
        public List<PictureInfo> selectPics;
        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            InitGame();
        }

        void InitGame()
        {
        }

        // Use this for initialization
        void Start()
        {
            eventDic = new Dictionary<string, bool>();
            eventID = new Dictionary<string, int>();
            eventIdList = new List<string>();
            pictures = new Dictionary<int, PictureInfo>();
            selectIDs = new List<int>();
            selectPics = new List<PictureInfo>();

            List<string> substanceType = new List<string> { "空" };

            Directory.CreateDirectory(picfold);

            options = new List<Dropdown.OptionData>();
            foreach (string t in substanceType)
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = t.ToString();
                options.Add(option);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (catchscreenButton.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    catchscreenButton.GetComponent<Button>().onClick.Invoke();
                }
            }
        }
        private void OnDestroy()
        {
            Directory.Delete(picfold, true);
        }
        public void ExitGame()
        {
            Application.Quit();
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
        IEnumerator Addpic(string imgPath,string description,int picID)
        {
            while (!File.Exists(imgPath)) { yield return new WaitForSeconds(0.1f); }
            GameObject p = Instantiate(picPrefab, scrollContent.transform);
            byte[] imgByte = LoadImage.SetImageToByte(imgPath);
            Texture2D tex = LoadImage.GetTextureByByte(imgByte);
            p.GetComponentInChildren<RawImage>().texture = tex;
            pictures[picID].Byte = imgByte;
            p.transform.Find("ID").GetComponent<Text>().text = picID.ToString();
            Debug.Log("capture successfully");
        }
        public void UpdatePicDes(int id,string des)
        {
            pictures[id].Des = des;
        }
        public void ReportOpenPic()
        {
            picturePanel.GetComponent<ObjectPartController>().pick();
            foreach (Transform t in scrollContent.transform)
            {
                t.Find("Toggle").gameObject.SetActive(true);
            }
        }
        public void SelectPic(int id, bool value)
        {
            if (value) selectIDs.Add(id);
            else selectIDs.Remove(id);
        }
        public void UploadPic()
        {
            foreach (Transform t in scrollContent.transform)
            {
                t.Find("Toggle").gameObject.SetActive(false);
            }
            for(int i = 0; i < selectIDs.Count; ++i)
            {
                selectPics.Add(pictures[selectIDs[i]]);
            }
            GameObject.Find("ReportPanel").GetComponent<ReportController>().SelectComplete();
            selectPics.Clear();
        }
        public void deletePic(int id)
        {
            Debug.Log(id);
            if (selectIDs.Contains(id)) selectIDs.Remove(id);
            pictures.Remove(id);
        }
        public void setPicturePanel(bool input)
        {
            picturePanel.SetActive(input);
        }

    }
}
