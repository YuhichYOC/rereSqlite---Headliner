﻿/*
*
* XMLNode.cs
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

public class XMLNode : TreeViewItem {
    public NodeEntity Node { get; set; }

    public void Fill() {
        Header = Node.NodeName;
        Name = Node.NodeName;
        Tag = Node.CloneWithoutChildren();
        Node.Children.ForEach(c => Fill(this, c));
    }

    private void Fill(XMLNode arg1, NodeEntity arg2) {
        var add = new XMLNode {Header = arg2.NodeName, Name = arg2.NodeName, Tag = arg2.CloneWithoutChildren()};
        arg2.Children.ForEach(c => Fill(add, c));
        arg1.Items.Add(add);
    }
}