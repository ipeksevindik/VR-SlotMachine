using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using System;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public List<SlotItem> Rows_1 = new List<SlotItem>();
    public List<SlotItem> Rows_2 = new List<SlotItem>();
    public List<SlotItem> Rows_3 = new List<SlotItem>();

    public List<SlotItem> SelectedItemsRow1 = new List<SlotItem>();
    public List<SlotItem> SelectedItemsRow2 = new List<SlotItem>();
    public List<SlotItem> SelectedItemsRow3 = new List<SlotItem>();

    public int StopedRowCount = 0;

    public int Final_prize;

    public TextMeshPro Prize_txt;
    public bool isStoped;
    private PhotonView photonView;

    public int rowspin1;
    public int rowspin2;
    public int rowspin3;

    AudioManager audioManager;
    public TrailRenderer line;
    TrailRenderer spawnedline;
    public ParticleSystem confetti;
    public GameObject winEffect;
    public GameObject winText;

    private void OnEnable()
    {
        Actions.OnHandlePulled += CallHandle;
    }

    private void OnDisable()
    {
        Actions.OnHandlePulled -= CallHandle;
    }

    private void Start()
    {
        audioManager = GetComponentInChildren<AudioManager>();
        photonView = GetComponent<PhotonView>();
        isStoped = true;
        Prize_txt = GetComponentInChildren<TextMeshPro>();

    }

    [ContextMenu("callhandle")]
    public void CallHandle()
    {
        isStoped = false;

        audioManager.PlayPullHandle();

        SelectRandomItemRow1();
        SelectRandomItemRow2();
        SelectRandomItemRow3();


        photonView.RPC(nameof(RowLoop), RpcTarget.AllBuffered);
    }

    public void SelectRandomItemRow1()
    {
        int row1_1 = UnityEngine.Random.Range(0, Rows_1.Count);
        int row1_2 = UnityEngine.Random.Range(0, Rows_1.Count);
        int row1_3 = UnityEngine.Random.Range(0, Rows_1.Count);
        int randomspin = SelectRandomRowSpin();

        if(row1_1 == row1_2 || row1_1 == row1_3 || row1_2 == row1_3)
        {
            SelectRandomItemRow1();
        }
        else
            photonView.RPC(nameof(SetItemIndex1), RpcTarget.AllBuffered, row1_1, row1_2, row1_3, randomspin);

    }
    public void SelectRandomItemRow2()
    {
        int row2_1 = UnityEngine.Random.Range(0, Rows_2.Count);
        int row2_2 = UnityEngine.Random.Range(0, Rows_2.Count);
        int row2_3 = UnityEngine.Random.Range(0, Rows_2.Count);
        int randomspin = SelectRandomRowSpin();

        if (row2_1 == row2_2 || row2_1 == row2_3 || row2_2 == row2_3)
        {
            SelectRandomItemRow2();
        }
        else
            photonView.RPC(nameof(SetItemIndex2), RpcTarget.AllBuffered, row2_1, row2_2, row2_3, randomspin);
    }
    public void SelectRandomItemRow3()
    {
        int row3_1 = UnityEngine.Random.Range(0, Rows_3.Count);
        int row3_2 = UnityEngine.Random.Range(0, Rows_3.Count);
        int row3_3 = UnityEngine.Random.Range(0, Rows_3.Count);
        int randomspin = SelectRandomRowSpin();

        if (row3_1 == row3_2 || row3_1 == row3_3 || row3_2 == row3_3)
        {
            SelectRandomItemRow3();
        }
        else
            photonView.RPC(nameof(SetItemIndex3), RpcTarget.AllBuffered, row3_1, row3_2, row3_3, randomspin);
    }

    [PunRPC]
    public void SetItemIndex1(int index1, int index2, int index3, int randomspin)
    {
        ResetSelectedData1();

        rowspin1 = randomspin;
        SelectedItemsRow1.Add(Rows_1[index1]);
        SelectedItemsRow1.Add(Rows_1[index2]);
        SelectedItemsRow1.Add(Rows_1[index3]);

    }
    [PunRPC]
    public void SetItemIndex2(int index1, int index2, int index3, int randomspin)
    {
        ResetSelectedData2();

        rowspin2 = randomspin;
        SelectedItemsRow2.Add(Rows_2[index1]);
        SelectedItemsRow2.Add(Rows_2[index2]);
        SelectedItemsRow2.Add(Rows_2[index3]);

    }
    [PunRPC]
    public void SetItemIndex3(int index1, int index2, int index3, int randomspin)
    {
        ResetSelectedData3();

        rowspin3 = randomspin;
        SelectedItemsRow3.Add(Rows_3[index1]);
        SelectedItemsRow3.Add(Rows_3[index2]);
        SelectedItemsRow3.Add(Rows_3[index3]);
    }
    public void ResetSelectedData1()
    {
        Final_prize = 0;
        Prize_txt.text = " ";
        // ResetRow Hep SelectedItems.Clear() dan önce çalýþsýn
        ResetRow(SelectedItemsRow1);
        SelectedItemsRow1.Clear();
    }
    public void ResetSelectedData2()
    {
        Final_prize = 0;
        Prize_txt.text = " ";
        ResetRow(SelectedItemsRow2);
        SelectedItemsRow2.Clear();
    }
    public void ResetSelectedData3()
    {
        Final_prize = 0;
        Prize_txt.text = " ";
        ResetRow(SelectedItemsRow3);
        SelectedItemsRow3.Clear();
    }
    public void CheckRowsStoped()
    {
        StopedRowCount++;
        if (StopedRowCount >= 3)
        {
            audioManager.PlayRowStoped();
            Prize();
            StopedRowCount = 0;
        }
    }

    public void Prize()
    {
        CheckLinePrize(SelectedItemsRow1, SelectedItemsRow2, SelectedItemsRow3);
        isStoped = true;
     

    }

    public void CheckLinePrize(List<SlotItem> list1, List<SlotItem> list2, List<SlotItem> list3)
    {
        CheckLines(list1[0], list2[0], list3[0]);
        CheckLines(list1[1], list2[1], list3[1]);
        CheckLines(list1[2], list2[2], list3[2]);
        CheckLines(list1[2], list2[1], list3[2]);
        CheckLines(list1[0], list2[1], list3[0]);
        CheckLines(list1[2], list2[1], list3[0]);
        CheckLines(list1[0], list2[1], list3[2]);
        CheckLines(list1[1], list2[0], list3[1]);
        CheckLines(list1[1], list2[2], list3[1]);
        
        Prize_txt.text = "Total Win: " + Final_prize.ToString() + "$";
    }

    public void CheckLines(SlotItem item1, SlotItem item2, SlotItem item3)
    {
        
        if (item1.ItemId == item2.ItemId)
        {
            if(item1.ItemId == item3.ItemId)
            {
                audioManager.PlayJackpot();
                Final_prize += item1.ItemPrize + item2.ItemPrize + item3.ItemPrize;
                //Prize_txt.text = "jackpot!! " + Final_prize.ToString() + "$";
                item1.GetComponent<PrizeEffect>().SpawnTrail(item1);
                item2.GetComponent<PrizeEffect>().SpawnTrail(item2);
                item3.GetComponent<PrizeEffect>().SpawnTrail(item3);

                JackpotLine(item1,item2, item3);
                ConfettiPlay();
                WinEffect();
            }
        }
    }

    [ContextMenu(nameof(Test))]
    public void Test()
    {
        ConfettiPlay();
        WinEffect();
    }


    public void JackpotLine(SlotItem item1, SlotItem item2, SlotItem item3)
    {
        spawnedline = Instantiate(line);

        spawnedline.transform.position = item1.patrolPoints[5].transform.transform.position;
        var sequence = DOTween.Sequence();

        sequence.Append(spawnedline.transform.DOLocalMove(item2.patrolPoints[5].transform.position, 0.4f));
        sequence.Append(spawnedline.transform.DOLocalMove(item3.patrolPoints[5].transform.position, 0.4f));

        sequence.SetLoops(-1, LoopType.Yoyo);

    }

    public void DestroyJackpotLine()
    {
        if (spawnedline != null)
            Destroy(spawnedline.gameObject);
    }


    [PunRPC]
    public void RowLoop()
    {
        audioManager.PlayRowMove();
        MoveNext(Rows_1, SelectedItemsRow1[0], SelectedItemsRow1[1], SelectedItemsRow1[2], rowspin1);
        MoveNext(Rows_2, SelectedItemsRow2[0], SelectedItemsRow2[1], SelectedItemsRow2[2], rowspin2);
        MoveNext(Rows_3, SelectedItemsRow3[0], SelectedItemsRow3[1], SelectedItemsRow3[2], rowspin3);
        
    }

    [PunRPC]
    public int SelectRandomRowSpin()
    {
        int index = UnityEngine.Random.Range(3, 5);
        return index;
    }

    public void ResetRow(List<SlotItem> list)
    {
        foreach (var item in list)
        {
            item.transform.DOLocalMoveY(4, 0);
            item.GetComponent<PrizeEffect>().DestroySpawnTrail();
            DestroyJackpotLine();
        }
    }

    public void MoveNext(List<SlotItem> list, SlotItem index1, SlotItem index2, SlotItem index3, int rowspin)
    {
        var sequence = DOTween.Sequence();
        float time= 0;
        foreach (SlotItem item in list)
        {
            sequence.Insert(time, item.transform.DOLocalMoveY(-2, 0.3f).SetEase(Ease.Linear));
            sequence.Append(item.transform.DOLocalMoveY(4, 0));
            time += 0.190f;
        }
       
        sequence.SetLoops(rowspin, LoopType.Restart).OnComplete(
            () => index1.transform.DOLocalMoveY(-0.2f, 0.2f).OnComplete(
                () => index2.transform.DOLocalMoveY(1.1f, 0.2f).OnComplete(
                    () => index3.transform.DOLocalMoveY(2.4f, 0.2f).OnComplete(
                        () => CheckRowsStoped()))));

    }

    async Task ConfettiPlay()
    {
        confetti.Play();

        await Task.Delay(3000);

        confetti.Stop();

    }
        
    public void WinEffect()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(winEffect.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 1f));
        sequence.Insert(2, winEffect.transform.DOScale(new Vector3(0, 0, 0), 1f));

        sequence.Insert(0, winText.transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 1f));
        sequence.Insert(2f, winText.transform.DOScale(new Vector3(0, 0, 0), 1f));

    }

}
