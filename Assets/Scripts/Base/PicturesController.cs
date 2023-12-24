using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PicturesController : MonoBehaviour
{
    GM.GM_Core gm;
    InputField inputField;
    int id;

    // Start is called before the first frame update
    void Start()
    {
        gm = GM.GM_Core.instance;
        inputField = this.GetComponentInChildren<InputField>();
        id = int.Parse(this.transform.Find("ID").GetComponent<Text>().text);
    }

    public void updatePictures()
    {
        gm.UpdatePicDes(id, inputField.text);
    }

    public void deletePicture()
    {
        gm.deletePic(id);
        Destroy(this.gameObject);
    }

    public void selectPicture(bool value)
    {
        if (id == 0) return;
        //Debug.Log(id + " " + value);
        gm.SelectPic(id, value);
    }
}
