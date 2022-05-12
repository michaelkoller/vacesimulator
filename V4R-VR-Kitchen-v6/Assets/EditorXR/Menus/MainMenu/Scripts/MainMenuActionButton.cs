﻿using System;
using UnityEditor.Experimental.EditorVR;
using UnityEngine;
using UnityEngine.UI;

#if INCLUDE_TEXT_MESH_PRO
using TMPro;
#endif

namespace UnityEditor.Experimental.EditorVR.Menus
{
    class MainMenuActionButton : MonoBehaviour
    {
        [SerializeField]
        Button m_Button;

        [SerializeField]
        Sprite m_Icon;

#if INCLUDE_TEXT_MESH_PRO
        [SerializeField]
        TextMeshProUGUI m_NameText;
#endif

        public Func<Action, bool> buttonPressed { get; set; }
    }
}
