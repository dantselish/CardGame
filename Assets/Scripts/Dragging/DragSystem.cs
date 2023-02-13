using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Dragging
{
    public class DragSystem : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float defaultCardYPosition;
        [SerializeField] private float draggedCardYPosition;

        private Hand hand;
        private DragCard dragCard { get; set; }
        private Vector3 mousePositionOffset { get; set; }

        private int dropZoneCastLayerMask { get; set; }

        public Camera mainCamera;

        private void Awake()
        {
            dropZoneCastLayerMask = LayerMask.GetMask("DropZone");
        }

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!dragCard)
                {
                    if (castRay(out RaycastHit raycast_hit))
                    {
                        dragCard = raycast_hit.collider.gameObject.GetComponentInParent<DragCard>();
                        if (!dragCard)
                            return;

                        dragCard.rememberStartState();
                        mousePositionOffset = raycast_hit.point - raycast_hit.collider.gameObject.transform.position;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
                releaseObject();

            updateCard();
        }

        [Inject]
        private void inject(Hand hand)
        {
            this.hand = hand;
        }

        private bool castRay(out RaycastHit raycast_hit)
        {
            Camera main_camera = mainCamera;

            Vector3 screen_mouse_pos_far = new Vector3(Input.mousePosition.x, Input.mousePosition.y, main_camera.farClipPlane);
            Vector3 screen_mouse_pos_near = new Vector3(Input.mousePosition.x, Input.mousePosition.y, main_camera.nearClipPlane);

            Vector3 world_mouse_pos_far = main_camera.ScreenToWorldPoint(screen_mouse_pos_far);
            Vector3 world_mouse_pos_near = main_camera.ScreenToWorldPoint(screen_mouse_pos_near);

            return Physics.Raycast(world_mouse_pos_near, world_mouse_pos_far - world_mouse_pos_near, out raycast_hit);
        }

        private void updateCard()
        {
            if (!dragCard)
                return;

            if (dragCard.isInHand)
            {
                Camera main_camera = mainCamera;
                Vector3 mouse_position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, main_camera.WorldToScreenPoint(dragCard.transform.position).z);
                Vector3 world_position = main_camera.ScreenToWorldPoint(mouse_position);
                if (dragCard.tryLeaveHand(main_camera.transform, out RaycastHit raycast_hit))
                {
                    dragCard.setFieldRotation();
                    mouse_position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.WorldToScreenPoint(raycast_hit.point).z);
                    world_position = mainCamera.ScreenToWorldPoint(mouse_position);
                    world_position.y = draggedCardYPosition;
                    Debug.Log($"Left hand {world_position}");
                    dragCard.moveCard(world_position);
                    dragCard.leaveHand();
                }
                else
                {
                    dragCard.moveCard(world_position);
                }
            }
            else
            {
                if (dragCard.tryReturnToHand(mainCamera.transform, out RaycastHit raycast_hit))
                {
                    Vector3 card_position = raycast_hit.point;
                    card_position.z *= 1.1f;
                    card_position.y *= 1.1f;
                    dragCard.setHandRotation();
                    dragCard.moveCard(card_position, true, true);
                    dragCard.returnToHand();
                    Debug.Log($"Returning to hand to {card_position.ToString()} {raycast_hit.collider.name}");
                }
                else
                {
                    Vector3 mouse_position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.WorldToScreenPoint(dragCard.transform.position).z);
                    Vector3 world_position = mainCamera.ScreenToWorldPoint(mouse_position);
                    world_position.y = draggedCardYPosition;

                    dragCard.moveCard(world_position);
                }
            }
        }

        private void releaseObject()
        {
            if (!dragCard)
                return;

            Physics.Raycast(dragCard.transform.position, Vector3.down, out RaycastHit raycast_hit, 10.0f, dropZoneCastLayerMask);

            CardDropZone card_drop_zone = null;
            if (raycast_hit.collider != null)
                card_drop_zone = raycast_hit.collider.gameObject.GetComponent<CardDropZone>();

            if (!card_drop_zone)
            {
                dragCard.returnToStartPosition();
                if (!dragCard.isInHand && dragCard.startState.startWasInHand)
                    dragCard.returnToHand();
            }
            else
            {
                Vector3 drop_zone_position = card_drop_zone.getCardPosition();
                drop_zone_position.y = defaultCardYPosition;
                dragCard.moveCard(drop_zone_position);
                dragCard.placedOnZone(card_drop_zone);
            }

            dragCard = null;
        }
    }

    public class CardStartDragState
    {
        public Vector3 startDragPosition { get; set; }
        public Vector3 startDragRotation { get; set; }
        public bool startWasInHand { get; set; }
    }
}
