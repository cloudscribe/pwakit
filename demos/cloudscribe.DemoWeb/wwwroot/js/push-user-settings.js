const PushNotificationSettings = (function () {
    let applicationServerPublicKey;
    let serviceWorkerRegistration;
    
   
    function initializeUIState() {

        var subscribeButtons = document.querySelectorAll('[data-push-subscribe-button]');
        for (i = 0; i < subscribeButtons.length; ++i) {
            var btnSubscribe = subscribeButtons[i];
            btnSubscribe.addEventListener('click', subscribeForPushNotifications);
        }


        var unsubscribeButtons = document.querySelectorAll('[data-push-unsubscribe-button]');
        for (i = 0; i < unsubscribeButtons.length; ++i) {
            var btnUnsubscribe = unsubscribeButtons[i];
            btnUnsubscribe.addEventListener('click', unsubscribeFromPushNotifications);
        }
        
        serviceWorkerRegistration.pushManager.getSubscription()
            .then(function (subscription) {
                
                changeUIState(Notification.permission === 'denied', subscription !== null);

                if (Notification.permission === 'denied', subscription !== null) {

                    console.log('found subscription');
                    console.log(subscription);
                    //update the sub on the server
                    PushNotificationsController.storePushSubscription(subscription)
                        .then(function (response) {
                            if (response.ok) {
                                console.log('Successfully saved subscription');
                            } else {
                                console.log('Failed to uppdate subscription');
                            }
                        }).catch(function (error) {
                            console.log('Failed to update subscription on server: ' + error);
                        });

                }
                

            });



    }

    function changeUIState(notificationsBlocked, isSubscibed) {

        var subscribeButtons = document.querySelectorAll('[data-push-subscribe-button]');
        for (i = 0; i < subscribeButtons.length; ++i) {
            var btnSubscribe = subscribeButtons[i];
            if (notificationsBlocked || isSubscibed) {
                btnSubscribe.style.display = 'none';
            } else {
                btnSubscribe.style.display = 'inline-block';
            }
        }

        var unsubscribeButtons = document.querySelectorAll('[data-push-unsubscribe-button]');
        for (i = 0; i < unsubscribeButtons.length; ++i) {
            var btnUnsubscribe = unsubscribeButtons[i];
            if (notificationsBlocked || !isSubscibed) {
                btnUnsubscribe.style.display = 'none';
            } else {
                btnUnsubscribe.style.display = 'block';
            }
        }

        var notifications = document.querySelectorAll('[data-show-if-not-push-subscribed]');
        for (i = 0; i < notifications.length; ++i) {
            var n = notifications[i];
            if (notificationsBlocked || !isSubscibed) {
                n.style.display = 'block';
                n.classList.add('show');
            } else {
                n.style.display = 'none';
                n.classList.remove('show');
            }
        }
        

       
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

document.addEventListener("DOMContentLoaded", function () {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.ready.then(function (serviceWorkerRegistration) {
            PushNotificationSettings.initialize(serviceWorkerRegistration);
        });
    } else {
        var warnings = document.querySelectorAll('[data-show-if-push-not-supported]');
        for (i = 0; i < warnings.length; ++i) {
            warnings[i].style.display = 'block';
            
        }
    }

});
