using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    [Serializable]
    [GameDataDrawer(GameDataLayout.Horizontal)]
    public struct ScriptRuleStruct : IRule, IUseGameDataDrawer
    {
        [Tooltip("A reference to a script type that implements IScriptResult<bool>")]
        [SerializeReference, SubclassSelector] private IScriptData<bool> script;

        [Tooltip("Enable to invert the result of the comparison")]
        [SerializeField] private bool invert;

        public RuleResult Evaluate(IGameContext context)
        {
            if (script == null)
                return RuleResult.BoolCheck(context.GetIndex(), context, 0f);

            var resultRaw = script.Calculate(context);
            var result = invert ? !resultRaw : resultRaw;
            return result ? RuleResult.Success() : RuleResult.BoolCheck(context.GetIndex(), context, resultRaw ? 1f : 0f);
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_SCRIPT, menuName = MENU_PATH.RULE_SCRIPT)]
    public class ScriptRule : Rule
    {
        [SerializeField] private ScriptRuleStruct rule;

        public override RuleResult Evaluate(IGameContext context) => rule.Evaluate(context);
    }
}