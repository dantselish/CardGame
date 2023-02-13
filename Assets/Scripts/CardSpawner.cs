using Dragging;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private DragCard dragCardPrefab;


    public DragCard spawnCard(Vector3 start_position)
    {
        return Instantiate(dragCardPrefab, start_position, Quaternion.identity);
    }
}