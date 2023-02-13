using UnityEngine;
using Zenject;

public class GameCheats : MonoBehaviour
{
    private Deck deck;


    [Inject]
    private void inject(Deck deck)
    {
        this.deck = deck;
    }

    public void drawCard()
    {
        deck.drawnCard();
    }
}