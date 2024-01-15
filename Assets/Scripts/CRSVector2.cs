using System.Numerics;
using UnityEngine;

public class CRSVector2
{
    private const double HALF_EQUATOR = 20037508.34;

    public string ObjectName 
    { 
        get 
        { 
            return _objectName; 
        } 
    }
    public VectorDouble Crs3857
    {
        get
        {
            return _crs3857;
        }
    }
    public VectorDouble Crs4326
    {
        get
        {
            return _crs4326;
        }
    }

    private string _objectName;
    private VectorDouble _crs3857;
    private VectorDouble _crs4326;

    //public static VectorDouble Epsg3857To4326(VectorDouble crs3857)
    //{
    //    double lon = (crs3857.x / HALF_EQUATOR) * 180;
    //    double lat = Mathf.Atan(Mathf.Exp(((float)(crs3857.y / HALF_EQUATOR) * 180) * Mathf.PI / 180)) / (Mathf.PI / 360) - 90;
    //    return new VectorDouble((float)lon, (float)lat);
    //}

    public CRSVector2(string objectName, VectorDouble crs3857, VectorDouble crs4326)
    {
        _objectName = objectName;
        _crs3857 = crs3857;
        _crs4326 = crs4326;
    }

    public override string ToString()
    {
        return string.Format("{0} crs3857 = ( {1}, {2} )", _objectName, _crs3857.x, _crs3857.y) +
        "\n" + string.Format("{0} crs4326 = ( {1}, {2} )", _objectName, _crs4326.x, _crs4326.y);
    }
}
