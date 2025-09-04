using UnityEngine;
using UnityModManagerNet;

namespace BreakdownCrane
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        public readonly string? version = Main.mod?.Info.Version;
        public int selectedLocationIndex = 0;

        private static string[] deliveryLocations = {"Harbor A-02-P", "Nearest player location"};

        [Draw("Enable logging")]
        public bool isLoggingEnabled =
#if DEBUG
            true;
#else
            false;
#endif

        [Draw("Furthest crane distance to player to enable rerailing", Min = 10f, Max = 50f)]
        public float craneDistance = 35f;

        [Draw("Disable crane rerail functionality (crane becomes purely cosmetic)")]
        public bool noFunction = false;

        [Draw("Deliver crane to selected location upon purchase")]
        public bool autoDeliver = false;

        public override void Save(UnityModManager.ModEntry entry)
        {
            Save(this, entry);
        }

        public void OnChange() { }

        public void DrawGUI(UnityModManager.ModEntry modEntry)
        {
            this.Draw(modEntry);
            DrawConfigs();
        }

        private void DrawConfigs()
        {
            GUILayout.BeginVertical(GUILayout.MinWidth(800), GUILayout.ExpandWidth(false));

            GUILayout.Label("Select location to deliver the crane to");

            GUILayout.BeginHorizontal(GUILayout.Width(350));
            UnityModManager.UI.PopupToggleGroup(ref selectedLocationIndex, deliveryLocations, "Available delivery locations");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}
