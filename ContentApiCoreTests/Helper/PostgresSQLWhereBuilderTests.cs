// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Helper;
using SqlKata;
using SqlKata.Compilers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ContentApiCoreTests.Helper
{
    public class PostgresSQLWhereBuilderTests
    {
        private readonly static PostgresCompiler compiler = new();

        [Fact]
        public void CreateExampleWhereExpression_LoggedUser()
        {
            var query =
                new Query()
                    .From("examples")
                    .ExampleWhereExpression(
                        languagelist: System.Array.Empty<string>(), 
                        idlist: System.Array.Empty<string>(),
                        typelist: System.Array.Empty<string>(),
                        activefilter: null,
                        sourcelist: System.Array.Empty<string>(),
                        publishedonlist: System.Array.Empty<string>(),
                        searchfilter: null,
                        language: null,
                        lastchange: null,
                        additionalfilter: null,
                        userroles: new List<string>() { "STA" }
                    );

            var result = compiler.Compile(query);

            Assert.Equal("SELECT * FROM \"examples\" WHERE gen_access_role @> array\\[$$\\]", result.RawSql);
        }

        [Fact]
        public void CreateExampleWhereExpression_Anonymous()
        {
            var query =
                new Query()
                    .From("examples")
                    .ExampleWhereExpression(
                        languagelist: System.Array.Empty<string>(),
                        idlist: System.Array.Empty<string>(),
                        typelist: System.Array.Empty<string>(),
                        activefilter: null,
                        sourcelist: System.Array.Empty<string>(),
                        publishedonlist: System.Array.Empty<string>(),
                        searchfilter: null,
                        language: null,
                        lastchange: null,
                        additionalfilter: null,
                        new List<string>() { "ANONYMOUS" }
                    );

            var result = compiler.Compile(query);

            Assert.Equal("SELECT * FROM \"examples\" WHERE gen_access_role @> array\\[$$\\]", result.RawSql);
        }

        [Fact]
        public void CreateExampleWhereExpression_IDMUser()
        {
            var query =
                new Query()
                    .From("examples")
                    .ExampleWhereExpression(
                                              languagelist: System.Array.Empty<string>(),
                        idlist: System.Array.Empty<string>(),
                        typelist: System.Array.Empty<string>(),
                        activefilter: null,
                        sourcelist: System.Array.Empty<string>(),
                        publishedonlist: System.Array.Empty<string>(),
                        searchfilter: null,
                        language: null,
                        lastchange: null,
                        additionalfilter: null,
                        userroles: new List<string>() { "IDM" }
                    );

            var result = compiler.Compile(query);

            Assert.Equal("SELECT * FROM \"examples\" WHERE gen_access_role @> array\\[$$\\]", result.RawSql);
        }
    }
}
