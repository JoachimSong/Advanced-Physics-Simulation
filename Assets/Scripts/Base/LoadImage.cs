using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static byte[] SetImageToByte(string imgPath)
    {
        FileStream fs = new FileStream(imgPath, FileMode.Open);
        byte[] imgByte = new byte[fs.Length];
        fs.Read(imgByte, 0, imgByte.Length);
        fs.Close();
        return imgByte;
    }

    public static Texture2D GetTextureByByte(byte[] imgByte)
    {
        Texture2D tex = new Texture2D(100, 100);
        tex.LoadImage(imgByte);
        return tex;
    }
}
