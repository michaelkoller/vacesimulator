using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.EditorVR.Data;
using UnityEngine;

namespace UnityEditor.Experimental.EditorVR.Modules
{
    sealed class SpatialHashModule : MonoBehaviour, ISystemModule
    {
        readonly List<Renderer> m_ChangedObjects = new List<Renderer>();

        public SpatialHash<Renderer> spatialHash { get; private set; }
        public Func<GameObject, bool> shouldExcludeObject { private get; set; }

        void Awake()
        {
            spatialHash = new SpatialHash<Renderer>();
        }

        internal void Setup()
        {
            SetupObjects();
            StartCoroutine(UpdateDynamicObjects());
        }

        void SetupObjects()
        {
            var meshFilters = FindObjectsOfType<MeshFilter>();
            foreach (var mf in meshFilters)
            {
                if (mf.sharedMesh)
                {
                    if (shouldExcludeObject != null && shouldExcludeObject(mf.gameObject))
                        continue;

                    Renderer renderer = mf.GetComponent<Renderer>();
                    if (renderer)
                        spatialHash.AddObject(renderer, renderer.bounds);
                }
            }

            var skinnedMeshRenderers = FindObjectsOfType<SkinnedMeshRenderer>();
            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                if (skinnedMeshRenderer.sharedMesh)
                {
                    if (shouldExcludeObject != null && shouldExcludeObject(skinnedMeshRenderer.gameObject))
                        continue;

                    spatialHash.AddObject(skinnedMeshRenderer, skinnedMeshRenderer.bounds);
                }
            }
        }

        IEnumerator UpdateDynamicObjects()
        {
            while (true)
            {
                m_ChangedObjects.Clear();

                // TODO AE 9/21/16: Hook updates of new objects that are created
                foreach (var obj in spatialHash.allObjects)
                {
                    if (!obj)
                    {
                        m_ChangedObjects.Add(obj);
                        continue;
                    }

                    if (obj.transform.hasChanged)
                    {
                        m_ChangedObjects.Add(obj);
                        obj.transform.hasChanged = false;
                    }
                }

                foreach (var changedObject in m_ChangedObjects)
                {
                    spatialHash.RemoveObject(changedObject);

                    if (changedObject)
                        spatialHash.AddObject(changedObject, changedObject.bounds);
                }

                yield return null;
            }
        }

        public void AddObject(GameObject gameObject)
        {
            foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                spatialHash.AddObject(renderer, renderer.bounds);
            }
        }

        public void RemoveObject(GameObject gameObject)
        {
            foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                spatialHash.RemoveObject(renderer);
            }
        }

        public Bounds GetMaxBounds()
        {
            return spatialHash.GetMaxBounds();
        }
    }
}
