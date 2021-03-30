using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

namespace Packages.realityflow_package.Runtime.scripts
{
    public class KeyboardTracker : MonoBehaviour
    {
        //public static KeyboardTracker Instance { get; private set; }
        public string Value;
        public string ShiftValue;

        private Text m_Text;
        public TMP_Text  textbox;
        public TMP_InputField inputField;
        private string text;

        private Button m_Button;

        private void Awake()
        {
            m_Button = GetComponent<Button>();
        }

        // Start is called before the first frame update
        void Start()
        {
            m_Text = gameObject.GetComponentInChildren<Text>();
            m_Text.text = Value;

            m_Button.onClick.RemoveAllListeners();
            m_Button.onClick.AddListener(delegate {
                AppendValue(this.Value);
            });
        }

        // Update is called once per frame
        void Update()
        {
            
            
        }

        public void AppendValue(string valueKey)
        {
            textbox.text += valueKey;
            inputField.text += valueKey;

            Debug.Log( textbox.text.ToString());
        }
    }
}