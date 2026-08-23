using UnityEngine;

[CreateAssetMenu(fileName = "SO_Minigame", menuName = "Scriptable Objects/SO_Minigame")]
public class SO_Minigame : ScriptableObject
{
    public MinigameData[] MinigameData;

    public MinigameData RandomizeMinigameData(string excludeMinigame)
    {
        MinigameData minigame;

        while (true)
        {
            int randomIdx = UnityEngine.Random.Range(0, MinigameData.Length);
            minigame = MinigameData[randomIdx];

            if (excludeMinigame != minigame.SceneName)
                break;
        }

        return minigame;
    }

}

[System.Serializable]
public class MinigameData
{
    public string SceneName = "";
}