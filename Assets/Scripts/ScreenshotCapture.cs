using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using static UnityEditor.FilePathAttribute;

public class ScreenshotCapture: MonoBehaviour
{
    [SerializeField] private string _screenshotPath = "Assets/Screenshots/";

    private Camera camera;
    private Transform cameraTransform;
    private int resWidth;
    private int resHeight;

    public void CaptureAndSendScreenshot()
    {
        StartCoroutine(SendScreenshotToPython(CaptureScreenshot()));
    }
    private void Start()
    {
        camera = GetComponent<Camera>();
        cameraTransform = GetComponent<Transform>();
        resWidth = Screen.width;
        resHeight = Screen.height;
    }

    private byte[] CaptureScreenshot()
    {
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] screenshotBytes = screenShot.EncodeToPNG();

        int lenghtListScreenshots = new DirectoryInfo(_screenshotPath).GetFiles().Length;
        string screenshotfilename = "screenshot" + lenghtListScreenshots.ToString() + ".png";
        File.WriteAllBytes(Path.Combine(_screenshotPath, screenshotfilename), screenshotBytes);

        return screenshotBytes;
    }

    private IEnumerator SendScreenshotToPython(byte[] screenshotBytes)
    {
        string pythonScriptURL = "http://localhost:5000/imageProcessing";

        WWWForm form = new WWWForm();
        form.AddBinaryData("image", screenshotBytes, "screenshot.png", "image/png");
        form.AddField("fieldOfView", camera.fieldOfView.ToString());
        form.AddField("cameraHeight", cameraTransform.position.y.ToString());
        form.AddField("cameraAzimut", cameraTransform.rotation.eulerAngles.y.ToString());
        form.AddField("cameraX", cameraTransform.position.x.ToString());
        form.AddField("cameraY", cameraTransform.position.z.ToString());
        form.AddField("screenWidth", resWidth.ToString());
        form.AddField("screenHeight", resHeight.ToString());

        UnityWebRequest www = UnityWebRequest.Post(pythonScriptURL, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error sending screenshot to Python: " + www.error);
        }
        else
        {
            string jsonString = www.downloadHandler.text;
            PointCoords[] response = JsonConvert.DeserializeObject<PointCoords[]>(jsonString);
            for (int i = 0; i < response.Length; i++)
            {
                CRSVector2 crsVector = new CRSVector2(
                    "Pothole",
                    new VectorDouble(response[i].crs3857.x, response[i].crs3857.y),
                    new VectorDouble(response[i].crs4326.x, response[i].crs4326.y)
                );
                print(crsVector.ToString());
            }
        }
    }
}


public class PointCoords
{
    public VectorDouble crs3857 { get; set; }
    public VectorDouble crs4326 { get; set; }
}

