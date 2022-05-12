﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.EditorVR.Core;
using UnityEditor.Experimental.EditorVR.Utilities;
using UnityEditor.Experimental.EditorVR.Workspaces;
using UnityEngine;

namespace UnityEditor.Experimental.EditorVR.Modules
{
    sealed class WorkspaceModule : MonoBehaviour, ISystemModule, IConnectInterfaces, ISerializePreferences
    {
        [Serializable]
        class Preferences
        {
            [SerializeField]
            List<WorkspaceLayout> m_WorkspaceLayouts = new List<WorkspaceLayout>();

            public List<WorkspaceLayout> workspaceLayouts { get { return m_WorkspaceLayouts; } }
        }

        [Serializable]
        class WorkspaceLayout
        {
            [SerializeField]
            string m_Name;
            [SerializeField]
            Vector3 m_LocalPosition;
            [SerializeField]
            Quaternion m_LocalRotation;
            [SerializeField]
            Bounds m_ContentBounds;
            [SerializeField]
            string m_PayloadType;
            [SerializeField]
            string m_Payload;

            public string name
            {
                get { return m_Name; }
                set { m_Name = value; }
            }

            public Vector3 localPosition
            {
                get { return m_LocalPosition; }
                set { m_LocalPosition = value; }
            }

            public Quaternion localRotation
            {
                get { return m_LocalRotation; }
                set { m_LocalRotation = value; }
            }

            public Bounds contentBounds
            {
                get { return m_ContentBounds; }
                set { m_ContentBounds = value; }
            }

            public string payloadType
            {
                get { return m_PayloadType; }
                set { m_PayloadType = value; }
            }

            public string payload
            {
                get { return m_Payload; }
                set { m_Payload = value; }
            }
        }

        internal static readonly Vector3 DefaultWorkspaceOffset = new Vector3(0, -0.15f, 0.4f);
        internal static readonly Quaternion DefaultWorkspaceTilt = Quaternion.AngleAxis(-45, Vector3.right);

        internal List<IWorkspace> workspaces { get { return m_Workspaces; } }

        readonly List<IWorkspace> m_Workspaces = new List<IWorkspace>();
        readonly List<IInspectorWorkspace> m_Inspectors = new List<IInspectorWorkspace>();

        internal event Action<IWorkspace> workspaceCreated;
        internal event Action<IWorkspace> workspaceDestroyed;

        internal static List<Type> workspaceTypes { get; private set; }

        internal Transform leftRayOrigin { private get; set; }
        internal Transform rightRayOrigin { private get; set; }

        internal bool preserveWorkspaces { get; set; }
        internal Type[] HiddenTypes { private get; set; }

        static WorkspaceModule()
        {
            workspaceTypes = ObjectUtils.GetImplementationsOfInterface(typeof(IWorkspace)).ToList();
        }

        void Awake()
        {
            preserveWorkspaces = true;
        }

        void OnDestroy()
        {
            while (m_Workspaces.Count > 0)
                ObjectUtils.Destroy(m_Workspaces[0].transform.gameObject);
        }

        public object OnSerializePreferences()
        {
            if (!preserveWorkspaces)
                return null;

            var preferences = new Preferences();
            var workspaceLayouts = preferences.workspaceLayouts;
            foreach (var workspace in workspaces)
            {
                var layout = new WorkspaceLayout();
                layout.name = workspace.GetType().FullName;
                layout.localPosition = workspace.transform.localPosition;
                layout.localRotation = workspace.transform.localRotation;
                layout.contentBounds = workspace.contentBounds;

                var serializeWorkspace = workspace as ISerializeWorkspace;
                if (serializeWorkspace != null)
                {
                    var payload = serializeWorkspace.OnSerializeWorkspace();
                    layout.payloadType = payload.GetType().FullName;
                    layout.payload = JsonUtility.ToJson(payload);
                }

                workspaceLayouts.Add(layout);
            }

            return preferences;
        }

        public void OnDeserializePreferences(object obj)
        {
            if (!preserveWorkspaces)
                return;

            var preferences = (Preferences)obj;

            foreach (var workspaceLayout in preferences.workspaceLayouts)
            {
                var layout = workspaceLayout;
                var workspaceType = Type.GetType(workspaceLayout.name);
                if (workspaceType != null)
                {
                    if (HiddenTypes.Contains(workspaceType))
                        continue;

                    CreateWorkspace(workspaceType, workspace =>
                    {
                        workspace.transform.localPosition = layout.localPosition;
                        workspace.transform.localRotation = layout.localRotation;
                        workspace.contentBounds = layout.contentBounds;

                        var serializeWorkspace = workspace as ISerializeWorkspace;
                        if (serializeWorkspace != null)
                        {
                            var payload = JsonUtility.FromJson(layout.payload, Type.GetType(layout.payloadType));
                            serializeWorkspace.OnDeserializeWorkspace(payload);
                        }
                    });
                }
            }
        }

        internal void CreateWorkspace(Type t, Action<IWorkspace> createdCallback = null)
        {
            if (!typeof(IWorkspace).IsAssignableFrom(t))
                return;

            // HACK: MiniWorldWorkspace is not working in single pass yet
#if UNITY_EDITOR
            if (t == typeof(MiniWorldWorkspace) && PlayerSettings.stereoRenderingPath != StereoRenderingPath.MultiPass)
            {
                Debug.LogWarning("The MiniWorld workspace is not working on single pass, currently.");
                return;
            }
#endif

            var cameraTransform = CameraUtils.GetMainCamera().transform;

            var workspace = (IWorkspace)ObjectUtils.CreateGameObjectWithComponent(t, CameraUtils.GetCameraRig(), false);
            m_Workspaces.Add(workspace);
            workspace.destroyed += OnWorkspaceDestroyed;
            this.ConnectInterfaces(workspace);

            var evrWorkspace = workspace as Workspace;
            if (evrWorkspace != null)
            {
                evrWorkspace.leftRayOrigin = leftRayOrigin;
                evrWorkspace.rightRayOrigin = rightRayOrigin;
            }

            //Explicit setup call (instead of setting up in Awake) because we need interfaces to be hooked up first
            workspace.Setup();

            var offset = DefaultWorkspaceOffset;
            offset.z += workspace.vacuumBounds.extents.z;

            var workspaceTransform = workspace.transform;
            workspaceTransform.position = cameraTransform.TransformPoint(offset);
            ResetRotation(workspace, cameraTransform.forward);

            if (createdCallback != null)
                createdCallback(workspace);

            if (workspaceCreated != null)
                workspaceCreated(workspace);
        }

        void OnWorkspaceDestroyed(IWorkspace workspace)
        {
            m_Workspaces.Remove(workspace);

            this.DisconnectInterfaces(workspace);

            if (workspaceDestroyed != null)
                workspaceDestroyed(workspace);
        }

        internal void ResetWorkspaceRotations()
        {
            var cameraTransform = CameraUtils.GetMainCamera().transform;
            foreach (var ws in workspaces)
            {
                var forward = (ws.transform.position - cameraTransform.position).normalized;
                ResetRotation(ws, forward);
            }
        }

        static void ResetRotation(IWorkspace workspace, Vector3 forward)
        {
            workspace.transform.rotation = Quaternion.LookRotation(forward) * DefaultWorkspaceTilt;
        }

        internal void UpdateInspectors(GameObject obj = null, bool fullRebuild = false)
        {
            foreach (var inspector in m_Inspectors)
            {
                inspector.UpdateInspector(obj, fullRebuild);
            }
        }

        public void AddInspector(IInspectorWorkspace inspectorWorkspace)
        {
            m_Inspectors.Add(inspectorWorkspace);
        }

        public void RemoveInspector(IInspectorWorkspace inspectorWorkspace)
        {
            m_Inspectors.Remove(inspectorWorkspace);
        }
    }
}
