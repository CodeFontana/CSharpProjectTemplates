(() => {
    const maximumRetryCount = 20;
    const retryIntervalMilliseconds = 5000;
    const reconnectModal = document.getElementById('reconnect-modal');
    const reconnectDialog = document.getElementById('reconnect-dialog');

    const startReconnectionProcess = () => {
        reconnectModal.classList.remove('hide');
        reconnectModal.classList.add('show');

        let isCanceled = false;

        (async () => {
            for (let i = 0; i < maximumRetryCount; i++) {

                await new Promise(resolve => setTimeout(resolve, retryIntervalMilliseconds));

                if (isCanceled) {
                    return;
                }

                try {
                    const result = await Blazor.reconnect();
                    if (!result) {
                        // The server was reached, but the connection was rejected; reload the page.
                        location.reload();
                        return;
                    }

                    // Successfully reconnected to the server.
                    return;
                } catch {
                    // Didn't reach the server; try again.
                }
            }

            // Retried too many times; reload the page.
            location.reload();
        })();

        return {
            cancelReconnection: () => {
                isCanceled = true;

                reconnectDialog.classList.remove('animate-slide-down');
                reconnectDialog.classList.add('animate-slide-up');

                setTimeout(() => {
                    reconnectModal.classList.remove('show');
                    reconnectModal.classList.add('hide');
                    reconnectDialog.classList.add('animate-slide-down');
                }, 300);
            },
        };
    };

    let currentReconnectionProcess = null;

    Blazor.start({
        circuit: {
            configureSignalR: function (builder) {
                builder.withServerTimeout(60000);
            },
            reconnectionHandler: {
                onConnectionDown: () => currentReconnectionProcess ??= startReconnectionProcess(),
                onConnectionUp: () => {
                    currentReconnectionProcess?.cancelReconnection();
                    currentReconnectionProcess = null;
                }
            }
        }
    });
})();