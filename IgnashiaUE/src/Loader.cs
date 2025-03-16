using IgnashiaUE.src.Helpers.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Log = IgnashiaUE.src.Helpers.Logger.DebugLogger;

namespace IgnashiaUE.src
{
    class Loader : MonoBehaviour
    {
        private static bool isDevMode = false;
        private static GameObject gameObject;

        public static void Load()
        {
            try
            {
                if (gameObject != null)
                {
                    Log.Warn("Loader already loaded");
                    return;
                }

                if (isDevMode)
                {
                    try
                    {
                        Log.Initialize(
                            logLevel: Log.LogLevel.TRACE,
                            useConsole: true,
                            useFile: true,
                            logFile: "IgnashiaUE_log.txt"
                        );
                        Log.Warn("Is in dev mode");
                    }
                    catch (IOException ioEx)
                    {
                        Log.Error($"Failed to initialize logging system: {ioEx.Message}");
                    }
                }

                Log.Info("Loading Assembly");

                gameObject = new GameObject("IgnashiaUE_Loader");
                UnityEngine.Object.DontDestroyOnLoad(gameObject);

                Log.Info("Assembly loaded successfully");
            }
            catch (TypeLoadException tle)
            {
                Log.Fatal($"Failed to load due to type issue: {tle.Message}");
            }
            catch (Exception ex)
            {
                Log.Fatal("Unexpected error while loading Assembly");
                Log.Debug($"Exception details: {ex}");
            }
        }


        public static void Unload()
        {
            try
            {
                if (gameObject == null)
                {
                    Log.Warn("No active Assembly to unload.");
                    return;
                }

                Log.Info("Unloading Assembly...");

                UnityEngine.Object.Destroy(gameObject);
                gameObject = null;

                Log.Info("Assembly unloaded successfully");

                Log.CloseConsole();
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Log.Fatal($"Failed to unload Assembly: {ex.Message}");
            }
        }

    }
}
