/*
 * MindTouch.Arpysee
 * 
 * Copyright (C) 2011 Arne F. Claassen
 * geekblog [at] claassen [dot] net
 * http://github.com/sdether/MindTouch.Arpysee
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
 */
using System;

namespace MindTouch.Arpysee.Server.Sync {
    public class SyncCommandRegistration {
        private static readonly Logger.ILog _log = Logger.CreateLog();

        private readonly DataExpectation _dataExpectation;
        private readonly Func<IRequest, IResponse> _handler;

        public SyncCommandRegistration(Func<IRequest, IResponse> handler) : this(handler, DataExpectation.Auto) { }

        public SyncCommandRegistration(Func<IRequest, IResponse> handler, DataExpectation dataExpectation) {
            _handler = handler;
            _dataExpectation = dataExpectation;
        }

        public DataExpectation DataExpectation { get { return _dataExpectation; } }
        public Func<IRequest, IResponse> Handler { get { return _handler; } }
    }
}