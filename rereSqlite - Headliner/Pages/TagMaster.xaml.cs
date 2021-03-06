﻿/*
*
* TagMaster.cs
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
using System.Windows;
using System.Windows.Media;
using rereSqlite___Headliner.UserControls;

namespace rereSqlite___Headliner.Pages {
    public partial class TagMaster {
        public TagMaster() {
            InitializeComponent();
        }

        public void Init() {
            FontFamily = new FontFamily(AppBehind.Get.FontFamily);
            FontSize = AppBehind.Get.FontSize;
        }

        private void PerformSelect() {
            FillCardList(new Data.TagMaster().Query(TagInput.Text));
        }

        private void FillCardList(List<List<object>> rows) {
            CardList.Children.Clear();
            rows.ForEach(row => { AddChild(row[0].ToString(), row[0].ToString(), new Thickness(0, 2, 0, 0)); });
            AddChild(@"", @"", new Thickness(0, 2, 0, 0));
        }

        private void AddChild(string newTag, string oldTag, Thickness margin) {
            var add = new TagCard {
                NewTag = newTag,
                OldTag = oldTag,
                Margin = margin
            };
            add.Init();
            CardList.Children.Add(add);
        }

        #region -- Event Handlers --

        private void Search_Click(object sender, RoutedEventArgs e) {
            try {
                PerformSelect();
            }
            catch (Exception ex) {
                AppBehind.Get.AppendError(ex.Message, ex);
            }
        }

        #endregion
    }
}