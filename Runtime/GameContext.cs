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

    public interface IGameDataReader<out T> : IGameDataReader
    {
        T Query() => throw new NotImplementedException();

        T Query(IGameDataBase data) => throw new NotImplementedException();
    }

    public interface IGameDataMutator<in T>
    {
        void Mutate(IGameDataBase data, T value) => throw new NotImplementedException();
    }

    public interface IGameContext : IGameDataBase
    {
        IGameDataReader Instigator { get; set; }

        IGameDataReader Subject { get; set; }

        IGameDataReader Environment { get; set; }

        T Resolve<T>(GameDataSource source, IGameDataBase gameData);
    }

    public class GameContext<TAction> : GameData<TAction>, IGameContext where TAction : struct
    {
        public GameContext(TAction index) : base(index) { }

        #region IGameContext

        public IGameDataReader Instigator { get; set; } = null;

        public IGameDataReader Subject { get; set; } = null;

        public IGameDataReader Environment { get; set; } = null;

        public TResult Resolve<TResult>(GameDataSource source, IGameDataBase data)
        {
            if (data is IRuleMetric<TResult> metric)
            {
                if (source != GameDataSource.Context)
                    throw new NotSupportedException($"{Index}: {nameof(GameDataSource)} '{source}' is not supported for {nameof(IRuleMetric<TResult>)}.");

                return metric.Calculate(this);
            }

            IGameDataReader sourceTarget;
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
                throw new NotSupportedException($"{Index}: {nameof(GameDataSource)} '{source}' does not support querying for type '{typeof(TResult).Name}'.");

            return provider.Query(data);
        }

        #endregion IGameContext
    }
}