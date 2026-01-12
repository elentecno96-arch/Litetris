using UnityEngine;

namespace Game.SO
{
    [CreateAssetMenu(fileName = "NewPhase", menuName = "ScriptableObjects/PatternPhase")]
    public class PatternPhaseSO : ScriptableObject
    {
        [Header("Phase Info")]
        public string phaseName;
        public float duration;

        [Header("Pattern Unlock")]
        public int unlockPatternID;

        [Header("Difficulty Scaling")]
        public float startSpawnInterval;
        public float endSpawnInterval;

        public float startWarnTime = 1.2f;
        public float endWarnTime = 0.6f;
    }
}