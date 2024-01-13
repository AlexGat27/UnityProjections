using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;

public class ScreenshotCapture: MonoBehaviour
{
    // ���� ��� ���������� ���������� (���� �����)
    public string ScreenshotPath = "Assets/Screenshots/";

    private Camera camera;
    private Transform cameraTransform;
    int resWidth;
    int resHeight;

    // ��������� ������� ������� ��������� � �������� �� Python
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

    // �������� ��� ������� ���������
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

        int lenghtListScreenshots = new DirectoryInfo(ScreenshotPath).GetFiles().Length;
        string screenshotfilename = "screenshot" + lenghtListScreenshots.ToString() + ".png";
        File.WriteAllBytes(Path.Combine(ScreenshotPath, screenshotfilename), screenshotBytes);

        return screenshotBytes;
    }

    // �������� ��� �������� ��������� �� Python-������
    private IEnumerator SendScreenshotToPython(byte[] screenshotBytes)
    {
        // ������ URL-������ ������ Python-������� (�������� ��� �����)
        string pythonScriptURL = "http://localhost:5000/imageProcessing";

        // ����������� UnityWebRequest ��� �������� ������ �� ������
        // ��������� ����� ��� �������� ������
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", screenshotBytes, "screenshot.png", "image/png");
        form.AddField("fieldOfView", camera.fieldOfView.ToString());
        form.AddField("cameraHeight", cameraTransform.position.y.ToString());
        form.AddField("cameraAzimut", cameraTransform.rotation.y.ToString());
        form.AddField("cameraX", cameraTransform.position.x.ToString());
        form.AddField("cameraY", cameraTransform.position.z.ToString());
        form.AddField("screenWidth", resWidth.ToString());
        form.AddField("screenHeight", resHeight.ToString());
        print(resWidth + " " + resHeight);

        UnityWebRequest www = UnityWebRequest.Post(pythonScriptURL, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error sending screenshot to Python: " + www.error);
        }
        else
        {
            string jsonString = www.downloadHandler.text;
            Debug.Log(jsonString);
        }
    }
}
