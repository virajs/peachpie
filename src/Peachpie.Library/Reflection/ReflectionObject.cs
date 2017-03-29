﻿using Pchp.Core;
using Pchp.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pchp.Library.Reflection
{
    [PhpType("[name]"), PhpExtension(ReflectionUtils.ExtensionName)]
    public class ReflectionObject : ReflectionClass
    {
        object _instance;

        [PhpFieldsOnlyCtor]
        protected ReflectionObject()
        { }

        public ReflectionObject(object instance)
            :base(instance.GetPhpTypeInfo())
        {
            _instance = instance;
        }

        public override void __construct(Context ctx, PhpValue @class)
        {
            _instance = @class.AsObject();
            if (_instance == null)
            {
                throw new ReflectionException();
            }
            _tinfo = _instance.GetPhpTypeInfo();
        }

        public override PhpArray getProperties(int filter)
        {
            var result = new PhpArray(8);
            foreach (var p in _tinfo.GetDeclaredProperties().Concat(_tinfo.GetRuntimeProperties(_instance)))
            {
                var pinfo = new ReflectionProperty(p);
                if (filter == 0 || ((int)pinfo.getModifiers() | filter) != 0)
                {
                    result.Add(PhpValue.FromClass(pinfo));
                }
            }

            return result;
        }

        [return: CastToFalse]
        public override ReflectionProperty getProperty(string name)
        {
            var p = _tinfo.GetDeclaredProperty(name) ?? _tinfo.GetRuntimeProperty(name, _instance);
            if (p == null)
            {
                return null;
            }

            return new ReflectionProperty(p);
        }
    }
}