using UnityEngine;

public class CRSVector2
{
    private const double HALF_EQUATOR = 20037508.34;

    public VectorDouble crs3857;
    public VectorDouble crs4326;
    public VectorDouble epsg3857To4326()
    {
        double lon = (crs3857.x / HALF_EQUATOR) * 180;
        double lat = (crs3857.y / HALF_EQUATOR) * 180;
        lat = Mathf.Atan(Mathf.Exp((float)lat * Mathf.PI / 180));
        lat = lat / (Mathf.PI / 360);
        lat -= 90;
        return new VectorDouble((float)lon, (float)lat);
    }
}
