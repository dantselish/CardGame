using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;
using Zenject;

namespace Dragging
{
    public class DragCard : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Ease movementEaseType;
        [SerializeField] private Ease rotationEaseType;
        [SerializeField] private float movementTweenTime;
        [SerializeField] private float rotationTweenTime;
        [SerializeField] private float handRotationX;
        [SerializeField] private float fieldRotationX;

        private int handLeaveLayerMask { get; set; }
        private int handReturnLayerMask { get; set; }
        private bool canMove { get; set; } = true;

        public CardStartDragState startState { get; private set; } = new CardStartDragState();

        public bool isInHand = true;
        public event Action<CardDropZone> placed;


        private void Awake()
        {
            handLeaveLayerMask = LayerMask.GetMask("HandLeaveZone", "Field");
            handReturnLayerMask = LayerMask.GetMask("HandLeaveZone");
        }

        private IEnumerator<float> blockMovementCoroutine(float duration)
        {
            canMove = false;
            yield return Timing.WaitForSeconds(duration);
            canMove = true;
        }

        public bool canUpdateCard() => canMove;

        public void rememberStartState()
        {
            startState.startDragPosition = transform.position;
            startState.startDragRotation = transform.rotation.eulerAngles;
            startState.startWasInHand = isInHand;
        }

        public bool tryLeaveHand(Transform camera_transform, out RaycastHit raycast_hit)
        {
            raycast_hit = default;
            if (!isInHand || !canMove)
                return false;

            Physics.Raycast(camera_transform.position, transform.position - camera_transform.position, out raycast_hit, 100.0f, handLeaveLayerMask);
            if (raycast_hit.transform?.gameObject.layer != LayerMask.NameToLayer("HandLeaveZone"))
                isInHand = false;

            return !isInHand;
        }

        public bool tryReturnToHand(Transform camera_transform, out RaycastHit raycast_hit)
        {
            raycast_hit = default;
            if (!canMove || isInHand)
                return false;

            Physics.Raycast(camera_transform.position, transform.position - camera_transform.position, out raycast_hit, 100.0f, handReturnLayerMask);
            bool returned = raycast_hit.collider != null;
            return raycast_hit.collider != null;
        }

        public void returnToHand()
        {
            isInHand = true;
            Timing.RunCoroutine(blockMovementCoroutine(movementTweenTime));
        }

        public void leaveHand()
        {
            isInHand = false;
            Timing.RunCoroutine(blockMovementCoroutine(movementTweenTime));
        }

        public Tween rotateCard(Vector3 end_rotation)
        {
            return getRotationTween(end_rotation);
        }

        public void moveCard(Vector3 end_position, bool immediate = false, bool block_movement = false)
        {
            if (block_movement)
                DOTween.Kill(transform);
            if (block_movement)
                canMove = false;
            var tween = getMovementTween(end_position, immediate ? -1.0f : movementTweenTime);
            tween.onComplete += () => {
                if (block_movement)
                    canMove = true;
            };
        }

        public Tween setHandRotation()
        {
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.x = handRotationX;
            return rotateCard(rotation);
        }

        public Tween setFieldRotation()
        {
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.x = fieldRotationX;
            return rotateCard(rotation);
        }

        public Tween getMovementTween(Vector3 end_position, float duration = -0.1f)
        {
            if (duration.Equals(-0.1f))
                duration = movementTweenTime;
            return transform.DOMove(end_position, duration).SetEase(movementEaseType);
        }

        public Tween getRotationTween(Vector3 end_rotation, float duration = -0.1f)
        {
            if (duration.Equals(-0.1f))
                duration = rotationTweenTime;
            return transform.DORotate(end_rotation, duration).SetEase(rotationEaseType);
        }

        public void returnToStartPosition()
        {
            transform.DOMove(startState.startDragPosition, 0.7f).SetEase(Ease.OutCubic);
            transform.DORotate(startState.startDragRotation, 0.7f);
        }

        public void placedOnZone(CardDropZone card_drop_zone)
        {
            placed?.Invoke(card_drop_zone);
        }
    }
}
