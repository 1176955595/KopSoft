﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xunit;
using YL.Utils.Extensions;
using YL.Utils.Pub;

public class EmunExtTest
{
    [Fact]
    public void TestEnum()
    {
        var list = EnumExt.ToKVList<PubDictType>();

        Assert.Equal(6, list.Count);
    }
}