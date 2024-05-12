using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogBox : MonoBehaviour
{
    // Start is called before the first frame update
    string header = ""; public void SetHeader(string newHeader) { header = newHeader; headerText.text = header; }
    string body = ""; public void SetBody(string newBody) { body = newBody; charactersRevealed = 0; bodyText.text = string.Empty; SetVisible(true); }
    int charactersRevealed = 0;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI bodyText;
    Counter printTextCounter = new Counter(0.05f);
    Counter waitToFadeCounter = new Counter(5f);
    Counter fadeCounter = new Counter(2f);
    SpriteRenderer render;
    void Start()
    {
        SetupDialogBox();
    }
    void SetupDialogBox()
    {
        float height = Screen.height * 0.003f;
        float width = Screen.width * 0.0085f;
        transform.localScale = new Vector3(width / 16.80f, height / 4.32f, 1f);
        transform.position = Vector3.up * Screen.height * 0.00325f;
        transform.SetParent(Camera.main.transform);
        render = GetComponent<SpriteRenderer>();
        headerText.fontSize = Screen.height / 18f;
        bodyText.fontSize = Screen.height / 20f;
        bodyText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * 0.7f);
        headerText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * 0.85f);
        bodyText.rectTransform.position = Vector3.up * Screen.height * 0.775f + Vector3.right * Screen.width * 0.5f;
        headerText.rectTransform.position = Vector3.up * Screen.height * 0.925f + Vector3.right * Screen.width * 0.5f;
        SetVisible(false);
        //bodyText.rectTransform.position = Vector3.up * Screen.height * 0.325f;
        //Vector3.up * Screen.height * 0.325f;headerText.rectTransform.position = Vector3.up * Screen.height * 0.375f;
        //render.enabled = false;
    }
    private void FixedUpdate()
    {
        if(charactersRevealed < body.Length) 
        {
            if (printTextCounter.UpdateCounter(Time.fixedDeltaTime))
            {
                bodyText.text += body[charactersRevealed];
                charactersRevealed++;
                printTextCounter.ResetCounter();
            }
        }
        else
        {
            if (render.enabled)
            {
                if (waitToFadeCounter.UpdateCounter(Time.fixedDeltaTime))
                {
                    waitToFadeCounter.ResetCounter();
                    SetVisible(false);
                }
            }
        }
    }
    void SetVisible(bool b)
    {
        render.enabled = b;
        bodyText.enabled = b;
        headerText.enabled = b;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
