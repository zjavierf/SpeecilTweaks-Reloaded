using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SpeecilTweaks.Features.Performance
{
    public class ObstacleDetector : MonoBehaviour
    {
        public List<ObstacleController> intersectingObstacles { get; protected set; } = new List<ObstacleController>();
        protected BoxCollider boxCollider = null!;
        protected Rigidbody rigidbody = null!;

        [Inject]
        internal void Inject() { }

        protected void Awake()
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = Vector3.one * 0.05f;

            gameObject.layer = 9;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 11)
                return;

            Transform obstacle = other.transform.parent.parent;
            var controller = obstacle?.GetComponent<ObstacleController>();
            if (controller != null && !intersectingObstacles.Contains(controller))
            {
                intersectingObstacles.Add(controller);
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != 11)
                return;

            Transform obstacle = other.transform.parent.parent;
            var controller = obstacle?.GetComponent<ObstacleController>();
            if (controller != null)
            {
                intersectingObstacles.Remove(controller);
            }
        }
    }
}