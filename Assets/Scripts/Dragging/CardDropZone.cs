using System;
using UnityEngine;

namespace Dragging
{
    [RequireComponent(typeof(Collider))]
    public class CardDropZone : MonoBehaviour
    {
        public Vector3 getCardPosition() => transform.position;
    }
}
