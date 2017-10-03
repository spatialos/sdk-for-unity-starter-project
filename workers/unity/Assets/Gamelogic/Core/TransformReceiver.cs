using Improbable;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
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
            transform.rotation = RotationReader.Data.rotation.ToUnityQuaternion();

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
            if (PositionReader.Authority == Authority.NotAuthoritative)
            {
                if (update.coords.HasValue)
                {
                    transform.position = update.coords.Value.ToUnityVector();
                }
            }
        }

        void OnRotationUpdated(Rotation.Update update)
        {
            if (RotationReader.Authority == Authority.NotAuthoritative)
            {
                if (update.rotation.HasValue)
                {
                    transform.rotation = update.rotation.Value.ToUnityQuaternion();
                }
            }
        }
    }
}