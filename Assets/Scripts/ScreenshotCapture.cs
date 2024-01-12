using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ScreenshotCapture: MonoBehaviour
{
    public Camera captureCamera;

    // Путь для сохранения скриншотов (если нужно)
    public string screenshotPath = "Assets/Screenshots/";

    // Название файла скриншота
    private string screenshotFilename;

    // Запускает процесс захвата скриншота и отправки на Python
    public void CaptureAndSendScreenshot(Action<VectorDouble[]> callback)
    {
        StartCoroutine(CaptureScreenshot(callback));
    }

    // Корутина для захвата скриншота
    private IEnumerator CaptureScreenshot(Action<VectorDouble[]> callback)
    {
        yield return new WaitForEndOfFrame();

        // Создаем текстуру
        Texture2D screenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        // Чтение данных из камеры в текстуру
        screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshotTexture.Apply();

        // Конвертация текстуры в массив байтов
        byte[] screenshotBytes = screenshotTexture.EncodeToPNG();

        // Сохранение скриншота (если нужно)
        int lenghtListScreenshots = new DirectoryInfo(screenshotPath).GetFiles().Length;
        screenshotFilename = "screenshot" + lenghtListScreenshots.ToString() + ".png";
        File.WriteAllBytes(Path.Combine(screenshotPath, screenshotFilename), screenshotBytes);

        // Отправка скриншота на Python-скрипт
        StartCoroutine(SendScreenshotToPython(screenshotBytes, callback));
    }

    // Корутина для отправки скриншота на Python-скрипт
    private IEnumerator SendScreenshotToPython(byte[] screenshotBytes, Action<VectorDouble[]> callback)
    {
        // Пример URL-адреса вашего Python-скрипта (замените его своим)
        string pythonScriptURL = "http://localhost:5000/imageProcessing";

        // Используйте UnityWebRequest для отправки данных на сервер
        UnityWebRequest www = new UnityWebRequest(pythonScriptURL, "POST");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(screenshotBytes);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "image/png");

        // Ожидание ответа от сервера
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
            // Дополнительная обработка ответа, если необходимо
        }
    }
}
