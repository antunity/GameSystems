using System;
using antunity.GameData;
using antunity.GameSystems.Rules;

namespace antunity.GameSystems
{
    public enum GameDataSource
    {
        Instigator,     // e.g. an external trigger for an action, e.g. a customer in a vendor transaction
        Subject,        // e.g. ability, item, ability
        Environment,    // e.g. ability book, inventory owner
        Context,        // e.g. the context containing all of the above
    }

    public interface IGameDataReader { }

    public interface IGameDataReader<in TIndex, out TData> : IGameDataReader where TIndex : struct where TData : IGameDataBase
    {
        TData QueryData(TIndex index);
    }

    public interface IGameDataReader<out TResult> : IGameDataReader where TResult : struct
    {
        TResult QueryProperty(IGameDataBase gameData = null);
    }

    public interface IGameDataMutator { }

    public interface IGameDataMutator<TResult> : IGameDataMutator where TResult : struct
    {
        void SetProperty(IGameDataBase gameData, TResult value);
    }

    public interface IGameDataModifier { }

    public interface IGameDataModifier<TResult> : IGameDataModifier where TResult : struct
    {
        void ModifyProperty(IGameDataBase gameData, TResult value);
    }

    public interface IGameContext : IGameDataBase
    {
        IGameDataReader Instigator { get; set; }

        IGameDataReader Subject { get; set; }

        IGameDataReader Environment { get; set; }

        TResult Resolve<TResult>(GameDataSource source, IGameDataBase gameData) where TResult : struct;
    }

    public class GameContext<TAction> : GameData<TAction>, IGameContext where TAction : struct
    {
        public GameContext(TAction index) : base(index) { }

        #region IGameContext

        public IGameDataReader Instigator { get; set; } = null;

        public IGameDataReader Subject { get; set; } = null;

        public IGameDataReader Environment { get; set; } = null;

        public TResult Resolve<TResult>(GameDataSource source, IGameDataBase data) where TResult : struct
        {
            IGameDataReader sourceTarget;

            if (data is IRuleScript<TResult> ruleScript)
            {
                if (source == GameDataSource.Context)
                    return ruleScript.Calculate(this);
                else
                    throw new NotSupportedException($"{nameof(GameDataSource)} for {nameof(IRuleScript<TResult>)} '{ruleScript}' must be set to {nameof(GameDataSource.Context)}.");
            }

            switch (source)
            {
                case GameDataSource.Environment:
                    sourceTarget = Environment;
                    break;
                case GameDataSource.Instigator:
                    sourceTarget = Instigator;
                    break;
                case GameDataSource.Subject:
                    sourceTarget = Subject;
                    break;
                default:
                    throw new NotSupportedException(nameof(source));
            }

            if (sourceTarget == null)
                throw new NullReferenceException($"{Index}: {nameof(GameDataSource)} '{source}' is not specified in this context.");

            if (sourceTarget is not IGameDataReader<TResult> provider)
                throw new NotSupportedException($"{Index}: {source} '{sourceTarget}' does not support querying for type '{typeof(TResult).Name}'.");

            return provider.QueryProperty(data);
        }

        #endregion IGameContext
    }
}