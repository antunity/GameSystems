using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    public interface IRuleMetric<TResult>
    {
        TResult Calculate(IGameContext context);
    }

    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_METRIC, menuName = MENU_PATH.RULE_METRIC)]
    public class RuleMetric : GameDataAsset<uint>, IRuleMetric<float>   
    {
        [Tooltip("A reference to a metric type that implements IRuleMetric<float>")]
        [SerializeReference, SubclassSelector] private IRuleMetric<float> metric;

        public float Calculate(IGameContext context) => metric.Calculate(context);
    }
}