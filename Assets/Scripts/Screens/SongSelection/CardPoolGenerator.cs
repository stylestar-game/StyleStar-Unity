using UnityEngine;

public class CardPoolGenerator : MonoBehaviour
{
    public GameObject SongCard;
    public GameObject FolderCard;
    public int AmountToPool;
    public bool CanExpand;
    public GameObject ParentObject;

    private void Awake()
    {
        Pools.SongCards = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.SongCards.SetPool(SongCard, AmountToPool, CanExpand, ParentObject);

        Pools.FolderCards = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.FolderCards.SetPool(FolderCard, AmountToPool, CanExpand, ParentObject);
    }
}
