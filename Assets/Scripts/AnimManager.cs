using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimManager : MonoBehaviour
{
    public bool animRobot = false;
    public Animator animatorL;
    public Animator animatorR;
    public RobotValues robotValuesA;
    public RobotValues robotValuesB;
    public Text TXT_CurrentClipName;

    int currentLoopIndex = 1;
    int LoopIndexCount = 3;

    void Start()
    {
        StartAnimLoop();
    }

    void StartAnimLoop(){
        StartCoroutine(AnimLoop());
    }

    IEnumerator AnimLoop(){
        while(true){
            if(animRobot){
                AnimRandom();
            } else {
                AnimStop();
            }

            if(TXT_CurrentClipName) TXT_CurrentClipName.text = $"Clip: {GetPlayingClipName(animatorL)}";
            yield return new WaitForSeconds(0.5f);
        }
    }

    void AnimRandom(){   
        if(GetPlayingClipName(animatorL).Contains("Loop") && GetPlayingClipName(animatorR).Contains("Loop"))
        {
            int randomTrigger = Random.Range(1, 11);
            animatorL.SetTrigger($"{randomTrigger}");
            animatorR.SetTrigger($"{randomTrigger}");

            int segment = KRGameManager.instance.showManager.currentSegmentIndex;
            Debug.Log($"Segment: {segment+1},Use Trigger: {randomTrigger} " + System.DateTime.Now.ToString("HH:mm:ss"));
        }
    }

    void AnimStop(){
        if(!GetPlayingClipName(animatorL).Contains("Loop") || !GetPlayingClipName(animatorR).Contains("Loop"))
        {
            animatorL.SetTrigger($"0{currentLoopIndex}");
            animatorR.SetTrigger($"0{currentLoopIndex}");
        }
    }

    public void IncreaseLoopIndex(){
        currentLoopIndex = (currentLoopIndex + 1) % LoopIndexCount;
    }

    public void BecomeManual(){
        animatorL.enabled = false;
        animatorR.enabled = false;
        robotValuesA.enableSendSignal = true;
        robotValuesB.enableSendSignal = true;
    }

    string GetPlayingClipName(Animator animator){
        var m_CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        var m_ClipName = m_CurrentClipInfo[0].clip.name;
        return m_ClipName;
    }
}

