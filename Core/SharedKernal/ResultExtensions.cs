namespace SharedKernal
{
    public static class ResultExtensions
    {
        public static Result<TNew> Then<TCurrent, TNew>(this Result<TCurrent> CurrentResult,
                    Func<TCurrent, Result<TNew>> NextResult)
        {
            if (!CurrentResult.IsSuccess)
                return Result<TNew>.Failure(CurrentResult.Error!);

            return NextResult(CurrentResult.Value!);
        }

        public static async Task<Result<TNew>> ThenAsync<TCurrent, TNew>(this Task<Result<TCurrent>> currentResultTask, Func<TCurrent, Result<TNew>> nextResult)
        {
            var result = await currentResultTask;

            if (!result.IsSuccess) return Result<TNew>.Failure(result.Error!);

            return nextResult(result.Value!);
        }
    }
}
