using System;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TestClassAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TestMethodAttribute : Attribute { }

    public static class Assert
    {
        public static void IsNotNull(object obj, string message = null)
        {
            if (obj == null) throw new AssertFailedException(message ?? "Assert.IsNotNull failed.");
        }

        public static void IsTrue(bool condition, string message = null)
        {
            if (!condition) throw new AssertFailedException(message ?? "Assert.IsTrue failed.");
        }

        public static void AreEqual<T>(T expected, T actual, string message = null)
        {
            if (!object.Equals(expected, actual))
                throw new AssertFailedException(message ?? $"Assert.AreEqual failed. Expected:<{expected}>. Actual:<{actual}>.");
        }
    }

    public static class CollectionAssert
    {
        public static void AreEqual(System.Collections.ICollection expected, System.Collections.ICollection actual)
        {
            if (expected == null && actual == null) return;
            if (expected == null || actual == null) throw new AssertFailedException("CollectionAssert.AreEqual failed. One of collections is null.");
            if (expected.Count != actual.Count) throw new AssertFailedException($"CollectionAssert.AreEqual failed. Counts differ. Expected:{expected.Count}, Actual:{actual.Count}");
            var enum1 = expected.GetEnumerator();
            var enum2 = actual.GetEnumerator();
            while (enum1.MoveNext() && enum2.MoveNext())
            {
                if (!object.Equals(enum1.Current, enum2.Current))
                    throw new AssertFailedException($"CollectionAssert.AreEqual failed. Item mismatch: Expected<{enum1.Current}>, Actual<{enum2.Current}>.");
            }
        }
    }

    public class AssertFailedException : Exception
    {
        public AssertFailedException(string message) : base(message) { }
    }
}
