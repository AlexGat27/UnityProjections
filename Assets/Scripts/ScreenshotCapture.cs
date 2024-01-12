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


    // ��������� ������� ������� ��������� � �������� �� Python
    public void CaptureAndSendScreenshot(Action<VectorDouble[]> callback)
    {
        StartCoroutine(SendScreenshotToPython(CaptureScreenshot(), callback));
    }
    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    // �������� ��� ������� ���������
    private byte[] CaptureScreenshot()
    {
        // ������� ��������
        //Texture2D screenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //// ������ ������ �� ������ � ��������
        //screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //screenshotTexture.Apply();

        //// ����������� �������� � ������ ������
        //byte[] screenshotBytes = screenshotTexture.EncodeToPNG();

        //// ���������� ��������� (���� �����)
        //int lenghtListScreenshots = new DirectoryInfo(ScreenshotPath).GetFiles().Length;
        //string screenshotFilename = "screenshot" + lenghtListScreenshots.ToString() + ".png";
        //File.WriteAllBytes(Path.Combine(ScreenshotPath, screenshotFilename), screenshotBytes);

        int resWidth = Screen.width;
        int resHeight = Screen.height;

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
    private IEnumerator SendScreenshotToPython(byte[] screenshotBytes, Action<VectorDouble[]> callback)
    {
        // ������ URL-������ ������ Python-������� (�������� ��� �����)
        string pythonScriptURL = "http://localhost:5000/imageProcessing";

        // ����������� UnityWebRequest ��� �������� ������ �� ������
        UnityWebRequest www = new UnityWebRequest(pythonScriptURL, "POST");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(screenshotBytes);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "image/png");

        // �������� ������ �� �������
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error sending screenshot to Python: " + www.error);
        }
        else
        {
            string jsonString = www.downloadHandler.text;
            VectorDouble[] myData = JsonConvert.DeserializeObject<VectorDouble[]>(jsonString);
            Debug.Log("Got data from Python");
            callback.Invoke(myData);
            // �������������� ��������� ������, ���� ����������
        }
    }
}
