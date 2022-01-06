# OBB Reflection Probe
This package adds basic support for oriented bounding box projection to Unity **Built-in Rendering Pipeline**.

#### Supported

- Forward and Legacy Deferred.
- Renderer.Probes.ReflectionProbes.Simple mode. May require probes to be anchored to the renderers.
- Single oriented bounding box reflection probe in a scene ( simple use case ).
- Multiple oriented bounding box reflection probes ( complicated use case ).

#### Not Supported

- Deferred.
- Blended Probes - no support at all - not sure what happens - best to set up renderers to use simple mode.

## Issues

It should be noted that whilst it technically works Unity’s built-in pipeline simply does not have the required hooks or systems to fully support a custom OBB projection system. The main issue is that Unity still performs various calculations on the original probe axis aligned bounding box which conflicts with the needs of an OBB probe. 

This can be worked around via anchoring probes to renderers but that is quite a restriction and can be painful to work with for a project of any complexity.

To fully support OBB projection it's likely you’d need to write your own reflection probe component and ALL necessary subsystems. e.g. tracking renderers and which probes they are affected by, culling probes etc, which is an extensive project with challenges for performance.

## Prerequisites
### Unity 2019.4.28f1
This package is in development, and requires Unity 2019.4.28f1

## Getting Started
Please refer to the **OOBB Reflection Probe Support Guide** pdf in the documentation folder.

