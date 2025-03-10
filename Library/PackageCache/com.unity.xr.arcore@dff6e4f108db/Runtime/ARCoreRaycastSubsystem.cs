using System.Runtime.InteropServices;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// ARCore implementation of the [XRRaycastSubsystem](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystem).
    /// Do not create this directly. Use the [SubsystemManager](xref:UnityEngine.SubsystemManager) instead.
    /// </summary>
    [Preserve]
    public sealed class ARCoreRaycastSubsystem : XRRaycastSubsystem
    {
        class ARCoreProvider : Provider
        {
            public override void Start() => UnityARCore_raycast_startTracking();
            public override void Stop() => UnityARCore_raycast_stopTracking();
            public override void Destroy() => UnityARCore_raycast_destroy();

            public override unsafe TrackableChanges<XRRaycast> GetChanges(
                XRRaycast defaultRaycast,
                Allocator allocator)
            {
                int addedLength, updatedLength, removedLength, elementSize;
                void* addedPtr, updatedPtr, removedPtr;
                var context = UnityARCore_raycast_acquireChanges(
                    out addedPtr, out addedLength,
                    out updatedPtr, out updatedLength,
                    out removedPtr, out removedLength,
                    out elementSize);

                try
                {
                    return new TrackableChanges<XRRaycast>(
                        addedPtr, addedLength,
                        updatedPtr, updatedLength,
                        removedPtr, removedLength,
                        defaultRaycast, elementSize,
                        allocator);
                }
                finally
                {
                    UnityARCore_raycast_releaseChanges(context);
                }
            }

            public override bool TryAddRaycast(Vector2 screenPoint, float estimatedDistance, out XRRaycast sessionRelativeData)
            {
                return UnityARCore_raycast_tryAddRaycast(screenPoint, estimatedDistance, out sessionRelativeData);
            }

            public override void RemoveRaycast(TrackableId trackableId)
            {
                UnityARCore_raycast_removeRaycast(trackableId);
            }

            public override unsafe NativeArray<XRRaycastHit> Raycast(
                XRRaycastHit defaultRaycastHit,
                Ray ray,
                TrackableType trackableTypeMask,
                Allocator allocator)
            {
                void* hitBuffer;
                int hitCount, elementSize;

                UnityARCore_raycast_acquireHitResultsRay(
                    ray.origin,
                    ray.direction,
                    trackableTypeMask,
                    out hitBuffer,
                    out hitCount,
                    out elementSize);

                try
                {
                    return NativeCopyUtility.PtrToNativeArrayWithDefault<XRRaycastHit>(
                        defaultRaycastHit,
                        hitBuffer, elementSize,
                        hitCount, allocator);
                }
                finally
                {
                    UnityARCore_raycast_releaseHitResults(hitBuffer);
                }
            }

            public override unsafe NativeArray<XRRaycastHit> Raycast(
                XRRaycastHit defaultRaycastHit,
                Vector2 screenPoint,
                TrackableType trackableTypeMask,
                Allocator allocator)
            {
                void* hitBuffer;
                int hitCount, elementSize;

                UnityARCore_raycast_acquireHitResults(
                    screenPoint,
                    trackableTypeMask,
                    out hitBuffer,
                    out hitCount,
                    out elementSize);

                try
                {
                    return NativeCopyUtility.PtrToNativeArrayWithDefault<XRRaycastHit>(
                        defaultRaycastHit,
                        hitBuffer, elementSize,
                        hitCount, allocator);
                }
                finally
                {
                    UnityARCore_raycast_releaseHitResults(hitBuffer);
                }
            }

            [DllImport(Constants.k_LibraryName)]
            static unsafe extern void UnityARCore_raycast_acquireHitResults(
                Vector2 screenPoint,
                TrackableType filter,
                out void* hitBuffer,
                out int hitCount,
                out int elementSize);

            [DllImport(Constants.k_LibraryName)]
            static unsafe extern void UnityARCore_raycast_acquireHitResultsRay(
                Vector3 rayOrigin,
                Vector3 rayDirection,
                TrackableType filter,
                out void* hitBuffer,
                out int hitCount,
                out int elementSize);

            [DllImport(Constants.k_LibraryName)]
            static unsafe extern void UnityARCore_raycast_releaseHitResults(
                void* buffer);

            [DllImport(Constants.k_LibraryName)]
            static unsafe extern bool UnityARCore_raycast_tryAddRaycast(
                Vector2 screenPoint,
                float estimatedDistance,
                out XRRaycast raycastOut);

            [DllImport(Constants.k_LibraryName)]
            static unsafe extern void UnityARCore_raycast_removeRaycast(
               TrackableId trackableId);

            [DllImport(Constants.k_LibraryName)]
            static extern void UnityARCore_raycast_startTracking();

            [DllImport(Constants.k_LibraryName)]
            static extern void UnityARCore_raycast_stopTracking();

            [DllImport(Constants.k_LibraryName)]
            static extern unsafe void* UnityARCore_raycast_acquireChanges(
                out void* addedPtr, out int addedLength,
                out void* updatedPtr, out int updatedLength,
                out void* removedPtr, out int removedLength,
                out int elementSize);

            [DllImport(Constants.k_LibraryName)]
            static extern unsafe void UnityARCore_raycast_releaseChanges(
                void* changes);

            [DllImport(Constants.k_LibraryName)]
            static extern void UnityARCore_raycast_destroy();

        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            if (!Api.platformAndroid || !Api.loaderPresent)
                return;

            XRRaycastSubsystemDescriptor.Register(new XRRaycastSubsystemDescriptor.Cinfo
            {
                id = "ARCore-Raycast",
                providerType = typeof(ARCoreRaycastSubsystem.ARCoreProvider),
                subsystemTypeOverride = typeof(ARCoreRaycastSubsystem),
                supportsViewportBasedRaycast = true,
                supportsWorldBasedRaycast = true,
                supportedTrackableTypes =
                    (TrackableType.Planes & ~TrackableType.PlaneWithinInfinity) |
                    TrackableType.FeaturePoint |
                    TrackableType.Depth,
                supportsTrackedRaycasts = true,
            });
        }
    }
}
