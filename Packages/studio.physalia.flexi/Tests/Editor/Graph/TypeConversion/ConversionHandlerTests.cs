using System;
using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class ConversionHandlerTests
    {
        [Test]
        public void Default_CanConvertReturnsFalse()
        {
            var handler = new ConversionHandler();

            bool result = handler.CanConvert<float, int>();
            Assert.AreEqual(false, result);
        }

        [Test]
        public void Default_GetConverterReturnsNull()
        {
            var handler = new ConversionHandler();

            Func<float, int> converter = handler.GetConverter<float, int>();
            Assert.IsNull(converter);
        }

        [Test]
        public void Default_GetConverterBoxedReturnsNull()
        {
            var handler = new ConversionHandler();

            Func<object, object> converterBoxed = handler.GetConverterBoxed(typeof(float), typeof(int));
            Assert.IsNull(converterBoxed);
        }

        [Test]
        public void HandleFloatToInt_CanConvertReturnsTrue()
        {
            var handler = new ConversionHandler();
            handler.Handle<float, int>(value => (int)value);

            bool result = handler.CanConvert<float, int>();
            Assert.AreEqual(true, result);
        }

        [Test]
        public void HandleFloatToInt_TheConverterCanReturn3Point5To3()
        {
            var handler = new ConversionHandler();
            handler.Handle<float, int>(value => (int)value);

            Func<float, int> converter = handler.GetConverter<float, int>();
            Assert.IsNotNull(converter);
            Assert.AreEqual(3, converter(3.5f));
        }

        [Test]
        public void HandleFloatToInt_TheConverterBoxedCanReturn3Point5To3()
        {
            var handler = new ConversionHandler();
            handler.Handle<float, int>(value => (int)value);

            Func<object, object> converterBoxed = handler.GetConverterBoxed(typeof(float), typeof(int));
            Assert.IsNotNull(converterBoxed);
            Assert.AreEqual(3, converterBoxed(3.5f));
        }
    }
}
