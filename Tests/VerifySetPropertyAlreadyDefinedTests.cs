using Xunit;
using SmokeTest;

namespace Tests
{
    public class VerifySetPropertyAlreadyDefinedTests
    {
        /// <summary>
        /// The instruction after the copy of value must to executed,
        /// even if it's the same reference value.
        /// </summary>
        [Fact]
        public void InstructionAfterValueCheckBox()
        {
            CheckInsertIL checkInsertIL = new();
            Assert.Equal(5 * 7 * 9, (checkInsertIL.Box as BoxInterne).V);
            checkInsertIL.Box = checkInsertIL.Box;
            Assert.Equal(9 * 10 * 11, (checkInsertIL.Box as BoxInterne).V);
        }

        /// <summary>
        /// The string must to be changed even if it's the same string and haven't copy
        /// the value to the backing field.
        /// </summary>
        [Fact]
        public void InstructionSameValueCheckString()
        {
            CheckInsertIL checkInsert = new();
            checkInsert.StringProperty = "Hello the World";
            Assert.NotEqual("Hello the World", checkInsert.StringProperty);
        }

        /// <summary>
        /// The instruction before the copy of value must to be executed,
        /// even if it's the same value.
        /// The instruction must to be executed after the copy of value.
        /// </summary>
        [Fact]
        public void InstructionBeforeValueCheckInt()
        {
            CheckInsertIL checkInsert = new();
            checkInsert.IntProperty = 256;
            Assert.True(checkInsert.CheckIntBeforeValue == int.MaxValue);
            Assert.True(checkInsert.CheckIntAfterValue == -256);
        }
    }
}
