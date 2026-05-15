/*
 * SurfaceAwarePrefabSpawner.cs
 *
 * Subclass of AnchorPrefabSpawner that correctly scales, aligns, and rotates
 * prefabs on CEILING, FLOOR, and WALL_ART anchors.
 *
 * HOW TO USE
 * ----------
 *  1. Replace AnchorPrefabSpawner with SurfaceAwarePrefabSpawner on your GameObject.
 *  2. For CEILING and FLOOR groups set ScalingMode = Custom, AlignMode = Custom.
 *  3. For WALL_ART groups set ScalingMode = Custom, AlignMode = Custom.
 *  4. Enable DebugLog in the Inspector on first run and check the Console.
 *     The log prints the anchor's local axes and the sizes it measured so you
 *     can confirm which axis is which on your device before tuning offsets.
 */

using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.Samples
{
    public class SurfaceAwarePrefabSpawner : AnchorPrefabSpawner
    {
        // -----------------------------------------------------------------------
        // Inspector
        // -----------------------------------------------------------------------

        [Header("Surface-Aware Settings")]

        [Tooltip("Log anchor axes and measured sizes to the Console on spawn. " +
                 "Use this to verify axis mapping on device before disabling.")]
        [SerializeField] private bool _debugLog = true;

        [Tooltip("Physical thickness in metres of the spawned ceiling/floor slab. " +
                 "Keep this very small (0.001 to 0.005) so it does not occlude the room.")]
        [SerializeField] private float _surfaceThickness = 0.002f;

        [Tooltip("Downward gap in metres between the ceiling surface and the prefab.")]
        [SerializeField] private float _ceilingDropOffset = 0.001f;

        [Tooltip("Upward gap in metres between the floor surface and the prefab.")]
        [SerializeField] private float _floorRiseOffset = 0.0f;

        [Tooltip("How far in metres wall-art prefabs stand off the wall plane.")]
        [SerializeField] private float _wallArtStandoffOffset = 0.005f;

        // -----------------------------------------------------------------------
        // SpawnPrefab override
        // Let the base class instantiate and parent the prefab, then replace every
        // transform value with our own correct calculation.
        // -----------------------------------------------------------------------

        protected override void SpawnPrefab(MRUKAnchor anchorInfo)
        {
            base.SpawnPrefab(anchorInfo);

            if (!AnchorPrefabSpawnerObjects.TryGetValue(anchorInfo, out var go))
            {
                return;
            }

            var label = anchorInfo.Label;

            if (IsCeiling(label) || IsFloor(label))
            {
                ApplyHorizontalSurface(anchorInfo, go, IsCeiling(label));
            }
            else if (IsWallArt(label))
            {
                ApplyWallArt(anchorInfo, go);
            }
        }

        // -----------------------------------------------------------------------
        // CEILING and FLOOR
        //
        // Meta MRUK ceiling and floor anchors:
        //   - Are PlaneRect anchors (flat quads, no VolumeBounds in most scenes).
        //   - The anchor transform has its LOCAL Y axis pointing toward the room
        //     interior (down for ceiling, up for floor) -- i.e. the anchor normal.
        //   - LOCAL X and LOCAL Z span the horizontal plane of the room.
        //   - PlaneRect.width  maps to the anchor LOCAL X extent.
        //   - PlaneRect.height maps to the anchor LOCAL Z extent.
        //
        // We therefore need to:
        //   Scale  : prefabX = rect.width, prefabZ = rect.height, prefabY = tiny
        //   Rotate : lay the prefab flat in the XZ plane
        //            floor   -> Euler(0, 0, 0)   already flat if authored upright,
        //                        but most prefabs are authored standing up so we
        //                        rotate -90 on X to lay them down.
        //            ceiling -> same lay-down rotation then flip 180 on Y so the
        //                        top face points into the room.
        //   Position: zero local position (anchor is already at the surface).
        // -----------------------------------------------------------------------

        private void ApplyHorizontalSurface(MRUKAnchor anchor, GameObject go, bool isCeiling)
        {
            // Measure the prefab's authored size before we apply any scale.
            var prefabSize = GetLocalMeshSize(go);

            if (_debugLog)
            {
                Debug.Log(string.Format(
                    "[SurfaceAwarePrefabSpawner] {0} anchor '{1}'\n" +
                    "  anchor localScale    : {2}\n" +
                    "  anchor localRight    : {3}\n" +
                    "  anchor localUp       : {4}\n" +
                    "  anchor localForward  : {5}\n" +
                    "  PlaneRect            : {6}\n" +
                    "  VolumeBounds         : {7}\n" +
                    "  prefab authored size : {8}",
                    isCeiling ? "CEILING" : "FLOOR",
                    anchor.name,
                    anchor.transform.localScale,
                    anchor.transform.right,
                    anchor.transform.up,
                    anchor.transform.forward,
                    anchor.PlaneRect.HasValue ? anchor.PlaneRect.Value.ToString() : "none",
                    anchor.VolumeBounds.HasValue ? anchor.VolumeBounds.Value.ToString() : "none",
                    prefabSize
                ));
            }

            float surfaceWidth;
            float surfaceDepth;
            float surfaceThickness;

            if (anchor.PlaneRect.HasValue)
            {
                var rect = anchor.PlaneRect.Value;
                surfaceWidth = rect.width;
                surfaceDepth = rect.height;
                surfaceThickness = 1f;  // plane has no thickness; keep prefab Z scale = 1
            }
            else if (anchor.VolumeBounds.HasValue)
            {
                // Volume axes in MRUK anchor local space:
                //   size.x = room X extent
                //   size.y = room Z extent  (depth)
                //   size.z = vertical thickness of the slab
                var vol = anchor.VolumeBounds.Value;
                surfaceWidth = vol.size.x;
                surfaceDepth = vol.size.y;
                surfaceThickness = vol.size.z;
            }
            else
            {
                // No geometry data -- skip.
                return;
            }

            float px = Mathf.Max(prefabSize.x, 0.0001f);
            float py = Mathf.Max(prefabSize.y, 0.0001f);
            float pz = Mathf.Max(prefabSize.z, 0.0001f);

            // Most prefabs are authored standing upright (tall in Y, flat in XZ).
            // To lay them flat on the ceiling/floor:
            //   - prefab X fills surface width  (anchor local X)
            //   - prefab Y fills surface thickness (very small, or 1 for planes)
            //   - prefab Z fills surface depth  (anchor local Z)
            //
            // Then we rotate -90 degrees on X to lay the prefab down so that
            // the prefab's XY face becomes the XZ world floor/ceiling face.

            // Clamp thickness to a tiny constant so the prefab never occludes
            // what is beneath it. For plane anchors surfaceThickness has no real
            // data so we ignore it entirely and use the inspector _surfaceThickness value.
            float thicknessScale = _surfaceThickness / Mathf.Max(py, 0.0001f);

            go.transform.localScale = new Vector3(
                surfaceWidth / px,
                thicknessScale,
                surfaceDepth / pz
            );

            // Offset by half the slab thickness so the visible face sits exactly
            // on the anchor surface rather than being half-buried inside it.
            float halfSlab = _surfaceThickness * 0.5f;

            if (isCeiling)
            {
                go.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);
                go.transform.localPosition = new Vector3(0f, -(halfSlab + _ceilingDropOffset), 0f);
            }
            else
            {
                go.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                go.transform.localPosition = new Vector3(0f, halfSlab + _floorRiseOffset, 0f);
            }
        }

        // -----------------------------------------------------------------------
        // WALL ART
        //
        // Wall-art anchors are PlaneRect anchors on a vertical wall.
        // The anchor local Y is the wall normal pointing into the room.
        // LOCAL X and LOCAL Z span the wall face.
        // PlaneRect.width  -> anchor LOCAL X
        // PlaneRect.height -> anchor LOCAL Z
        //
        // Prefabs authored as flat quads (thin in Y) need:
        //   Scale  : prefabX = rect.width, prefabZ = rect.height, prefabY = standoff
        //   Rotate : none required if the prefab XZ face is already the art face,
        //            otherwise -90 on X to reorient.
        //   Position: push out by standoff along local Y (normal direction).
        // -----------------------------------------------------------------------

        private void ApplyWallArt(MRUKAnchor anchor, GameObject go)
        {
            if (!anchor.PlaneRect.HasValue)
            {
                return;
            }

            var rect = anchor.PlaneRect.Value;
            var prefabSize = GetLocalMeshSize(go);

            if (_debugLog)
            {
                Debug.Log(string.Format(
                    "[SurfaceAwarePrefabSpawner] WALL_ART anchor '{0}'\n" +
                    "  anchor localRight   : {1}\n" +
                    "  anchor localUp      : {2}\n" +
                    "  anchor localForward : {3}\n" +
                    "  PlaneRect           : {4}\n" +
                    "  prefab authored size: {5}",
                    anchor.name,
                    anchor.transform.right,
                    anchor.transform.up,
                    anchor.transform.forward,
                    rect,
                    prefabSize
                ));
            }

            float px = Mathf.Max(prefabSize.x, 0.0001f);
            float pz = Mathf.Max(prefabSize.z, 0.0001f);

            go.transform.localScale = new Vector3(
                rect.width / px,
                1f,
                rect.height / pz
            );

            // No rotation needed if the prefab is already a flat XZ quad.
            // If the prefab is upright (tall in Y) swap these euler values to taste
            // after checking the debug log.
            go.transform.localRotation = Quaternion.identity;

            // Stand the prefab off the wall surface along the anchor local Y (normal).
            go.transform.localPosition = new Vector3(0f, _wallArtStandoffOffset, 0f);
        }

        // -----------------------------------------------------------------------
        // CustomPrefabScaling / CustomPrefabAlignment
        // The base class calls these when ScalingMode/AlignMode == Custom.
        // Return neutral values here -- our post-spawn fixup in SpawnPrefab
        // overwrites everything anyway.
        // -----------------------------------------------------------------------

        public override Vector3 CustomPrefabScaling(Vector3 localScale)
        {
            return Vector3.one;
        }

        public override Vector2 CustomPrefabScaling(Vector2 localScale)
        {
            return Vector2.one;
        }

        public override Vector3 CustomPrefabAlignment(Bounds anchorVolumeBounds, Bounds? prefabBounds)
        {
            return Vector3.zero;
        }

        public override Vector3 CustomPrefabAlignment(Rect anchorPlaneRect, Bounds? prefabBounds)
        {
            return Vector3.zero;
        }

        // -----------------------------------------------------------------------
        // Measure a prefab's authored size in its own local space.
        // Walks all child Renderers and encapsulates their bounds, then converts
        // back to the root's local space so scale is not included.
        // -----------------------------------------------------------------------

        private static Vector3 GetLocalMeshSize(GameObject go)
        {
            // Temporarily reset scale so bounds are in authored (scale=1) space.
            var savedScale = go.transform.localScale;
            go.transform.localScale = Vector3.one;

            var renderers = go.GetComponentsInChildren<Renderer>(true);

            if (renderers == null || renderers.Length == 0)
            {
                go.transform.localScale = savedScale;
                return Vector3.one;
            }

            var combined = new Bounds(renderers[0].bounds.center, Vector3.zero);
            foreach (var r in renderers)
            {
                combined.Encapsulate(r.bounds);
            }

            go.transform.localScale = savedScale;

            // Return size in world units when scale=1, which equals authored local size.
            return combined.size;
        }

        // -----------------------------------------------------------------------
        // Label helpers
        // -----------------------------------------------------------------------

        private static bool IsCeiling(MRUKAnchor.SceneLabels label)
        {
            return (label & MRUKAnchor.SceneLabels.CEILING) != 0;
        }

        private static bool IsFloor(MRUKAnchor.SceneLabels label)
        {
            return (label & MRUKAnchor.SceneLabels.FLOOR) != 0;
        }

        private static bool IsWallArt(MRUKAnchor.SceneLabels label)
        {
            return (label & MRUKAnchor.SceneLabels.WALL_ART) != 0;
        }

        // -----------------------------------------------------------------------
        // Editor gizmos
        // -----------------------------------------------------------------------

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach (var kv in AnchorPrefabSpawnerObjects)
            {
                if (kv.Value == null)
                {
                    continue;
                }

                var label = kv.Key.Label;

                if (IsCeiling(label))
                {
                    Gizmos.color = Color.cyan;
                }
                else if (IsFloor(label))
                {
                    Gizmos.color = Color.green;
                }
                else if (IsWallArt(label))
                {
                    Gizmos.color = Color.yellow;
                }
                else
                {
                    Gizmos.color = Color.white;
                }

                Gizmos.DrawWireCube(kv.Value.transform.position, Vector3.one * 0.1f);
                Gizmos.DrawRay(kv.Value.transform.position, kv.Value.transform.forward * 0.2f);
            }
        }
#endif
    }
}