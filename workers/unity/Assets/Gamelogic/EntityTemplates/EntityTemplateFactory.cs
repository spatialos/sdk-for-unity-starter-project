using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;
using Quaternion = UnityEngine.Quaternion;
using UnityEngine;
using Improbable.Unity.Entity;
using Improbable.Collections;

namespace Assets.Gamelogic.EntityTemplates
{
    public class EntityTemplateFactory : MonoBehaviour
    {
        public static Entity CreatePlayerCreatorTemplate()
        {
            var playerCreatorEntityTemplate = EntityBuilder.Begin()
                .AddPositionComponent(Improbable.Coordinates.ZERO.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.PlayerCreatorPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new PlayerCreation.Data(), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new ClientEntityStore.Data(new Map<string, EntityId>()), CommonRequirementSets.PhysicsOnly)
                .Build();

            return playerCreatorEntityTemplate;
        }

        public static Entity CreatePlayerTemplate(string clientId, EntityId playerCreatorId)
        {
            var playerTemplate = EntityBuilder.Begin()
                .AddPositionComponent(Improbable.Coordinates.ZERO.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.PlayerPrefabName)
                .SetPersistence(false)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new ClientAuthorityCheck.Data(), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddComponent(new ClientConnection.Data(SimulationSettings.TotalHeartbeatsBeforeTimeout, clientId, playerCreatorId), CommonRequirementSets.PhysicsOnly)
                .Build();

            return playerTemplate;
        }

        public static Entity CreateCubeTemplate()
        {
            var cubeTemplate = EntityBuilder.Begin()
                .AddPositionComponent(Improbable.Coordinates.ZERO.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.CubePrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .Build();

            return cubeTemplate;
        }
    }
}
