using DV;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BreakdownCrane
{
    public class CraneDistanceWatcher : MonoBehaviour
    {
        private static CraneDistanceWatcher instance;

        private CommsRadioController radio;
        private TrainCar? currentCrane;
        private Coroutine? watcherCoroutine;

        private static readonly float CHECK_INTERVAL = 0.25f;

        public static void StartWatcher(CommsRadioController radio, TrainCar crane)
        {
            if (instance == null)
            {
                var go = new GameObject("CraneDistanceWatcher");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<CraneDistanceWatcher>();
            }

            instance.radio = radio;
            instance.currentCrane = crane;

            if (instance.watcherCoroutine != null)
                instance.StopCoroutine(instance.watcherCoroutine);

            float distLimitSqr = Main.settings.craneDistance * Main.settings.craneDistance;
            instance.watcherCoroutine = instance.StartCoroutine(instance.WatchDistanceLoop(distLimitSqr));
        }

        public static void StopWatcher()
        {
            if (instance == null) return;

            if (instance.watcherCoroutine != null)
            {
                instance.StopCoroutine(instance.watcherCoroutine);
                instance.watcherCoroutine = null;
            }
        }

        private IEnumerator WatchDistanceLoop(float distLimitSqr)
        {
            while (true)
            {
                if (PlayerManager.PlayerTransform == null || radio == null)
                {
                    StopWatcher();
                    yield break;
                }

                if (currentCrane == null || !CraneRerailChecker.ValidateCrane(currentCrane, distLimitSqr))
                {
                    currentCrane = CraneRerailChecker.FindValidCrane(distLimitSqr);

                    if (currentCrane == null)
                    {
                        radio.DeactivateMode<RerailController>();
                        StopWatcher();
                        yield break;
                    }
                }

                yield return new WaitForSeconds(CHECK_INTERVAL);
            }
        }
    }
}
