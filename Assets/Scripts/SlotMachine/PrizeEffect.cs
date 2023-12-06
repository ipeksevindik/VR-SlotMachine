using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Drawing;


public class PrizeEffect : MonoBehaviour
{
    
    //public Transform[] patrolPoints;
    public TrailRenderer trail;

    TrailRenderer spawnedtrail;

    private void Start()
    {
        //trail = GetComponentInChildren<TrailRenderer>();
        //trail.sortingLayerName = "Trail";
       
    }

    public void SpawnTrail(SlotItem item)
    {
        spawnedtrail = Instantiate(trail);
        spawnedtrail.transform.position = item.patrolPoints[0].position;
        var sequence = DOTween.Sequence();

        sequence.Append(spawnedtrail.transform.DOMove(item.patrolPoints[1].transform.position, 0.4f));
        sequence.Append(spawnedtrail.transform.DOMove(item.patrolPoints[2].transform.position, 0.4f));
        sequence.Append(spawnedtrail.transform.DOMove(item.patrolPoints[3].transform.position, 0.4f));
        sequence.Append(spawnedtrail.transform.DOMove(item.patrolPoints[4].transform.position, 0.4f));

        sequence.SetLoops(-1, LoopType.Restart);

    }

    public void DestroySpawnTrail()
    {
        if(spawnedtrail!=null)
            Destroy(spawnedtrail.gameObject);
    }

}
