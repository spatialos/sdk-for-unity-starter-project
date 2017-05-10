using Assets.Gamelogic.Utils;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public class TransformReceiver : MonoBehaviour
    {
        [Require]
        private WorldTransform.Reader WorldTransformReader;

        void OnEnable()
        {
            transform.position = WorldTransformReader.Data.position.ToVector3();
            transform.rotation = MathUtils.ToUnityQuaternion(WorldTransformReader.Data.rotation);

            WorldTransformReader.ComponentUpdated.Add(OnComponentUpdated);
        }

        void OnDisable()
        {
            WorldTransformReader.ComponentUpdated.Remove(OnComponentUpdated);
        }

        void OnComponentUpdated(WorldTransform.Update update)
        {
            if (!WorldTransformReader.HasAuthority)
            {
                if (update.position.HasValue)
                {
                    transform.position = update.position.Value.ToVector3();
                }
                if (update.rotation.HasValue)
                {
                    transform.rotation = MathUtils.ToUnityQuaternion(WorldTransformReader.Data.rotation);
                }
            }
        }
    }
}