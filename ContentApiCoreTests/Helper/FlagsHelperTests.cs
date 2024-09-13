// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Helper;
using System;
using System.Linq;
using Xunit;

namespace ContentApiCoreTests.Helper
{
    public class FlagsHelperTests
    {
        [Fact]
        public void GetFlags_ValidEnum()
        {
            var @enum = ExampleTypeFlag.examplecategory1 | ExampleTypeFlag.examplecategory2;
            var flags = @enum.GetFlags();
            Assert.Equal(2, flags.Count());
            Assert.Contains(ExampleTypeFlag.examplecategory1, flags);
            Assert.Contains(ExampleTypeFlag.examplecategory2, flags);
        }

        [Fact]
        public void GetFlags_InvalidEnumType()
        {
            var @enum = 12333;
            Assert.Throws<ArgumentException>("withFlags", () =>
            {
                var flags = @enum.GetFlags().ToList();
            });
        }

        [Fact]
        public void GetFlags_InvalidNonFlaggedEnum()
        {
            var @enum = DayOfWeek.Monday;
            Assert.Throws<ArgumentException>("withFlags", () =>
            {
                var flags = @enum.GetFlags().ToList();
            });
        }

        [Fact]
        public void SetFlags_Test()
        {
            var @enum = ExampleTypeFlag.examplecategory1 | ExampleTypeFlag.examplecategory2;
            var newenum = @enum.SetFlags(ExampleTypeFlag.examplecategory3);
            var flags = newenum.GetFlags();
            Assert.Equal(3, flags.Count());
            Assert.True(newenum.HasFlag(ExampleTypeFlag.examplecategory3));

            // Functionally equivalent
            Assert.Equal(@enum | ExampleTypeFlag.examplecategory3, @enum.SetFlags(ExampleTypeFlag.examplecategory3));
        }

        [Fact]
        public void SetFlags_TestWithOnFalse()
        {
            var @enum = ExampleTypeFlag.examplecategory1 | ExampleTypeFlag.examplecategory2 | ExampleTypeFlag.examplecategory3;

            // Attention, not functionally equivalent!
            //Assert.Equal(@enum & ActivityTypeBerg.Bergtouren, @enum.SetFlags(ActivityTypeBerg.Bergtouren, false));

            var newenum2 = @enum.SetFlags(ExampleTypeFlag.examplecategory1, false);
            Assert.False(newenum2.HasFlag(ExampleTypeFlag.examplecategory1));
            Assert.True(newenum2.HasFlag(ExampleTypeFlag.examplecategory2));
            Assert.True(newenum2.HasFlag(ExampleTypeFlag.examplecategory3));
            Assert.Equal(2, newenum2.GetFlags().Count());
        }

        [Fact]
        public void IsFlagSet_Test()
        {
            var @enum = ExampleTypeFlag.examplecategory1 | ExampleTypeFlag.examplecategory2;
            // Functionally equivalent
            Assert.Equal(@enum.HasFlag(ExampleTypeFlag.examplecategory3), @enum.IsFlagSet(ExampleTypeFlag.examplecategory3));
        }
    }
}
