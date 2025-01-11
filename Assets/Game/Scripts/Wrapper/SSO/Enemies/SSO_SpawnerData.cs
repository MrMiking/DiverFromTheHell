using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SSO_SpawnerData", menuName = "ScriptableObject/SSO_SpawnerData")]
public class SSO_SpawnerData : ScriptableObject
{
    public List<SSO_WaveData> waves;
    public float wavesDelay;
}