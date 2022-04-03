using UnityEngine;

public class maximumScript : MonoBehaviour
{
    public GameObject spotlight;
    public float xRange;
    public float yRange;
    public ComputeShader shader;
    public Texture2D inputTexture;
    public RenderTexture renderTexture;
    public uint[] groupMaxData;
    public int groupMax;
    public float xOffset;
    public float yOffset;
    private ComputeBuffer groupMaxBuffer;

    private int handleMaximumMain;
    private CameraScript webcam;
    Texture2D tex;
    void Start()
    {
        webcam = GetComponent<CameraScript>();
        tex = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        if (null == shader || (null == inputTexture && null == renderTexture))
        {
            Debug.Log("Shader or input texture missing.");
            return;
        }
        inputTexture = toTexture2D(renderTexture);
        
        handleMaximumMain = shader.FindKernel("MaximumMain");
        groupMaxBuffer = new ComputeBuffer((inputTexture.height + 63) / 64, sizeof(uint) * 3);
        groupMaxData = new uint[((inputTexture.height + 63) / 64) * 3];

        if (handleMaximumMain < 0 || null == groupMaxBuffer || null == groupMaxData)
        {
            Debug.Log("Initialization failed.");
            return;
        }

        shader.SetTexture(handleMaximumMain, "InputTexture", inputTexture);
        shader.SetInt("InputTextureWidth", inputTexture.width);
        shader.SetBuffer(handleMaximumMain, "GroupMaxBuffer", groupMaxBuffer);

    }

    void OnDestroy()
    {
        if (null != groupMaxBuffer)
        {
            groupMaxBuffer.Release();
        }
    }

    void Update()
    {
        inputTexture = toTexture2D(renderTexture);
       // inputTexture = webcam.Convert_WebCamTexture_To_Texture2d();
        shader.SetTexture(handleMaximumMain, "InputTexture", inputTexture);
        shader.Dispatch(handleMaximumMain, (inputTexture.height + 63) / 64, 1, 1);
        // divided by 64 in x because of [numthreads(64,1,1)] in the compute shader code
        // added 63 to make sure that there is a group for all rows

        // get maxima of groups
        groupMaxBuffer.GetData(groupMaxData);

        // find maximum of all groups
        for (int group = 1; group < (inputTexture.height + 63) / 64; group++)
        {
            if (groupMaxData[3 * group + 2] > groupMaxData[3 * groupMax + 2])
            {
                groupMax = group;
            }
        }
        spotlight.transform.position = new Vector3(-xRange / 2 + xRange * ((float)groupMaxData[3 * groupMax + 0] / inputTexture.width) + xOffset, -yRange / 2 + yRange * ((float)groupMaxData[3 * groupMax + 1] / inputTexture.height)+yOffset, spotlight.transform.position.z);
        spotlight.transform.LookAt(new Vector3(0,-0.5f,0));
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}