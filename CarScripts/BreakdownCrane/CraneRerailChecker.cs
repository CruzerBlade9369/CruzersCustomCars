using DV;
using DV.CabControls;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace BreakdownCrane
{
    public class CraneRerailChecker
    {
        // Based on https://github.com/gercj187/ReRailRequirement/blob/main/ReRailRequirement/main.cs

        [HarmonyPatch(typeof(CommsRadioController), "Awake")]
        static class DetectGrabbed
        {
            static void Postfix(CommsRadioController __instance)
            {
                if (Main.settings.noFunction)
                {
                    if (!__instance.IsModeActivated<RerailController>())
                    {
                        __instance.ActivateMode<RerailController>();
                    }
                    return;
                }

                ItemBase itemBase = __instance.GetComponent<ItemBase>();
                if (itemBase == null)
                {
                    Debug.LogError("ItemBase is null, this shouldn't happen!");
                    return;
                }

                itemBase.Grabbed += _ => CheckRerailAvailability(__instance);
            }
        }

        private static void CheckRerailAvailability(CommsRadioController radio)
        {
            if (PlayerManager.PlayerTransform == null || CarSpawner.Instance == null)
            {
                return;
            }

            Vector3 playerPos = PlayerManager.PlayerTransform.position;
            float distLimitSqr = Main.settings.craneDistance * Main.settings.craneDistance;

            TrainCar? crane = FindValidCrane(distLimitSqr);

            if (crane == null)
            {
                radio.DeactivateMode<RerailController>();
                CraneDistanceWatcher.StopWatcher();
                return;
            }

            radio.ActivateMode<RerailController>();
            CraneDistanceWatcher.StartWatcher(radio, crane);
        }

        public static TrainCar? FindValidCrane(float distLimitSqr)
        {
            Vector3 playerPos = PlayerManager.PlayerTransform.position;

            return CarSpawner.Instance.AllCars
                .FirstOrDefault(car =>
                car != null &&
                car.carLivery.id == "Crane" &&
                ValidateCrane(car, distLimitSqr));
        }

        public static bool ValidateCrane(TrainCar crane, float distLimitsqr)
        {
            float sqrDist = (crane.transform.position - PlayerManager.PlayerTransform.position).sqrMagnitude;
            if (sqrDist > distLimitsqr) return false;

            if (crane.frontCoupler == null || !crane.frontCoupler.IsCoupled()) return false;

            TrainCar otherCar = crane.frontCoupler.coupledTo.train;
            if (otherCar == null || otherCar.carLivery.id != "CraneFlat") return false;

            if (otherCar.rearCoupler == null || !otherCar.rearCoupler.IsCoupled()) return false;
            if (otherCar.rearCoupler.coupledTo.train != crane) return false;

            return true;
        }
    }
}