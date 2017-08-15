using Improbable;
using UnityEngine;

public static class MathUtils {

    public static Quaternion ToUnityQuaternion(this Improbable.Core.Quaternion quaternion)
    {
        return new Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }

    public static Improbable.Core.Quaternion ToNativeQuaternion(this Quaternion quaternion)
    {
        return new Improbable.Core.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }
}
