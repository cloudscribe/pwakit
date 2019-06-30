const PushNotifications = (function () {
    let applicationServerPublicKey;
    let serviceWorkerRegistration;
    let subscribeButton, unsubscribeButton;
    

    function initializeUIState() {
        subscribeButton = document.getElementById('subscribe');
        subscribeButton.addEventListener('click', subscribeForPushNotifications);

        unsubscribeButton = document.getElementById('unsubscribe');
        unsubscribeButton.addEventListener('click', unsubscribeFromPushNotifications);
        
        serviceWorkerRegistration.pushManager.getSubscription()
            .then(function (subscription) {
                changeUIState(Notification.permission === 'denied', subscription !== null);
            });
    }

    function changeUIState(notificationsBlocked, isSubscibed) {
        subscribeButton.disabled = notificationsBlocked || isSubscibed;
        unsubscribeButton.disabled = notificationsBlocked || !isSubscibed;
        
    }

    function subscribeForPushNotifications() {
        if (applicationServerPublicKey) {
            subscribeForPushNotificationsInternal();
        } else {
            PushNotificationsController.retrievePublicKey()
                .then(function (retrievedPublicKey) {
                    applicationServerPublicKey = retrievedPublicKey;
                    subscribeForPushNotificationsInternal();
                }).catch(function (error) {
                    console.log(error);
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
                            console.log('Successfully subscribed for Push Notifications');
                        } else {
                            console.log('Failed to store the Push Notifications subscrition on server');
                        }
                    }).catch(function (error) {
                        console.log('Failed to store the Push Notifications subscrition on server: ' + error);
                    });

                changeUIState(false, true);
            }).catch(function (error) {
                if (Notification.permission === 'denied') {
                    changeUIState(true, false);
                } else {
                    console.log('Failed to subscribe for Push Notifications: ' + error);
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
                                        console.log('Successfully unsubscribed from Push Notifications');
                                    } else {
                                        console.log('Failed to discard the Push Notifications subscrition from server');
                                    }
                                }).catch(function (error) {
                                    console.log('Failed to discard the Push Notifications subscrition from server: ' + error);
                                });

                            changeUIState(false, false);
                        }).catch(function (error) {
                            console.log('Failed to unsubscribe from Push Notifications: ' + error);
                        });
                }
            });
    }

    

    return {
        initialize: function (serviceWorkerReg) {
            
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