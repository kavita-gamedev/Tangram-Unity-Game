using System.Collections;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public Piece[] pieces;
    public GameObject winEffect;


    void Awake()
    {
        Instance = this;
        Camera.main.backgroundColor = new Color(0.8f, 1f, 0.9f);
    }

    public void CheckCompletion()
    {
        foreach (var piece in pieces)
        {
            if (!piece.locked)
                return;
        }

        PuzzleCompleted();
    }

    void PuzzleCompleted()
    {
        Debug.Log("Puzzle Completed!");

        if (winEffect)
            Instantiate(winEffect, Vector3.zero, Quaternion.identity);

        StartCoroutine(Celebration());
        AudioManager.Instance.PlaySFX(AudioManager.Instance.winClip);
    }

    IEnumerator Celebration()
    {
        foreach (var piece in pieces)
        {
            piece.transform.localScale *= 1.1f;
            yield return new WaitForSeconds(0.05f);
            piece.transform.localScale = Vector3.one;
        }
    }
}
