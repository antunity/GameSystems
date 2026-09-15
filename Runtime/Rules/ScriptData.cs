using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    public interface IScriptData<TResult>
    {
        TResult Calculate(IGameContext context);
    }

    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.SCRIPT_DATA, menuName = MENU_PATH.SCRIPT_DATA)]
    public class ScriptData : GameDataAsset<uint>, IScriptData<float>
    {
        [Tooltip("A reference to a metric type that implements IScriptData<float>")]
        [SerializeReference, SubclassSelector] private IScriptData<float> metric;

        public float Calculate(IGameContext context) => metric.Calculate(context);
    }
}