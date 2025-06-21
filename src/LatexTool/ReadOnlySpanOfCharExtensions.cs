internal static class ReadOnlySpanOfCharExtensions
{
    public static ReadOnlySpanOfCharSplitByAnyStringEnumerator Split(
        this ReadOnlySpan<char> span,
        ReadOnlySpan<string> separators,
        StringComparison comparison = StringComparison.Ordinal,
        bool includeEmpty = false,
        bool includeSeparators = false)
    {
        return new ReadOnlySpanOfCharSplitByAnyStringEnumerator(
            span, separators, comparison, includeEmpty, includeSeparators);
    }
}

internal ref struct ReadOnlySpanOfCharSplitByAnyStringEnumerator
{
    private readonly ReadOnlySpan<char> _span;
    private readonly ReadOnlySpan<string> _separators;
    private readonly StringComparison _comparison;
    private readonly bool _includeEmpty;
    private readonly bool _includeSeparators;

    private int _i;
    private int _length;

    public ReadOnlySpanOfCharSplitByAnyStringEnumerator(
        ReadOnlySpan<char> span,
        ReadOnlySpan<string> separators,
        StringComparison comparison,
        bool includeEmpty,
        bool includeSeparators)
    {
        _span = span;
        _separators = separators;
        _comparison = comparison;
        _includeEmpty = includeEmpty;
        _includeSeparators = includeSeparators;
    }

    public readonly ReadOnlySpan<char> Current => _span[_i..(_i + _length)];

    public readonly ReadOnlySpanOfCharSplitByAnyStringEnumerator GetEnumerator()
    {
        return this;
    }

    public bool MoveNext()
    {
        var offset = _i + _length;
        if (offset >= _span.Length)
        {
            return false;
        }

        var searchSpan = _span[offset..];
        var separatorIndex = searchSpan.Length;
        var separatorLength = 0;

        foreach (var separator in _separators)
        {
            var index = searchSpan.IndexOf(separator, _comparison);
            if (index == -1)
            {
                continue;
            }

            if (index < separatorIndex)
            {
                separatorIndex = index;
                separatorLength = separator.Length;
            }
        }

        if (separatorIndex == searchSpan.Length)
        {
            _i = offset;
            _length = searchSpan.Length;
        }
        else if (separatorIndex == 0 && _includeSeparators)
        {
            _i = offset;
            _length = separatorLength;
        }
        else if (separatorIndex == 0 && !_includeSeparators)
        {
            _i = offset + separatorLength;
            _length = 0;
            // TODO: Bug: extra empty string at the end of separator when _includeEmpty is true.
        }
        else
        {
            _i = offset;
            _length = separatorIndex;
        }

        if (_length == 0 && !_includeEmpty)
        {
            return MoveNext();
        }

        return true;
    }
}
