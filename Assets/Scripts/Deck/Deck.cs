using DI.Signals;
using Dragging;
using UnityEngine;
using Zenject;

public class Deck : MonoBehaviour
{
    [SerializeField] private Transform cardDrawTransform;
    [SerializeField] private CardSpawner cardSpawner;

    private SignalBus signalBus;


    [Inject]
    private void inject(SignalBus signal_bus)
    {
        signalBus = signal_bus;
    }

    public void drawnCard()
    {
        DragCard drag_card = spawnCard();
        signalBus.FireId(0, new CardDrawnSignal(drag_card));
    }

    private DragCard spawnCard()
    {
        
        return cardSpawner.spawnCard(cardDrawTransform.position);
    }
}
