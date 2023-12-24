using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HairController : MonoBehaviour
{
    //compute shader线程数和组数
    private const int THREAD_X = 8;
    private int _groupX;
    //控制顶点总数
    private int totalNodeCount;
    //头发结构体
    struct HairData
    {
        public Vector3[] pos;     //控制顶点
        public Vector3[] points;  //绘制点
    }
    //头发数组
    private HairData[] Hairs;
    //控制顶点的位置
    private Vector3[] position;

    public ComputeShader hairSimulate;

    //compute shader中的缓存
    private ComputeBuffer positionBuffer;
    private ComputeBuffer prePositionBuffer;

    //kernel函数
    private int _initKernel;
    private int _stepKernel;

    //使用松弛法时迭代的次数
    public int iterations = 2;
    //渲染半径
    public float hairRadius = 0.1f;
    //时间步长的平方
    public float sqrDt = 0.08f;
    //重量
    public float gravity = -0.5f;
    //阻尼系数
    public float damping = 0.1f;
    //发丝数量
    public int hairCount = 128;
    //每根头发控制顶点的数量
    public int nodeCount = 9;
    //每个头发节点的间距
    public float nodeDistance = 0.5f;
    //绘制顶点的数量
    private int pointCount = 50;
    //头发生成范围
    public int hairRangeY = 0;
    public int hairRangeXZ = 360;
    public float headRadius = 0.5f;

    //记录头的位姿
    public GameObject head;
    private Quaternion oldRotation;
    private Vector3 oldPosition;

    //用于绘制
    public Material material;
    private LineRenderer[] lines;
    public GameObject hairLine;

    //UI显示和控制的参数
    public Text gravityText;
    public Slider gravitySlider;

    public Text dampingText;
    public Slider dampingSlider;

    public Text quantityText;
    public Slider quantitySlider;

    public Text lengthText;
    public Slider lengthSlider;

    public Text radiusText;
    public Slider radiusSlider;

    //隐式欧拉
    public Toggle implicitEulerToggle;
    public bool implicitEuler = false;

    public void Initialize()
    {
        //根据实际头发数量和线程数计算组数
        _groupX = hairCount / THREAD_X;
        //设置kernel的系统参数
        hairSimulate.SetInt("iterations", iterations);
        hairSimulate.SetFloat("gravity", gravity);
        hairSimulate.SetFloat("damping", damping);
        hairSimulate.SetFloat("headRadius", headRadius);
        hairSimulate.SetInt("hairCount", hairCount);
        hairSimulate.SetInt("nodeCount", nodeCount);
        hairSimulate.SetFloat("nodeDistance", nodeDistance);
        hairSimulate.SetVector("headPos", head.transform.position);
        hairSimulate.SetFloat("sqrDt", sqrDt);
        hairSimulate.SetBool("implicitEuler", implicitEuler);
        //绑定kernel函数
        _initKernel = hairSimulate.FindKernel("Init");
        _stepKernel = hairSimulate.FindKernel("Step");
        //初始化Buffer
        GenerateBuffer();
        //记录head的初始位姿
        oldRotation = head.transform.rotation;
        oldPosition = head.transform.position;

    }
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        gravityText.text = "重量：" + ((int)(gravity * -10)).ToString();
        gravitySlider.value = (int)(gravity * -10);

        dampingText.text = "阻尼系数：" + ((int)(damping * 10)).ToString();
        dampingSlider.value = (int)(damping * 10);

        quantityText.text = "发丝数量：" + hairCount.ToString();
        quantitySlider.value = (int)(hairCount / 32);

        lengthText.text = "发丝长度：" + ((int)(nodeDistance * 10)).ToString();
        lengthSlider.value = (int)(nodeDistance * 10);

        radiusText.text = "发丝半径：" + ((int)(hairRadius * 50)).ToString();
        radiusSlider.value = (int)(hairRadius * 50);

    }

    // Update is called once per frame
    void Update()
    {
        //更新head位置,用于计算碰撞
        transform.position = head.transform.position;
        transform.localRotation = head.transform.rotation;
        hairSimulate.SetVector("headPos", transform.position);
        //执行 step kernel
        hairSimulate.Dispatch(_stepKernel, _groupX, 1, 1);
        //获取控制顶点信息
        GetPos();
        //生成绘制点信息
        GetPathPoints();
        int i = 0;
        foreach (HairData hair in Hairs)
        {
            lines[i].positionCount = pointCount + 1;
            lines[i].startWidth = hairRadius;
            lines[i].endWidth = hairRadius / 2;
            lines[i].SetPositions(hair.points);
            i++;
        }

    }
    public void GetPos()
    {
        //拷贝位置信息
        positionBuffer.GetData(position);
        //计算head的位置和角度的变化量，用于更新buffer内的头发位置和角度
        Quaternion deltaRotation = transform.localRotation * Quaternion.Inverse(oldRotation);
        Vector3 deltaPosition = transform.position - oldPosition;
        for (int i = 0; i < hairCount; i++)
        {
            if (deltaRotation != Quaternion.Euler(0, 0, 0) || Vector3.Dot(deltaPosition, deltaPosition) != 0)
                //将compute shader计算的结果进行偏移，得到当前head下的位姿
                position[i * nodeCount] = transform.position + deltaRotation * (position[i * nodeCount] - oldPosition);
            for (int j = 0; j < nodeCount; j++)
            {
                Hairs[i].pos[j] = position[i * nodeCount + j];
            }
        }
        if(deltaRotation != Quaternion.Euler(0, 0, 0) || Vector3.Dot(deltaPosition, deltaPosition) != 0)
        {   
            //将位姿更新同步到compute shader
            positionBuffer.SetData(position);
            oldRotation = transform.localRotation;
            oldPosition = transform.position;
        }
    }
    //将每根头发的节点看作控制顶点，生成Bézier曲线用于渲染
    void GetPathPoints()
    {   
        float pointNumber = (float)pointCount;
        foreach (HairData hair in Hairs)
            for (int i = 0; i < (int)pointNumber + 1; i++)
            {
                List<Vector3> ListOfPoints = new List<Vector3>();
                for (int j = 0; j < nodeCount - 1; j++)
                {
                    ListOfPoints.Add(Vector3.Lerp(hair.pos[j], hair.pos[j + 1], i / pointNumber));
                }
                while (ListOfPoints.ToArray().Length != 1)
                {
                    int len = ListOfPoints.ToArray().Length;
                    for (int j = 0; j < len - 1; j++)
                    {
                        ListOfPoints.Add(Vector3.Lerp(ListOfPoints[j], ListOfPoints[j + 1], i / pointNumber));
                    }
                    ListOfPoints.RemoveRange(0, len);
                }
                hair.points[i] = (ListOfPoints[0] - transform.position) + transform.position;
            }
    }
    //UI更新函数
    public void OnGravityChanged(float newValue)
    {
        //TODO:更新重力
        gravity = newValue / -10;
        hairSimulate.SetFloat("gravity", gravity);
        gravityText.text = "重量：" + ((int)(gravity * -10)).ToString();
        gravitySlider.value = (int)(gravity * -10);
    }
    public void OnDampingChanged(float newValue)
    {
        //TODO:更新阻尼系数
        damping = newValue / 10;
        hairSimulate.SetFloat("damping", damping);
        dampingText.text = "阻尼系数：" + ((int)(damping * 10)).ToString();
        dampingSlider.value = (int)(damping * 10);

    }
    public void OnQuantityChanged(float newValue)
    {
        //TODO:更新头发数量
        hairCount = (int)(newValue * 32);
        hairSimulate.SetInt("hairCount", hairCount);
        quantityText.text = "发丝数量：" + hairCount.ToString();
        quantitySlider.value = (int)(newValue);
        _groupX = hairCount / THREAD_X;
        UpdateHair();
    }
    public void OnLengthChanged(float newValue)
    {
        //TODO:更新头发长度
        nodeDistance = newValue / 10;
        hairSimulate.SetFloat("nodeDistance", nodeDistance);
        lengthText.text = "发丝长度：" + ((int)newValue).ToString();
        lengthSlider.value = (int)newValue;
        UpdateHair();
    }
    public void OnRadiusChanged(float newValue)
    {
        //TODO:更新头发半径
        hairRadius = newValue / 50;
        hairSimulate.SetFloat("headRadius", headRadius);
        radiusText.text = "发丝半径：" + ((int)newValue).ToString();
        radiusSlider.value = (int)newValue;
    }
    public void OnEulerChanged(bool isOn)
    {
        implicitEuler = !implicitEuler;
        hairSimulate.SetBool("implicitEuler", implicitEuler);
        UpdateHair();
    }
    //在更新头发的长度与数量时需要调用，用于重新生成缓存
    public void UpdateHair()
    {
        ReleaseBuffer();
        GenerateBuffer();
        oldRotation = head.transform.rotation;
        oldPosition = head.transform.position;
    }
    //生成头发缓存
    private void GenerateBuffer()
    {
        totalNodeCount = hairCount * nodeCount;

        position = new Vector3[totalNodeCount];
        Hairs = new HairData[hairCount];
        lines = new LineRenderer[hairCount];

        for (int i = 0; i < hairCount; i++)
        {
            GameObject newHairLine = Instantiate<GameObject>(hairLine, transform.position, Quaternion.identity, transform) as GameObject;
            lines[i] = newHairLine.GetComponent<LineRenderer>();
            lines[i].material = material;
            Hairs[i].pos = new Vector3[nodeCount];
            Hairs[i].points = new Vector3[pointCount + 1];
        }

        positionBuffer = new ComputeBuffer(totalNodeCount, sizeof(float) * 3);
        prePositionBuffer = new ComputeBuffer(totalNodeCount, sizeof(float) * 3);

        System.Action<int> setBufferForKernel = (k) =>
        {
            hairSimulate.SetBuffer(k, "positions", positionBuffer);
            hairSimulate.SetBuffer(k, "prePositions", prePositionBuffer);
        };
        setBufferForKernel(_initKernel);
        setBufferForKernel(_stepKernel);

        hairSimulate.Dispatch(_initKernel, _groupX, 1, 1);

    }
    //释放头发缓存
    private void ReleaseBuffer()
    {

        positionBuffer?.Release();
        positionBuffer = null;
        prePositionBuffer?.Release();
        prePositionBuffer = null;

        for (int i = 0; i < lines.Length; i++)
        {
            Destroy(lines[i].gameObject);
        }

        Array.Clear(position, 0, position.Length);
        position = null;
        Array.Clear(Hairs, 0, Hairs.Length);
        Hairs = null;
        Array.Clear(lines, 0, lines.Length);
        lines = null;
    }
    private void OnDestroy()
    {
        ReleaseBuffer();
    }

}
