﻿using System;

namespace Common
{
    public delegate void EventHandler<in TSender, in TArgs>(TSender sender, TArgs args) where TArgs : EventArgs;
}
