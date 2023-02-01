// https://codinghelmet.com/articles/custom-implementation-of-the-option-maybe-type-in-cs

public abstract class Option<T>
{
    public static implicit operator Option<T>(T value) =>
        new Some<T>(value);

    public static implicit operator Option<T>(None none) =>
        new None<T>();

    public abstract Option<TResult> Map<TResult>(Func<T, TResult> map);
    public abstract Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map);
    public abstract T Reduce(T whenNone);
    public abstract T Reduce(Func<T> whenNone);
}

public sealed class Some<T> : Option<T>
{
    public T Content { get; }

    public Some(T value)
    {
        this.Content = value;
    }

    public static implicit operator T(Some<T> some) =>
        some.Content;

    public override Option<TResult> Map<TResult>(Func<T, TResult> map) =>
        map(this.Content);

    public override Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) =>
        map(this.Content);

    public override T Reduce(T whenNone) =>
        this.Content;

    public override T Reduce(Func<T> whenNone) =>
        this.Content;
}

public sealed class None<T> : Option<T>
{
    public override Option<TResult> Map<TResult>(Func<T, TResult> map) =>
        None.Value;

    public override Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) =>
        None.Value;

    public override T Reduce(T whenNone) =>
        whenNone;

    public override T Reduce(Func<T> whenNone) =>
        whenNone();
}

public sealed class None
{
    public static None Value { get; } = new None();

    private None() { }
}