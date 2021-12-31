# OBB Reflection Probe
This package adds basic support for oriented bounding box projection to Unitys Built-in Rendering Pipeline.

Currently it only supports a single oriented bounding box and whilst it could be extended to support several using the same strategy, doing it correctly is a far more complex task.

The problem is currently it’s not possible to integrate passing the required parameters directly into the Unity source. To get around this issue the project uses shader globals to pass the required matrix to a custom Standard shader. Any materials that use this shader can then take advantage of OBB projection instead of AABB projection that Unity offers.

In the future I want to take another look into this and see if there is a better method to achieve this objective, streamlining its use and supporting multiple OBB reflection probes.


## Prerequisites
### Unity 2019.4
This package is in development, and requires Unity 2019.4.

## Getting Started
TBD

