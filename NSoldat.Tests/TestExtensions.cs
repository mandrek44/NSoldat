using System.Collections.Generic;
using Xunit;

namespace NSoldat.Tests
{
    public static class TestExtensions
    {
        public static void AssertContains<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            params (TKey, TValue)[] expected)
        {
            foreach (var keyValuePair in expected)
            {
                Assert.Equal(keyValuePair.Item2, dictionary[keyValuePair.Item1]);
            }
        }

        public static void AssertEquals<T>(this T actual, T expected)
        {
            Assert.Equal(expected, actual);
        }
    }
}