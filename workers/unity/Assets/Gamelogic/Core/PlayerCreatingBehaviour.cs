using Assets.Gamelogic.EntityTemplates;
using Improbable;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Worker;

namespace Assets.Gamelogic.Core
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class PlayerCreatingBehaviour : MonoBehaviour
    {
        [Require]
        private PlayerCreation.Writer PlayerCreationWriter;

        private void OnEnable()
        {
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.RegisterAsyncResponse(OnCreatePlayer);
        }

        private void OnDisable()
        {
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.DeregisterResponse();
        }

        private void OnCreatePlayer(ResponseHandle<PlayerCreation.Commands.CreatePlayer, CreatePlayerRequest, CreatePlayerResponse> responseHandle)
        {
            var clientWorkerId = responseHandle.CallerInfo.CallerWorkerId;
            var playerEntityTemplate = EntityTemplateFactory.CreatePlayerTemplate(clientWorkerId);
            SpatialOS.Commands.CreateEntity (PlayerCreationWriter, playerEntityTemplate)
                .OnSuccess (_ => responseHandle.Respond (new CreatePlayerResponse ((int) StatusCode.Success)))
                .OnFailure (failure => responseHandle.Respond (new CreatePlayerResponse ((int) failure.StatusCode)));
        }
    }
}
