using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_GROUP, menuName = MENU_PATH.RULE_GROUP)]
    public class GroupRule : Rule
    {
        [Tooltip("The rules to evaluate")]
        [SerializeField] private GameDataRegistry<Rule> rules;

        public override RuleResult Evaluate(IGameContext context)
        {
            foreach (var rule in rules)
            {
                var result = rule.Evaluate(context);
                if (result.IsSuccess)
                    return result;
            }

            return RuleResult.Success();
        }
    }
}