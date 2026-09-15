using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    [Serializable]
    [GameDataDrawer(GameDataLayout.Horizontal)]
    public struct DataRuleStruct : IRule, IUseGameDataDrawer
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
    [CreateAssetMenu(fileName = FILE_NAME.RULE_DATA, menuName = MENU_PATH.RULE_DATA)]
    public class DataRule : Rule
    {
        [SerializeField] private DataRuleStruct rule;

        public override RuleResult Evaluate(IGameContext context) => rule.Evaluate(context);
    }
}