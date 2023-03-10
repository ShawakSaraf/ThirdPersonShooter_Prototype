using UnityEngine;

public static class Mathff
{
    public const float TAU = 6.28318530718f;
    public static Vector2 AngToDir2D(float angleRad)
    {
        return new Vector2
        (
            Mathf.Cos(angleRad),
            Mathf.Sin(angleRad)
        );
    } 
    
    public static bool IsInEllipse(Vector2 pt, Vector2 center, float majorRadius, float minorRadius)
    {
        Vector2 ptRel = pt - center; // Vector relative to center
        float a = majorRadius * majorRadius;
        float b = minorRadius * minorRadius;
        float x = (ptRel.x * ptRel.x);
        float y = (ptRel.y * ptRel.y);

        return (x / a) + (y / b) <= 1; // same as below, but faster
        // return Mathf.Pow((pt.x - center.x)/majorRadius, 2) + Mathf.Pow((pt.y - center.y)/minorRadius, 2) <= 1;
    }
    public static bool IsInCircle(Vector3 pt, Vector3 center, float radius)
    {
        Vector3 ptRel = pt - center; // Vector relative to center

        return ptRel.magnitude <= radius;
    }

    public static Vector2 Rotate2D(Vector2 v, float delta)
    {
        // Basically 2x2 Rotation Matrix multiplication
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public static float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z ; // *2 to scale it up to 360deg
    }
    public static Quaternion CalculateQuaternion(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from); // *2 to scale it up to 360deg
    }

    public static float GetCosWave( float amp, float freq, float angularSpeed, float xOffset, float yOffset)
    {
        return amp * Mathf.Cos(freq * (Mathff.TAU + Time.timeSinceLevelLoad * angularSpeed)  + xOffset) + yOffset;
    }

    public static float GetSinWave( float amp, float freq, float angularSpeed, float xOffset, float yOffset)
    {
        return amp * Mathf.Sin((freq * Mathff.TAU) + Time.timeSinceLevelLoad * angularSpeed + xOffset) + yOffset;
    }

    public static float Remap( float iMin, float iMax, float oMin, float oMax, float v)
    {
        float t = Mathf.InverseLerp(iMin, iMax, v);
        return Mathf.Lerp(oMin, oMax, t);
    }
    public static Vector3 Remap( float iMin, float iMax, Vector3 oMin, Vector3 oMax, float v)
    {
        float t = Mathf.InverseLerp(iMin, iMax, v);
        return Vector3.Lerp(oMin, oMax, t);
    }
}
