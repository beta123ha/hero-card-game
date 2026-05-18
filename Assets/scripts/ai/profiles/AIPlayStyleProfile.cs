using UnityEngine;

[CreateAssetMenu(fileName = "NewAIPlayStyleProfile", menuName = "AI/Play Style Profile")]
public class AIPlayStyleProfile : ScriptableObject
{
    [Header("Identity")]
    public AIPlayStyle playStyle;

    [Header("Deck Scoring Weights")]
    public float attackWeight = 3f;
    public float defenseWeight = 3f;
    public float healthWeight = 3f;
    public float comboWeight = 3f;
    public float tacticSynergyWeight = 3f;

    [Header("Terrain Scoring Weights")]
    public float terrainSynergyWeight = 3f;
    public float counterPlayerWeight = 3f;

    [Header("Battle Behavior")]
    public float directDamageWeight = 3f;
    public float survivalWeight = 3f;
    public float finishEnemyWeight = 5f;
    public float riskTakingWeight = 3f;

    [Header("Randomness")]
    public float randomnessWeight = 1f;
}