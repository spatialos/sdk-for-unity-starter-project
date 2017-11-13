using Assets.Gamelogic.EntityTemplates;
using Improbable;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Worker;
using Improbable.Unity.Core.EntityQueries;
using System;
using System.Collections.Generic;
using Improbable.Collections;
using System.Linq;
using Improbable.Unity.Core.Acls;

namespace Assets.Gamelogic.Core
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class PlayerCreatingBehaviour : MonoBehaviour
    {
        [Require]
        private PlayerCreation.Writer PlayerCreationWriter;

        [Require]
        private ClientEntityStore.Writer ClientEntityStoreWriter;

        private HashSet<string> inFlightClientRequests;

        private void OnEnable()
        {
            inFlightClientRequests = new HashSet<string>();
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.RegisterAsyncResponse(OnCreatePlayer);
            PlayerCreationWriter.CommandReceiver.OnDeletePlayer.RegisterResponse(OnDeletePlayer);
        }

        private void OnDisable()
        {
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.DeregisterResponse();
            PlayerCreationWriter.CommandReceiver.OnDeletePlayer.DeregisterResponse();
        }

        private void OnCreatePlayer(ResponseHandle<PlayerCreation.Commands.CreatePlayer, CreatePlayerRequest, CreatePlayerResponse> responseHandle)
        {
            var clientWorkerId = responseHandle.CallerInfo.CallerWorkerId;

            if (inFlightClientRequests.Contains(clientWorkerId))
            {
                responseHandle.Respond(new CreatePlayerResponse(ResponseCode.ExistingRequestInProgress, null));
            }
            else
            {
                RequestStarted(responseHandle);

                EntityId playerEntityId;
                if (ClientEntityStoreWriter.Data.playerEntities.TryGetValue(clientWorkerId, out playerEntityId))
                {
                    CheckPlayerEntityExists(playerEntityId, (entityExists) =>
                    {
                        if (entityExists)
                        {
                            RequestEnded(responseHandle, ResponseCode.EntityForPlayerExists);
                        }
                        else
                        {
                            CreatePlayerEntity(clientWorkerId, responseHandle);
                        }
                    }, (statusCode) =>
                    {
                        RequestEnded(responseHandle, ResponseCode.Failure, statusCode);
                    });
                }
                else
                {
                    CreatePlayerEntity(clientWorkerId, responseHandle);
                }

            }
        }

        private DeletePlayerResponse OnDeletePlayer(DeletePlayerRequest request, ICommandCallerInfo callerInfo)
        {
            if (callerInfo.CallerAttributeSet.Contains("physics"))
            {
                EntityId playerEntityId;
                if (ClientEntityStoreWriter.Data.playerEntities.TryGetValue(request.clientId, out playerEntityId))
                {
                    SpatialOS.Commands.DeleteEntity(PlayerCreationWriter, playerEntityId)
                             .OnSuccess(_ => RemoveClientId(request.clientId));
                }
            }
            else
            {
                Debug.LogWarningFormat("PlayerCreator ignoring command because it was sent from {0}", callerInfo.CallerWorkerId);
            }

            return new DeletePlayerResponse();
        }

        private void CreatePlayerEntity(string clientWorkerId,
                                        ResponseHandle<PlayerCreation.Commands.CreatePlayer, CreatePlayerRequest, CreatePlayerResponse> responseHandle)
        {
            var playerEntityTemplate = EntityTemplateFactory.CreatePlayerTemplate(clientWorkerId, gameObject.EntityId());
            SpatialOS.Commands.CreateEntity(PlayerCreationWriter, playerEntityTemplate)
                .OnSuccess(response =>
                     {
                         AddPlayerEntityId(clientWorkerId, response.CreatedEntityId);
                         RequestEnded(responseHandle, ResponseCode.SuccessfullyCreated);
                     })
                     .OnFailure(failure => RequestEnded(responseHandle, ResponseCode.Failure, failure.StatusCode));
        }

        private void CheckPlayerEntityExists(EntityId playerEntityId, Action<bool> onSuccess, Action<StatusCode> onFailure)
        {
            var query = Query.HasEntityId(playerEntityId).ReturnCount();

            SpatialOS.Commands.SendQuery(PlayerCreationWriter, query)
                .OnSuccess(result => onSuccess(result.EntityCount > 0))
                .OnFailure(failure => onFailure(failure.StatusCode));
        }

        private void AddPlayerEntityId(string clientWorkerId, EntityId playerEntityId)
        {
            var playerEntities = ClientEntityStoreWriter.Data.playerEntities;
            playerEntities.Add(clientWorkerId, playerEntityId);
            ClientEntityStoreWriter.Send(new ClientEntityStore.Update().SetPlayerEntities(playerEntities));
        }

        private void RemoveClientId(string clientWorkerId)
        {
            var playerEntities = ClientEntityStoreWriter.Data.playerEntities;
            playerEntities.Remove(clientWorkerId);
            ClientEntityStoreWriter.Send(new ClientEntityStore.Update().SetPlayerEntities(playerEntities));
        }

        private void RequestStarted(ResponseHandle<PlayerCreation.Commands.CreatePlayer, CreatePlayerRequest, CreatePlayerResponse> responseHandle)
        {
            inFlightClientRequests.Add(responseHandle.CallerInfo.CallerWorkerId);
        }

        private void RequestEnded(ResponseHandle<PlayerCreation.Commands.CreatePlayer, CreatePlayerRequest, CreatePlayerResponse> responseHandle,
                                  ResponseCode responseCode, Option<StatusCode> failureCode = new Option<StatusCode>())
        {
            Option<int> intFailureCode = failureCode.HasValue ? (int)failureCode.Value : new Option<int>();
            inFlightClientRequests.Remove(responseHandle.CallerInfo.CallerWorkerId);
            responseHandle.Respond(new CreatePlayerResponse(responseCode, intFailureCode));
        }
    }
}
