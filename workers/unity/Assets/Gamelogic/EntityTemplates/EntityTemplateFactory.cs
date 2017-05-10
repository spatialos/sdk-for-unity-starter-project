using Assets.Gamelogic.Core;
using Improbable.Core;
using Improbable.Math;
using Improbable.Player;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;
using Quaternion = Improbable.Core.Quaternion;
using UnityEngine;

namespace Assets.Gamelogic.EntityTemplates
{
    public class EntityTemplateFactory : MonoBehaviour
    {
        public static SnapshotEntity CreatePlayerCreatorTemplate()
        {
            var playerCreatorEntityTemplate = new SnapshotEntity { Prefab = SimulationSettings.PlayerCreatorPrefabName };

            playerCreatorEntityTemplate.Add(new WorldTransform.Data(Coordinates.ZERO, new Quaternion(0,0,0,0)));
            playerCreatorEntityTemplate.Add(new PlayerCreation.Data());

            var acl = Acl.GenerateServerAuthoritativeAcl(playerCreatorEntityTemplate);
            playerCreatorEntityTemplate.SetAcl(acl);

            return playerCreatorEntityTemplate;
        }

        public static Entity CreatePlayerTemplate(string clientId)
        {
            var playerTemplate = new SnapshotEntity { Prefab = SimulationSettings.PlayerPrefabName };

            playerTemplate.Add(new WorldTransform.Data(Coordinates.ZERO, new Quaternion(0, 0, 0, 0)));
            playerTemplate.Add(new ClientAuthorityCheck.Data());
            playerTemplate.Add(new ClientConnection.Data(SimulationSettings.TotalHeartbeatsBeforeTimeout));

            var acl = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<WorldTransform>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<ClientAuthorityCheck>(CommonRequirementSets.SpecificClientOnly(clientId))
                .SetWriteAccess<ClientConnection>(CommonRequirementSets.PhysicsOnly);
            playerTemplate.SetAcl(acl);

            return playerTemplate;
        }

        public static SnapshotEntity CreateCubeTemplate()
        {
            var cubeTemplate = new SnapshotEntity { Prefab = SimulationSettings.CubePrefabName };

            cubeTemplate.Add(new WorldTransform.Data(new Coordinates(0,0,5), new Quaternion(0, 0, 0, 0)));

            var acl = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<WorldTransform>(CommonRequirementSets.PhysicsOnly);
            cubeTemplate.SetAcl(acl);

            return cubeTemplate;
        }
    }
}
