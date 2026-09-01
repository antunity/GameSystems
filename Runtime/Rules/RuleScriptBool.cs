using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_SCRIPT_BOOL, menuName = MENU_PATH.RULE_SCRIPT_BOOL)]
    public class RuleScriptBool : GameDataAsset<uint>, IRuleScript<bool>
    {
        [Tooltip("A reference to a metric type that implements IRuleMetric<bool>")]
        [SerializeReference, SubclassSelector] private IRuleScript<bool> metric;

        public bool Calculate(IGameContext context) => metric.Calculate(context);
    }
}