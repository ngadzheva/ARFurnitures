using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARPlaneMeshVisualizer), typeof(MeshRenderer), typeof(ARPlane))]
    public class ARFeatheredPlaneMeshVisualizer : MonoBehaviour
    {
        private MeshCollider meshCollider;
        static List<Vector3> featheringUVs = new List<Vector3>();
        static List<Vector3> vertices = new List<Vector3>();
        ARPlaneMeshVisualizer planeMeshVisualizer;
        ARPlane plane;
        Material featheredPlaneMaterial;

        [SerializeField]
        float featheredPlaneWidth = 0.2f;

        public float featheringWidth
        {
            get { return featheredPlaneWidth; }
            set { featheredPlaneWidth = value; }
        }

        void Awake()
        {
            planeMeshVisualizer = GetComponent<ARPlaneMeshVisualizer>();
            featheredPlaneMaterial = GetComponent<MeshRenderer>().material;
            plane = GetComponent<ARPlane>();
            meshCollider = GetComponent<MeshCollider>();
        }

        void Update()
        {
            meshCollider.sharedMesh = planeMeshVisualizer.mesh; 
        }

        void OnEnable()
        {
            plane.boundaryChanged += ARPlaneBoundaryUpdated;
        }

        void OnDisable()
        {
            plane.boundaryChanged -= ARPlaneBoundaryUpdated;
        }

        void ARPlaneBoundaryUpdated(ARPlaneBoundaryChangedEventArgs eventArgs)
        {
            GenerateBoundaryUVs(planeMeshVisualizer.mesh);
        }

        void GenerateBoundaryUVs(Mesh mesh)
        {
            int vertexCount = mesh.vertexCount;

            featheringUVs.Clear();
            if (featheringUVs.Capacity < vertexCount) { featheringUVs.Capacity = vertexCount; }

            mesh.GetVertices(vertices);

            Vector3 centerInPlaneSpace = vertices[vertices.Count - 1];
            Vector3 uv = new Vector3(0, 0, 0);
            float shortestUVMapping = float.MaxValue;

            for (int i = 0; i < vertexCount - 1; ++i)
            {
                float vertexDist = Vector3.Distance(vertices[i], centerInPlaneSpace);

                float uvMapping = vertexDist / Mathf.Max(vertexDist - featheringWidth, 0.001f);
                uv.x = uvMapping;

                if (shortestUVMapping > uvMapping) { shortestUVMapping = uvMapping; }

                featheringUVs.Add(uv);
            }

            featheredPlaneMaterial.SetFloat("_ShortestUVMapping", shortestUVMapping);

            uv.Set(0, 0, 0);
            featheringUVs.Add(uv);

            mesh.SetUVs(1, featheringUVs);
            mesh.UploadMeshData(false);
        }
    }
}
