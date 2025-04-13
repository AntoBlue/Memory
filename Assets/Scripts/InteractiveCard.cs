using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class InteractiveCard : MonoBehaviour
{
    bool selected;
    float fadeSpeed = 1;
    bool rotating = false;
    AudioSource audioSource;

    public delegate void ClickAction (InteractiveCard card, bool selected);
    public event ClickAction OnClicked;

    private string _imageName;

    public string imageName { get => _imageName; set => _imageName = value; }

    private void Start()
    {
        transform.rotation = Quaternion.Euler(0, 180, 0); //start flipped
        audioSource = GetComponent<AudioSource>();
    }

    private void OnMouseUp()
    {
        //swap card

        if (rotating) return;

        rotating = true;

        selected = !selected;

        StartCoroutine(RotateMe(Vector3.zero, 0.8f, selected));
    }

    public void ResetMe()
    {
        selected = false;
        StartCoroutine(RotateMe(Vector3.up * -180, 0.8f, selected));
    }

    IEnumerator RotateMe(Vector3 byAngles, float inTime, bool isSelected)
    {
        audioSource.Play();

        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(byAngles);

        for (var t = 0f; t <= 1; t += Time.deltaTime / inTime)
        {
            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }

        OnClicked(this, isSelected); //notify Manager card was selected

        rotating = false;


    }

    public bool Compare(InteractiveCard other)
    {
        return imageName == other.imageName;
    }

    internal void HideAndDestroy()
    {
        //animation fade, or script fade or shader fade

        var material = GetComponent<Renderer>().material;

        StartCoroutine(FadeAndHideCoroutine(material));
    }

    IEnumerator FadeAndHideCoroutine(Material mat)
    {
        while (mat.GetFloat("_Alpha") < 1)
        {
            var newAlpha = Mathf.MoveTowards(mat.GetFloat("_Alpha"), 1, fadeSpeed * Time.deltaTime);
            mat.SetFloat("Alpha", newAlpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}
