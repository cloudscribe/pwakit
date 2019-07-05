const PushNotificationTester = (function () {
    let consoleOutput;
    let serviceWorkerRegistration;
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
        titleInput = document.getElementById('title');
        bodyInput = document.getElementById('body');
        iconInput = document.getElementById('icon');
        badgeInput = document.getElementById('badge');
        imageInput = document.getElementById('image');
        document.getElementById('send').addEventListener('click', broadcastNotification);
        document.getElementById('sendSelf').addEventListener('click', sendNotificationToSelf);

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

if ('serviceWorker' in navigator) {

    navigator.serviceWorker.ready.then(function (serviceWorkerRegistration) {
        PushNotificationTester.initialize(serviceWorkerRegistration);
    });
}

