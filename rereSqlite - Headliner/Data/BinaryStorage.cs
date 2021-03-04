﻿/*
*
* BinaryStorage.cs
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
using System.IO;

namespace rereSqlite___Headliner.Data {
    public class BinaryStorage : DaoCommon {
        public void SetUp() {
            if (TableExists()) return;
            CreateTable();
        }

        public List<List<object>> Query(string key, object tag) {
            return null == tag || string.IsNullOrEmpty(tag.ToString())
                ? base.Query(SELECT, new Dictionary<string, string> {{@"@key", key}})
                : base.Query(new Dictionary<string, string> {{@"@key", key}, {@"@tag", tag.ToString()}});
        }

        public static void Retrieve(string key, FileStream outputStream) {
            using var accessor = new SqliteAccessor {
                DataSource = AppBehind.Get.DBFilePath,
                Password = AppBehind.Get.Password,
                QueryString = SELECT_BLOB_WITH_KEY
            };
            accessor.Open();
            var command = accessor.CreateCommand();
            command.Parameters.AddWithValue(@"@key", key);
            accessor.RetrieveBlob(command, outputStream, 0);
        }

        public static void Register(
            bool insert,
            string key,
            string filePath,
            string fileName,
            List<object> tags
        ) {
            using var accessor = new SqliteAccessor {
                DataSource = AppBehind.Get.DBFilePath,
                Password = AppBehind.Get.Password,
                QueryString = insert ? INSERT : UPDATE
            };
            accessor.Open();
            var transaction = accessor.Begin();
            try {
                var command = accessor.CreateCommand();
                command.Parameters.AddWithValue(@"@key", key);
                command.Parameters.AddWithValue(@"@fileName", fileName);
                command.Parameters.AddWithValue(@"@length", new FileInfo(filePath).Length);
                using var inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                accessor.WriteBlob(
                    inputStream,
                    @"BINARY_STORAGE",
                    @"VALUE",
                    (long) accessor.ExecuteScalar(command)
                );
                BinaryTags.Register(accessor, key, tags);
                transaction.Commit();
            }
            catch (Exception) {
                transaction.Rollback();
                throw;
            }
        }

        protected override string GetQueryTableExists() {
            return TABLE_EXISTS;
        }

        protected override string GetQueryCreateTable() {
            return CREATE_TABLE;
        }

        protected override string GetQuerySelect() {
            return SELECT_WITH_TAG;
        }

        #region -- Query Strings --

        private const string TABLE_EXISTS =
            @" SELECT                                                                         " +
            @"     COUNT(NAME) AS COUNT_TABLES                                                " +
            @" FROM                                                                           " +
            @"     sqlite_master                                                              " +
            @" WHERE                                                                          " +
            @"     TYPE   = 'table'                                                           " +
            @" AND NAME   = 'BINARY_STORAGE'                                                  ";

        private const string CREATE_TABLE =
            @" CREATE                                                                         " +
            @" TABLE                                                                          " +
            @"     BINARY_STORAGE                                                             " +
            @"     (                                                                          " +
            @"     KEY         TEXT,                                                          " +
            @"     FILE_NAME   TEXT,                                                          " +
            @"     VALUE       BLOB,                                                          " +
            @"     PRIMARY KEY                                                                " +
            @"       (                                                                        " +
            @"       KEY                                                                      " +
            @"       )                                                                        " +
            @"     )                                                                          ";

        private const string SELECT_WITH_TAG =
            SELECT +
            @" AND T.TAG            = @tag                                                    ";

        private const string SELECT =
            @" SELECT                                                                         " +
            @"     S.KEY,                                                                     " +
            @"     S.FILE_NAME,                                                               " +
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
            @"     BINARY_STORAGE   S                                                         " +
            @" LEFT OUTER JOIN                                                                " +
            @"     BINARY_TAGS      T                                                         " +
            @" ON                                                                             " +
            @"     S.KEY            = T.KEY                                                   " +
            @" WHERE                                                                          " +
            @"     S.KEY            LIKE @key || '%'                                          ";

        private const string SELECT_BLOB_WITH_KEY =
            @" SELECT                                                                         " +
            @"     VALUE                                                                      " +
            @" FROM                                                                           " +
            @"     BINARY_STORAGE                                                             " +
            @" WHERE                                                                          " +
            @"     KEY   = @key                                                               ";

        private const string INSERT =
            @" INSERT                                                                         " +
            @" INTO                                                                           " +
            @"     BINARY_STORAGE                                                             " +
            @"     (                                                                          " +
            @"     KEY,                                                                       " +
            @"     FILE_NAME,                                                                 " +
            @"     VALUE                                                                      " +
            @"     )                                                                          " +
            @" VALUES                                                                         " +
            @"     (                                                                          " +
            @"     @key,                                                                      " +
            @"     @fileName,                                                                 " +
            @"     zeroblob(@length)                                                          " +
            @"     )                                                                          " +
            @" ;                                                                              " +
            @" SELECT                                                                         " +
            @"     rowid                                                                      " +
            @" FROM                                                                           " +
            @"     BINARY_STORAGE                                                             " +
            @" WHERE                                                                          " +
            @"     KEY   = @key                                                               " +
            @" ;                                                                              ";

        private const string UPDATE =
            @" DELETE                                                                         " +
            @" FROM                                                                           " +
            @"     BINARY_STORAGE                                                             " +
            @" WHERE                                                                          " +
            @"     KEY   = @key                                                               " +
            @" ;                                                                              " +
            INSERT;

        #endregion
    }
}