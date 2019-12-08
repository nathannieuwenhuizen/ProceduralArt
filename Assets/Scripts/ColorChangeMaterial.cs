using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeMaterial : MonoBehaviour
{
    public Material material;
    private Color currentColor;

    private float speed = 5f;
    // Start is called before the first frame update
    public void Setup(Material _mat)
    {
        material = _mat;
        currentColor = new Color(0.3f,0.9f,0.3f);
        material.SetColor("_Color", currentColor);

    }

    public void Pulse(float intensity = 0.3f)
    {
        //StopAllCoroutines();
        Color destColor = currentColor;
        currentColor = new Color(currentColor.r + intensity, currentColor.g + intensity, currentColor.b + intensity, currentColor.a);
        StartCoroutine(ToColor(destColor));
    }
    public void ChangeColor(Color destColor)
    {
        //StopAllCoroutines();
        StartCoroutine(ToColor(destColor));
    }

    private Color Lerp(Color cur, Color dest, float time)
    {
        return new Color(
            Mathf.Lerp(cur.r, dest.r, time), 
            Mathf.Lerp(cur.g, dest.g, time),
            Mathf.Lerp(cur.b, dest.b, time), 
            Mathf.Lerp(cur.a, dest.a, time));
    }
    IEnumerator ToColor(Color destColor)
    {
        while (Mathf.Abs(currentColor.r - destColor.r) > 0.1f)
        {
            currentColor = Lerp(currentColor, destColor, Time.deltaTime * speed);
            material.SetColor("_Color", currentColor);
            yield return new WaitForFixedUpdate();
        }
        currentColor = destColor;
        material.SetColor("_Color", currentColor);

    }

}
