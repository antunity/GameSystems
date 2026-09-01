using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    [Serializable]
    [GameDataDrawer(GameDataLayout.Horizontal)]
    public struct BoolCompareStruct : IRule, IUseGameDataDrawer
    {
        [Tooltip("The source for the data to check")]
        [SerializeField] private GameDataSource source;

        [Tooltip("The data to check")]
        [SerializeField] private GameDataAsset<uint> data;

        [Tooltip("Enable to invert the result of the comparison")]
        [SerializeField] private bool invert;

        public GameDataSource Source => source;

        public IGameDataBase Data => data;

        public RuleResult Evaluate(IGameContext context)
        {
            var resultRaw = context.Resolve<bool>(source, data);
            var result = invert ? !resultRaw : resultRaw;
            return result ? RuleResult.Success() : RuleResult.BoolCheck(context.GetIndex(), data, resultRaw ? 1f : 0f);
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_BOOL_COMPARE, menuName = MENU_PATH.RULE_BOOL_COMPARE)]
    public class BoolCompare : Rule
    {
        [SerializeField] private BoolCompareStruct rule;

        public override RuleResult Evaluate(IGameContext context) => rule.Evaluate(context);
    }
}