// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using global::System;
using global::System.Threading;
using global::System.Reflection;
using global::System.Diagnostics;
using global::System.Collections.Generic;

using global::Internal.Runtime.Augments;
using global::Internal.Reflection.Execution;
using global::Internal.Reflection.Core.Execution;

namespace Internal.Reflection.Execution.FieldAccessors
{
    internal sealed class ReferenceTypeFieldAccessorForThreadStaticFields : WritableStaticFieldAccessor
    {
        private int _threadStaticsBlockOffset;
        private int _fieldOffset;
        private RuntimeTypeHandle _declaringTypeHandle;

        public ReferenceTypeFieldAccessorForThreadStaticFields(IntPtr cctorContext, RuntimeTypeHandle declaringTypeHandle, int threadStaticsBlockOffset, int fieldOffset, RuntimeTypeHandle fieldTypeHandle)
            : base(cctorContext, fieldTypeHandle)
        {
            _threadStaticsBlockOffset = threadStaticsBlockOffset;
            _fieldOffset = fieldOffset;
            _declaringTypeHandle = declaringTypeHandle;
        }

        protected sealed override Object GetFieldBypassCctor()
        {
            IntPtr fieldAddress = RuntimeAugments.GetThreadStaticFieldAddress(_declaringTypeHandle, _threadStaticsBlockOffset, _fieldOffset);
            return RuntimeAugments.LoadReferenceTypeField(fieldAddress);
        }

        protected sealed override void UncheckedSetFieldBypassCctor(Object value)
        {
            IntPtr fieldAddress = RuntimeAugments.GetThreadStaticFieldAddress(_declaringTypeHandle, _threadStaticsBlockOffset, _fieldOffset);
            RuntimeAugments.StoreReferenceTypeField(fieldAddress, value);
        }
    }
}
