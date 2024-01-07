using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorDouble : MonoBehaviour
{
    public double x;
    public double y;
    public VectorDouble(double x = 0, double y = 0)
    {
        this.x = x; this.y = y;
    }
    public static VectorDouble operator +(VectorDouble vec1, VectorDouble vec2)
    {
        return new VectorDouble(vec1.x + vec2.x, vec1.y + vec2.y);
    }
    public static VectorDouble operator *(VectorDouble vec1, VectorDouble vec2)
    {
        return new VectorDouble(vec1.x * vec2.x, vec1.y * vec2.y);
    }
    public static VectorDouble operator /(VectorDouble vec1, VectorDouble vec2)
    {
        return new VectorDouble(vec1.x / vec2.x, vec1.y / vec2.y);
    }
    public static VectorDouble operator -(VectorDouble vec1, VectorDouble vec2)
    {
        return new VectorDouble(vec1.x - vec2.x, vec1.y - vec2.y);
    }

    public static VectorDouble operator +(VectorDouble vec1, double a)
    {
        return new VectorDouble(vec1.x + a, vec1.y + a);
    }
    public static VectorDouble operator *(VectorDouble vec1, double a)
    {
        return new VectorDouble(vec1.x * a, vec1.y * a);
    }
    public static VectorDouble operator /(VectorDouble vec1, double a)
    {
        return new VectorDouble(vec1.x / a, vec1.y / a);
    }
    public static VectorDouble operator -(VectorDouble vec1, double a)
    {
        return new VectorDouble(vec1.x - a, vec1.y - a);
    }
}
