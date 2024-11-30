using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class ScoreData : MonoBehaviour
{

    public string userName;
    public int highestScore;

    public ScoreData(string userNameParamater, int highestScoreParamater)
    {
        userName = userNameParamater;
        highestScore = highestScoreParamater;
    }
}
   