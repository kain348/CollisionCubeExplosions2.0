using UnityEngine;

internal class Separator : MonoBehaviour
{
    private int maxRandom = 100;
    private int minRandom = 1;

    public bool ShouldSplit(int chanceToSplit)
    {
        Debug.Log("split chance - " + chanceToSplit);
        return chanceToSplit >= Random.Range(minRandom, maxRandom + 1);
    }
}