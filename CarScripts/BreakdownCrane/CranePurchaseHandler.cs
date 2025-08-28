using CCL.Importer;
using CCL.Importer.Types;
using CCL.Importer.WorkTrains;
using DV.ThingTypes;
using DV.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BreakdownCrane
{
    public class CranePurchaseHandler : MonoBehaviour
    {
        private static string[] validDeliveryTracks = { "[Y]_[HB]_[A-02-P]" };
        private static List<Vector3> validDeliveryTrackLocations = new List<Vector3>();

        public static void Initialize()
        {
            WorkTrainPurchaseHandler.LiveryUnlocked += OnCranePurchased;
            WorldStreamingInit.LoadingFinished += FindValidTrackPositions;
        }

        private static void OnCranePurchased(CCL_CarVariant crane)
        {
            if (crane == null || crane.id != "Crane") return;

            CCL_CarVariant? craneFlat = null;
            foreach (var carType in CarManager.CustomCarTypes)
            {
                if (carType.id != "CraneFlat") continue;

                if (carType.Variants.Count() > 1)
                {
                    Debug.LogError($"{carType.id} cartype has more than 1 entries! This is not allowed!");
                    return;
                }

                craneFlat = carType.Variants.First();
                WorkTrainPurchaseHandler.WorkTrainLiveries.Add(craneFlat);
                WorkTrainPurchaseHandler.Unlock(craneFlat);

                break;
            }

            if (!Main.settings.autoDeliver)
            {
                return;
            }

            if (crane == null || craneFlat == null)
            {
                Debug.LogError("Unable to deliver crane! Either one of the vehicle is null!");
                return;
            }

            DeliverCraneAfterPurchase(crane, craneFlat);
            WorkTrainPurchaseHandler.SetAsSummoned(crane);
            WorkTrainPurchaseHandler.SetAsSummoned(craneFlat);
        }

        private static void DeliverCraneAfterPurchase(CCL_CarVariant crane, CCL_CarVariant craneFlat)
        {
            Vector3 spawnPos = PlayerManager.PlayerTransform.position;
            List<TrainCarLivery> liveries = [craneFlat, crane];

            if (validDeliveryTrackLocations.Count != 0)
            {
                if (Main.settings.selectedLocationIndex < validDeliveryTrackLocations.Count)
                {
                    spawnPos = validDeliveryTrackLocations[Main.settings.selectedLocationIndex];
                }
                else if (Main.settings.selectedLocationIndex > validDeliveryTrackLocations.Count ||
                         Main.settings.selectedLocationIndex < 0)
                {
                    Debug.LogError("Invalid delivary location! Delivering to nearest player location instead");
                }
            }
            else
            {
                Debug.LogError("No valid delivery locations detected! Delivering to nearest player location instead");
            }

            CarSpawner.Instance.SpawnCarTypesOnClosestTrack(liveries, spawnPos, [false, false], false, true, uniqueSpawnedCars: true);
            Main.DebugLog("Crane successfully delivered");
        }

        private static void FindValidTrackPositions()
        {
            HashSet<RailTrack> railHashes = new(SingletonBehaviour<RailTrackRegistryBase>.Instance.AllTracks);

            if (railHashes.Count <= 0)
            {
                Debug.LogError("No track exists in the world! can't do lookup!");
                return;
            }

            foreach (string trackName in validDeliveryTracks)
            {
                foreach (RailTrack track in railHashes)
                {
                    if (track.name != trackName) continue;

                    validDeliveryTrackLocations.Add(track.gameObject.transform.position);
                    break;
                }
            }

            validDeliveryTrackLocations = validDeliveryTrackLocations.Distinct().ToList();

            if (Main.settings.isLoggingEnabled)
            {
                Debug.Log("Finished finding valid track positions:");
                foreach (var trackPos in validDeliveryTrackLocations)
                {
                    Debug.Log($"{trackPos.x}, {trackPos.y}, {trackPos.z}");
                }
            }
        }
    }
}
