using System;
using UnityEngine;
public class SimpleCam : MonoBehaviour
{
    const int waitWidth = 16;

    public int width = 640;
    public int height = 360;
    public GameObject prefab;

    public int minR = 40;
    public int maxR = 80;
    public int minG = 70;
    public int maxG = 110;
    public int minB = 130;
    public int maxB = 170;

    Color32[] pixels;

    WebCamTexture cam;
    Texture2D tex;
    Vector3 move;
    Action update;
    void Start()
    {
        update = WaitingForCam;///initially set to the waiting for cam method
        Vector3 start = new Vector3(0, 0, -1);

        cam = new WebCamTexture(WebCamTexture.devices[0].name, width, height);
        cam.Play();
    }
    private void Update()
    {
        update();//changes from waiting for cam to cam is on
    }
    void WaitingForCam()
    {
        if (cam.width > waitWidth)//will be true once the cam turns on
        {
            width = cam.width;
            height = cam.height;
            pixels = new Color32[cam.width * cam.height];
            tex = new Texture2D(cam.width, cam.height, TextureFormat.RGBA32, false);
            Renderer renderer = GetComponent<Renderer>(); //draws the picture
            renderer.material.mainTexture = tex;
            update = CamIsOn;//change when cam is on
        }
    }
    void CamIsOn()//waiting for cam will make this run once it is on
    {
        if (cam.didUpdateThisFrame)//checks if the cam if it grabbed a new raster of data to be sure we are not processing the same data over and over since unity update resets faster than cam
        {
            cam.GetPixels32(pixels);
            MirrorPixels(width, height, pixels);
            tex.SetPixels32(pixels);
            tex.Apply();

            //int redPureThreshold = 30;
            float Redx = 0;
            float Redy = 0;
            float redPixelCount = 0;
            
            for (int x = 0; x < (int)cam.width; x++)
            {
                for (int y = 0; y < (int)cam.height; y++)
                {
                   // if (pixels[x + y * cam.width].r > 255 - redPureThreshold && pixels[x + y * cam.width].g < 100 && pixels[x + y * cam.width].b < 100) - part of thought process
                    Color32 pixelColor = pixels[x + y * cam.width];

                    if (pixelColor.r > minR &&
                        pixelColor.r < maxR &&
                        pixelColor.g > minG &&
                        pixelColor.g < maxG &&
                        pixelColor.b > minB &&
                        pixelColor.b < maxB
                        )
                    {
                        Redx += x;
                        Redy += y;

                        redPixelCount += 1;
                    }
                }
            }
            //Debug.Log("Redx=" + Redx);
            //Debug.Log("Redy=" + Redy);
          

            if (redPixelCount != 0)//cant divide by 0
            {
                //Debug.Log("final x=" + (Redx / redPixelCount));
                //Debug.Log("final y=" + (Redy / redPixelCount));
                float x = Redx / redPixelCount;
                float y = Redy / redPixelCount;
                float finalX = (.025f * x) - 8;
                float finalY = (.025f * y) - 4.5f;
                Debug.Log("final x=" + finalX);
                Debug.Log("final y=" + finalY);
                move = new Vector3(finalX, finalY, -1);
                prefab.transform.position = move;
            }
        }
    }

    void MirrorPixels(int width, int height, Color32[] pixels)
    {
        for (int y = 0; y < height; ++y)
        {
            int offsetLft = y * width;
            int offsetRgt = offsetLft + width - 1;

            for (int x = 0; x < width / 2; ++x)
            {
                Color32 temp = pixels[offsetLft + x];
                pixels[offsetLft + x] = pixels[offsetRgt - x];
                pixels[offsetRgt - x] = temp;
            }
        }
    }
}