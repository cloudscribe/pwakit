const PushNotifications = (function () {
    let applicationServerPublicKey;

    let consoleOutput;
    let serviceWorkerRegistration;
    let subscribeButton, unsubscribeButton;
    let titleInput, bodyInput, iconInput, badgeInput, imageInput;

    function initializeConsole() {
        consoleOutput = document.getElementById('output');
        document.getElementById('clear').addEventListener('click', clearConsole);
    }

    function clearConsole() {
        while (consoleOutput.childNodes.length > 0) {
            consoleOutput.removeChild(consoleOutput.lastChild);
        }
    }

    function writeToConsole(text) {
        var paragraph = document.createElement('p');
        paragraph.style.wordWrap = 'break-word';
        paragraph.appendChild(document.createTextNode(text));

        consoleOutput.appendChild(paragraph);
    }
    
    function initializeUIState() {
        subscribeButton = document.getElementById('subscribe');
        subscribeButton.addEventListener('click', subscribeForPushNotifications);

        unsubscribeButton = document.getElementById('unsubscribe');
        unsubscribeButton.addEventListener('click', unsubscribeFromPushNotifications);

        titleInput = document.getElementById('title');
        bodyInput = document.getElementById('body');
        iconInput = document.getElementById('icon');
        badgeInput = document.getElementById('badge');
        imageInput = document.getElementById('image');


        document.getElementById('send').addEventListener('click', broadcastNotification);

        document.getElementById('sendSelf').addEventListener('click', sendNotificationToSelf);

        serviceWorkerRegistration.pushManager.getSubscription()
            .then(function (subscription) {
                changeUIState(Notification.permission === 'denied', subscription !== null);
            });
    }

    function changeUIState(notificationsBlocked, isSubscibed) {
        subscribeButton.disabled = notificationsBlocked || isSubscibed;
        unsubscribeButton.disabled = notificationsBlocked || !isSubscibed;

        if (notificationsBlocked) {
            writeToConsole('Permission for Push Notifications has been denied');
        }
    }

    function subscribeForPushNotifications() {
        if (applicationServerPublicKey) {
            subscribeForPushNotificationsInternal();
        } else {
            PushNotificationsController.retrievePublicKey()
                .then(function (retrievedPublicKey) {
                    applicationServerPublicKey = retrievedPublicKey;
                    writeToConsole('Successfully retrieved Public Key');

                    subscribeForPushNotificationsInternal();
                }).catch(function (error) {
                    writeToConsole('Failed to retrieve Public Key: ' + error);
                });
        }
    }

    function subscribeForPushNotificationsInternal() {
        serviceWorkerRegistration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: applicationServerPublicKey
        })
            .then(function (pushSubscription) {
                PushNotificationsController.storePushSubscription(pushSubscription)
                    .then(function (response) {
                        if (response.ok) {
                            writeToConsole('Successfully subscribed for Push Notifications');
                        } else {
                            writeToConsole('Failed to store the Push Notifications subscrition on server');
                        }
                    }).catch(function (error) {
                        writeToConsole('Failed to store the Push Notifications subscrition on server: ' + error);
                    });

                changeUIState(false, true);
            }).catch(function (error) {
                if (Notification.permission === 'denied') {
                    changeUIState(true, false);
                } else {
                    writeToConsole('Failed to subscribe for Push Notifications: ' + error);
                }
            });
    }

    function unsubscribeFromPushNotifications() {
        serviceWorkerRegistration.pushManager.getSubscription()
            .then(function (pushSubscription) {
                if (pushSubscription) {
                    pushSubscription.unsubscribe()
                        .then(function () {
                            PushNotificationsController.discardPushSubscription(pushSubscription)
                                .then(function (response) {
                                    if (response.ok) {
                                        writeToConsole('Successfully unsubscribed from Push Notifications');
                                    } else {
                                        writeToConsole('Failed to discard the Push Notifications subscrition from server');
                                    }
                                }).catch(function (error) {
                                    writeToConsole('Failed to discard the Push Notifications subscrition from server: ' + error);
                                });

                            changeUIState(false, false);
                        }).catch(function (error) {
                            writeToConsole('Failed to unsubscribe from Push Notifications: ' + error);
                        });
                }
            });
    }

    function broadcastNotification() {
        let payload = { title: titleInput.value, body: bodyInput.value, icon: iconInput.value, badge: badgeInput.value, image: imageInput.value };

        fetch('/pwa/broadcastnotification', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        })
            .then(function (response) {
                if (response.ok) {
                    writeToConsole('Successfully sent Push Notification');
                } else {
                    writeToConsole('Failed to send Push Notification');
                }
            }).catch(function (error) {
                writeToConsole('Failed to send Push Notification: ' + error);
            });
    }

    function sendNotificationToSelf() {
        let payload = { title: titleInput.value, body: bodyInput.value, icon: iconInput.value, badge: badgeInput.value, image: imageInput.value };

        fetch('/pwa/sendnotificationtoself', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        })
            .then(function (response) {
                if (response.ok) {
                    writeToConsole('Successfully sent Push Notification');
                } else {
                    writeToConsole('Failed to send Push Notification');
                }
            }).catch(function (error) {
                writeToConsole('Failed to send Push Notification: ' + error);
            });
    }

    return {
        initialize: function (serviceWorkerReg) {
            initializeConsole();

            serviceWorkerRegistration = serviceWorkerReg;
            if (serviceWorkerRegistration) {
                console.log('found registration');
                initializeUIState();
            } else {
                console.log('registration not found');
            }

            
        }
    };
})();

window.swRegisteredHandler = PushNotifications.initialize;

