﻿/*
*
* StringTags.cs
*
* Copyright 2020 Yuichi Yoshii
*     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System.Collections.Generic;

namespace rereSqlite___Headliner.Data {
    public class StringTags : DaoCommon {
        public void SetUp(AppBehind appBehind) {
            if (TableExists(appBehind)) return;
            CreateTable(appBehind);
        }

        public List<List<object>> Query(AppBehind appBehind, string key) {
            return base.Query(appBehind, new Dictionary<string, string> {{@"@key", key}});
        }

        public static void Register(
            SqliteAccessor accessor,
            string key,
            List<object> tags
        ) {
            accessor.QueryString = QueryDelete;
            var command = accessor.CreateCommand();
            command.Parameters.AddWithValue(@"@key", key);
            accessor.Execute(command);
            accessor.QueryString = QueryInsert;
            command = accessor.CreateCommand();
            for (var i = 0; tags.Count > i; ++i) {
                command.Parameters.Clear();
                command.Parameters.AddWithValue(@"@key", key);
                command.Parameters.AddWithValue(@"@tag", tags[i].ToString());
                accessor.Execute(command);
            }
        }

        protected override string GetQueryTableExists() {
            return QueryTableExists;
        }

        protected override string GetQueryCreateTable() {
            return QueryCreateTable;
        }

        protected override string GetQuerySelect() {
            return QuerySelect;
        }

        #region -- Query Strings --

        private const string QueryTableExists =
            @" SELECT                                                                         " +
            @"     COUNT(NAME) AS COUNT_TABLES                                                " +
            @" FROM                                                                           " +
            @"     sqlite_master                                                              " +
            @" WHERE                                                                          " +
            @"     TYPE   = 'table'                                                           " +
            @" AND NAME   = 'STRING_TAGS'                                                     ";

        private const string QueryCreateTable =
            @" CREATE                                                                         " +
            @" TABLE                                                                          " +
            @"     STRING_TAGS                                                                " +
            @"     (                                                                          " +
            @"     KEY   TEXT,                                                                " +
            @"     TAG   TEXT,                                                                " +
            @"     PRIMARY KEY                                                                " +
            @"       (                                                                        " +
            @"       KEY,                                                                     " +
            @"       TAG                                                                      " +
            @"       ),                                                                       " +
            @"     FOREIGN KEY                                                                " +
            @"       (                                                                        " +
            @"       KEY                                                                      " +
            @"       )                                                                        " +
            @"       REFERENCES                                                               " +
            @"       STRING_STORAGE                                                           " +
            @"       (                                                                        " +
            @"       KEY                                                                      " +
            @"       ),                                                                       " +
            @"     FOREIGN KEY                                                                " +
            @"       (                                                                        " +
            @"       TAG                                                                      " +
            @"       )                                                                        " +
            @"       REFERENCES                                                               " +
            @"       TAG_MASTER                                                               " +
            @"       (                                                                        " +
            @"       TAG                                                                      " +
            @"       )                                                                        " +
            @"     )                                                                          ";

        private const string QuerySelect =
            @" SELECT                                                                         " +
            @"     TAG                                                                        " +
            @" FROM                                                                           " +
            @"     STRING_TAGS                                                                " +
            @" WHERE                                                                          " +
            @"     KEY   = @key                                                               ";

        private const string QueryInsert =
            @" INSERT                                                                         " +
            @" INTO                                                                           " +
            @"     STRING_TAGS                                                                " +
            @"     (                                                                          " +
            @"     KEY,                                                                       " +
            @"     TAG                                                                        " +
            @"     )                                                                          " +
            @" VALUES                                                                         " +
            @"     (                                                                          " +
            @"     @key,                                                                      " +
            @"     @tag                                                                       " +
            @"     )                                                                          ";

        private const string QueryDelete =
            @" DELETE                                                                         " +
            @" FROM                                                                           " +
            @"     STRING_TAGS                                                                " +
            @" WHERE                                                                          " +
            @"     KEY   = @key                                                               ";

        #endregion
    }
}