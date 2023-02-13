using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DI.Signals;
using UnityEngine;
using Zenject;

namespace Dragging
{
    public class Hand : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int maxCardsCount;

        [Header("References")]
        [SerializeField] private Transform defaultCardPosition;
        [SerializeField] private List<DragCard> debugStartDragCards = new List<DragCard>();

        private DragCardsArray dragCardsArray;

        private void Awake()
        {
            dragCardsArray = new DragCardsArray(maxCardsCount, gameObject);
        }

        private void Start()
        {
            foreach (DragCard drag_card in debugStartDragCards)
            {
                addCard(drag_card, false);
            }
            rearrangeCards();
        }

        public void onCardDrawn(CardDrawnSignal card_drawn_signal)
        {
            addCard(card_drawn_signal.dragCard);
        }

        public void addCard(DragCard drag_card, bool rearrange = true)
        {
            dragCardsArray.addCard(drag_card);
            drag_card.placed += _ => removeCard(drag_card);
            if (rearrange)
                rearrangeCards();
        }

        private void removeCard(DragCard drag_card)
        {
            dragCardsArray.removeCard(drag_card);
            rearrangeCards();
        }

        private void rearrangeCards()
        {
            Sequence sequence = DOTween.Sequence();
            float middle_card_index = dragCardsArray.middleCardIndex();
            for (int i = 0; i <= dragCardsArray.lastCardIndex(); i++)
            {
                Vector3 position = defaultCardPosition.position;
                position.x += 2.0f * (i - middle_card_index);
                DragCard drag_card = dragCardsArray.cards[i];
                sequence.Join(drag_card.getMovementTween(position));
                sequence.Join(drag_card.setHandRotation());
            }
            sequence.Play();
        }

        private class DragCardsArray
        {
            public DragCard[] cards;


            public DragCardsArray(int max_cards_count, GameObject game_object)
            {
                cards = new DragCard[max_cards_count];
            }

            public void addCard(DragCard drag_card)
            {
                if (isMaxCards())
                    throw new OverflowException("Can not add cards when hand is full");

                cards[firstAvailablePosition()] = drag_card;
            }

            public void removeCard(DragCard drag_card)
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    if (drag_card == cards[i])
                    {
                        cards[i] = null;
                        moveCardsLeft(i);
                    }
                }
            }

            public float middleCardIndex()
            {
                return lastCardIndex() / 2.0f;
            }

            public bool isMaxCards()
            {
                return firstAvailablePosition() == -1;
            }

            public int firstAvailablePosition()
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    if (cards[i] == null)
                        return i;
                }

                return -1;
            }

            private void moveCardsLeft(int destination_index)
            {
                if (isMaxCards())
                    throw new OverflowException("Can't move cards left when full hand");

                int source_index = destination_index + 1;
                Array.Copy(cards, source_index, cards, destination_index, cards.Length - source_index );
                cards[^1] = null;
            }

            public int lastCardIndex()
            {
                int first_available_position = firstAvailablePosition();
                return first_available_position == -1 ? cards.Length - 1 : first_available_position - 1;
            }

            private bool containsCard(int index)
            {
                return cards[index] != null;
            }
        }
    }
}
