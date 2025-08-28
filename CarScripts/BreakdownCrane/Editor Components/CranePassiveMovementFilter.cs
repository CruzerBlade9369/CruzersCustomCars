using UnityEngine;

namespace BreakdownCrane.Editor
{
    [AddComponentMenu("Breakdown Crane/Components/Crane Passive Movement Filter")]
    public class CranePassiveMovementFilter : MonoBehaviour
    {
        [Tooltip("The reference transform to copy rotation from.")]
        public Transform source;

        [Header("Axis Filtering")]
        public bool copyX = true;
        public bool copyY = false;
        public bool copyZ = false;

        private void LateUpdate()
        {
            if (source == null)
                return;

            Vector3 sourceEuler = source.localEulerAngles;
            Vector3 myEuler = transform.localEulerAngles;

            if (copyX) myEuler.x = sourceEuler.x;
            if (copyY) myEuler.y = sourceEuler.y;
            if (copyZ) myEuler.z = sourceEuler.z;

            transform.localEulerAngles = myEuler;
        }
    }
}