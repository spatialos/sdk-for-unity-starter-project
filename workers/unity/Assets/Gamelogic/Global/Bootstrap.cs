using Improbable;
using Improbable.Global;
using Improbable.Unity.Core;
using Improbable.Unity.Core.EntityQueries;
using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Configuration;
using Assets.Gamelogic.Global;
using Assets.Gamelogic.Utils;

// Placed on a GameObject in the ClientScene to execute connection logic on client startup.
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
    public static void CreatePlayer()
    {
        var playerCreatorQuery = Query.HasComponent<PlayerCreation>().ReturnOnlyEntityIds();
        SpatialOS.WorkerCommands.SendQuery(playerCreatorQuery)
            .OnSuccess(OnSuccessfulPlayerCreatorQuery)
            .OnFailure(OnFailedPlayerCreatorQuery);
    }

    private static void OnSuccessfulPlayerCreatorQuery(EntityQueryResult queryResult)
    {
        if (queryResult.EntityCount < 1)
        {
            Debug.LogError("Failed to find PlayerCreator. SpatialOS probably hadn't finished loading the initial snapshot. Try again in a few seconds.");
            return;
        }

        var playerCreatorEntityId = queryResult.Entities.First.Value.Key;
        RequestPlayerCreation(playerCreatorEntityId);
    }

    // Retry a failed search for the PlayerCreator entity after a short delay.
    private static void OnFailedPlayerCreatorQuery(ICommandErrorDetails _)
    {
        Debug.LogError("PlayerCreator query failed. SpatialOS workers probably haven't started yet. Try again in a few seconds.");
        TimerUtils.WaitAndPerform(SimulationSettings.PlayerCreatorQueryRetrySecs, CreatePlayer);
    }

    // Send a CreatePlayer command to the PLayerCreator entity requesting a Player entity be spawned.
    private static void RequestPlayerCreation(EntityId playerCreatorEntityId)
    {
        SpatialOS.WorkerCommands.SendCommand(PlayerCreation.Commands.CreatePlayer.Descriptor, new CreatePlayerRequest(), playerCreatorEntityId)
            .OnFailure(response => OnCreatePlayerFailure(response, playerCreatorEntityId));
    }

    // Retry a failed creation of the Player entity after a short delay.
    private static void OnCreatePlayerFailure(ICommandErrorDetails _, EntityId playerCreatorEntityId)
    {
        Debug.LogWarning("CreatePlayer command failed - you probably tried to connect too soon. Try again in a few seconds.");
        TimerUtils.WaitAndPerform(SimulationSettings.PlayerEntityCreationRetrySecs, () => RequestPlayerCreation(playerCreatorEntityId));
    }
}