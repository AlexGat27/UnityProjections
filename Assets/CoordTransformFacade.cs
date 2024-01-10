using System.Collections.Generic;
using UnityEngine;

public class CoordTransformFacade: MonoBehaviour
{

    [SerializeField] private Camera cam;
    private Transform camTransform;
    private CRSVector2 coordsCam;
    private CRSVector2[] coordsItems;
    private VectorDouble camResolution;
    [SerializeField] private double camHeight;
    [SerializeField] private double azimut;
    private ScreenshotCapture screenshotCapture;

    public void Start()
    {
        camResolution = new VectorDouble(Screen.width, Screen.height);
        camTransform = cam.GetComponent<Transform>();
        coordsCam = new CRSVector2();
        screenshotCapture = new ScreenshotCapture();
    }
    public void Update()
    {
        azimut = camTransform.rotation.y;
        camHeight = camTransform.position.y;
        coordsCam.crs3857 = new VectorDouble(camTransform.position.x, camTransform.position.z);
        coordsCam.crs4326 = coordsCam.epsg3857To4326();
    }
    public void sendScreenshot()
    {
        screenshotCapture.CaptureAndSendScreenshot(convertCoordsFromImage);
    }
    //private VectorDouble vectorWithAzimut(VectorDouble vec, double azimut)
    //{
    //    double x = vec.x * Mathf.Cos((float)azimut) - vec.y * Mathf.Sin((float)azimut);
    //    double y = vec.x * Mathf.Sin((float)azimut) + vec.y * Mathf.Cos((float)azimut);
    //    return new VectorDouble(x, y);
    //}
    private void convertCoordsFromImage(VectorDouble[] itemsImagePos)
    {
        coordsItems = new CRSVector2[itemsImagePos.Length];
        for (int i = 0; i < itemsImagePos.Length; i++)
        {
            VectorDouble vectorPos = itemsImagePos[i] / (camResolution.y / 2);
            vectorPos = vectorPos * camHeight + coordsCam.crs3857;
            coordsItems[i].crs3857 = vectorPos;
            coordsItems[i].crs4326 = coordsItems[i].epsg3857To4326();
        }
    }
}
