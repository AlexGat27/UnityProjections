using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ScreenshotCapture: MonoBehaviour
{
    public Camera captureCamera;

    // ���� ��� ���������� ���������� (���� �����)
    public string screenshotPath = "Assets/Screenshots/";

    // �������� ����� ���������
    private string screenshotFilename;

    // ��������� ������� ������� ��������� � �������� �� Python
    public void CaptureAndSendScreenshot(Action<VectorDouble[]> callback)
    {
        StartCoroutine(CaptureScreenshot(callback));
    }

    // �������� ��� ������� ���������
    private IEnumerator CaptureScreenshot(Action<VectorDouble[]> callback)
    {
        yield return new WaitForEndOfFrame();

        // ������� ��������
        Texture2D screenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        // ������ ������ �� ������ � ��������
        screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshotTexture.Apply();

        // ����������� �������� � ������ ������
        byte[] screenshotBytes = screenshotTexture.EncodeToPNG();

        // ���������� ��������� (���� �����)
        int lenghtListScreenshots = new DirectoryInfo(screenshotPath).GetFiles().Length;
        screenshotFilename = "screenshot" + lenghtListScreenshots.ToString() + ".png";
        File.WriteAllBytes(Path.Combine(screenshotPath, screenshotFilename), screenshotBytes);

        // �������� ��������� �� Python-������
        StartCoroutine(SendScreenshotToPython(screenshotBytes, callback));
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
