
/********************************************************************************
 * Copyright (c) 2021 LTU
 *
 * This program and the accompanying materials are made available under the
 * terms of the Eclipse Public License 2.0 which is available at
 * http://www.eclipse.org/legal/epl-2.0.
 *
 * SPDX-License-Identifier: EPL-2.0
 *
 * Contributors:
 *   LTU - implementation
 ********************************************************************************/
using System;
using System.Runtime.Serialization;

namespace Arrowhead.Models
{
    public class OrchestrationStoreEntryExistsException : Exception
    {
        public OrchestrationStoreEntryExistsException() { }
        public OrchestrationStoreEntryExistsException(string message) : base(message) { }
        public OrchestrationStoreEntryExistsException(string message, Exception inner) : base(message, inner) { }
        protected OrchestrationStoreEntryExistsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }

    public class IntracloudRulesetExistsException : Exception
    {
        public IntracloudRulesetExistsException() { }
        public IntracloudRulesetExistsException(string message) : base(message) { }
        public IntracloudRulesetExistsException(string message, Exception inner) : base(message, inner) { }
        protected IntracloudRulesetExistsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }

    public class CouldNotUnregisterServiceException : Exception
    {
        public CouldNotUnregisterServiceException() { }
        public CouldNotUnregisterServiceException(string message) : base(message) { }
        public CouldNotUnregisterServiceException(string message, Exception inner) : base(message, inner) { }
        protected CouldNotUnregisterServiceException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }

    public class NoExistingServicesException : Exception
    {
        public NoExistingServicesException() { }
        public NoExistingServicesException(string message) : base(message) { }
        public NoExistingServicesException(string message, Exception inner) : base(message, inner) { }
        protected NoExistingServicesException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }

    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException() { }
        public ServiceNotFoundException(string message) : base(message) { }
        public ServiceNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected ServiceNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}
