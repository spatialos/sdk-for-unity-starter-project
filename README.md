# Starter Project
---

*Copyright (C) 2017 Improbable Worlds Limited. All rights reserved.*

- *GitHub repository*: [https://github.com/spatialos/StarterProject](https://github.com/spatialos/StarterProject)

---

## Introduction

This is a SpatialOS starter project with useful core features that you can extend to build your own SpatialOS application.

It contains:

* A Player spawned on client connection as per the [Unity Client Lifecycle Guide](https://github.com/spatialos/UnitySDK/blob/master/docs/tutorials/recipes/client-lifecycle.md).
* A Cube spawned through a snapshot via an entity template method and an Unity prefab.
* The rest of the features included in the [BlankProject](https://github.com/spatialos/BlankProject).

If you run into problems, or want to give us feedback, please visit the [SpatialOS forums](https://forums.improbable.io/).

## Running the project

To run the project locally, first build it by running `spatial worker build`, then start the server with `spatial local start`. You can connect a client by opening the Unity project and pressing the play button, or by running `spatial local worker launch UnityClient default`. See the [documentation](https://docs.improbable.io/reference/13.0/shared/deploy/deploy-local) for more details.

To deploy the project to the cloud, first build it by running `spatial worker build -t=deployment`, then upload the assembly with `spatial cloud upload <assembly name>`, and finally deploy it with `spatial cloud launch <assembly name> <launch configuration file> <deployment name> --snapshot=<snapshot file>`. You can obtain and share links to connect to the deployment from the [console](http://console.improbable.io/projects). See the [documentation](https://spatialos.improbable.io/docs/reference/13.0/shared/deploy/deploy-cloud) for more details.
