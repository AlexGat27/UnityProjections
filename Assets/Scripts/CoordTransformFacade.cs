using System.Collections.Generic;
using UnityEngine;

public class CoordTransformFacade: MonoBehaviour
{

    private Transform _cameraTransform;
    private Camera _camera;
    private CRSVector2 _coordsCameraXY;
    private VectorDouble _cameraResolution;
    private ScreenshotCapture _screenshotCapture;

    public void Start()
    {
        _cameraResolution = new VectorDouble(Screen.width, Screen.height);
        _cameraTransform = GetComponent<Transform>();
        _camera = GetComponent<Camera>();
        _screenshotCapture = GetComponent<ScreenshotCapture>();
    }

    public void sendScreenshot()
    {
        _screenshotCapture.CaptureAndSendScreenshot(convertCoordsFromImage);
    }

    private VectorDouble vectorWithAzimut(VectorDouble vec, double azimut, double camHeight, float fieldOfView)
    {
        vec *= camHeight * Mathf.Tan(fieldOfView / 2 * Mathf.PI / 180);
        vec.x *= 16 / 9;
        //double x = vec.x * Mathf.Cos((float)azimut) - vec.y * Mathf.Sin((float)azimut);
        //double y = vec.x * Mathf.Sin((float)azimut) + vec.y * Mathf.Cos((float)azimut);
        return new VectorDouble(vec.x, vec.y);
    }

    private void convertCoordsFromImage(VectorDouble[] itemsImagePos)
    {
        double azimut = _cameraTransform.rotation.y * Mathf.PI / 180;
        double camHeight = _cameraTransform.position.y;
        _coordsCameraXY = new CRSVector2("Camera", new VectorDouble(_cameraTransform.position.x, _cameraTransform.position.z));
        Debug.Log(_coordsCameraXY.ToString());
        for (int i = 0; i < itemsImagePos.Length; i++)
        {
            CRSVector2 coordsItem = new CRSVector2("Item_" + i.ToString(),
                vectorWithAzimut(new VectorDouble(itemsImagePos[i].x / (_cameraResolution.y / 2), -itemsImagePos[i].y / (_cameraResolution.y / 2)),
                azimut, camHeight, _camera.fieldOfView)
                + _coordsCameraXY.Crs3857);
            Debug.Log(coordsItem.ToString());
        }
    }
}
