using System.Collections.Generic;
using UnityEngine;

public class CoordTransformFacade: MonoBehaviour
{

    [SerializeField] private Camera cam;

    private Transform _camTransform;
    private CRSVector2 _coordsCamXY;
    private VectorDouble _camResolution;
    private ScreenshotCapture _screenshotCapture;

    public void Start()
    {
        _camResolution = new VectorDouble(Screen.width, Screen.height);
        _camTransform = cam.GetComponent<Transform>();
        _screenshotCapture = GetComponent<ScreenshotCapture>();
    }
    public void sendScreenshot()
    {
        _screenshotCapture.CaptureAndSendScreenshot(convertCoordsFromImage);
    }
    private VectorDouble vectorWithAzimut(VectorDouble vec, double azimut)
    {
        double x = vec.x * Mathf.Cos((float)azimut) - vec.y * Mathf.Sin((float)azimut);
        double y = vec.x * Mathf.Sin((float)azimut) + vec.y * Mathf.Cos((float)azimut);
        return new VectorDouble(x, y);
    }
    private void convertCoordsFromImage(VectorDouble[] itemsImagePos)
    {
        double azimut = _camTransform.rotation.y * Mathf.PI / 180;
        double camHeight = _camTransform.position.y;
        _coordsCamXY = new CRSVector2("Camera", new VectorDouble(_camTransform.position.x, _camTransform.position.z));
        Debug.Log(_coordsCamXY.ToString());
        for (int i = 0; i < itemsImagePos.Length; i++)
        {
            CRSVector2 coordsItem = new CRSVector2("Item_" + i.ToString(),
                vectorWithAzimut(new VectorDouble(itemsImagePos[i].x / (_camResolution.y / 2), -itemsImagePos[i].y / (_camResolution.y / 2)), azimut)
                * camHeight + _coordsCamXY.Crs3857);
            Debug.Log(coordsItem.ToString());
        }
    }
}
