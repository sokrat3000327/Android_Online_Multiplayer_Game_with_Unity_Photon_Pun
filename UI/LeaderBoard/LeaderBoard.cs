using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    public TextMeshProUGUI playerNameText, scoreText;




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDetails(string name, int score)
    {
        playerNameText.text = name;
        scoreText.text = score.ToString();
    }
}
