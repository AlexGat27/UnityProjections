using System.Collections.Generic;
using UnityEngine;

public class CoordTransformFacade: MonoBehaviour
{

    [SerializeField] private Camera cam;
    private Transform camTransform;
    private CRSVector2 coordsCam;
    private VectorDouble camResolution;
    [SerializeField] private double camHeight;
    [SerializeField] private double azimut;
    private ScreenshotCapture screenshotCapture;

    public void Start()
    {
        camResolution = new VectorDouble(Screen.width, Screen.height);
        camTransform = cam.GetComponent<Transform>();
        coordsCam = new CRSVector2("Camera");
        screenshotCapture = GetComponent<ScreenshotCapture>();
    }
    public void Update()
    {
        azimut = camTransform.rotation.y * Mathf.PI / 180;
        camHeight = camTransform.position.y;
        coordsCam.crs3857 = new VectorDouble(camTransform.position.x, camTransform.position.z);
        coordsCam.crs4326 = coordsCam.epsg3857To4326();
    }
    public void sendScreenshot()
    {
        screenshotCapture.CaptureAndSendScreenshot(convertCoordsFromImage);
    }
    private VectorDouble vectorWithAzimut(VectorDouble vec, double azimut)
    {
        double x = vec.x * Mathf.Cos((float)azimut) - vec.y * Mathf.Sin((float)azimut);
        double y = vec.x * Mathf.Sin((float)azimut) + vec.y * Mathf.Cos((float)azimut);
        return new VectorDouble(x, y);
    }
    private void convertCoordsFromImage(VectorDouble[] itemsImagePos)
    {
        coordsCam.displayVectorCRS();
        for (int i = 0; i < itemsImagePos.Length; i++)
        {
            CRSVector2 coordsItem = new CRSVector2("Item_" + i.ToString());
            coordsItem.crs3857 = new VectorDouble(itemsImagePos[i].x / (camResolution.y / 2), -itemsImagePos[i].y / (camResolution.y / 2));
            coordsItem.crs3857 = vectorWithAzimut(coordsItem.crs3857, azimut);
            coordsItem.crs3857 = coordsItem.crs3857 * camHeight + coordsCam.crs3857;
            coordsItem.crs4326 = coordsItem.epsg3857To4326();
            coordsItem.displayVectorCRS();
        }
    }
}
