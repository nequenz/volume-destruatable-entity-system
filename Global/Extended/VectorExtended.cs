using UnityEngine;
/*
 * 
 *  by nequenz
 * 
*/

public static class VectorExtended
{
    private static float ValidateAtan2Result( float angle ) => angle < 0 ? (angle + 360) : angle;

    public static Vector3 GetNormalTo(this Vector3 me, Vector3 otherVector) => (otherVector - me) / Vector3.Distance(me, otherVector);

    public static Vector3 Get2DNormalTo(this Vector3 me, Vector3 otherVector)
    {
        Vector2 me2d = new Vector2(me.x, me.z);
        Vector2 otherVector2d = new Vector2(otherVector.x, otherVector.z);
        Vector2 normal2d = (otherVector2d - me2d) / Vector2.Distance(me2d, otherVector2d);

        return new Vector3(normal2d.x, 0.0f, normal2d.y);
    }

    public static float Get2DDistance(this Vector3 me, Vector3 otherVector)
    {
        Vector2 me2d = new Vector2(me.x, me.z);
        Vector2 otherVector2d = new Vector2(otherVector.x, otherVector.z);
        Vector2 normal2d = (otherVector2d - me2d);

        return normal2d.magnitude;
    }

    public static float GetDistance(this Vector3 me, Vector3 otherVector) => Vector3.Distance(me, otherVector);

    public static float GetXAxisAngle(this Vector3 me) => ValidateAtan2Result(Mathf.Atan2(me.z, me.y) * Mathf.Rad2Deg);

    public static float GetYAxisAngle(this Vector3 me) => ValidateAtan2Result(Mathf.Atan2(me.z, me.x) * Mathf.Rad2Deg);

    public static float GetZAxisAngle(this Vector3 me) => ValidateAtan2Result(Mathf.Atan2(me.x, me.y) * Mathf.Rad2Deg);

    public static float ValidateAngleBeetwen(float angle1, float angle2)
    {
        float result = angle1 - angle2;
        float resultAbs = Mathf.Abs(result);

        return resultAbs > 180 ? 360 - resultAbs : result;
    }


}