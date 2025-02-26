using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class RobotValues : MonoBehaviour
{
    public bool enableSendSignal;
    public int index;
    public float sendSignalDelay = 0.25f;
    public float angleThreshold = 1;
    public List<RotationLimitHinge> listRotationLimitHinge;
    public List<int> listAxis;

    public System.Action<RobotAngles, int> OnSendSignal;

    RobotAngles lastAngles;

    void Start()
    {
        listAxis = new List<int>();
        for (int i = 0; i < listRotationLimitHinge.Count; i++)
        {
            listAxis.Add(0);
        }
        StartCoroutine(SendSignal());
    }

    void Update()
    {
        for (int i = 0; i < listRotationLimitHinge.Count; i++)
        {
            listAxis[i] = Mathf.FloorToInt(listRotationLimitHinge[i].lastAngle);
        }
    }

    IEnumerator SendSignal(){
        yield return new WaitForSeconds(KRGameManager.instance.autoStartDelay);
        if(KRGameManager.instance.autoStart){
            enableSendSignal = true;
        }
        while (true)
        {
            yield return new WaitForSeconds(sendSignalDelay);
            RobotAngles angles = new RobotAngles(); 
            angles.SetAngles(-listAxis[0], listAxis[1], listAxis[2], listAxis[3], listAxis[4], listAxis[5]);

            if(lastAngles != null && CompareAngles(lastAngles, angles, angleThreshold)){
                continue;
            }

            if (enableSendSignal)   
            {
                OnSendSignal?.Invoke(angles, index + 1);    //API從1開始
            }
            lastAngles = angles;
        }
    }

    bool CompareAngles(RobotAngles angles1, RobotAngles angles2, float threshold){
        return Mathf.Abs(angles1.J1 - angles2.J1) < threshold && Mathf.Abs(angles1.J2 - angles2.J2) < threshold && Mathf.Abs(angles1.J3 - angles2.J3) < threshold && Mathf.Abs(angles1.J4 - angles2.J4) < threshold && Mathf.Abs(angles1.J5 - angles2.J5) < threshold && Mathf.Abs(angles1.J6 - angles2.J6) < threshold;
    }
}

[System.Serializable]
public class RobotAngles
{
    // 公共字段用於 JSON 序列化
    public int J1;
    public int J2;
    public int J3;
    public int J4;
    public int J5;
    public int J6;

    // 驗證並設置所有角度
    public void SetAngles(int j1, int j2, int j3, int j4, int j5, int j6)
    {
        J1 = Mathf.Clamp(j1, -170, 170);
        J2 = Mathf.Clamp(j2, -150, 95);
        J3 = Mathf.Clamp(j3, -85, 185);
        J4 = Mathf.Clamp(j4, -190, 190);
        J5 = Mathf.Clamp(j5, -135, 135);
        J6 = Mathf.Clamp(j6, -360, 360);
    }

    // 驗證單個角度的方法
    public void ValidateAngles()
    {
        J1 = Mathf.Clamp(J1, -170, 170);
        J2 = Mathf.Clamp(J2, -150, 95);
        J3 = Mathf.Clamp(J3, -85, 185);
        J4 = Mathf.Clamp(J4, -190, 190);
        J5 = Mathf.Clamp(J5, -135, 135);
        J6 = Mathf.Clamp(J6, -360, 360);
    }
}