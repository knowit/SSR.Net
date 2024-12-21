if (typeof MessageChannel === "undefined") {
    (function () {
        function MessageChannelPolyfill() {
            var self = this;
            this.port1 = {
                _onmessage: null,
                postMessage: function (msg) {
                    setTimeout(function () {
                        if (self.port2._onmessage) {
                            self.port2._onmessage({ data: msg });
                        }
                    }, 0);
                },
            };

            this.port2 = {
                _onmessage: null,
                postMessage: function (msg) {
                    setTimeout(function () {
                        if (self.port1._onmessage) {
                            self.port1._onmessage({ data: msg });
                        }
                    }, 0);
                },
            };

            Object.defineProperty(this.port1, "onmessage", {
                get: function () {
                    return this._onmessage;
                },
                set: function (handler) {
                    this._onmessage = handler;
                },
            });

            Object.defineProperty(this.port2, "onmessage", {
                get: function () {
                    return this._onmessage;
                },
                set: function (handler) {
                    this._onmessage = handler;
                },
            });
        }

        MessageChannel = MessageChannelPolyfill;
    })();
}