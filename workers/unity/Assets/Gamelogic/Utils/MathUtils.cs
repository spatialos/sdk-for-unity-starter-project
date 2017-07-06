using Improbable;
using UnityEngine;

namespace Assets.Gamelogic.Utils
{
    public static class MathUtils {

        public static Quaternion ToUnityQuaternion(Improbable.Core.Quaternion quaternion)
        {
            return new Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        public static Improbable.Core.Quaternion ToNativeQuaternion(Quaternion quaternion)
        {
            return new Improbable.Core.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }
}
