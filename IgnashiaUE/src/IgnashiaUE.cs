using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Log = IgnashiaUE.src.Helpers.Logger.DebugLogger;

namespace IgnashiaUE.src
{
    class IgnashiaUE : MonoBehaviour
    {
        private void Start()
        {
            Log.Info("IgnashiaUE started");
        }
        private void Update()
        {
            Log.Info("IgnashiaUE updated");
        }
        private void OnGUI()
        {
            Log.Info("IgnashiaUE OnGUI");
        }
        private void OnDisable()
        {
            Log.Info("IgnashiaUE disabled");
        }
        private void OnDestroy()
        {
            Log.Info("IgnashiaUE destroyed");
        }
    }
}
