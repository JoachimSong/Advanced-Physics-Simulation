using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMove : MonoBehaviour
{
    public float speed = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))//↑
        {
            this.transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.DownArrow))//↓
        {
            this.transform.Translate(new Vector3(0, 0, -1 * speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.RightArrow))//→
        {
            this.transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))//←
        {
            this.transform.Translate(new Vector3(-1 * speed * Time.deltaTime , 0, 0));
        }
        if (Input.GetKey(KeyCode.J))//J
        {
            this.transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.K))//K
        {
            this.transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
