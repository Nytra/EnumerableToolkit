﻿namespace EnumerableToolkit.Builder.AsyncBlocks
{
    /// <summary>
    /// Represents a building block of the <see cref="AsyncEnumerableBuilder{T}"/>
    /// that inserts a sequence of items after the first item matching a <see cref="InsertAfter(T, int)">predicate</see>.
    /// </summary>
    /// <inheritdoc/>
    public abstract class AsyncInsertAfterFirstItemBlock<T> : AsyncAddingBlock<T>
    {
        /// <inheritdoc/>
        protected AsyncInsertAfterFirstItemBlock(IAsyncEnumerable<T> sequence) : base(sequence)
        { }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<T> Apply(IAsyncEnumerable<T> current)
        {
            var i = 0;
            var matched = false;

            await foreach (var item in current)
            {
                yield return item;

                if (!matched && InsertAfter(item, i++))
                {
                    matched = true;

                    await foreach (var insertedItem in sequence)
                        yield return insertedItem;
                }
            }
        }

        /// <summary>
        /// Determines whether this block's additions should be inserted after the given item (and only that).
        /// </summary>
        /// <param name="current">The last item that was returned.</param>
        /// <param name="index">The index of the last item that was returned.</param>
        /// <returns><c>true</c> if the additions should be inserted after the item; otherwise, <c>false</c>.</returns>
        protected abstract bool InsertAfter(T current, int index);
    }

    /// <summary>
    /// Represents a building block of the <see cref="AsyncEnumerableBuilder{T}"/>
    /// that inserts a sequence of items after the first item matching a given <see cref="Func{T1, T2, TResult}">predicate</see>.
    /// </summary>
    /// <inheritdoc/>
    public sealed class AsyncInsertAfterFirstItemLambdaBlock<T> : AsyncInsertAfterFirstItemBlock<T>
    {
        private readonly Func<T, int, bool> _predicate;

        /// <summary>
        /// Creates a new building block that inserts the given <paramref name="sequence"/>
        /// after the first item that matches the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A predicate determining whether the <paramref name="sequence"/> of items should be inserted after that item.</param>
        /// <param name="sequence">A sequence of items that should be added.</param>
        public AsyncInsertAfterFirstItemLambdaBlock(Func<T, int, bool> predicate, IAsyncEnumerable<T> sequence) : base(sequence)
        {
            _predicate = predicate;
        }

        /// <inheritdoc/>
        protected override bool InsertAfter(T current, int index)
            => _predicate(current, index);
    }
}