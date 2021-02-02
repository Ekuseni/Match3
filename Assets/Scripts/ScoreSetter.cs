using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSetter : MonoBehaviour
{
    TMPro.TextMeshProUGUI scoreText;
    public IntVariable score;

    private void Start()
    {
        scoreText = GetComponent<TMPro.TextMeshProUGUI>();
        score.Value = 0;
    }

    public void UpdateScore()
    {
        scoreText.text = score.Value.ToString();
    }
}
