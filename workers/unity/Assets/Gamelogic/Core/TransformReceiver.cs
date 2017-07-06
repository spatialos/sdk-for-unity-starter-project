using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Unity.CodeGeneration;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public class TransformReceiver : MonoBehaviour
    {
        [Require] private Position.Reader PositionReader;
        [Require] private Rotation.Reader RotationReader;

        void OnEnable()
        {
            transform.position = PositionReader.Data.coords.ToUnityVector();
            transform.rotation = MathUtils.ToUnityQuaternion(RotationReader.Data.rotation);

            PositionReader.ComponentUpdated.Add(OnPositionUpdated);
            RotationReader.ComponentUpdated.Add(OnRotationUpdated);
        }

        void OnDisable()
        {
            PositionReader.ComponentUpdated.Remove(OnPositionUpdated);
            RotationReader.ComponentUpdated.Remove(OnRotationUpdated);
        }

        void OnPositionUpdated(Position.Update update)
        {
            if (!PositionReader.HasAuthority)
            {
                if (update.coords.HasValue)
                {
                    transform.position = update.coords.Value.ToUnityVector();
                }
            }
        }

        void OnRotationUpdated(Rotation.Update update)
        {
            if (!RotationReader.HasAuthority)
            {
                if (update.rotation.HasValue)
                {
                    transform.rotation = MathUtils.ToUnityQuaternion(update.rotation.Value);
                }
            }
        }
    }
}