using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class TransformFollower : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        [SerializeField]
        private Vector3 positionOffset;
        [SerializeField]
        private float distance;
        [SerializeField]
        private float angle;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 lookPosition = new Vector3(target.position.x, 0f, target.position.z) + positionOffset;

            Vector3 desiredPosition = lookPosition + Quaternion.Euler(angle, 0f, 0f) * new Vector3(0f, 0f, -distance);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 0.25f);
            transform.position = smoothedPosition;

            Quaternion desiredRotation = Quaternion.LookRotation(lookPosition - transform.position);
            Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, 0.25f);
            transform.rotation = smoothedRotation;
        }
    }
}
