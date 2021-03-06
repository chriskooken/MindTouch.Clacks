﻿/*
 * MindTouch.Clacks
 * 
 * Copyright (C) 2011-2013 Arne F. Claassen
 * geekblog [at] claassen [dot] net
 * http://github.com/sdether/MindTouch.Clacks
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
using System.Collections.Generic;

namespace MindTouch.Clacks.Server.Async {
    public class AsyncFluentCommandRegistration : IAsyncFluentCommandRegistration {
        private readonly ServerBuilder _serverBuilder;
        private readonly AsyncCommandRepository _repository;
        private readonly string _command;
        private bool _isDisconnect;
        private DataExpectation _dataExpectation = DataExpectation.Auto;
        private Action<IRequest, Action<IResponse>> _singleResponseHandler;
        private Action<IRequest, Action<IResponse, Action>> _multiAsyncResponseHandler;
        private Action<IRequest, Action<IEnumerable<IResponse>>> _multiSyncResponseHandler;

        public AsyncFluentCommandRegistration(ServerBuilder serverBuilder, AsyncCommandRepository repository, string command) {
            _serverBuilder = serverBuilder;
            _repository = repository;
            _command = command;
        }

        public IAsyncFluentCommandRegistration IsDisconnect() {
            _isDisconnect = true;
            return this;
        }

        public IAsyncFluentCommandRegistration HandledBy(Func<IRequest, IResponse> handler) {
            _singleResponseHandler = (request, responseCallback) => responseCallback(handler(request));
            return this;
        }

        public IAsyncFluentCommandRegistration HandledBy(Func<IRequest, IEnumerable<IResponse>> handler) {
            _multiSyncResponseHandler = (request, responseCallback) => responseCallback(handler(request));
            return this;
        }

        public IAsyncFluentCommandRegistration HandledBy(Action<IRequest, Action<IResponse>> handler) {
            _singleResponseHandler = handler;
            return this;
        }

        public IAsyncFluentCommandRegistration HandledBy(Action<IRequest, Action<IResponse, Action>> handler) {
            _multiAsyncResponseHandler = handler;
            return this;
        }

        public IAsyncFluentCommandRegistration HandledBy(Action<IRequest, Action<IEnumerable<IResponse>>> handler) {
            _multiSyncResponseHandler = handler;
            return this;
        }

        public IAsyncFluentCommandRegistration ExpectsData() {
            _dataExpectation = DataExpectation.Always;
            return this;
        }

        public IAsyncFluentCommandRegistration ExpectsNoData() {
            _dataExpectation = DataExpectation.Never;
            return this;
        }

        public IAsyncServerBuilder Register() {
            if(_singleResponseHandler == null && _multiAsyncResponseHandler == null && _multiSyncResponseHandler == null) {
                throw new CommandConfigurationException(string.Format("Must define a handler for command '{0}'", _command));
            }
            if(_isDisconnect) {
                _repository.Disconnect(_command, _singleResponseHandler);
            } else if(_singleResponseHandler != null) {
                _repository.AddCommand(_command, _singleResponseHandler, _dataExpectation);
            } else if(_multiSyncResponseHandler != null) {
                _repository.AddCommand(_command, _multiSyncResponseHandler, _dataExpectation);
            } else {
                _repository.AddCommand(_command, _multiAsyncResponseHandler, _dataExpectation);
            }
            return _serverBuilder;
        }
    }
}