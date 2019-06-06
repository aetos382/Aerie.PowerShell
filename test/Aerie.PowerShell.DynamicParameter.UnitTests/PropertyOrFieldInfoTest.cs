using System;

using NUnit.Framework;

namespace Aerie.PowerShell.DynamicParameter.UnitTests
{
    public class PropertyOrFieldInfoTest
    {
        [Test]
        public void プロパティからインスタンス作成()
        {
            var p1 = typeof(TestObject).GetProperty(nameof(TestObject.Property1));

            var info = new PropertyOrFieldInfo(p1);

            Assert.AreEqual("Property1", info.Name);
            Assert.AreEqual(typeof(int), info.PropertyOrFieldType);
            Assert.True(info.CanRead);
            Assert.True(info.CanWrite);
            Assert.False(info.IsStatic);
            Assert.AreSame(p1, info.BaseMemberInfo);
        }

        [Test]
        public void プロパティ値を取得()
        {
            var p1 = typeof(TestObject).GetProperty(nameof(TestObject.Property1));

            var info = new PropertyOrFieldInfo(p1);
            var obj = new TestObject { Property1 = 42 };

            var value = info.GetValue(obj);

            Assert.AreEqual(42, value);
        }

        [Test]
        public void プロパティ値を設定()
        {
            var p1 = typeof(TestObject).GetProperty(nameof(TestObject.Property1));

            var info = new PropertyOrFieldInfo(p1);
            var obj = new TestObject { Property1 = 42 };

            info.SetValue(obj, 89);

            Assert.AreEqual(89, obj.Property1);
        }

        [Test]
        public void フィールドからインスタンス作成()
        {
            var f1 = typeof(TestObject).GetField(nameof(TestObject.Field1));

            var info = new PropertyOrFieldInfo(f1);

            Assert.AreEqual("Field1", info.Name);
            Assert.AreEqual(typeof(string), info.PropertyOrFieldType);
            Assert.True(info.CanRead);
            Assert.True(info.CanWrite);
            Assert.False(info.IsStatic);
            Assert.AreSame(f1, info.BaseMemberInfo);
        }

        [Test]
        public void フィールド値を取得()
        {
            var f1 = typeof(TestObject).GetField(nameof(TestObject.Field1));

            var info = new PropertyOrFieldInfo(f1);
            var obj = new TestObject { Field1 = "Hello" };

            var value = info.GetValue(obj);

            Assert.AreEqual("Hello", value);
        }

        [Test]
        public void フィールド値を設定()
        {
            var f1 = typeof(TestObject).GetField(nameof(TestObject.Field1));

            var info = new PropertyOrFieldInfo(f1);
            var obj = new TestObject { Field1 = "Hello" };

            info.SetValue(obj, "How are you?");

            Assert.AreEqual("How are you?", obj.Field1);
        }

        public class TestObject
        {
            public int Property1 { get; set; }

            public string Field1;
        }
    }
}
