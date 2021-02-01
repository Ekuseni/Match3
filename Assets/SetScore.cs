using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScore : MonoBehaviour
{
    public IntVariable score;

    private void OnEnable()
    {
        score.Value = 0;
    }

    public void Set()
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = score.Value.ToString();
    }
}
