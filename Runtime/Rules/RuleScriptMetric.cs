using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    public interface IRuleScript<TResult>
    {
        TResult Calculate(IGameContext context);
    }

    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_SCRIPT_METRIC, menuName = MENU_PATH.RULE_SCRIPT_METRIC)]
    public class RuleScriptMetric : GameDataAsset<uint>, IRuleScript<float>
    {
        [Tooltip("A reference to a metric type that implements IRuleMetric<float>")]
        [SerializeReference, SubclassSelector] private IRuleScript<float> metric;

        public float Calculate(IGameContext context) => metric.Calculate(context);
    }
}