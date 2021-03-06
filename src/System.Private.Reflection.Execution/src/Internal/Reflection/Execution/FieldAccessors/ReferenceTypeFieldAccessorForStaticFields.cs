// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using global::System;
using global::System.Reflection;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;

using global::Internal.Runtime.Augments;

namespace Internal.Reflection.Execution.FieldAccessors
{
    internal sealed class ReferenceTypeFieldAccessorForStaticFields : WritableStaticFieldAccessor
    {
        private IntPtr _staticsBase;
        private bool _isGcStaticsBase;
        private int _fieldOffset;

        public ReferenceTypeFieldAccessorForStaticFields(IntPtr cctorContext, IntPtr staticsBase, int fieldOffset, bool isGcStatic, RuntimeTypeHandle fieldTypeHandle)
            : base(cctorContext, fieldTypeHandle)
        {
            _staticsBase = staticsBase;
            _isGcStaticsBase = isGcStatic;
            _fieldOffset = fieldOffset;
        }

        unsafe protected sealed override Object GetFieldBypassCctor()
        {
#if CORERT
            if (_isGcStaticsBase)
            {
                // The _staticsBase variable points to a GC handle, which points at the GC statics base of the type.
                // We need to perform a double indirection in a GC-safe manner.
                object gcStaticsRegion = RuntimeAugments.LoadReferenceTypeField(*(IntPtr*)_staticsBase);
                return RuntimeAugments.LoadReferenceTypeField(gcStaticsRegion, _fieldOffset);
            }
#endif
            return RuntimeAugments.LoadReferenceTypeField(_staticsBase + _fieldOffset);
        }

        unsafe protected sealed override void UncheckedSetFieldBypassCctor(Object value)
        {

#if CORERT
            if (_isGcStaticsBase)
            {
                // The _staticsBase variable points to a GC handle, which points at the GC statics base of the type.
                // We need to perform a double indirection in a GC-safe manner.
                object gcStaticsRegion = RuntimeAugments.LoadReferenceTypeField(*(IntPtr*)_staticsBase);
                RuntimeAugments.StoreReferenceTypeField(gcStaticsRegion, _fieldOffset, value);
                return;
            }
#endif
            RuntimeAugments.StoreReferenceTypeField(_staticsBase + _fieldOffset, value);
        }
    }
}
