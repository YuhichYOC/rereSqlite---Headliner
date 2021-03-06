﻿/*
*
* FileSystemTree.cs
*
* Copyright 2017 Yuichi Yoshii
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

using System.Windows.Controls;

public class FileSystemTree {
    private FileSystemNode root;

    public TreeView OwnTree { get; private set; }

    public void Prepare(TreeView arg) {
        OwnTree = arg;
    }

    public void Fill(string path) {
        root = new FileSystemNode {Header = path, NodeName = path, FullPath = path};
        root.Fill();
        OwnTree.Items.Add(root);
    }
}