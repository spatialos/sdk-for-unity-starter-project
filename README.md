# SpatialOS SDK for Unity Starter Project

**New from October 2018: The SpatialOS GDK for Unity**<br/>
Check out the alpha release of our [SpatialOS Game Development Kit (GDK) for Unity](https://docs.improbable.io/unity/latest/welcome). Using the Unity Entity Component System (ECS), the GDK is the next evolution in developing SpatialOS games in Unity. The SpatialOS GDK for Unity is designed to replace the SpatialOS SDK for Unity and we recommend using it over using the SDK for new game projects. See our [blog post anouncement](https://improbable.io/games/blog/spatialos-gdk-for-unity-launch?utm_medium=docs&utm_source=onboarding&utm_campaign=spatialos-gdk-unity-launch&utm_content=10-oct) for more information.

---

- *GitHub repository*: [github.com/spatialos/StarterProject](https://github.com/spatialos/StarterProject)

## Introduction

This is a [SpatialOS](https://docs.improbable.io/reference/latest/shared/concepts/spatialos) starter project to use with the [SDK for Unity](https://github.com/spatialos/UnitySDK) with useful core features that you can extend to build your own SpatialOS application.

It contains:

* A Player spawned on client connection as per the [Unity Client Lifecycle Guide](https://github.com/spatialos/UnitySDK/blob/master/docs/tutorials/recipes/client-lifecycle.md).
* A Cube spawned through a snapshot via an entity template method and an Unity prefab.
* The rest of the features included in the [BlankProject](https://github.com/spatialos/BlankProject).

If you run into problems, or want to give us feedback, please visit the [SpatialOS forums](https://forums.improbable.io/).

## Running the project

To run the project locally, first build it by running `spatial worker build`, then start the server with `spatial local start`. You can connect a client by opening the Unity project and pressing the play button, or by running `spatial local worker launch UnityClient default`. See the [documentation](https://docs.improbable.io/reference/13.0/shared/deploy/deploy-local) for more details.

To deploy the project to the cloud, first build it by running `spatial worker build -t=deployment`, then upload the assembly with `spatial cloud upload <assembly name>`, and finally deploy it with `spatial cloud launch <assembly name> <launch configuration file> <deployment name> --snapshot=<snapshot file>`. You can obtain and share links to connect to the deployment from the [console](http://console.improbable.io/projects). See the [documentation](https://spatialos.improbable.io/docs/reference/13.0/shared/deploy/deploy-cloud) for more details.

----
*Copyright (C) 2017 Improbable Worlds Limited. All rights reserved.*
