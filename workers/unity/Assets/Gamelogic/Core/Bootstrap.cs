using System;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Configuration;
using Improbable.Unity.Core;
using Improbable.Unity.Core.EntityQueries;
using UnityEngine;
using Improbable.Worker;


namespace Assets.Gamelogic.Core
{
    // Placed on a GameObject in a Unity scene to execute SpatialOS connection logic on startup.
    public class Bootstrap : MonoBehaviour
    {
        public WorkerConfigurationData Configuration = new WorkerConfigurationData();

        // Called when the Play button is pressed in Unity.
        public void Start()
        {
            SpatialOS.ApplyConfiguration(Configuration);

            Time.fixedDeltaTime = 1.0f / SimulationSettings.FixedFramerate;

            // Distinguishes between when the Unity is running as a client or a server.
            switch (SpatialOS.Configuration.WorkerPlatform)
            {
                case WorkerPlatform.UnityWorker:
                    Application.targetFrameRate = SimulationSettings.TargetServerFramerate;
                    SpatialOS.OnDisconnected += reason => Application.Quit();
                    break;
                case WorkerPlatform.UnityClient:
                    Application.targetFrameRate = SimulationSettings.TargetClientFramerate;
                    SpatialOS.OnConnected += CreatePlayer;
                    break;
            }

            // Enable communication with the SpatialOS layer of the simulation.
            SpatialOS.Connect(gameObject);
        }

        // Search for the PlayerCreator entity in the world in order to send a CreatePlayer command.
        public void CreatePlayer()
        {
            var playerCreatorQuery = Query.HasComponent<PlayerCreation>().ReturnOnlyEntityIds();
            SpatialOS.WorkerCommands.SendQuery(playerCreatorQuery)
                .OnSuccess(OnSuccessfulPlayerCreatorQuery)
                .OnFailure(OnFailedPlayerCreatorQuery);
        }

        private void OnSuccessfulPlayerCreatorQuery(EntityQueryResult queryResult)
        {
            if (queryResult.EntityCount < 1)
            {
                Debug.LogError("Failed to find PlayerCreator. SpatialOS probably hadn't finished loading the initial snapshot. Try again in a few seconds.");
                StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.PlayerCreatorQueryRetrySecs, CreatePlayer));
                return;
            }

            var playerCreatorEntityId = queryResult.Entities.First.Value.Key;
            RequestPlayerCreation(playerCreatorEntityId);
        }

        // Retry a failed search for the PlayerCreator entity after a short delay.
        private void OnFailedPlayerCreatorQuery(ICommandErrorDetails _)
        {
            Debug.LogError("PlayerCreator query failed. SpatialOS workers probably haven't started yet. Try again in a few seconds.");
            StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.PlayerCreatorQueryRetrySecs, CreatePlayer));
        }

        // Send a CreatePlayer command to the PLayerCreator entity requesting a Player entity be spawned.
        private void RequestPlayerCreation(EntityId playerCreatorEntityId)
        {
            SpatialOS.WorkerCommands.SendCommand(PlayerCreation.Commands.CreatePlayer.Descriptor, new CreatePlayerRequest(), playerCreatorEntityId)
                .OnSuccess(response => OnCreatePlayerCommandSuccess(response, playerCreatorEntityId))
                .OnFailure(response => OnCreatePlayerCommandFailure(response, playerCreatorEntityId));
        }

        private void OnCreatePlayerCommandSuccess(CreatePlayerResponse response, EntityId playerCreatorEntityId)
        {
            var statusCode = (StatusCode) response.statusCode;
            if (statusCode != StatusCode.Success) {
                Debug.LogWarningFormat("PlayerCreator failed to create the player entity. Status code = {0}. Try again in a few seconds.", statusCode.ToString());
                RetryCreatePlayerCommand(playerCreatorEntityId);
            }
        }

        private void OnCreatePlayerCommandFailure(ICommandErrorDetails details, EntityId playerCreatorEntityId){
            Debug.LogWarningFormat("CreatePlayer command failed. Status code = {0}. - you probably tried to connect too soon. Try again in a few seconds.", details.StatusCode.ToString());
            RetryCreatePlayerCommand(playerCreatorEntityId);
        }

        // Retry a failed creation of the Player entity after a short delay.
        private void RetryCreatePlayerCommand(EntityId playerCreatorEntityId)
        {
            StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.PlayerEntityCreationRetrySecs, () => RequestPlayerCreation(playerCreatorEntityId)));
        }
    }
}
