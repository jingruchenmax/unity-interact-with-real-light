using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript: MonoBehaviour
{
    static WebCamTexture webCam;
    public RawImage[] rawImage;
    // Start is called before the first frame update
    void Start()
    {
        if(webCam == null)
        {
            webCam = new WebCamTexture(1920,1080,60);
        }
        webCam.Play();
        foreach(RawImage raw in rawImage) 
             raw.texture = webCam;
    }

    public Texture2D Convert_WebCamTexture_To_Texture2d(WebCamTexture _webCamTexture)
    {
        Texture2D _texture2D = new Texture2D(_webCamTexture.width, _webCamTexture.height);
        _texture2D.SetPixels32(_webCamTexture.GetPixels32());

        return _texture2D;
    }

    public Texture2D Convert_WebCamTexture_To_Texture2d()
    {
        return Convert_WebCamTexture_To_Texture2d(webCam);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
