using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptPanel : UIBase
{
    
    private void Awake()
    {
        Bind(UIEvent.PROMPT_MSG);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.PROMPT_MSG:
                PromptMsg msg = message as PromptMsg;
                PromptMessage(msg.Text, msg.Color);
                break;
            default:
                break;
        }
    }

    private Text txtPrompt;
    private CanvasGroup cg;
    
    [SerializeField] [Range(0, 3)] 
    private float showTime = 1f;
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        txtPrompt = transform.Find("txtPrompt").GetComponent<Text>();
        cg = transform.Find("txtPrompt").GetComponent<CanvasGroup>();

        cg.alpha = 0;
    }

    private void PromptMessage(string text, Color color)
    {
        txtPrompt.text = text;
        txtPrompt.color = color;
        cg.alpha = 0;
        timer = 0;

        // show animation
        StartCoroutine(PromptAnim());
    }

    /// <summary>
    /// design prompt panel animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator PromptAnim()
    {
        while (cg.alpha < 1f)
        {
            cg.alpha += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }

        while (timer < showTime)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
    }
}