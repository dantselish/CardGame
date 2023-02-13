using Dragging;

namespace DI.Signals
{
    public class CardDrawnSignal
    {
        public DragCard dragCard { get; private set; }


        public CardDrawnSignal(DragCard drag_card)
        {
            dragCard = drag_card;
        }
    }
}
