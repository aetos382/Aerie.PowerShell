using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Aerie.PowerShell.DynamicParameter.UnitTests
{
    public class PropertyOrFieldChainTest
    {
        [Test]
        public void プロパティからインスタンス作成()
        {
            var barProperty = typeof(Foo).GetProperty(nameof(Foo.BarProperty));
            var property1 = typeof(Bar).GetProperty(nameof(Bar.Property1));

            var info = new PropertyOrFieldChain(barProperty, property1);

            Assert.AreEqual("Property1", info.Name);
            Assert.AreEqual(typeof(int), info.PropertyOrFieldType);
            Assert.True(info.CanRead);
            Assert.True(info.CanWrite);
            Assert.False(info.IsStatic);
        }

        [Test]
        public void プロパティ値を取得()
        {
            var barProperty = typeof(Foo).GetProperty(nameof(Foo.BarProperty));
            var property1 = typeof(Bar).GetProperty(nameof(Bar.Property1));

            var info = new PropertyOrFieldChain(barProperty, property1);
            var obj = new Foo { BarProperty = new Bar { Property1 = 42 } };

            var value = info.GetValue(obj);

            Assert.AreEqual(42, value);
        }

        [Test]
        public void プロパティ値を設定()
        {
            var barProperty = typeof(Foo).GetProperty(nameof(Foo.BarProperty));
            var property1 = typeof(Bar).GetProperty(nameof(Bar.Property1));

            var info = new PropertyOrFieldChain(barProperty, property1);
            var obj = new Foo { BarProperty = new Bar { Property1 = 42 } };

            info.SetValue(obj, 89);

            Assert.AreEqual(89, obj.BarProperty.Property1);
        }

        [Test]
        public void フィールド値を取得()
        {
            var barField = typeof(Foo).GetField(nameof(Foo.BarField));
            var field1 = typeof(Bar).GetField(nameof(Bar.Field1));

            var info = new PropertyOrFieldChain(barField, field1);
            var obj = new Foo { BarField = new Bar { Field1 = "Hello" } };

            var value = info.GetValue(obj);

            Assert.AreEqual("Hello", value);
        }

        [Test]
        public void フィールド値を設定()
        {
            var barField = typeof(Foo).GetField(nameof(Foo.BarField));
            var field1 = typeof(Bar).GetField(nameof(Bar.Field1));

            var info = new PropertyOrFieldChain(barField, field1);
            var obj = new Foo { BarField = new Bar { Field1 = "Hello" } };

            info.SetValue(obj, "How are you?");

            Assert.AreEqual("How are you?", obj.BarField.Field1);
        }

        public class Foo
        {
            public Bar BarProperty { get; set; }

            public Bar BarField;
        }

        public class Bar
        {
            public int Property1 { get; set; }

            public string Field1;
        }
    }
}
