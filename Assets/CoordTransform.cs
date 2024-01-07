using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordTransform: MonoBehaviour
{
    private const double HALF_EQUATOR = 20037508.34;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform item;
    [SerializeField] private Vector2 coordsCam3857;
    [SerializeField] private Vector2 coordsCam4326;
    [SerializeField] private Vector2 camResolution;
    [SerializeField] private Vector2 coordsItemImage;
    [SerializeField] private Vector2 coordsItem4326;
    [SerializeField] private float camHeight;
    [SerializeField] private float azimut;

    public void Start()
    {
        camResolution = new Vector2(Screen.width, Screen.height);
        Debug.Log(cam.fieldOfView);
    }
    public void Update()
    {
        camHeight = cam.GetComponent<Transform>().position.y - item.position.y;
        azimut = cam.GetComponent<Transform>().rotation.y;
        coordsCam3857 = new Vector2(cam.GetComponent<Transform>().position.x, cam.GetComponent<Transform>().position.z);
        coordsCam4326 = epsg3857To4326(coordsCam3857);
        convertItemToImagePos();
        convertCoords();
    }

    public void convertCoords()
    { 
        Vector2 vectorPos = coordsItemImage / (camResolution.y / 2);
        vectorPos = vectorWithAzimut(vectorPos, azimut);
        vectorPos = vectorPos * camHeight + coordsCam3857;
        Debug.Log(vectorPos);
        coordsItem4326 = epsg3857To4326(vectorPos);
    }

    private Vector2 epsg3857To4326(Vector2 coords3857)
    {
        double lon = (coords3857.x / HALF_EQUATOR) * 180;
        double lat = (coords3857.y / HALF_EQUATOR) * 180;
        lat = Math.Atan(Math.Exp(lat * Math.PI / 180));
        lat = lat / (Math.PI / 360);
        lat -= 90;
        return new Vector2((float)lon, (float)lat);
    }
    private Vector2 vectorWithAzimut(Vector2 vec, float azimut)
    {
        float x = vec.x * MathF.Cos(azimut) - vec.y * MathF.Sin(azimut);
        float y = vec.x * MathF.Sin(azimut) + vec.y * MathF.Cos(azimut);
        return new Vector2(x, y);
    }
    private void convertItemToImagePos()
    {
        Vector2 vec = cam.WorldToScreenPoint(item.position);
        if (vec.y > camResolution.y && vec.y < 0 && vec.x > camResolution.x && vec.x < 0)
            vec.Set(0,0);
        coordsItemImage.Set(vec.x - camResolution.x/2, vec.y - camResolution.y / 2);
    }
}
