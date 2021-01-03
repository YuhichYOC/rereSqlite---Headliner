﻿/*
*
* StringStorage.cs
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

using System;
using System.Collections.Generic;

namespace rereSqlite___Headliner.Data {
    public class StringStorage : DaoCommon {
        public void SetUp() {
            if (TableExists()) return;
            CreateTable();
        }

        public List<List<object>> Query(string key, object tag) {
            return null == tag || string.IsNullOrEmpty(tag.ToString())
                ? base.Query(QuerySelect, new Dictionary<string, string> {{@"@key", key}})
                : base.Query(new Dictionary<string, string> {{@"@key", key}, {@"@tag", tag.ToString()}});
        }

        public static void Register(
            bool insert,
            string key,
            string value,
            List<object> tags
        ) {
            using var accessor = new SqliteAccessor {
                DataSource = AppBehind.Get.DBFilePath,
                Password = AppBehind.Get.Password,
                QueryString = insert ? QueryInsert : QueryUpdate
            };
            accessor.Open();
            var transaction = accessor.Begin();
            try {
                var command = accessor.CreateCommand();
                command.Parameters.AddWithValue(@"@key", key);
                command.Parameters.AddWithValue(@"@value", value);
                accessor.Execute(command);
                StringTags.Register(accessor, key, tags);
                transaction.Commit();
            }
            catch (Exception) {
                transaction.Rollback();
                throw;
            }
        }

        protected override string GetQueryTableExists() {
            return QueryTableExists;
        }

        protected override string GetQueryCreateTable() {
            return QueryCreateTable;
        }

        protected override string GetQuerySelect() {
            return QuerySelectWithTag;
        }

        #region -- Query Strings --

        private const string QueryTableExists =
            @" SELECT                                                                         " +
            @"     COUNT(NAME) AS COUNT_TABLES                                                " +
            @" FROM                                                                           " +
            @"     sqlite_master                                                              " +
            @" WHERE                                                                          " +
            @"     TYPE   = 'table'                                                           " +
            @" AND NAME   = 'STRING_STORAGE'                                                  ";

        private const string QueryCreateTable =
            @" CREATE                                                                         " +
            @" TABLE                                                                          " +
            @"     STRING_STORAGE                                                             " +
            @"     (                                                                          " +
            @"     KEY     TEXT,                                                              " +
            @"     VALUE   TEXT,                                                              " +
            @"     PRIMARY KEY                                                                " +
            @"       (                                                                        " +
            @"       KEY                                                                      " +
            @"       )                                                                        " +
            @"     )                                                                          ";

        private const string QuerySelectWithTag =
            QuerySelect +
            @" AND T.TAG            = @tag                                                    ";

        private const string QuerySelect =
            @" SELECT                                                                         " +
            @"     S.KEY,                                                                     " +
            @"     S.VALUE,                                                                   " +
            @"     T.TAG,                                                                     " +
            @"     ROW_NUMBER() OVER (                                                        " +
            @"       PARTITION BY                                                             " +
            @"         S.KEY                                                                  " +
            @"       ORDER BY                                                                 " +
            @"         T.TAG                                                                  " +
            @"     )                AS NUMBER_KEY,                                            " +
            @"     COUNT(S.KEY) OVER (                                                        " +
            @"       PARTITION BY                                                             " +
            @"         S.KEY                                                                  " +
            @"     )                AS TOTAL_KEY                                              " +
            @" FROM                                                                           " +
            @"     STRING_STORAGE   S                                                         " +
            @" LEFT OUTER JOIN                                                                " +
            @"     STRING_TAGS      T                                                         " +
            @" ON                                                                             " +
            @"     S.KEY            = T.KEY                                                   " +
            @" WHERE                                                                          " +
            @"     S.KEY            LIKE @key || '%'                                          ";

        private const string QueryInsert =
            @" INSERT                                                                         " +
            @" INTO                                                                           " +
            @"     STRING_STORAGE                                                             " +
            @"     (                                                                          " +
            @"     KEY,                                                                       " +
            @"     VALUE                                                                      " +
            @"     )                                                                          " +
            @" VALUES                                                                         " +
            @"     (                                                                          " +
            @"     @key,                                                                      " +
            @"     @value                                                                     " +
            @"     )                                                                          ";

        private const string QueryUpdate =
            @" UPDATE                                                                         " +
            @"     STRING_STORAGE                                                             " +
            @" SET                                                                            " +
            @"     VALUE   = @value                                                           " +
            @" WHERE                                                                          " +
            @"     KEY     = @key                                                             ";

        #endregion
    }
}