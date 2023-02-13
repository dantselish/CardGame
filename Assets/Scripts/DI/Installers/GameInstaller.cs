using System;
using DI.Signals;
using Dragging;
using UnityEngine;
using Zenject;

namespace DI.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private Hand handPrefab;
        [SerializeField] private Deck deckPrefab;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Hand hand_go = FindObjectOfType<Hand>();
            if (!hand_go)
                hand_go = Container.InstantiatePrefabForComponent<Hand>(handPrefab);
            Container.BindInstance(hand_go).AsSingle().NonLazy();

            Deck deck_go = FindObjectOfType<Deck>();
            if (!deck_go)
                deck_go = Container.InstantiatePrefabForComponent<Deck>(deckPrefab);
            Container.BindInstance(deck_go).AsSingle().NonLazy();

            Container.DeclareSignal<CardDrawnSignal>().WithId(0);
            Container.BindSignal<CardDrawnSignal>().WithId(0).ToMethod<Hand>(hand => hand.onCardDrawn).FromResolve();
        }
    }
}
